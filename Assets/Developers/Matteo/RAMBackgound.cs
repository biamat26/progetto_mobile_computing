using UnityEngine;
using System.Collections.Generic;

public class RAMBackground : MonoBehaviour
{
    [Header("Griglia")]
    public int cols = 30;
    public int rows = 20;
    public float tileSize = 1f;

    [Header("Effetto")]
    [Range(0f, 10f)] public float frequency = 4f;
    [Range(1, 20)]   public int   particleCount = 8;

    [Header("Colori tile")]
    public Color bgColor    = new Color(0.04f, 0.05f, 0.09f);
    public Color tileColor0 = new Color(0.055f, 0.067f, 0.118f);
    public Color tileColor1 = new Color(0.063f, 0.078f, 0.137f);
    public Color tileColor2 = new Color(0.047f, 0.059f, 0.102f);
    public Color edgeLight  = new Color(0.118f, 0.165f, 0.255f);
    public Color edgeDark   = new Color(0.031f, 0.039f, 0.071f);

    [Header("Colori particelle")]
    public Color[] particleColors = new Color[]
    {
        new Color(0.267f, 0.400f, 1.000f),
        new Color(0.400f, 0.200f, 0.800f),
        new Color(0.533f, 0.267f, 0.933f),
        new Color(0.200f, 0.333f, 0.867f),
        new Color(0.467f, 0.133f, 0.733f),
        new Color(0.333f, 0.267f, 1.000f),
    };

    private class Tile
    {
        public int   type;
        public float alpha = 1f, scale = 1f;
        public float vy, vx, y, x;
        public float timer;
        public int   newType;
        public enum State { Idle, Leaving, Empty, Arriving }
        public State state = State.Idle;
    }

    private class Particle
    {
        public int   pathIndex;
        public float t, speed;
        public Color color;
        public float size;
        public List<Vector2> trail = new List<Vector2>();
    }

    private const int TS = 16;

    private Tile[,]      _grid;
    private Particle[]   _particles;
    private Vector2[][]  _paths;
    private float        _deallocTimer;
    private Material     _mat;
    private Texture2D    _tex;
    private int          _texW, _texH;
    private Color[]      _pixels;

    void Start()
    {
        _texW = cols * TS;
        _texH = rows * TS;

        _tex = new Texture2D(_texW, _texH, TextureFormat.RGBA32, false);
        _tex.filterMode = FilterMode.Point;
        _pixels = new Color[_texW * _texH];

        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "RAMBackgroundQuad";
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale    = new Vector3(cols * tileSize, rows * tileSize, 1f);
        Destroy(go.GetComponent<MeshCollider>());

        _mat = new Material(Shader.Find("Sprites/Default"));
        _mat.mainTexture = _tex;
        var mr = go.GetComponent<MeshRenderer>();
        mr.material     = _mat;
        mr.sortingOrder = -10;

        BuildGrid();
        BuildPaths();
        BuildParticles();
    }

    void BuildGrid()
    {
        _grid = new Tile[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                _grid[r, c] = new Tile
                {
                    type  = (r * 7 + c * 3) % 3,
                    timer = Random.Range(0f, 120f),
                };
    }

    
    void BuildPaths()
{
    _paths = new Vector2[][]
    {
        new Vector2[]{ new(0, _texH*0.2f), new(_texW*0.4f, _texH*0.2f), new(_texW*0.4f, _texH*0.5f), new(_texW, _texH*0.5f) },
        new Vector2[]{ new(0, _texH*0.6f), new(_texW*0.3f, _texH*0.6f), new(_texW*0.3f, _texH*0.3f), new(_texW*0.7f, _texH*0.3f), new(_texW*0.7f, _texH*0.7f), new(_texW, _texH*0.7f) },
        new Vector2[]{ new(_texW*0.2f, 0), new(_texW*0.2f, _texH*0.4f), new(_texW*0.6f, _texH*0.4f), new(_texW*0.6f, _texH) },
        new Vector2[]{ new(_texW*0.8f, 0), new(_texW*0.8f, _texH*0.5f), new(_texW*0.4f, _texH*0.5f), new(_texW*0.4f, _texH) },
        new Vector2[]{ new(0, _texH*0.85f), new(_texW*0.6f, _texH*0.85f), new(_texW*0.6f, _texH*0.15f), new(_texW, _texH*0.15f) },
    };
}

    void BuildParticles()
    {
        _particles = new Particle[particleCount];
        for (int i = 0; i < particleCount; i++)
            _particles[i] = new Particle
            {
                pathIndex = Random.Range(0, _paths.Length),
                t         = Random.value,
                speed     = 0.002f + Random.value * 0.003f,
                color     = particleColors[Random.Range(0, particleColors.Length)],
                size      = 1.5f + Random.value * 2f,
            };
    }

    void Update()
    {
        UpdateGrid();
        UpdateParticles();
        RenderFrame();
    }

    void UpdateGrid()
    {
        _deallocTimer += Time.deltaTime * frequency;
        if (_deallocTimer > 1f) { _deallocTimer = 0f; TriggerDealloc(); }

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            var t = _grid[r, c];
            t.timer -= Time.deltaTime * 60f;

            switch (t.state)
            {
                case Tile.State.Leaving:
                    t.y += t.vy * 2.5f; t.x += t.vx * 2f;
                    t.alpha -= 0.05f;   t.scale += 0.025f;
                    if (t.alpha <= 0f)
                    {
                        t.state = Tile.State.Empty;
                        t.timer = Random.Range(15f, 40f);
                        t.alpha = 0f; t.y = 0f; t.x = 0f; t.scale = 1f;
                    }
                    break;
                case Tile.State.Empty:
                    if (t.timer <= 0f)
                    {
                        t.state = Tile.State.Arriving;
                        t.type  = t.newType;
                        t.y     = -(TS * 2.5f);
                        t.alpha = 0f; t.scale = 1.4f;
                    }
                    break;
                case Tile.State.Arriving:
                    t.y += 2.5f; t.alpha += 0.07f; t.scale -= 0.018f;
                    if (t.y >= 0f)
                    { t.y = 0f; t.alpha = 1f; t.scale = 1f; t.state = Tile.State.Idle; }
                    break;
            }
        }
    }

    void TriggerDealloc()
    {
        var idle = new List<(int r, int c)>();
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            if (_grid[r, c].state == Tile.State.Idle) idle.Add((r, c));
        if (idle.Count == 0) return;
        var (rr, cc) = idle[Random.Range(0, idle.Count)];
        var t = _grid[rr, cc];
        t.state   = Tile.State.Leaving;
        t.vy      = -(0.4f + Random.value * 0.5f);
        t.vx      = (Random.value - 0.5f) * 0.4f;
        t.newType = Random.Range(0, 3);
    }

    void UpdateParticles()
    {
        foreach (var p in _particles)
        {
            p.trail.Add(PathPoint(_paths[p.pathIndex], p.t));
            if (p.trail.Count > 8) p.trail.RemoveAt(0);
            p.t = (p.t + p.speed) % 1f;
        }
    }

    Vector2 PathPoint(Vector2[] path, float t)
    {
        int segs = path.Length - 1;
        int seg  = Mathf.Min(Mathf.FloorToInt(t * segs), segs - 1);
        float lt = t * segs - seg;
        return Vector2.Lerp(path[seg], path[seg + 1], lt);
    }

    void RenderFrame()
{
    // reset sicuro
    System.Array.Clear(_pixels, 0, _pixels.Length);
    for (int i = 0; i < _pixels.Length; i++) _pixels[i] = bgColor;

    for (int r = 0; r < rows; r++)
    for (int c = 0; c < cols; c++)
    {
        var t = _grid[r, c];
        if (t.state == Tile.State.Empty) continue;
        DrawTile(c, r, t);
    }

    foreach (var path in _paths)
        for (int i = 0; i < path.Length - 1; i++)
            DrawLine(path[i], path[i + 1], new Color(0.12f, 0.18f, 0.28f, 0.5f));

    foreach (var p in _particles)
    {
        for (int i = 0; i < p.trail.Count; i++)
        {
            float a = (float)i / p.trail.Count * 0.5f;
            var pt = p.trail[i];
            DrawDot(pt, p.size * 0.7f, new Color(p.color.r, p.color.g, p.color.b, a));
        }
        DrawDot(PathPoint(_paths[p.pathIndex], p.t), p.size, p.color);
    }

    _tex.SetPixels(_pixels);
    _tex.Apply();
}

    Color[] TileColors => new[]{ tileColor0, tileColor1, tileColor2 };

    void DrawTile(int c, int r, Tile t)
{
    int ox = Mathf.RoundToInt(c * TS + t.x);
    int oy = Mathf.RoundToInt(r * TS + t.y);

    // skip se completamente fuori texture
    if (ox + TS < 0 || ox >= _texW) return;
    if (oy + TS < 0 || oy >= _texH) return;

    Color fill = TileColors[t.type % 3];
    for (int py = 0; py < TS; py++)
    for (int px = 0; px < TS; px++)
    {
        bool isTopLeft = px == 0 || py == 0;
        bool isEdge    = px == 0 || py == 0 || px == TS-1 || py == TS-1;
        Color col = isEdge ? (isTopLeft ? edgeLight : edgeDark) : fill;
        col.a = Mathf.Clamp01(t.alpha);
        SetPixel(ox + px, oy + py, col);
    }
}

    void DrawDot(Vector2 pos, float size, Color col)
    {
        int r = Mathf.RoundToInt(size / 2);
        for (int dy = -r; dy <= r; dy++)
        for (int dx = -r; dx <= r; dx++)
            SetPixel(Mathf.RoundToInt(pos.x)+dx, Mathf.RoundToInt(pos.y)+dy, col);
    }

    void DrawLine(Vector2 a, Vector2 b, Color col)
    {
        int steps = Mathf.RoundToInt(Vector2.Distance(a, b));
        for (int i = 0; i <= steps; i++)
        {
            Vector2 p = Vector2.Lerp(a, b, (float)i / steps);
            SetPixel(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y), col);
        }
    }

void SetPixel(int x, int y, Color col)
{
    x = Mathf.Clamp(x, 0, _texW - 1);
    y = Mathf.Clamp(y, 0, _texH - 1);
    int idx = y * _texW + x;
    if (col.a >= 1f) { _pixels[idx] = col; return; }
    _pixels[idx] = Color.Lerp(_pixels[idx], col, col.a);
}
}