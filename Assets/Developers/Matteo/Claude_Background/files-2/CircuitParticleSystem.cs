using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CircuitParticleSystem v5
/// - Particelle con vita più lunga (percorsi più lunghi, più loop)
/// - Circuiti orizzontali e verticali
/// - Seguono la camera in tutte le direzioni
/// </summary>
public class CircuitParticleSystem : MonoBehaviour
{
    [Header("Sprite laser")]
    [Tooltip("Trascina le sprite 01-10 da Sprites-LaserBullets")]
    public Sprite[] LaserSprites;

    [Header("Camera")]
    public Transform CamTransform;

    [Header("Sorting")]
    public string SortingLayer = "Background";
    public int    SortingOrder = -8;

    [Header("Circuiti")]
    public int   CircuitCount  = 8;
    public float SegMin        = 2f;
    public float SegMax        = 8f;
    [Tooltip("= TileWorld + TileGap (con gap=0 → 1.0)")]
    public float JogSize       = 1.0f;
    [Range(0f, 1f)]
    public float JogProb       = 0.5f;
    [Range(0f, 1f)]
    [Tooltip("Percentuale di circuiti verticali")]
    public float VerticalRatio = 0.4f;
    public bool  ShowTracks    = true;
    public Color TrackColor    = new Color(0.2f, 0.5f, 1f, 0.15f);

    [Header("Particelle")]
    public int   PerCircuit    = 2;
    public float SpeedMin      = 2f;
    public float SpeedMax      = 5f;
    public float SpriteScale   = 0.5f;
    [Tooltip("Quante volte la particella percorre il circuito prima di essere riciclata. " +
             "0 = infinito (vive finché il circuito esce dalla vista)")]
    public int   LoopsBeforeRecycle = 0;

    // ── internals ─────────────────────────────────────────────────────────
    Camera         _cam;
    List<Circuit>  _circuits  = new List<Circuit>();
    List<Particle> _particles = new List<Particle>();
    GOPool         _pool;

    void Start()
    {
        _cam = Camera.main;
        if (CamTransform == null) CamTransform = _cam.transform;

        if (LaserSprites == null || LaserSprites.Length == 0)
        {
            Debug.LogWarning("[CircuitParticles] Nessuna sprite laser assegnata!");
            return;
        }

        _pool = new GOPool(LaserSprites, SortingLayer, SortingOrder, transform, 200);
        for (int i = 0; i < CircuitCount; i++) SpawnCircuit();
    }

    void Update()
    {
        RefreshCircuits();
        MoveParticles();
    }

    // ── Spawn ─────────────────────────────────────────────────────────────

    void SpawnCircuit()
    {
        bool isVert = Random.value < VerticalRatio;
        var  pts    = isVert ? BuildVerticalPath() : BuildHorizontalPath();

        float[] dists = new float[pts.Count];
        for (int i = 1; i < pts.Count; i++)
            dists[i] = dists[i-1] + Vector2.Distance(pts[i-1], pts[i]);

        var c = new Circuit
        {
            Pts   = pts, Dists = dists,
            Total = dists[pts.Count-1],
            IsVert = isVert,
            Track  = ShowTracks ? MakeTrack(pts) : null
        };
        _circuits.Add(c);

        for (int i = 0; i < PerCircuit; i++)
        {
            var go = _pool.Get();
            go.transform.localScale = Vector3.one * SpriteScale;
            _particles.Add(new Particle
            {
                C     = c,
                Dist  = Random.Range(0f, c.Total),
                Speed = Random.Range(SpeedMin, SpeedMax),
                GO    = go,
                Loops = 0
            });
        }
    }

    // ── Recycle ───────────────────────────────────────────────────────────

    void RefreshCircuits()
    {
        float camH   = _cam.orthographicSize * 2f;
        float camW   = camH * _cam.aspect;
        float margin = Mathf.Max(SegMax, 3f);
        float left   = CamTransform.position.x - camW*.5f - margin;
        float right  = CamTransform.position.x + camW*.5f + margin;
        float bottom = CamTransform.position.y - camH*.5f - margin;
        float top    = CamTransform.position.y + camH*.5f + margin;

        for (int i = _circuits.Count-1; i >= 0; i--)
        {
            var c   = _circuits[i];
            var end = c.Pts[c.Pts.Count-1];
            bool out4 = end.x < left || end.x > right || end.y < bottom || end.y > top;
            if (!out4) continue;

            if (c.Track != null) Destroy(c.Track);
            _circuits.RemoveAt(i);
            for (int p = _particles.Count-1; p >= 0; p--)
                if (_particles[p].C == c) { _pool.Return(_particles[p].GO); _particles.RemoveAt(p); }
            SpawnCircuit();
        }
    }

    // ── Path builders ─────────────────────────────────────────────────────

    List<Vector2> BuildHorizontalPath()
    {
        float camH   = _cam.orthographicSize * 2f;
        float camW   = camH * _cam.aspect;
        float left   = CamTransform.position.x - camW*.5f - 1f;
        float right  = CamTransform.position.x + camW*.5f + 2f;
        float bottom = CamTransform.position.y - camH*.5f + .3f;
        float top    = CamTransform.position.y + camH*.5f - .3f;

        var   pts = new List<Vector2>();
        float x = left, y = bottom + Random.Range(0f, top-bottom);
        pts.Add(new Vector2(x, y));
        while (x < right)
        {
            x += Random.Range(SegMin, SegMax);
            pts.Add(new Vector2(x, y));
            if (Random.value < JogProb && x < right-SegMin)
            {
                float dir = Random.value < .5f ? 1f : -1f;
                float rep = Mathf.Floor(1+Random.Range(0f, 2.9f));
                y = Mathf.Clamp(y + dir*JogSize*rep, bottom, top);
                pts.Add(new Vector2(x, y));
            }
        }
        return pts;
    }

    List<Vector2> BuildVerticalPath()
    {
        float camH   = _cam.orthographicSize * 2f;
        float camW   = camH * _cam.aspect;
        float left   = CamTransform.position.x - camW*.5f + .3f;
        float right  = CamTransform.position.x + camW*.5f - .3f;
        float bottom = CamTransform.position.y - camH*.5f - 1f;
        float top    = CamTransform.position.y + camH*.5f + 2f;

        var   pts = new List<Vector2>();
        float y = bottom, x = left + Random.Range(0f, right-left);
        pts.Add(new Vector2(x, y));
        while (y < top)
        {
            y += Random.Range(SegMin, SegMax);
            pts.Add(new Vector2(x, y));
            if (Random.value < JogProb && y < top-SegMin)
            {
                float dir = Random.value < .5f ? 1f : -1f;
                float rep = Mathf.Floor(1+Random.Range(0f, 2.9f));
                x = Mathf.Clamp(x + dir*JogSize*rep, left, right);
                pts.Add(new Vector2(x, y));
            }
        }
        return pts;
    }

    GameObject MakeTrack(List<Vector2> pts)
    {
        var go = new GameObject("Track");
        go.transform.SetParent(transform, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount    = pts.Count;
        lr.startWidth       = lr.endWidth = 0.03f;
        lr.material         = new Material(Shader.Find("Sprites/Default"));
        lr.startColor       = lr.endColor = TrackColor;
        lr.sortingLayerName = SortingLayer;
        lr.sortingOrder     = SortingOrder - 1;
        lr.useWorldSpace    = true;
        for (int i = 0; i < pts.Count; i++)
            lr.SetPosition(i, new Vector3(pts[i].x, pts[i].y, 0f));
        return go;
    }

    // ── Movement ──────────────────────────────────────────────────────────

    void MoveParticles()
    {
        float dt = Time.deltaTime;
        for (int i = _particles.Count-1; i >= 0; i--)
        {
            var p = _particles[i];
            float prev = p.Dist;
            p.Dist += p.Speed * dt;

            // Conta loop completati
            if (p.Dist >= p.C.Total)
            {
                p.Loops++;
                p.Dist %= p.C.Total;

                // Ricicla se ha fatto abbastanza loop (solo se LoopsBeforeRecycle > 0)
                if (LoopsBeforeRecycle > 0 && p.Loops >= LoopsBeforeRecycle)
                {
                    _pool.Return(p.GO);
                    _particles.RemoveAt(i);
                    // Spawna nuova particella sullo stesso circuito
                    var go = _pool.Get();
                    go.transform.localScale = Vector3.one * SpriteScale;
                    _particles.Add(new Particle
                    {
                        C = p.C, Dist = 0f,
                        Speed = Random.Range(SpeedMin, SpeedMax),
                        GO = go, Loops = 0
                    });
                    continue;
                }
            }

            Vector2 pos = Sample(p.C, p.Dist);
            p.GO.transform.position = new Vector3(pos.x, pos.y, 0f);

            Vector2 dir = SampleDir(p.C, p.Dist);
            if (dir.sqrMagnitude > 0.001f)
                p.GO.transform.rotation = Quaternion.Euler(0f, 0f,
                    Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }
    }

    Vector2 Sample(Circuit c, float d)
    {
        d = ((d % c.Total) + c.Total) % c.Total;
        int lo = 0, hi = c.Pts.Count-1;
        while (lo < hi-1) { int m=(lo+hi)>>1; if (c.Dists[m]<=d) lo=m; else hi=m; }
        float span = c.Dists[hi]-c.Dists[lo];
        float t    = span > 0.0001f ? (d-c.Dists[lo])/span : 0f;
        return Vector2.Lerp(c.Pts[lo], c.Pts[hi], t);
    }

    Vector2 SampleDir(Circuit c, float d)
    {
        d = ((d % c.Total) + c.Total) % c.Total;
        int lo = 0, hi = c.Pts.Count-1;
        while (lo < hi-1) { int m=(lo+hi)>>1; if (c.Dists[m]<=d) lo=m; else hi=m; }
        return (c.Pts[hi]-c.Pts[lo]).normalized;
    }

    // ── Data ──────────────────────────────────────────────────────────────

    class Circuit
    {
        public List<Vector2> Pts;
        public float[]       Dists;
        public float         Total;
        public bool          IsVert;
        public GameObject    Track;
    }

    class Particle
    {
        public Circuit    C;
        public float      Dist, Speed;
        public int        Loops;
        public GameObject GO;
    }

    // ── Pool ──────────────────────────────────────────────────────────────

    class GOPool
    {
        readonly Stack<GameObject> _s = new Stack<GameObject>();
        readonly Sprite[]  _spr;
        readonly string    _layer;
        readonly int       _order;
        readonly Transform _parent;

        public GOPool(Sprite[] spr, string layer, int order, Transform parent, int n)
        { _spr=spr; _layer=layer; _order=order; _parent=parent; for(int i=0;i<n;i++) _s.Push(Make()); }

        GameObject Make()
        {
            var go = new GameObject("Laser");
            go.transform.SetParent(_parent, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _spr[Random.Range(0, _spr.Length)];
            sr.sortingLayerName = _layer;
            sr.sortingOrder     = _order;
            go.SetActive(false);
            return go;
        }

        public GameObject Get()
        {
            var go = _s.Count > 0 ? _s.Pop() : Make();
            go.GetComponent<SpriteRenderer>().sprite = _spr[Random.Range(0, _spr.Length)];
            go.SetActive(true);
            return go;
        }

        public void Return(GameObject go) { go.SetActive(false); _s.Push(go); }
    }
}
