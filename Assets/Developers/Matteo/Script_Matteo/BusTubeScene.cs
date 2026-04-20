using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Scena "BUS TUBE" — navicella che scorre automaticamente nel tubo.
/// 
/// Setup scena:
///   1. Camera ortografica, size ~3, sfondo nero
///   2. Due sprite lunghi orizzontali per le pareti (top e bottom) — tag "Wall"
///   3. Trascina lo sprite navicella in scena, assegna a "shipSprite"
///   4. Crea un ParticleSystem, assegna a "bitParticles"
///   5. Aggiungi questo script a un GameObject vuoto "SceneManager"
///   6. Imposta "Next Scene Name" con il nome della scena destinazione
/// </summary>
public class BusTubeScene : MonoBehaviour
{
    [Header("Navicella")]
    [SerializeField] private GameObject shipObject;
    [SerializeField] private float shipSpeed = 4f;
    [SerializeField] private float travelDistance = 30f;   // unità da percorrere prima di uscire

    [Header("Scena destinazione")]
    [SerializeField] private string nextSceneName = "ScenaDestinazione";
    [SerializeField] private float fadeDuration = 0.8f;

    [Header("Effetto entrata")]
    [SerializeField] private float entryDelay = 0.5f;      // pausa prima di partire

    private bool _traveling = false;
    private float _distanceTraveled = 0f;
    private Vector3 _startPos;

    // ─── Start ─────────────────────────────────────────────────────

    private void Start()
    {
        if (shipObject == null) return;
        _startPos = shipObject.transform.position;

        // Fade in all'arrivo
        if (SceneTransition.Instance != null)
            StartCoroutine(SceneTransition.Instance.FadeIn(fadeDuration));

        StartCoroutine(BeginTravel());
    }

    // ─── Travel ────────────────────────────────────────────────────

    private IEnumerator BeginTravel()
    {
        yield return new WaitForSeconds(entryDelay);
        _traveling = true;
    }

    private void Update()
    {
        if (!_traveling || shipObject == null) return;

        float step = shipSpeed * Time.deltaTime;
        shipObject.transform.Translate(Vector3.right * step);
        _distanceTraveled += step;

        if (_distanceTraveled >= travelDistance)
        {
            _traveling = false;
            StartCoroutine(ExitTube());
        }
    }

    // ─── Uscita dal tubo ───────────────────────────────────────────

    private IEnumerator ExitTube()
    {
        // Accelera verso l'uscita
        float exitTime = 0.6f;
        float elapsed = 0f;
        while (elapsed < exitTime)
        {
            elapsed += Time.deltaTime;
            shipObject.transform.Translate(Vector3.right * shipSpeed * 2f * Time.deltaTime);
            yield return null;
        }

        if (SceneTransition.Instance != null)
            yield return StartCoroutine(SceneTransition.Instance.FadeOut(fadeDuration));

        SceneManager.LoadScene(nextSceneName);
    }
}
