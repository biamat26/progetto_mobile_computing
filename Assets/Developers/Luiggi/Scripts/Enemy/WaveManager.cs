using UnityEngine;
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

    [Header("UI (opzionale)")]
    public TMPro.TMP_Text waveStatusText;

    private List<GameObject> aliveEnemies = new List<GameObject>();
    private int currentWaveIndex = -1;
    private bool wavesInProgress = false;
    private bool wavesCompleted = false;

    private SceneAudioController sceneAudioController;

    public bool AreWavesCompleted() => wavesCompleted;
    public bool AreWavesInProgress() => wavesInProgress;

    public void StartWaves()
    {
        if (wavesInProgress || wavesCompleted) return;
        wavesInProgress = true;
        
        StartCoroutine(SequenzaInizioBattaglia());
    }

    private IEnumerator SequenzaInizioBattaglia()
    {
        sceneAudioController = FindFirstObjectByType<SceneAudioController>();
        
        // 1. ZITTIAMO LA MUSICA DI SCENA CHIRURGICAMENTE
        if (sceneAudioController != null) 
        {
            sceneAudioController.StopAllCoroutines(); // Ferma il loop
            
            if (sceneAudioController.musicaScena != null)
            {
                // Cerca tutti i riproduttori audio nella scena
                AudioSource[] tuttiGliAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
                foreach (AudioSource src in tuttiGliAudio)
                {
                    // Se sta suonando esattamente la musica di esplorazione, fermala!
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
            // Aspetta che finisca
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
            wavesInProgress = false;
            wavesCompleted = true;
            if (waveStatusText) waveStatusText.text = "TUTTE LE ONDATE SCONFITTE";
            onAllWavesCompleted?.Invoke();
            
            StartCoroutine(SequenzaFineBattaglia());
            yield break;
        }

        Wave wave = waves[currentWaveIndex];
        if (waveStatusText) waveStatusText.text = wave.nome;
        onWaveStarted?.Invoke();

        SpawnWave(wave);

        yield return StartCoroutine(WaitUntilWaveCleared());
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(NextWave());
    }
    
    private IEnumerator SequenzaFineBattaglia()
    {
        // 4. ZITTIAMO LA MUSICA DI COMBATTIMENTO CHIRURGICAMENTE
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

        // 5. PARTE IL SUONO DI FINE BATTAGLIA
        if (suonoFineBattaglia != null)
        {
            AudioSource.PlayClipAtPoint(suonoFineBattaglia, Camera.main.transform.position);
            yield return new WaitForSeconds(suonoFineBattaglia.length);
        }

        // 6. RIPRENDE QUELLO INIZIALE
        if (sceneAudioController != null)
        {
            sceneAudioController.SendMessage("Start");
        }
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
    }

    IEnumerator WaitUntilWaveCleared()
    {
        while (true)
        {
            aliveEnemies.RemoveAll(e => e == null);

            if (aliveEnemies.Count == 0)
                yield break;

            if (waveStatusText)
                waveStatusText.text = waves[currentWaveIndex].nome + " — Nemici rimasti: " + aliveEnemies.Count;

            yield return new WaitForSeconds(0.3f);
        }
    }
}