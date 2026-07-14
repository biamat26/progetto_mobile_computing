using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gestisce 3 ondate di nemici. Non tocca Enemy.cs/EnemyHealth.cs —
/// traccia i nemici vivi controllando se i GameObject spawnati sono ancora validi.
/// </summary>
public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string nome = "Ondata";
        public GameObject[] nemiciDaSpawnare; // prefab, uno per ogni nemico dell'ondata
    }

    [Header("Punti di spawn fissi")]
    public Transform[] spawnPoints;

    [Header("Ondate (in ordine)")]
    public Wave[] waves;

    [Header("Eventi")]
    public UnityEngine.Events.UnityEvent onAllWavesCompleted;
    public UnityEngine.Events.UnityEvent onWaveStarted;

    [Header("UI (opzionale)")]
    public TMPro.TMP_Text waveStatusText;

    private List<GameObject> aliveEnemies = new List<GameObject>();
    private int currentWaveIndex = -1;
    private bool wavesInProgress = false;
    private bool wavesCompleted = false;

    public bool AreWavesCompleted() => wavesCompleted;
    public bool AreWavesInProgress() => wavesInProgress;

    // ── Avvio ────────────────────────────────────────────
    public void StartWaves()
{
    Debug.Log("StartWaves chiamato. wavesInProgress=" + wavesInProgress + " wavesCompleted=" + wavesCompleted);
    if (wavesInProgress || wavesCompleted) return;
    wavesInProgress = true;
    currentWaveIndex = -1;
    StartCoroutine(NextWave());
}

    IEnumerator NextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            // tutte le ondate completate
            wavesInProgress = false;
            wavesCompleted = true;
            if (waveStatusText) waveStatusText.text = "TUTTE LE ONDATE SCONFITTE";
            onAllWavesCompleted?.Invoke();
            yield break;
        }

        Wave wave = waves[currentWaveIndex];
        if (waveStatusText) waveStatusText.text = wave.nome;
        onWaveStarted?.Invoke();

        SpawnWave(wave);

        // aspetta che tutti i nemici dell'ondata siano morti
        yield return StartCoroutine(WaitUntilWaveCleared());

        // piccola pausa tra un'ondata e l'altra
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(NextWave());
    }

    void SpawnWave(Wave wave)
{
    Debug.Log("SpawnWave chiamato. Nemici da spawnare: " + wave.nemiciDaSpawnare.Length + " Spawn points: " + spawnPoints.Length);
        aliveEnemies.Clear();

        for (int i = 0; i < wave.nemiciDaSpawnare.Length; i++)
        {
            if (spawnPoints.Length == 0) break;

            // usa i punti di spawn in modo ciclico se i nemici sono più dei punti
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];

            GameObject enemy = Instantiate(wave.nemiciDaSpawnare[i], spawnPoint.position, spawnPoint.rotation);
            aliveEnemies.Add(enemy);
        }
    }

    IEnumerator WaitUntilWaveCleared()
    {
        while (true)
        {
            // rimuovi dalla lista i nemici già distrutti
            aliveEnemies.RemoveAll(e => e == null);

            if (aliveEnemies.Count == 0)
                yield break;

            if (waveStatusText)
                waveStatusText.text = waves[currentWaveIndex].nome + " — Nemici rimasti: " + aliveEnemies.Count;

            yield return new WaitForSeconds(0.3f);
        }
    }
}