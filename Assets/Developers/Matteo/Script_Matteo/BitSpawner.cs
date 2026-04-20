using System.Collections.Generic;
using UnityEngine;

public class BitSpawner : MonoBehaviour
{
    public enum BitDirection { RightToLeft, LeftToRight }

    [Header("Direzione")]
    [Tooltip("RightToLeft = andata, LeftToRight = ritorno")]
    [SerializeField] private BitDirection direction = BitDirection.RightToLeft;

    [Header("Sprites laser")]
    [SerializeField] private Sprite[] bitSprites;

    [Header("Spawning")]
    [SerializeField] private float spawnRate     = 0.12f;
    [SerializeField] private int   maxBits        = 35;
    [SerializeField] private float tubeHalfHeight = 1.6f;

    [Header("Velocità")]
    [SerializeField] private float minSpeed = 4f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Scala")]
    [SerializeField] private float minScale = 0.08f;
    [SerializeField] private float maxScale = 0.18f;

    private float   _spawnX;
    private float   _despawnX;
    private Vector3 _moveDir;
    private float   _rotation;

    private readonly List<BitParticle> _bits = new();
    private float _timer = 0f;

    private void Start()
    {
        if (direction == BitDirection.RightToLeft)
        {
            _spawnX   =  12f;
            _despawnX = -12f;
            _moveDir  = Vector3.left;
            _rotation = 180f;
        }
        else
        {
            _spawnX   = -12f;
            _despawnX =  12f;
            _moveDir  = Vector3.right;
            _rotation = 0f;
        }

        // Pre-popola il tubo
        for (int i = 0; i < maxBits; i++)
        {
            float x = Random.Range(_despawnX, _spawnX);
            SpawnBitAt(x);
        }

        _timer = spawnRate;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnRate && _bits.Count < maxBits)
        {
            _timer = 0f;
            SpawnBitAt(_spawnX);
        }

        for (int i = _bits.Count - 1; i >= 0; i--)
        {
            var b = _bits[i];
            if (b.obj == null) { _bits.RemoveAt(i); continue; }

            b.obj.transform.Translate(_moveDir * b.speed * Time.deltaTime, Space.World);

            bool outOfBounds = direction == BitDirection.RightToLeft
                ? b.obj.transform.position.x < _despawnX
                : b.obj.transform.position.x > _despawnX;

            if (outOfBounds)
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
        obj.transform.rotation   = Quaternion.Euler(0f, 0f, _rotation);
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