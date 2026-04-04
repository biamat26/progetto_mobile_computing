using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CircuitParticleSystem — particelle blu/viola che seguono percorsi
/// Manhattan (tracce PCB) in world space.
///
/// GERARCHIA OGGETTI DA CREARE IN UNITY:
///   GameObject "CircuitParticles" (figlio di RAMSystem oppure separato)
///     └── aggiungi questo script
///
/// SORTING: tenere a -8 (sopra i tile, sotto il gameplay)
///
/// MATERIAL GLOW (opzionale ma consigliato):
///   1. Create → Material → chiama "BitAdditive"
///   2. Shader: Sprites/Default
///   3. Blend Mode: Additive (Source: One, Dest: One)
///   4. Assegna a GlowMaterial nell'Inspector
/// </summary>
public class CircuitParticleSystem : MonoBehaviour
{
    [Header("Sprites & Material")]
    [Tooltip("bit_particle.png importato come Sprite")]
    public Sprite   ParticleSprite;
    [Tooltip("Material Sprites/Default con Additive blending — opzionale")]
    public Material GlowMaterial;

    [Header("Camera")]
    public Transform CamTransform;

    [Header("Sorting")]
    public string SortingLayer = "Background";
    public int    SortingOrder = -8;

    [Header("Circuiti")]
    [Tooltip("Quanti circuiti visibili contemporaneamente")]
    public int   CircuitCount   = 7;
    public float SegMinH        = 2f;     // lunghezza min segmento orizzontale
    public float SegMaxH        = 7f;     // lunghezza max segmento orizzontale
    public float JogH           = 1.05f;  // altezza jog verticale (= TileWorld + TileGap)
    [Range(0f,1f)]
    public float JogProb        = 0.5f;   // probabilità di jog dopo ogni segmento
    public bool  ShowTracks     = true;   // mostra tracce LineRenderer
    public Color TrackBlue      = new Color(0.15f, 0.40f, 0.85f, 0.18f);
    public Color TrackPurple    = new Color(0.45f, 0.18f, 0.85f, 0.14f);

    [Header("Particelle")]
    public int   PerCircuit     = 2;
    public float SpeedMin       = 2f;
    public float SpeedMax       = 5f;
    public int   TrailLen       = 12;    // ghost nella scia
    public float TrailStep      = 0.12f; // distanza tra ghost (unità Unity)
    public float SizeMin        = 0.06f;
    public float SizeMax        = 0.11f;
    public Color ColorBlue      = new Color(0.35f, 0.65f, 1.00f, 1f);
    public Color ColorPurple    = new Color(0.62f, 0.38f, 1.00f, 1f);

    // ── Internals ─────────────────────────────────────────────────────────
    Camera          _cam;
    List<Circuit>   _circuits  = new List<Circuit>();
    List<Particle>  _particles = new List<Particle>();
    SRPool          _pool;

    void Start()
    {
        _cam = Camera.main;
        if (CamTransform == null) CamTransform = _cam.transform;
        _pool = new SRPool(ParticleSprite, GlowMaterial, SortingLayer, SortingOrder, transform, 300);

        for (int i = 0; i < CircuitCount; i++)
            SpawnCircuit();
    }

    void Update()
    {
        RecycleOldCircuits();
        MoveParticles();
    }

    // ── Circuit spawn / recycle ───────────────────────────────────────────

    void SpawnCircuit()
    {
        float camH = _cam.orthographicSize * 2f;
        float camW = camH * _cam.aspect;
        float left   = CamTransform.position.x - camW * 0.5f - 1f;
        float right  = CamTransform.position.x + camW * 0.5f + 2f;
        float bottom = CamTransform.position.y - camH * 0.5f + 0.3f;
        float top    = CamTransform.position.y + camH * 0.5f - 0.3f;

        bool isBlue = Random.value > 0.45f;
        List<Vector2> pts = BuildPath(left, right, bottom, top);

        // Distanze cumulative
        float[] dists = new float[pts.Count];
        for (int i = 1; i < pts.Count; i++)
            dists[i] = dists[i-1] + Vector2.Distance(pts[i-1], pts[i]);

        var circuit = new Circuit
        {
            Pts    = pts,
            Dists  = dists,
            Total  = dists[pts.Count-1],
            IsBlue = isBlue,
            Track  = ShowTracks ? MakeTrack(pts, isBlue ? TrackBlue : TrackPurple) : null
        };
        _circuits.Add(circuit);

        Color col = isBlue ? ColorBlue : ColorPurple;
        for (int i = 0; i < PerCircuit; i++)
        {
            float sz  = Random.Range(SizeMin, SizeMax);
            float spd = Random.Range(SpeedMin, SpeedMax);

            var head = _pool.Get();
            head.color = col;
            SetSize(head, sz);

            var trail = new SpriteRenderer[TrailLen];
            for (int t = 0; t < TrailLen; t++)
            {
                trail[t] = _pool.Get();
                float alpha = (1f - (float)(t+1)/TrailLen) * 0.5f;
                trail[t].color = new Color(col.r, col.g, col.b, alpha);
                SetSize(trail[t], sz * Mathf.Lerp(1f, 0.2f, (float)t/TrailLen));
            }

            _particles.Add(new Particle
            {
                C     = circuit,
                Dist  = Random.Range(0f, circuit.Total),
                Speed = spd,
                Head  = head,
                Trail = trail
            });
        }
    }

    void RecycleOldCircuits()
    {
        float camH = _cam.orthographicSize * 2f;
        float camW = camH * _cam.aspect;
        float leftEdge = CamTransform.position.x - camW * 0.5f - 2f;

        for (int i = _circuits.Count-1; i >= 0; i--)
        {
            var c = _circuits[i];
            if (c.Pts[c.Pts.Count-1].x < leftEdge)
            {
                if (c.Track != null) Destroy(c.Track);
                _circuits.RemoveAt(i);
                for (int p = _particles.Count-1; p >= 0; p--)
                    if (_particles[p].C == c)
                    {
                        ReturnParticle(_particles[p]);
                        _particles.RemoveAt(p);
                    }
                SpawnCircuit();
            }
        }
    }

    // ── Path generation ───────────────────────────────────────────────────

    List<Vector2> BuildPath(float left, float right, float bottom, float top)
    {
        var pts = new List<Vector2>();
        float x = left;
        float y = bottom + Random.Range(0f, top - bottom);
        pts.Add(new Vector2(x, y));

        while (x < right)
        {
            x += Random.Range(SegMinH, SegMaxH);
            pts.Add(new Vector2(x, y));
            if (Random.value < JogProb && x < right - SegMinH)
            {
                float dir  = Random.value < 0.5f ? 1f : -1f;
                float reps = Mathf.Floor(1 + Random.Range(0f, 2.9f));
                y = Mathf.Clamp(y + dir * JogH * reps, bottom, top);
                pts.Add(new Vector2(x, y));
            }
        }
        return pts;
    }

    GameObject MakeTrack(List<Vector2> pts, Color col)
    {
        var go = new GameObject("Track");
        go.transform.SetParent(transform, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.positionCount      = pts.Count;
        lr.startWidth         = lr.endWidth = 0.025f;
        lr.material           = new Material(Shader.Find("Sprites/Default"));
        lr.startColor         = lr.endColor = col;
        lr.sortingLayerName   = SortingLayer;
        lr.sortingOrder       = SortingOrder - 1;
        lr.useWorldSpace      = true;
        for (int i = 0; i < pts.Count; i++)
            lr.SetPosition(i, new Vector3(pts[i].x, pts[i].y, 0f));
        return go;
    }

    // ── Particle movement ─────────────────────────────────────────────────

    void MoveParticles()
    {
        float dt = Time.deltaTime;
        foreach (var p in _particles)
        {
            p.Dist = (p.Dist + p.Speed * dt) % p.C.Total;
            Vector2 hp = Sample(p.C, p.Dist);
            p.Head.transform.position = new Vector3(hp.x, hp.y, 0f);
            for (int t = 0; t < p.Trail.Length; t++)
            {
                Vector2 tp = Sample(p.C, p.Dist - TrailStep * (t+1));
                p.Trail[t].transform.position = new Vector3(tp.x, tp.y, 0f);
            }
        }
    }

    Vector2 Sample(Circuit c, float d)
    {
        float total = c.Total;
        d = ((d % total) + total) % total;
        int lo = 0, hi = c.Pts.Count - 1;
        while (lo < hi-1) { int m=(lo+hi)>>1; if (c.Dists[m]<=d) lo=m; else hi=m; }
        float span = c.Dists[hi] - c.Dists[lo];
        float t    = span > 0.0001f ? (d - c.Dists[lo]) / span : 0f;
        return Vector2.Lerp(c.Pts[lo], c.Pts[hi], t);
    }

    void SetSize(SpriteRenderer sr, float s)
    {
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size     = new Vector2(s, s);
    }

    void ReturnParticle(Particle p)
    {
        _pool.Return(p.Head);
        foreach (var t in p.Trail) _pool.Return(t);
    }

    // ── Data classes ──────────────────────────────────────────────────────

    class Circuit
    {
        public List<Vector2> Pts;
        public float[]       Dists;
        public float         Total;
        public bool          IsBlue;
        public GameObject    Track;
    }

    class Particle
    {
        public Circuit          C;
        public float            Dist, Speed;
        public SpriteRenderer   Head;
        public SpriteRenderer[] Trail;
    }

    // ── Object pool ───────────────────────────────────────────────────────

    class SRPool
    {
        Stack<SpriteRenderer> _s = new Stack<SpriteRenderer>();
        Sprite _spr; Material _mat; string _layer; int _order; Transform _p;

        public SRPool(Sprite spr, Material mat, string layer, int order, Transform parent, int n)
        {
            _spr=spr; _mat=mat; _layer=layer; _order=order; _p=parent;
            for (int i=0;i<n;i++) _s.Push(Make());
        }

        SpriteRenderer Make()
        {
            var go = new GameObject("Bit");
            go.transform.SetParent(_p, false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite           = _spr;
            if (_mat != null) sr.material = _mat;
            sr.sortingLayerName = _layer;
            sr.sortingOrder     = _order;
            return sr;
        }

        public SpriteRenderer Get()
        {
            var sr = _s.Count > 0 ? _s.Pop() : Make();
            sr.gameObject.SetActive(true);
            return sr;
        }

        public void Return(SpriteRenderer sr)
        {
            sr.gameObject.SetActive(false);
            _s.Push(sr);
        }
    }
}
