using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RAMBackground2 : MonoBehaviour
{
    [Header("Grid")]
    public Sprite TileSprite;
    public int   GridColumns = 40;
    public int   GridRows    = 25;
    public float TileSize    = 1f;
    public float TileGap     = 0.05f; // Un gap piccolo crea l'effetto contorno

    [Header("Colors")]
    public Color[] BaseColors = new Color[] {
        new Color(0.04f, 0.05f, 0.09f, 0.92f),
        new Color(0.055f, 0.067f, 0.118f, 0.92f),
        new Color(0.047f, 0.059f, 0.102f, 0.92f)
    };
    public Color ColorAllocated = new Color(0.07f, 0.15f, 0.30f, 1.00f);
    
    // NUOVO: Colore del contorno per staccare i tile
    public Color BorderColor = new Color(0.1f, 0.15f, 0.25f, 0.5f);

    [Header("Sorting")]
    public string SortingLayerName = "Default";
    public int BaseSortingOrder = -2;

    [Header("Lift Animation (FREQUENZA ALTA)")]
    public int MaxLiftedAtOnce = 8;        // Aumentato (prima era 3)
    public float LiftHeight   = 0.35f;
    public float LiftSpeed    = 5.0f;       // Più veloce a salire
    public float HoldDuration = 0.8f;       // Resta su meno tempo
    public float CooldownMin  = 0.5f;       // Quasi nessun attesa
    public float CooldownMax  = 2.0f;       // Molto più frequente

    private RamTile[] _tiles;
    private int _liftedCount = 0;

    void Start()
    {
        float step = TileSize + TileGap;
        float ox = -(GridColumns * step) * 0.5f + step * 0.5f;
        float oy = -(GridRows    * step) * 0.5f + step * 0.5f;

        _tiles = new RamTile[GridColumns * GridRows];

        for (int r = 0; r < GridRows; r++)
        for (int c = 0; c < GridColumns; c++)
        {
            int idx = r * GridColumns + c;
            Vector3 baseLocal = new Vector3(ox + c * step, oy + r * step, 0f);

            // CREIAMO IL TILE
            var go = new GameObject($"T_{r}_{c}");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = baseLocal;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = TileSprite;
            
            Color randomBaseColor = BaseColors[Random.Range(0, BaseColors.Length)];
            sr.color = randomBaseColor;
            sr.sortingLayerName = SortingLayerName;
            sr.sortingOrder = BaseSortingOrder;
            
            // Impostiamo il quadratino
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(TileSize, TileSize);

            // NUOVO: Aggiungiamo un piccolo "bordo" interno creando un figlio
            var borderGO = new GameObject("Border");
            borderGO.transform.SetParent(go.transform, false);
            borderGO.transform.localPosition = Vector3.zero;
            var borderSR = borderGO.AddComponent<SpriteRenderer>();
            borderSR.sprite = TileSprite;
            borderSR.color = BorderColor;
            borderSR.sortingOrder = BaseSortingOrder - 1; // Appena sotto
            // Rendiamo il bordo leggermente più grande del tile
            borderSR.size = new Vector2(TileSize + 0.05f, TileSize + 0.05f);

            _tiles[idx] = new RamTile { 
                GO = go, 
                SR = sr, 
                BaseLocal = baseLocal, 
                OriginalColor = randomBaseColor 
            };

            StartCoroutine(TileLifecycle(_tiles[idx], Random.Range(0f, 2f)));
        }
    }

    void Update()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            var t = _tiles[i];
            if (!t.NeedsLerp) continue;

            t.GO.transform.localPosition = Vector3.Lerp(
                t.GO.transform.localPosition, t.TargetLocal, Time.deltaTime * LiftSpeed);
            t.SR.color = Color.Lerp(
                t.SR.color, t.TargetColor, Time.deltaTime * LiftSpeed * 1.5f);

            if (Vector3.Distance(t.GO.transform.localPosition, t.TargetLocal) < 0.001f)
            {
                t.GO.transform.localPosition = t.TargetLocal;
                t.SR.color = t.TargetColor;
                t.NeedsLerp = false;
                t.LerpDone = true;
            }
        }
    }

    private IEnumerator TileLifecycle(RamTile t, float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            while (_liftedCount >= MaxLiftedAtOnce)
                yield return new WaitForSeconds(0.1f);

            _liftedCount++;
            t.TargetLocal = t.BaseLocal + Vector3.up * LiftHeight;
            t.TargetColor = ColorAllocated;
            t.LerpDone = false;
            t.NeedsLerp = true;
            
            while (!t.LerpDone) yield return null;

            yield return new WaitForSeconds(HoldDuration + Random.Range(-0.1f, 0.3f));

            t.TargetLocal = t.BaseLocal;
            t.TargetColor = t.OriginalColor;
            t.LerpDone = false;
            t.NeedsLerp = true;
            
            while (!t.LerpDone) yield return null;

            _liftedCount = Mathf.Max(0, _liftedCount - 1);

            yield return new WaitForSeconds(Random.Range(CooldownMin, CooldownMax));
        }
    }

    private class RamTile
    {
        public GameObject GO;
        public SpriteRenderer SR;
        public Vector3 BaseLocal;
        public Vector3 TargetLocal;
        public Color TargetColor;
        public Color OriginalColor;
        public bool NeedsLerp;
        public bool LerpDone;
    }
}