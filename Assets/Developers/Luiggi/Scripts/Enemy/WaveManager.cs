using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string nome = "Ondata";
        public GameObject[] nemiciDaSpawnare;
    }

    [Header("Punti di spawn fissi")]
    public Transform[] spawnPoints;

    [Header("Ondate (in ordine)")]
    public Wave[] waves;

    [Header("Eventi")]
    public UnityEngine.Events.UnityEvent onAllWavesCompleted;
    public UnityEngine.Events.UnityEvent onWaveStarted;

    [Header("UI - Testo")]
    public TMPro.TMP_Text waveNameText;      // es. "Ondata 2 / 3"
    public TMPro.TMP_Text enemiesLeftText;   // es. "Nemici rimasti: 3"

    [Header("UI - Barra di caricamento")]
    public Slider progressBar;
    public TMPro.TMP_Text progressPercentText;

    [Header("HUD Container")]
    public GameObject hudContainer; // il GameObject padre di tutta la UI dell'ondata

    private List<GameObject> aliveEnemies = new List<GameObject>();
    private int currentWaveIndex = -1;
    private bool wavesInProgress = false;
    private bool wavesCompleted = false;

    public bool AreWavesCompleted() => wavesCompleted;
    public bool AreWavesInProgress() => wavesInProgress;

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.minValue = 0f;
            progressBar.maxValue = 1f;
            progressBar.value = 0f;
        }
        UpdateProgressText(0f);

        if (hudContainer != null) hudContainer.SetActive(false);
    }

    public void StartWaves()
    {
        if (wavesInProgress || wavesCompleted) return;
        wavesInProgress = true;
        currentWaveIndex = -1;
        if (hudContainer != null) hudContainer.SetActive(true);
        StartCoroutine(NextWave());
    }

    IEnumerator NextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            wavesInProgress = false;
            wavesCompleted = true;
            if (waveNameText) waveNameText.text = "TUTTE LE ONDATE SCONFITTE";
            if (enemiesLeftText) enemiesLeftText.text = "";
            SetProgress(1f);
            onAllWavesCompleted?.Invoke();

            yield return new WaitForSeconds(2f);
            if (hudContainer != null) hudContainer.SetActive(false);

            yield break;
        }

        Wave wave = waves[currentWaveIndex];
        if (waveNameText) waveNameText.text = wave.nome + " / " + waves.Length;
        onWaveStarted?.Invoke();

        SpawnWave(wave);

        yield return StartCoroutine(WaitUntilWaveCleared());

        // ondata completata -> aggiorna barra di caricamento
        float progresso = (float)(currentWaveIndex + 1) / waves.Length;
        SetProgress(progresso);

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(NextWave());
    }

    void SpawnWave(Wave wave)
    {
        aliveEnemies.Clear();

        for (int i = 0; i < wave.nemiciDaSpawnare.Length; i++)
        {
            if (spawnPoints.Length == 0) break;
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject enemy = Instantiate(wave.nemiciDaSpawnare[i], spawnPoint.position, spawnPoint.rotation);
            aliveEnemies.Add(enemy);
        }

        UpdateEnemiesLeftText();
    }

    IEnumerator WaitUntilWaveCleared()
    {
        while (true)
        {
            aliveEnemies.RemoveAll(e => e == null);
            UpdateEnemiesLeftText();

            if (aliveEnemies.Count == 0)
                yield break;

            yield return new WaitForSeconds(0.3f);
        }
    }

    void UpdateEnemiesLeftText()
{
    if (enemiesLeftText != null)
        enemiesLeftText.text = aliveEnemies.Count + " LEFT";
}

    void SetProgress(float value)
    {
        if (progressBar != null)
            progressBar.value = value;
        UpdateProgressText(value);
    }

    void UpdateProgressText(float value)
    {
        if (progressPercentText != null)
            progressPercentText.text = Mathf.RoundToInt(value * 100f) + "% COMPLETED";
    }
}