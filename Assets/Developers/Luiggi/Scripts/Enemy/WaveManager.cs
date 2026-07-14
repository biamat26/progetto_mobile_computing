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

    [Header("Audio Battaglia")]
    public AudioClip musicaCombattimento;
    public AudioClip suonoInizioBattaglia;
    public AudioClip suonoFineBattaglia;

    [Header("Eventi")]
    public UnityEngine.Events.UnityEvent onAllWavesCompleted;
    public UnityEngine.Events.UnityEvent onWaveStarted;

    [Header("UI - Testo")]
    public TMPro.TMP_Text waveNameText;
    public TMPro.TMP_Text enemiesLeftText;

    [Header("UI - Barra di caricamento")]
    public Slider progressBar;
    public TMPro.TMP_Text progressPercentText;

    [Header("HUD Container")]
    public GameObject hudContainer;

    private List<GameObject> aliveEnemies = new List<GameObject>();
    private int currentWaveIndex = -1;
    private bool wavesInProgress = false;
    private bool wavesCompleted = false;

    private SceneAudioController sceneAudioController;

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

    // ── Avvio ────────────────────────────────────────────
    public void StartWaves()
    {
        if (wavesInProgress || wavesCompleted) return;
        wavesInProgress = true;

        if (hudContainer != null) hudContainer.SetActive(true);

        StartCoroutine(SequenzaInizioBattaglia());
    }

    private IEnumerator SequenzaInizioBattaglia()
    {
        sceneAudioController = FindFirstObjectByType<SceneAudioController>();

        // 1. ZITTIAMO LA MUSICA DI SCENA CHIRURGICAMENTE
        if (sceneAudioController != null)
        {
            sceneAudioController.StopAllCoroutines();

            if (sceneAudioController.musicaScena != null)
            {
                AudioSource[] tuttiGliAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
                foreach (AudioSource src in tuttiGliAudio)
                {
                    if (src.clip == sceneAudioController.musicaScena)
                    {
                        src.Stop();
                    }
                }
            }
        }

        // 2. PARTE L'INIZIO DELLA BATTAGLIA NEL SILENZIO ASSOLUTO
        if (suonoInizioBattaglia != null)
        {
            AudioSource.PlayClipAtPoint(suonoInizioBattaglia, Camera.main.transform.position);
            yield return new WaitForSeconds(suonoInizioBattaglia.length);
        }

        // 3. ORA PARTE LA MUSICA DI COMBATTIMENTO
        if (AudioManager.instance != null && musicaCombattimento != null)
        {
            AudioManager.instance.PlayMusic(musicaCombattimento, 0f, 0f);
        }

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
            if (waveNameText) waveNameText.text = "TUTTE LE ONDATE SCONFITTE";
            if (enemiesLeftText) enemiesLeftText.text = "";
            SetProgress(1f);
            onAllWavesCompleted?.Invoke();

            StartCoroutine(SequenzaFineBattaglia());
            yield break;
        }

        Wave wave = waves[currentWaveIndex];
        if (waveNameText) waveNameText.text = "ONDATA " + (currentWaveIndex + 1) + " / " + waves.Length;
        onWaveStarted?.Invoke();

        SpawnWave(wave);

        yield return StartCoroutine(WaitUntilWaveCleared());

        // ondata completata -> aggiorna barra di caricamento
        float progresso = (float)(currentWaveIndex + 1) / waves.Length;
        SetProgress(progresso);

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(NextWave());
    }

    private IEnumerator SequenzaFineBattaglia()
    {
        // ZITTIAMO LA MUSICA DI COMBATTIMENTO CHIRURGICAMENTE
        if (musicaCombattimento != null)
        {
            AudioSource[] tuttiGliAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
            foreach (AudioSource src in tuttiGliAudio)
            {
                if (src.clip == musicaCombattimento)
                {
                    src.Stop();
                }
            }
        }

        // PARTE IL SUONO DI FINE BATTAGLIA
        if (suonoFineBattaglia != null)
        {
            AudioSource.PlayClipAtPoint(suonoFineBattaglia, Camera.main.transform.position);
            yield return new WaitForSeconds(suonoFineBattaglia.length);
        }

        // RIPRENDE QUELLO INIZIALE
        if (sceneAudioController != null)
        {
            sceneAudioController.SendMessage("Start");
        }

        // nascondi l'HUD dopo un attimo
        yield return new WaitForSeconds(1f);
        if (hudContainer != null) hudContainer.SetActive(false);
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