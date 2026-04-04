using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RAMChunkSystem — sfondo RAM 300x300 tile con chunk pooling.
///
/// GERARCHIA OGGETTI DA CREARE IN UNITY:
///   GameObject "RAMSystem" (posizione 0,0,0)
///     └── aggiungi questo script
///
/// SORTING LAYERS (Project Settings → Graphics → Sorting Layers):
///   Background       ← tile base e mattonelle
///   (tutto il gameplay: Default oppure un layer separato con order >= 0)
///
/// I tile base stanno a sortingOrder = -10
/// Le mattonelle fancy stanno a sortingOrder = -9
/// → impossibile che coprano oggetti di gameplay a order >= 0
/// </summary>
public class RAMChunkSystem : MonoBehaviour
{
    // ── Sprites (assegna dall'Inspector) ─────────────────────────────────
    [Header("Sprites — trascina i PNG importati")]
    [Tooltip("tile_base_0 … tile_base_3  (4 sprite)")]
    public Sprite[] BaseSprites;
    [Tooltip("tile_fancy_0 … tile_fancy_7  (8 sprite)")]
    public Sprite[] FancySprites;

    // ── Griglia ───────────────────────────────────────────────────────────
    [Header("Griglia")]
    public int   MapWidth    = 300;
    public int   MapHeight   = 300;
    public int   ChunkSize   = 16;
    [Tooltip("Dimensione tile in unità Unity. Se PPU=16 → 1.0")]
    public float TileWorld   = 1f;
    [Tooltip("Gap tra tile in unità Unity")]
    public float TileGap     = 0.05f;

    // ── Sorting ───────────────────────────────────────────────────────────
    [Header("Sorting")]
    public string SortingLayer = "Background";
    public int    OrderBase    = -10;   // tile base (sotto)
    public int    OrderFancy   = -9;    // mattonelle (sopra base, sotto gameplay)

    // ── Animazione mattonella ─────────────────────────────────────────────
    [Header("Animazione mattonella")]
    [Tooltip("Quante mattonelle possono animarsi contemporaneamente")]
    public int   MaxAnimating   = 4;
    [Tooltip("Altezza sollevamento in unità Unity")]
    public float LiftHeight     = 0.28f;
    [Tooltip("Velocità lerp sollevamento")]
    public float LiftSpeed      = 4f;
    [Tooltip("Secondi che la mattonella resta alzata prima di dissolversi")]
    public float HoldTime       = 0.6f;
    [Tooltip("Secondi della dissolvenza (fade out + fade in nuova)")]
    public float FadeTime       = 0.35f;
    [Tooltip("Secondi tra un'attivazione e la prossima (media)")]
    public float TriggerInterval = 1.2f;

    // ── Camera ────────────────────────────────────────────────────────────
    [Header("Camera")]
    public Transform CamTransform;
    public int       CullBuffer = 1;

    // ── Internals ─────────────────────────────────────────────────────────
    float _step;
    int   _chunksX, _chunksY;
    float _mapOriginX, _mapOriginY;

    Dictionary<Vector2Int, RamChunk> _active = new Dictionary<Vector2Int, RamChunk>();
    Queue<RamChunk>                  _pool   = new Queue<RamChunk>();

    int              _animating = 0;
    List<RamTile>    _candidates = new List<RamTile>();

    void Start()
    {
        if (CamTransform == null) CamTransform = Camera.main.transform;
        _step      = TileWorld + TileGap;
        _chunksX   = Mathf.CeilToInt((float)MapWidth  / ChunkSize);
        _chunksY   = Mathf.CeilToInt((float)MapHeight / ChunkSize);
        _mapOriginX = -MapWidth  * _step * 0.5f;
        _mapOriginY = -MapHeight * _step * 0.5f;

        StartCoroutine(TriggerLoop());
    }

    void Update()
    {
        UpdateChunks();
        UpdateLerp();
    }

    // ════════════════════════════════════════════════════════════════════
    // CHUNK MANAGEMENT
    // ════════════════════════════════════════════════════════════════════

    void UpdateChunks()
    {
        Camera cam   = Camera.main;
        float  camH  = cam.orthographicSize * 2f;
        float  camW  = camH * cam.aspect;
        float  chunkW = ChunkSize * _step;
        Vector3 cp   = CamTransform.position;

        int minCX = Mathf.FloorToInt((cp.x - camW*0.5f - _mapOriginX) / chunkW) - CullBuffer;
        int maxCX = Mathf.FloorToInt((cp.x + camW*0.5f - _mapOriginX) / chunkW) + CullBuffer;
        int minCY = Mathf.FloorToInt((cp.y - camH*0.5f - _mapOriginY) / chunkW) - CullBuffer;
        int maxCY = Mathf.FloorToInt((cp.y + camH*0.5f - _mapOriginY) / chunkW) + CullBuffer;

        minCX = Mathf.Clamp(minCX, 0, _chunksX-1);
        maxCX = Mathf.Clamp(maxCX, 0, _chunksX-1);
        minCY = Mathf.Clamp(minCY, 0, _chunksY-1);
        maxCY = Mathf.Clamp(maxCY, 0, _chunksY-1);

        // Rimuovi chunk fuori vista
        var toRemove = new List<Vector2Int>();
        foreach (var kv in _active)
        {
            var k = kv.Key;
            if (k.x < minCX || k.x > maxCX || k.y < minCY || k.y > maxCY)
            {
                ReturnChunk(kv.Value);
                toRemove.Add(k);
            }
        }
        foreach (var k in toRemove) _active.Remove(k);

        // Aggiungi chunk visibili
        for (int cy = minCY; cy <= maxCY; cy++)
        for (int cx = minCX; cx <= maxCX; cx++)
        {
            var key = new Vector2Int(cx, cy);
            if (!_active.ContainsKey(key))
            {
                var chunk = GetChunk();
                chunk.Setup(cx, cy, _mapOriginX, _mapOriginY, _step, ChunkSize,
                            MapWidth, MapHeight, TileWorld,
                            BaseSprites, FancySprites,
                            SortingLayer, OrderBase, OrderFancy);
                _active[key] = chunk;
            }
        }

        // Aggiorna candidati per animazione
        _candidates.Clear();
        foreach (var kv in _active)
            foreach (var t in kv.Value.Tiles)
                if (t.Fancy != null) _candidates.Add(t);
    }

    void ReturnChunk(RamChunk c)
    {
        // Ferma animazioni in corso su questo chunk
        foreach (var t in c.Tiles)
        {
            if (t.Animating)
            {
                t.Animating = false;
                _animating  = Mathf.Max(0, _animating - 1);
            }
            if (t.Fancy != null)
            {
                var col = t.Fancy.color;
                t.Fancy.color = new Color(col.r, col.g, col.b, 1f);
                t.Fancy.transform.localPosition = t.FancyBaseLocal;
            }
        }
        c.Root.SetActive(false);
        _pool.Enqueue(c);
    }

    RamChunk GetChunk()
    {
        if (_pool.Count > 0)
        {
            var c = _pool.Dequeue();
            c.Root.SetActive(true);
            return c;
        }
        return new RamChunk(ChunkSize, transform);
    }

    // ════════════════════════════════════════════════════════════════════
    // ANIMAZIONE LERP (Update, zero allocazioni)
    // ════════════════════════════════════════════════════════════════════

    void UpdateLerp()
    {
        float dt = Time.deltaTime;
        foreach (var kv in _active)
        foreach (var t in kv.Value.Tiles)
        {
            if (!t.NeedsLerp || t.Fancy == null) continue;

            t.Fancy.transform.localPosition = Vector3.Lerp(
                t.Fancy.transform.localPosition, t.LerpTarget, dt * LiftSpeed);

            if (Vector3.Distance(t.Fancy.transform.localPosition, t.LerpTarget) < 0.003f)
            {
                t.Fancy.transform.localPosition = t.LerpTarget;
                t.NeedsLerp = false;
                t.LerpDone  = true;
            }
        }
    }

    // ════════════════════════════════════════════════════════════════════
    // TRIGGER LOOP — sceglie tile da animare
    // ════════════════════════════════════════════════════════════════════

    IEnumerator TriggerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(TriggerInterval * Random.Range(0.7f, 1.3f));

            if (_animating >= MaxAnimating || _candidates.Count == 0) continue;

            // Scegli tile idle a caso
            int tries = 0;
            while (tries < 30)
            {
                var tile = _candidates[Random.Range(0, _candidates.Count)];
                if (!tile.Animating)
                {
                    StartCoroutine(AnimateTile(tile));
                    break;
                }
                tries++;
            }
        }
    }

    IEnumerator AnimateTile(RamTile t)
    {
        if (t.Animating || t.Fancy == null) yield break;
        t.Animating = true;
        _animating++;

        SpriteRenderer fancy = t.Fancy;

        // 1. LIFT (sale)
        t.LerpTarget = t.FancyBaseLocal + Vector3.up * LiftHeight;
        t.LerpDone   = false;
        t.NeedsLerp  = true;
        while (!t.LerpDone) yield return null;

        // 2. HOLD
        yield return new WaitForSeconds(HoldTime);

        // 3. FADE OUT
        float elapsed = 0f;
        Color startCol = fancy.color;
        while (elapsed < FadeTime)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Clamp01(1f - elapsed / FadeTime);
            fancy.color = new Color(startCol.r, startCol.g, startCol.b, a);
            yield return null;
        }
        fancy.color = new Color(startCol.r, startCol.g, startCol.b, 0f);

        // 4. SWAP sprite + reset posizione (invisibile)
        fancy.transform.localPosition = t.FancyBaseLocal;
        fancy.sprite = FancySprites[Random.Range(0, FancySprites.Length)];

        // Piccola pausa (slot vuoto visibile)
        yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));

        // 5. FADE IN nuova mattonella
        elapsed = 0f;
        while (elapsed < FadeTime)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Clamp01(elapsed / FadeTime);
            fancy.color = new Color(startCol.r, startCol.g, startCol.b, a);
            yield return null;
        }
        fancy.color = new Color(startCol.r, startCol.g, startCol.b, 1f);

        t.Animating = false;
        _animating  = Mathf.Max(0, _animating - 1);
    }
}

// ════════════════════════════════════════════════════════════════════════
// CHUNK
// ════════════════════════════════════════════════════════════════════════

public class RamChunk
{
    public GameObject Root;
    public RamTile[]  Tiles;
    int _size;

    public RamChunk(int size, Transform parent)
    {
        _size = size;
        Root  = new GameObject("Chunk");
        Root.transform.SetParent(parent, false);
        Tiles = new RamTile[size * size];
        for (int i = 0; i < Tiles.Length; i++)
            Tiles[i] = new RamTile(Root.transform);
    }

    public void Setup(int cx, int cy,
                      float mapOX, float mapOY, float step, int chunkSize,
                      int mapW, int mapH, float tileWorld,
                      Sprite[] baseSprites, Sprite[] fancySprites,
                      string sortLayer, int orderBase, int orderFancy)
    {
        float chunkWorld = chunkSize * step;
        int idx = 0;
        for (int ty = 0; ty < _size; ty++)
        for (int tx = 0; tx < _size; tx++)
        {
            int gx = cx * chunkSize + tx;
            int gy = cy * chunkSize + ty;
            var t  = Tiles[idx++];
            bool inMap = gx < mapW && gy < mapH;
            t.Base.gameObject.SetActive(inMap);
            if (t.Fancy != null) t.Fancy.gameObject.SetActive(inMap);
            if (!inMap) continue;

            Vector3 wp = new Vector3(
                mapOX + gx * step,
                mapOY + gy * step, 0f);

            // BASE
            t.Base.transform.position = wp;
            t.Base.sprite             = baseSprites[UnityEngine.Random.Range(0, baseSprites.Length)];
            t.Base.sortingLayerName   = sortLayer;
            t.Base.sortingOrder       = orderBase;
            t.Base.drawMode           = SpriteDrawMode.Sliced;
            t.Base.size               = new Vector2(tileWorld, tileWorld);
            t.Base.color              = Color.white;

            // FANCY (mattonella sopra)
            if (t.Fancy != null)
            {
                t.Fancy.transform.position = wp;
                t.Fancy.sprite             = fancySprites[UnityEngine.Random.Range(0, fancySprites.Length)];
                t.Fancy.sortingLayerName   = sortLayer;
                t.Fancy.sortingOrder       = orderFancy;
                t.Fancy.drawMode           = SpriteDrawMode.Sliced;
                t.Fancy.size               = new Vector2(tileWorld, tileWorld);
                t.Fancy.color              = Color.white;
                t.FancyBaseLocal           = t.Fancy.transform.localPosition;
                t.LerpTarget               = t.FancyBaseLocal;
            }
            t.Animating = false;
            t.NeedsLerp = false;
            t.LerpDone  = true;
        }
    }
}

// ════════════════════════════════════════════════════════════════════════
// TILE DATA
// ════════════════════════════════════════════════════════════════════════

public class RamTile
{
    public SpriteRenderer Base;
    public SpriteRenderer Fancy;      // null se non ci sono FancySprites
    public Vector3        FancyBaseLocal;
    public Vector3        LerpTarget;
    public bool           Animating;
    public bool           NeedsLerp;
    public bool           LerpDone = true;

    public RamTile(Transform parent)
    {
        var goBase = new GameObject("B");
        goBase.transform.SetParent(parent, false);
        Base = goBase.AddComponent<SpriteRenderer>();

        var goFancy = new GameObject("F");
        goFancy.transform.SetParent(parent, false);
        Fancy = goFancy.AddComponent<SpriteRenderer>();
    }
}
