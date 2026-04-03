using UnityEngine;
using System.Collections.Generic;

public class RAMBackground : MonoBehaviour
{
    [Header("Griglia (Non esagerare con i numeri!)")]
    public int cols = 30;
    public int rows = 20;
    public float tileSize = 1f;

    [Header("Effetto")]
    [Range(0f, 10f)] public float frequency = 4f;
    [Range(1, 20)]   public int   particleCount = 12;

    [Header("Colori")]
    public Color bgColor     = new Color(0.04f, 0.05f, 0.09f);
    public Color tileColor0  = new Color(0.055f, 0.067f, 0.118f);
    public Color tileColor1  = new Color(0.063f, 0.078f, 0.137f);
    public Color tileColor2  = new Color(0.047f, 0.059f, 0.102f);
    public Color edgeLight   = new Color(0.118f, 0.165f, 0.255f);
    public Color edgeDark    = new Color(0.031f, 0.039f, 0.071f);

    public Color[] particleColors = new Color[] {
        new Color(0.267f, 0.400f, 1.000f),
        new Color(0.400f, 0.200f, 0.800f),
        new Color(0.533f, 0.267f, 0.933f)
    };

    // --- Logica Interna ---
    private class Tile {
        public int type;
        public float alpha = 1f;
        public float vy, vx, y, x;
        public float timer;
        public int newType;
        public enum State { Idle, Leaving, Empty, Arriving }
        public State state = State.Idle;
    }

    private class Particle {
        public int pathIndex;
        public float t, speed;
        public Color color;
        public float size;
        public List<Vector2> trail = new List<Vector2>();
    }

    private Tile[,] _grid;
    private Particle[] _particles;
    private Vector2[][] _paths;
    private float _deallocTimer;
    private Texture2D _tex;
    private Color[] _pixels;
    private int _texW, _texH;
    private float _frameTimer;
    private const float TARGET_FPS = 0.05f; // 20 FPS per il rendering (molto più leggero)

    void Start() {
        _texW = cols * 24;
        _texH = rows * 24;
        _tex = new Texture2D(_texW, _texH, TextureFormat.RGBA32, false);
        _tex.filterMode = FilterMode.Point;
        _pixels = new Color[_texW * _texH];

        // Crea il Quad visivo
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = "Background_Visual";
        go.transform.SetParent(transform);
        
        // POSIZIONE Z = 10 (Dietro a tutto)
        go.transform.localPosition = new Vector3(0, 0, 10f); 
        go.transform.localScale = new Vector3(cols * tileSize, rows * tileSize, 1f);

        // RIMUOVI FISICA (Cruciale per le sentinelle)
        Destroy(go.GetComponent<MeshCollider>());
        go.layer = 2; // Ignore Raycast

        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.mainTexture = _tex;
        go.GetComponent<MeshRenderer>().material = mat;
        go.GetComponent<MeshRenderer>().sortingOrder = -100;

        BuildGrid();
        BuildPaths();
        BuildParticles();
    }

    void Update() {
        UpdateGrid();
        UpdateParticles();

        // Limita il rendering per evitare il lag
        _frameTimer += Time.deltaTime;
        if (_frameTimer >= TARGET_FPS) {
            RenderFrame();
            _frameTimer = 0f;
        }
    }

    // --- Metodi di Costruzione e Update ---
    void BuildGrid() {
        _grid = new Tile[rows, cols];
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                _grid[r, c] = new Tile { type = (r * 7 + c * 3) % 3, timer = Random.Range(0f, 100f) };
    }

    void BuildPaths() {
        int[][] raw = { new[]{0,2, 8,2, 8,6, 18,6}, new[]{0,8, 5,8, 13,4, 18,10}, new[]{3,0, 10,5, 10,12} };
        _paths = new Vector2[raw.Length][];
        for (int i = 0; i < raw.Length; i++) {
            _paths[i] = new Vector2[raw[i].Length / 2];
            for (int j = 0; j < raw[i].Length; j += 2)
                _paths[i][j / 2] = new Vector2(raw[i][j] * 24 + 12, raw[i][j + 1] * 24 + 12);
        }
    }

    void BuildParticles() {
        _particles = new Particle[particleCount];
        for (int i = 0; i < particleCount; i++)
            _particles[i] = new Particle { pathIndex = Random.Range(0, _paths.Length), t = Random.value, speed = 0.003f, color = particleColors[Random.Range(0, 3)], size = 2f };
    }

    void UpdateGrid() {
        _deallocTimer += Time.deltaTime * frequency;
        if (_deallocTimer > 1f) { _deallocTimer = 0f; TriggerDealloc(); }
        foreach (var t in _grid) {
            t.timer -= Time.deltaTime * 30f;
            if (t.state == Tile.State.Leaving) {
                t.y += t.vy; t.alpha -= 0.05f;
                if (t.alpha <= 0) { t.state = Tile.State.Empty; t.timer = 20f; }
            } else if (t.state == Tile.State.Empty && t.timer <= 0) {
                t.state = Tile.State.Arriving; t.type = t.newType; t.y = -10f;
            } else if (t.state == Tile.State.Arriving) {
                t.y += 1f; t.alpha += 0.05f;
                if (t.y >= 0) { t.y = 0; t.alpha = 1; t.state = Tile.State.Idle; }
            }
        }
    }

    void TriggerDealloc() {
        int r = Random.Range(0, rows), c = Random.Range(0, cols);
        if (_grid[r, c].state == Tile.State.Idle) {
            _grid[r, c].state = Tile.State.Leaving;
            _grid[r, c].vy = -0.5f;
            _grid[r, c].newType = Random.Range(0, 3);
        }
    }

    void UpdateParticles() {
        foreach (var p in _particles) {
            p.trail.Add(PathPoint(_paths[p.pathIndex], p.t));
            if (p.trail.Count > 6) p.trail.RemoveAt(0);
            p.t = (p.t + p.speed) % 1f;
        }
    }

    Vector2 PathPoint(Vector2[] path, float t) {
        int segs = path.Length - 1;
        int seg = Mathf.Min(Mathf.FloorToInt(t * segs), segs - 1);
        return Vector2.Lerp(path[seg], path[seg + 1], t * segs - seg);
    }

    void RenderFrame() {
        for (int i = 0; i < _pixels.Length; i++) _pixels[i] = bgColor;
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (_grid[r, c].state != Tile.State.Empty) DrawTile(c, r, _grid[r, c]);
        
        foreach (var p in _particles) DrawDot(PathPoint(_paths[p.pathIndex], p.t), p.size, p.color);

        _tex.SetPixels(_pixels);
        _tex.Apply(false); // Falso = non genera mipmaps, più veloce
    }

    void DrawTile(int c, int r, Tile t) {
        int ox = c * 24 + (int)t.x;
        int oy = r * 24 + (int)t.y;
        Color col = (t.type == 0) ? tileColor0 : (t.type == 1 ? tileColor1 : tileColor2);
        col.a = t.alpha;
        for (int i = 1; i < 23; i++)
            for (int j = 1; j < 23; j++)
                SetPixel(ox + i, oy + j, col);
    }

    void DrawDot(Vector2 pos, float size, Color col) {
        for (int i = 0; i < (int)size; i++)
            for (int j = 0; j < (int)size; j++)
                SetPixel((int)pos.x + i, (int)pos.y + j, col);
    }

    void SetPixel(int x, int y, Color col) {
        if (x >= 0 && x < _texW && y >= 0 && y < _texH)
            _pixels[y * _texW + x] = col;
    }

    private void OnDestroy() {
        if (_tex != null) Destroy(_tex); // Evita memory leaks
    }
}