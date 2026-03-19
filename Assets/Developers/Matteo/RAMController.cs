using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class RAMController : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [Range(0f, 1f)]  public float chaos       = 0.25f;
    [Range(0f, 10f)] public float glitchSpeed = 4f;

    private List<Vector3Int> _allPositions = new List<Vector3Int>();


    // fade lento — tile che spariscono e riappaiono
    IEnumerator FadeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f / Mathf.Max(glitchSpeed, 0.1f));

            int count = Mathf.Max(1, Mathf.RoundToInt(_allPositions.Count * chaos * 0.1f));
            for (int i = 0; i < count; i++)
            {
                var pos = _allPositions[Random.Range(0, _allPositions.Count)];
                StartCoroutine(FadeTile(pos));
            }
        }
    }

    // glitch rapido — tile che flickerano velocemente
    IEnumerator GlitchLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.05f / Mathf.Max(glitchSpeed, 0.1f));

            if (Random.value < chaos * 0.3f)
            {
                var pos = _allPositions[Random.Range(0, _allPositions.Count)];
                StartCoroutine(GlitchTile(pos));
            }
        }
    }

    IEnumerator FadeTile(Vector3Int pos)
    {
        // fade out
        for (float a = 1f; a >= 0f; a -= 0.1f)
        {
            tilemap.SetColor(pos, new Color(1, 1, 1, a));
            yield return new WaitForSecondsRealtime(0.03f);
        }

        // pausa
        yield return new WaitForSecondsRealtime(Random.Range(0.05f, 0.3f));

        // fade in
        for (float a = 0f; a <= 1f; a += 0.1f)
        {
            tilemap.SetColor(pos, new Color(1, 1, 1, a));
            yield return new WaitForSecondsRealtime(0.02f);
        }

        tilemap.SetColor(pos, Color.white);
    }

    IEnumerator GlitchTile(Vector3Int pos)
    {
        Color original = tilemap.GetColor(pos);

        // flicker 2-4 volte rapidamente
        int flickers = Random.Range(2, 5);
        for (int i = 0; i < flickers; i++)
        {
            tilemap.SetColor(pos, new Color(1, 1, 1, Random.Range(0.1f, 0.4f)));
            yield return new WaitForSecondsRealtime(0.04f);
            tilemap.SetColor(pos, original);
            yield return new WaitForSecondsRealtime(0.04f);
        }
    }

void Start()
{
    // cerca il Tilemap su questo GameObject o sui figli
    if (tilemap == null)
        tilemap = GetComponentInChildren<Tilemap>();

    // se ancora null, cerca nel parent
    if (tilemap == null)
        tilemap = GetComponentInParent<Tilemap>();

    if (tilemap == null)
    {
        Debug.LogError("Tilemap non trovata!");
        return;
    }

    // forza il refresh dei bounds
    tilemap.CompressBounds();

    foreach (var pos in tilemap.cellBounds.allPositionsWithin)
        if (tilemap.HasTile(pos))
            _allPositions.Add(pos);

    Debug.Log("Tile trovati: " + _allPositions.Count);

    if (_allPositions.Count > 0)
    {
        StartCoroutine(FadeLoop());
        StartCoroutine(GlitchLoop());
    }
}
}