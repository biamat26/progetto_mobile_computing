using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RAMChunkSystem v6 — riscrittura completa
/// 
/// Cambiamenti chiave rispetto alle versioni precedenti:
/// - Ogni SpriteRenderer ha transform.localScale = Vector3.one * TileWorld
///   invece di drawMode Sliced/Simple — funziona sempre, indipendente dal PPU
/// - I chunk non hanno più transform intermedi che possono causare offset
/// - Il lift è testato con Debug.Log espliciti
/// - Se vedi "[RAM] LIFT START" in Console ma non vedi il tile muoversi,
///   significa che il tile è coperto da qualcosa sopra
///
/// SETUP RAPIDO:
///   1. RAMSystem (0,0,0) → RAMChunkSystem
///   2. BaseSprites  → 4 sprite tile_base_*
///   3. FancySprites → 8 sprite tile_fancy_*
///   4. CamTransform → Main Camera
///   5. Tutti i PNG importati con PPU=16, Filter=Point, Compression=None
///   6. SortingLayer "Background" in Project Settings → Graphics → Sorting Layers
///   7. Player SortingOrder >= 0  (i tile fancy stanno a -9, base a -10)
/// </summary>
public class RAMChunkSystem : MonoBehaviour
{
    [Header("Sprites (PPU=16, Filter=Point)")]
    public Sprite[] BaseSprites;
    public Sprite[] FancySprites;

    [Header("Griglia")]
    public int   MapWidth   = 300;
    public int   MapHeight  = 300;
    public int   ChunkSize  = 16;
    [Tooltip("Lascia 1.0 se PPU=16 e il tuo tilemap usa tile 16px = 1 unità Unity")]
    public float TileWorld  = 1f;

    [Header("Sorting — Background deve esistere in Project Settings → Graphics")]
    public string SortingLayer = "Background";
    public int    OrderBase    = -100;
    public int    OrderFancy   = -99;

    [Header("Animazione lift")]
    public int   MaxAnimating    = 4;
    [Tooltip("Quante unità Unity si alza il tile. 1.5 = 1.5 tile in su")]
    public float LiftHeight      = 1.5f;
    public float LiftSpeed       = 4f;
    public float HoldTime        = 0.7f;
    public float FadeTime        = 0.3f;
    public float TriggerInterval = 1.5f;

    [Header("Camera")]
    public Transform CamTransform;
    public int       CullBuffer = 1;

    // ─────────────────────────────────────────────────────────────────────
    float _mapOX, _mapOY;
    int   _chunksX, _chunksY;

    // Chunk pool
    readonly Dictionary<Vector2Int, ChunkData> _active = new Dictionary<Vector2Int, ChunkData>();
    readonly Queue<ChunkData>                  _pool   = new Queue<ChunkData>();

    // Lift state
    int           _animating  = 0;
    List<TileGO>  _candidates = new List<TileGO>();

    // ─────────────────────────────────────────────────────────────────────
    void Start()
    {
        if (CamTransform == null) CamTransform = Camera.main.transform;

        // Controlla sorting layer
        bool ok = false;
        foreach (var sl in UnityEngine.SortingLayer.layers)
            if (sl.name == SortingLayer) { ok = true; break; }
        if (!ok)
        {
            Debug.LogError($"[RAM] Sorting Layer '{SortingLayer}' NON ESISTE. " +
                           "Aprì Project Settings → Graphics → Sorting Layers e aggiungilo!");
            SortingLayer = "Default";
        }

        if (BaseSprites  == null || BaseSprites.Length  == 0) { Debug.LogError("[RAM] BaseSprites vuoto!");  enabled = false; return; }
        if (FancySprites == null || FancySprites.Length == 0) { Debug.LogError("[RAM] FancySprites vuoto!"); enabled = false; return; }

        _mapOX   = -MapWidth  * TileWorld * 0.5f;
        _mapOY   = -MapHeight * TileWorld * 0.5f;
        _chunksX = Mathf.CeilToInt((float)MapWidth  / ChunkSize);
        _chunksY = Mathf.CeilToInt((float)MapHeight / ChunkSize);

        Debug.Log($"[RAM] OK — mappa {MapWidth}x{MapHeight}, TileWorld={TileWorld}, " +
                  $"LiftHeight={LiftHeight}, Layer={SortingLayer} (order base={OrderBase} fancy={OrderFancy})");

        StartCoroutine(LiftLoop());
    }

    void Update()
    {
        ManageChunks();
        DoLiftLerp();
    }

    // ── Chunk management ─────────────────────────────────────────────────

    void ManageChunks()
    {
        Camera  cam  = Camera.main;
        float   camH = cam.orthographicSize * 2f;
        float   camW = camH * cam.aspect;
        float   cw   = ChunkSize * TileWorld;
        Vector3 cp   = CamTransform.position;

        int minCX = Mathf.Clamp(Mathf.FloorToInt((cp.x - camW*.5f - _mapOX)/cw) - CullBuffer, 0, _chunksX-1);
        int maxCX = Mathf.Clamp(Mathf.FloorToInt((cp.x + camW*.5f - _mapOX)/cw) + CullBuffer, 0, _chunksX-1);
        int minCY = Mathf.Clamp(Mathf.FloorToInt((cp.y - camH*.5f - _mapOY)/cw) - CullBuffer, 0, _chunksY-1);
        int maxCY = Mathf.Clamp(Mathf.FloorToInt((cp.y + camH*.5f - _mapOY)/cw) + CullBuffer, 0, _chunksY-1);

        // Rimuovi chunk fuori vista
        var remove = new List<Vector2Int>();
        foreach (var kv in _active)
        {
            var k = kv.Key;
            if (k.x < minCX || k.x > maxCX || k.y < minCY || k.y > maxCY)
            { PoolChunk(kv.Value); remove.Add(k); }
        }
        foreach (var k in remove) _active.Remove(k);

        // Aggiungi chunk mancanti
        for (int cy = minCY; cy <= maxCY; cy++)
        for (int cx = minCX; cx <= maxCX; cx++)
        {
            var key = new Vector2Int(cx, cy);
            if (_active.ContainsKey(key)) continue;
            var chunk = GetChunk();
            PlaceChunk(chunk, cx, cy);
            _active[key] = chunk;
        }

        // Raccoglie candidati lift (solo tile non animati)
        _candidates.Clear();
        foreach (var kv in _active)
            foreach (var t in kv.Value.Tiles)
                if (!t.Animating) _candidates.Add(t);
    }

    void PlaceChunk(ChunkData chunk, int cx, int cy)
    {
        int idx = 0;
        for (int ty = 0; ty < ChunkSize; ty++)
        for (int tx = 0; tx < ChunkSize; tx++)
        {
            int gx = cx * ChunkSize + tx;
            int gy = cy * ChunkSize + ty;
            var t  = chunk.Tiles[idx++];

            bool inMap = gx < MapWidth && gy < MapHeight;
            t.BaseGO.SetActive(inMap);
            t.FancyGO.SetActive(inMap);
            if (!inMap) continue;

            // Posizione world esatta
            float wx = _mapOX + gx * TileWorld + TileWorld * 0.5f;
            float wy = _mapOY + gy * TileWorld + TileWorld * 0.5f;
            Vector3 wp = new Vector3(wx, wy, 0f);

            // BASE
            t.BaseGO.transform.position   = wp;
            t.BaseSR.sprite               = BaseSprites[Random.Range(0, BaseSprites.Length)];
            t.BaseSR.sortingLayerName     = SortingLayer;
            t.BaseSR.sortingOrder         = OrderBase;
            t.BaseSR.color                = Color.white;

            // FANCY
            t.FancyGO.transform.position  = wp;
            t.FancySR.sprite              = FancySprites[Random.Range(0, FancySprites.Length)];
            t.FancySR.sortingLayerName    = SortingLayer;
            t.FancySR.sortingOrder        = OrderFancy;
            t.FancySR.color               = Color.white;

            t.HomePos   = wp;
            t.Animating = false;
            t.NeedsLerp = false;
            t.LerpDone  = true;
            t.LiftPos   = wp;
        }
    }

    void PoolChunk(ChunkData c)
    {
        foreach (var t in c.Tiles)
        {
            if (t.Animating) { t.Animating = false; _animating = Mathf.Max(0, _animating-1); }
            t.FancyGO.transform.position = t.HomePos;
            t.FancySR.color = Color.white;
            t.BaseGO.SetActive(false);
            t.FancyGO.SetActive(false);
            t.NeedsLerp = false;
            t.LerpDone  = true;
        }
        _pool.Enqueue(c);
    }

    ChunkData GetChunk()
    {
        if (_pool.Count > 0) return _pool.Dequeue();
        return CreateChunk();
    }

    ChunkData CreateChunk()
    {
        var chunk = new ChunkData();
        chunk.Tiles = new TileGO[ChunkSize * ChunkSize];
        for (int i = 0; i < chunk.Tiles.Length; i++)
        {
            // BASE GO
            var gb = new GameObject("B");
            gb.transform.SetParent(transform, false);
            var srb = gb.AddComponent<SpriteRenderer>();
            // Scala il GO invece di usare drawMode — funziona con qualsiasi PPU
            gb.transform.localScale = new Vector3(TileWorld, TileWorld, 1f);
            gb.SetActive(false);

            // FANCY GO
            var gf = new GameObject("F");
            gf.transform.SetParent(transform, false);
            var srf = gf.AddComponent<SpriteRenderer>();
            gf.transform.localScale = new Vector3(TileWorld, TileWorld, 1f);
            gf.SetActive(false);

            chunk.Tiles[i] = new TileGO
            {
                BaseGO  = gb, BaseSR  = srb,
                FancyGO = gf, FancySR = srf
            };
        }
        return chunk;
    }

    // ── Lerp (world position) ─────────────────────────────────────────────

    void DoLiftLerp()
    {
        float dt = Time.deltaTime;
        foreach (var kv in _active)
        foreach (var t in kv.Value.Tiles)
        {
            if (!t.NeedsLerp) continue;
            t.FancyGO.transform.position = Vector3.Lerp(
                t.FancyGO.transform.position, t.LiftPos, dt * LiftSpeed);
            if (Vector3.Distance(t.FancyGO.transform.position, t.LiftPos) < 0.005f)
            {
                t.FancyGO.transform.position = t.LiftPos;
                t.NeedsLerp = false;
                t.LerpDone  = true;
            }
        }
    }

    // ── Lift loop ─────────────────────────────────────────────────────────

    IEnumerator LiftLoop()
    {
        yield return null; // aspetta un frame
        int triggered = 0;
        while (true)
        {
            yield return new WaitForSeconds(TriggerInterval * Random.Range(0.5f, 1.5f));
            if (_animating >= MaxAnimating || _candidates.Count == 0) continue;

            var tile = _candidates[Random.Range(0, _candidates.Count)];
            if (!tile.Animating)
            {
                triggered++;
                Debug.Log($"[RAM] Lift #{triggered} — tile @ {tile.HomePos}");
                StartCoroutine(AnimateTile(tile));
            }
        }
    }

    IEnumerator AnimateTile(TileGO t)
    {
        if (t.Animating) yield break;
        t.Animating = true;
        _animating++;

        var sr = t.FancySR;

        // LIFT
        t.LiftPos  = t.HomePos + Vector3.up * LiftHeight;
        t.LerpDone = false;
        t.NeedsLerp = true;
        while (!t.LerpDone) yield return null;

        // HOLD
        yield return new WaitForSeconds(HoldTime);

        // FADE OUT
        float e = 0f;
        while (e < FadeTime)
        { e += Time.deltaTime; sr.color = new Color(1,1,1, Mathf.Clamp01(1f-e/FadeTime)); yield return null; }
        sr.color = new Color(1,1,1,0);

        // SWAP + reset
        t.FancyGO.transform.position = t.HomePos;
        sr.sprite = FancySprites[Random.Range(0, FancySprites.Length)];
        yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));

        // FADE IN
        e = 0f;
        while (e < FadeTime)
        { e += Time.deltaTime; sr.color = new Color(1,1,1, Mathf.Clamp01(e/FadeTime)); yield return null; }
        sr.color = Color.white;

        t.Animating = false;
        _animating  = Mathf.Max(0, _animating-1);
        Debug.Log($"[RAM] Lift completato @ {t.HomePos}");
    }
}

// ════════════════════════════════════════════════════════════════════════

class ChunkData
{
    public TileGO[] Tiles;
}

class TileGO
{
    public GameObject     BaseGO,  FancyGO;
    public SpriteRenderer BaseSR,  FancySR;
    public Vector3        HomePos;   // posizione a riposo (world)
    public Vector3        LiftPos;   // posizione sollevata (world)
    public bool           Animating;
    public bool           NeedsLerp;
    public bool           LerpDone = true;
}