using System.Collections.Generic;
using UnityEngine;

public class BitSpawner : MonoBehaviour
{
    [Header("Sprites laser")]
    [Tooltip("Trascina qui direttamente gli sprite — niente prefab")]
    [SerializeField] private Sprite[] bitSprites;

    [Header("Spawning")]
    [SerializeField] private float spawnRate     = 0.12f;
    [SerializeField] private int   maxBits        = 35;
    [SerializeField] private float tubeHalfHeight = 1.6f;
    [SerializeField] private float spawnX         = 12f;
    [SerializeField] private float despawnX       = -12f;

    [Header("Velocità")]
    [SerializeField] private float minSpeed = 4f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Scala")]
    [SerializeField] private float minScale = 0.08f;
    [SerializeField] private float maxScale = 0.18f;

    private readonly List<BitParticle> _bits = new();
    private float _timer = 0f;

    private void Start()
    {
        // Pre-popola il tubo con bit già distribuiti su tutta la larghezza
        // così dall'inizio sembra già a regime
        int preCount = maxBits;
        for (int i = 0; i < preCount; i++)
        {
            // X random su tutto il tubo (da despawnX a spawnX)
            float x = Random.Range(despawnX, spawnX);
            SpawnBitAt(x);
        }

        _timer = spawnRate; // evita burst al primo frame
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnRate && _bits.Count < maxBits)
        {
            _timer = 0f;
            SpawnBitAt(spawnX);
        }

        for (int i = _bits.Count - 1; i >= 0; i--)
        {
            var b = _bits[i];
            if (b.obj == null) { _bits.RemoveAt(i); continue; }

            b.obj.transform.Translate(Vector3.left * b.speed * Time.deltaTime, Space.World);

            if (b.obj.transform.position.x < despawnX)
            {
                Destroy(b.obj);
                _bits.RemoveAt(i);
            }
        }
    }

    private void SpawnBitAt(float x)
    {
        if (bitSprites == null || bitSprites.Length == 0) return;

        float y     = Random.Range(-tubeHalfHeight, tubeHalfHeight);
        float speed = Random.Range(minSpeed, maxSpeed);
        float scale = Random.Range(minScale, maxScale);

        var obj = new GameObject("Bit");
        obj.transform.position   = new Vector3(x, y, 0f);
        obj.transform.rotation   = Quaternion.Euler(0f, 0f, 180f);
        obj.transform.localScale = Vector3.one * scale;

        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite       = bitSprites[Random.Range(0, bitSprites.Length)];
        sr.sortingOrder = 1;

        _bits.Add(new BitParticle { obj = obj, speed = speed });
    }

    private void OnDestroy()
    {
        foreach (var b in _bits)
            if (b.obj != null) Destroy(b.obj);
        _bits.Clear();
    }

    private class BitParticle
    {
        public GameObject obj;
        public float speed;
    }
}