using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RAMChunkSystem v4
/// Fix: aggiunto debug log per verificare che il lift funzioni,
/// e fallback automatico se il SortingLayer non esiste.
/// </summary>
public class RAMChunkSystem : MonoBehaviour
{
    [Header("Sprites")]
    [Tooltip("4 sprite: tile_base_0 … tile_base_3")]
    public Sprite[] BaseSprites;
    [Tooltip("8 sprite: tile_fancy_0 … tile_fancy_7")]
    public Sprite[] FancySprites;

    [Header("Griglia")]
    public int   MapWidth   = 300;
    public int   MapHeight  = 300;
    public int   ChunkSize  = 16;
    public float TileWorld  = 1f;
    public float TileGap    = 0f;

    [Header("Sorting")]
    [Tooltip("Deve esistere in Project Settings → Graphics → Sorting Layers")]
    public string SortingLayer = "Background";
    public int    OrderBase    = -10;
    public int    OrderFancy   = -9;

    [Header("Animazione mattonella")]
    public int   MaxAnimating    = 4;
    [Tooltip("Unità Unity di sollevamento. Con PPU=16 e TileWorld=1, usa 1.5")]
    public float LiftHeight      = 1.5f;
    public float LiftSpeed       = 5f;
    public float HoldTime        = 0.5f;
    public float FadeTime        = 0.3f;
    [Tooltip("Secondi medi tra un lift e il prossimo")]
    public float TriggerInterval = 1.0f;

    [Header("Camera")]
    public Transform CamTransform;
    public int       CullBuffer = 1;

    // ── internals ────────────────────────────────────────────────────────
    float _step;
    int   _chunksX, _chunksY;
    float _mapOX, _mapOY;

    readonly Dictionary<Vector2Int, RamChunk> _active = new Dictionary<Vector2Int, RamChunk>();
    readonly Queue<RamChunk>                  _pool   = new Queue<RamChunk>();

    int            _animating  = 0;
    List<RamTile>  _candidates = new List<RamTile>();

    void Start()
    {
        if (CamTransform == null) CamTransform = Camera.main.transform;

        // Verifica che il sorting layer esista — fallback a "Default"
        bool layerExists = false;
        foreach (var sl in UnityEngine.SortingLayer.layers)
            if (sl.name == SortingLayer) { layerExists = true; break; }
        if (!layerExists)
        {
            Debug.LogWarning($"[RAMChunkSystem] Sorting Layer '{SortingLayer}' non trovato! " +
                             "Aggiungilo in Project Settings → Graphics → Sorting Layers. " +
                             "Uso 'Default' come fallback.");
            SortingLayer = "Default";
        }

        if (BaseSprites  == null || BaseSprites.Length  == 0) { Debug.LogError("[RAMChunkSystem] BaseSprites è vuoto!"); return; }
        if (FancySprites == null || FancySprites.Length == 0) { Debug.LogError("[RAMChunkSystem] FancySprites è vuoto!"); return; }

        _step    = TileWorld + TileGap;
        _chunksX = Mathf.CeilToInt((float)MapWidth  / ChunkSize);
        _chunksY = Mathf.CeilToInt((float)MapHeight / ChunkSize);
        _mapOX   = -MapWidth  * _step * 0.5f;
        _mapOY   = -MapHeight * _step * 0.5f;

        Debug.Log($"[RAMChunkSystem] Avviato. Mappa {MapWidth}x{MapHeight}, step={_step}, " +
                  $"LiftHeight={LiftHeight}, Layer='{SortingLayer}'");

        StartCoroutine(TriggerLoop());
    }

    void Update()
    {
        UpdateChunks();
        UpdateLerp();
    }

    // ── Chunk visibility ─────────────────────────────────────────────────

    void UpdateChunks()
    {
        Camera  cam  = Camera.main;
        float   camH = cam.orthographicSize * 2f;
        float   camW = camH * cam.aspect;
        float   cw   = ChunkSize * _step;
        Vector3 cp   = CamTransform.position;

        int minCX = Mathf.Clamp(Mathf.FloorToInt((cp.x - camW*.5f - _mapOX)/cw) - CullBuffer, 0, _chunksX-1);
        int maxCX = Mathf.Clamp(Mathf.FloorToInt((cp.x + camW*.5f - _mapOX)/cw) + CullBuffer, 0, _chunksX-1);
        int minCY = Mathf.Clamp(Mathf.FloorToInt((cp.y - camH*.5f - _mapOY)/cw) - CullBuffer, 0, _chunksY-1);
        int maxCY = Mathf.Clamp(Mathf.FloorToInt((cp.y + camH*.5f - _mapOY)/cw) + CullBuffer, 0, _chunksY-1);

        var toRemove = new List<Vector2Int>();
        foreach (var kv in _active)
        {
            var k = kv.Key;
            if (k.x < minCX || k.x > maxCX || k.y < minCY || k.y > maxCY)
            { ReturnChunk(kv.Value); toRemove.Add(k); }
        }
        foreach (var k in toRemove) _active.Remove(k);

        for (int cy = minCY; cy <= maxCY; cy++)
        for (int cx = minCX; cx <= maxCX; cx++)
        {
            var key = new Vector2Int(cx, cy);
            if (_active.ContainsKey(key)) continue;
            var chunk = GetChunk();
            chunk.Setup(cx, cy, _mapOX, _mapOY, _step, ChunkSize,
                        MapWidth, MapHeight, TileWorld,
                        BaseSprites, FancySprites,
                        SortingLayer, OrderBase, OrderFancy);
            _active[key] = chunk;
        }

        _candidates.Clear();
        foreach (var kv in _active)
            foreach (var t in kv.Value.Tiles)
                if (t.Fancy != null) _candidates.Add(t);
    }

    void ReturnChunk(RamChunk c)
    {
        foreach (var t in c.Tiles)
        {
            if (t.Animating) { t.Animating = false; _animating = Mathf.Max(0, _animating-1); }
            if (t.Fancy != null)
            {
                t.Fancy.color = Color.white;
                t.Fancy.transform.localPosition = t.FancyBase;
            }
            t.NeedsLerp = false;
            t.LerpDone  = true;
        }
        c.Root.SetActive(false);
        _pool.Enqueue(c);
    }

    RamChunk GetChunk()
    {
        if (_pool.Count > 0) { var c = _pool.Dequeue(); c.Root.SetActive(true); return c; }
        return new RamChunk(ChunkSize, transform);
    }

    // ── Lerp ─────────────────────────────────────────────────────────────

    void UpdateLerp()
    {
        float dt = Time.deltaTime;
        foreach (var kv in _active)
        foreach (var t in kv.Value.Tiles)
        {
            if (!t.NeedsLerp || t.Fancy == null) continue;
            t.Fancy.transform.localPosition = Vector3.Lerp(
                t.Fancy.transform.localPosition, t.LerpTarget, dt * LiftSpeed);
            if (Vector3.Distance(t.Fancy.transform.localPosition, t.LerpTarget) < 0.005f)
            {
                t.Fancy.transform.localPosition = t.LerpTarget;
                t.NeedsLerp = false;
                t.LerpDone  = true;
            }
        }
    }

    // ── Trigger loop ─────────────────────────────────────────────────────

    IEnumerator TriggerLoop()
    {
        // Aspetta un frame perché i chunk vengano creati
        yield return null;

        while (true)
        {
            yield return new WaitForSeconds(TriggerInterval * Random.Range(0.6f, 1.4f));

            if (_candidates.Count == 0) { Debug.Log("[RAMChunkSystem] Nessun candidato per lift."); continue; }
            if (_animating >= MaxAnimating) continue;

            bool found = false;
            for (int attempt = 0; attempt < 30; attempt++)
            {
                var tile = _candidates[Random.Range(0, _candidates.Count)];
                if (!tile.Animating) { StartCoroutine(AnimateTile(tile)); found = true; break; }
            }
            if (!found) Debug.Log("[RAMChunkSystem] Tutti i candidati sono già in animazione.");
        }
    }

    IEnumerator AnimateTile(RamTile t)
    {
        if (t.Animating || t.Fancy == null) yield break;
        t.Animating = true;
        _animating++;

        var sr = t.Fancy;

        // LIFT
        t.LerpTarget = t.FancyBase + Vector3.up * LiftHeight;
        t.LerpDone   = false;
        t.NeedsLerp  = true;
        while (!t.LerpDone) yield return null;

        // HOLD
        yield return new WaitForSeconds(HoldTime);

        // FADE OUT
        float e = 0f;
        while (e < FadeTime)
        {
            e += Time.deltaTime;
            sr.color = new Color(1f, 1f, 1f, Mathf.Clamp01(1f - e / FadeTime));
            yield return null;
        }
        sr.color = new Color(1f, 1f, 1f, 0f);

        // SWAP
        sr.transform.localPosition = t.FancyBase;
        sr.sprite = FancySprites[Random.Range(0, FancySprites.Length)];
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));

        // FADE IN
        e = 0f;
        while (e < FadeTime)
        {
            e += Time.deltaTime;
            sr.color = new Color(1f, 1f, 1f, Mathf.Clamp01(e / FadeTime));
            yield return null;
        }
        sr.color = Color.white;

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
    readonly int _size;

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

            Vector3 wp = new Vector3(mapOX + gx * step, mapOY + gy * step, 0f);

            t.Base.transform.position = wp;
            t.Base.sprite             = baseSprites[Random.Range(0, baseSprites.Length)];
            t.Base.sortingLayerName   = sortLayer;
            t.Base.sortingOrder       = orderBase;
            t.Base.drawMode           = SpriteDrawMode.Sliced;
            t.Base.size               = new Vector2(tileWorld, tileWorld);
            t.Base.color              = Color.white;

            if (t.Fancy != null)
            {
                t.Fancy.transform.position = wp;
                t.Fancy.sprite             = fancySprites[Random.Range(0, fancySprites.Length)];
                t.Fancy.sortingLayerName   = sortLayer;
                t.Fancy.sortingOrder       = orderFancy;
                t.Fancy.drawMode           = SpriteDrawMode.Sliced;
                t.Fancy.size               = new Vector2(tileWorld, tileWorld);
                t.Fancy.color              = Color.white;
                t.FancyBase                = t.Fancy.transform.localPosition;
                t.LerpTarget               = t.FancyBase;
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
    public SpriteRenderer Fancy;
    public Vector3        FancyBase;
    public Vector3        LerpTarget;
    public bool           Animating;
    public bool           NeedsLerp;
    public bool           LerpDone = true;

    public RamTile(Transform parent)
    {
        var gb = new GameObject("B");
        gb.transform.SetParent(parent, false);
        Base = gb.AddComponent<SpriteRenderer>();

        var gf = new GameObject("F");
        gf.transform.SetParent(parent, false);
        Fancy = gf.AddComponent<SpriteRenderer>();
    }
}