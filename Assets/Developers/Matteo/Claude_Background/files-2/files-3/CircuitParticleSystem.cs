using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CircuitParticleSystem v5
/// Fix:
/// - I circuiti seguono la camera (si rigenerano intorno ad essa)
/// - Aggiunta direzione VERTICALE: alcuni circuiti partono dall'alto o dal basso
/// - Rotazione sprite corretta in tutte le direzioni
/// </summary>
public class CircuitParticleSystem : MonoBehaviour
{
    [Header("Sprite laser")]
    [Tooltip("Trascina qui le sprite 01-10 dalla cartella Sprites-LaserBullets")]
    public Sprite[] LaserSprites;

    [Header("Camera")]
    public Transform CamTransform;

    [Header("Sorting")]
    public string SortingLayer = "Background";
    public int    SortingOrder = -8;

    [Header("Circuiti")]
    public int   CircuitCount  = 8;
    [Tooltip("Lunghezza minima segmento (unità Unity)")]
    public float SegMin        = 2f;
    public float SegMax        = 8f;
    [Tooltip("= TileWorld + TileGap (con gap=0 → uguale a TileWorld=1)")]
    public float JogSize       = 1.0f;
    [Range(0f,1f)]
    public float JogProb       = 0.5f;
    [Tooltip("% di circuiti verticali (0=tutti orizzontali, 1=tutti verticali)")]
    [Range(0f,1f)]
    public float VerticalRatio = 0.4f;
    public bool  ShowTracks    = true;
    public Color TrackColor    = new Color(0.2f, 0.5f, 1f, 0.15f);

    [Header("Particelle")]
    public int   PerCircuit    = 2;
    public float SpeedMin      = 3f;
    public float SpeedMax      = 7f;
    public float SpriteScale   = 0.5f;

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

        for (int i = 0; i < CircuitCount; i++)
            SpawnCircuit();
    }

    void Update()
    {
        RefreshCircuits();
        MoveParticles();
    }

    // ── Spawn / recycle ───────────────────────────────────────────────────

    void SpawnCircuit()
    {
        bool isVertical = Random.value < VerticalRatio;
        var  pts        = isVertical ? BuildVerticalPath() : BuildHorizontalPath();

        float[] dists = new float[pts.Count];
        for (int i = 1; i < pts.Count; i++)
            dists[i] = dists[i-1] + Vector2.Distance(pts[i-1], pts[i]);

        var c = new Circuit
        {
            Pts        = pts,
            Dists      = dists,
            Total      = dists[pts.Count-1],
            IsVertical = isVertical,
            Track      = ShowTracks ? MakeTrack(pts) : null
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
                GO    = go
            });
        }
    }

    /// <summary>
    /// Controlla tutti e 4 i bordi della camera.
    /// Se un circuito è uscito completamente dalla vista, lo ricicla e ne spawna uno nuovo.
    /// </summary>
    void RefreshCircuits()
    {
        float camH     = _cam.orthographicSize * 2f;
        float camW     = camH * _cam.aspect;
        float margin   = Mathf.Max(SegMax, 2f);
        float left     = CamTransform.position.x - camW * 0.5f - margin;
        float right    = CamTransform.position.x + camW * 0.5f + margin;
        float bottom   = CamTransform.position.y - camH * 0.5f - margin;
        float top      = CamTransform.position.y + camH * 0.5f + margin;

        for (int i = _circuits.Count-1; i >= 0; i--)
        {
            var c   = _circuits[i];
            var end = c.Pts[c.Pts.Count-1];

            // Il circuito è uscito dal viewport espanso?
            bool outOfBounds = c.IsVertical
                ? (end.y < bottom || end.y > top   || end.x < left || end.x > right)
                : (end.x < left  || end.x > right  || end.y < bottom || end.y > top);

            if (outOfBounds)
            {
                if (c.Track != null) Destroy(c.Track);
                _circuits.RemoveAt(i);
                for (int p = _particles.Count-1; p >= 0; p--)
                    if (_particles[p].C == c) { _pool.Return(_particles[p].GO); _particles.RemoveAt(p); }
                SpawnCircuit();
            }
        }
    }

    // ── Path builders ─────────────────────────────────────────────────────

    /// Percorso che va da sinistra a destra con jog verticali
    List<Vector2> BuildHorizontalPath()
    {
        float camH   = _cam.orthographicSize * 2f;
        float camW   = camH * _cam.aspect;
        float left   = CamTransform.position.x - camW * 0.5f - 1f;
        float right  = CamTransform.position.x + camW * 0.5f + 2f;
        float bottom = CamTransform.position.y - camH * 0.5f + 0.3f;
        float top    = CamTransform.position.y + camH * 0.5f - 0.3f;

        var   pts = new List<Vector2>();
        float x   = left;
        float y   = bottom + Random.Range(0f, top - bottom);
        pts.Add(new Vector2(x, y));

        while (x < right)
        {
            x += Random.Range(SegMin, SegMax);
            pts.Add(new Vector2(x, y));
            if (Random.value < JogProb && x < right - SegMin)
            {
                float dir  = Random.value < 0.5f ? 1f : -1f;
                float reps = Mathf.Floor(1 + Random.Range(0f, 2.9f));
                y = Mathf.Clamp(y + dir * JogSize * reps, bottom, top);
                pts.Add(new Vector2(x, y));
            }
        }
        return pts;
    }

    /// Percorso che va dal basso verso l'alto con jog orizzontali
    List<Vector2> BuildVerticalPath()
    {
        float camH   = _cam.orthographicSize * 2f;
        float camW   = camH * _cam.aspect;
        float left   = CamTransform.position.x - camW * 0.5f + 0.3f;
        float right  = CamTransform.position.x + camW * 0.5f - 0.3f;
        float bottom = CamTransform.position.y - camH * 0.5f - 1f;
        float top    = CamTransform.position.y + camH * 0.5f + 2f;

        var   pts = new List<Vector2>();
        float y   = bottom;
        float x   = left + Random.Range(0f, right - left);
        pts.Add(new Vector2(x, y));

        while (y < top)
        {
            y += Random.Range(SegMin, SegMax);
            pts.Add(new Vector2(x, y));
            if (Random.value < JogProb && y < top - SegMin)
            {
                float dir  = Random.value < 0.5f ? 1f : -1f;
                float reps = Mathf.Floor(1 + Random.Range(0f, 2.9f));
                x = Mathf.Clamp(x + dir * JogSize * reps, left, right);
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
        foreach (var p in _particles)
        {
            p.Dist = (p.Dist + p.Speed * dt) % p.C.Total;

            Vector2 pos = Sample(p.C, p.Dist);
            p.GO.transform.position = new Vector3(pos.x, pos.y, 0f);

            // Rotazione in base alla direzione del segmento corrente
            Vector2 dir = SampleDir(p.C, p.Dist);
            if (dir.sqrMagnitude > 0.001f)
                p.GO.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        }
    }

    Vector2 Sample(Circuit c, float d)
    {
        d = ((d % c.Total) + c.Total) % c.Total;
        int lo = 0, hi = c.Pts.Count-1;
        while (lo < hi-1) { int m=(lo+hi)>>1; if (c.Dists[m]<=d) lo=m; else hi=m; }
        float span = c.Dists[hi] - c.Dists[lo];
        float t    = span > 0.0001f ? (d - c.Dists[lo]) / span : 0f;
        return Vector2.Lerp(c.Pts[lo], c.Pts[hi], t);
    }

    Vector2 SampleDir(Circuit c, float d)
    {
        d = ((d % c.Total) + c.Total) % c.Total;
        int lo = 0, hi = c.Pts.Count-1;
        while (lo < hi-1) { int m=(lo+hi)>>1; if (c.Dists[m]<=d) lo=m; else hi=m; }
        return (c.Pts[hi] - c.Pts[lo]).normalized;
    }

    // ── Data ──────────────────────────────────────────────────────────────

    class Circuit
    {
        public List<Vector2> Pts;
        public float[]       Dists;
        public float         Total;
        public bool          IsVertical;
        public GameObject    Track;
    }

    class Particle
    {
        public Circuit    C;
        public float      Dist, Speed;
        public GameObject GO;
    }

    // ── Pool ──────────────────────────────────────────────────────────────

    class GOPool
    {
        readonly Stack<GameObject> _s = new Stack<GameObject>();
        readonly Sprite[]  _sprites;
        readonly string    _layer;
        readonly int       _order;
        readonly Transform _parent;

        public GOPool(Sprite[] sprites, string layer, int order, Transform parent, int n)
        {
            _sprites=sprites; _layer=layer; _order=order; _parent=parent;
            for (int i=0; i<n; i++) _s.Push(Make());
        }

        GameObject Make()
        {
            var go = new GameObject("Laser");
            go.transform.SetParent(_parent, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite           = _sprites[Random.Range(0, _sprites.Length)];
            sr.sortingLayerName = _layer;
            sr.sortingOrder     = _order;
            go.SetActive(false);
            return go;
        }

        public GameObject Get()
        {
            var go = _s.Count > 0 ? _s.Pop() : Make();
            go.GetComponent<SpriteRenderer>().sprite = _sprites[Random.Range(0, _sprites.Length)];
            go.SetActive(true);
            return go;
        }

        public void Return(GameObject go) { go.SetActive(false); _s.Push(go); }
    }
}