using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class RAMController : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [Range(0f, 1f)]  public float chaos       = 0.25f;
    [Range(0f, 10f)] public float glitchSpeed = 4f;
    public Color tintColor = new Color(0.06f, 0.1f, 0.28f, 1f);

    private List<Vector3Int> _allPositions = new List<Vector3Int>();
    private bool _initialized = false;

    void Update()
    {
        if (!_initialized) Init();
    }

    void Init()
    {
        tilemap.CompressBounds();
        _allPositions.Clear();

        foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            if (tilemap.HasTile(pos))
                _allPositions.Add(pos);

        if (_allPositions.Count == 0) return;

        Debug.Log("Init OK — tile: " + _allPositions.Count);

        foreach (var pos in _allPositions)
            tilemap.SetColor(pos, tintColor);

        _initialized = true;
        StartCoroutine(FadeLoop());
        StartCoroutine(GlitchLoop());
    }

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
        for (float a = 1f; a >= 0f; a -= 0.1f)
        {
            tilemap.SetColor(pos, new Color(tintColor.r*a, tintColor.g*a, tintColor.b*a, 1f));
            yield return new WaitForSecondsRealtime(0.03f);
        }
        yield return new WaitForSecondsRealtime(Random.Range(0.05f, 0.3f));
        for (float a = 0f; a <= 1f; a += 0.1f)
        {
            tilemap.SetColor(pos, new Color(tintColor.r*a, tintColor.g*a, tintColor.b*a, 1f));
            yield return new WaitForSecondsRealtime(0.02f);
        }
        tilemap.SetColor(pos, tintColor);
    }

    IEnumerator GlitchTile(Vector3Int pos)
    {
        int flickers = Random.Range(2, 5);
        for (int i = 0; i < flickers; i++)
        {
            tilemap.SetColor(pos, new Color(tintColor.r*0.2f, tintColor.g*0.2f, tintColor.b*0.2f, 1f));
            yield return new WaitForSecondsRealtime(0.04f);
            tilemap.SetColor(pos, tintColor);
            yield return new WaitForSecondsRealtime(0.04f);
        }
    }
}