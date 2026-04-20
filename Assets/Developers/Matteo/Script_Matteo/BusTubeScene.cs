using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BusTubeScene : MonoBehaviour
{
    public enum TravelDirection { LeftToRight, RightToLeft }

    [Header("Direzione")]
    [SerializeField] private TravelDirection direction = TravelDirection.LeftToRight;

    [Header("Navicella")]
    [SerializeField] private GameObject shipObject;
    [SerializeField] private float shipSpeed      = 2f;
    [SerializeField] private float travelDistance = 50f;

    [Header("Scena destinazione")]
    [SerializeField] private string nextSceneName = "Test_RAM";
    [SerializeField] private float fadeDuration   = 0.8f;

    [Header("Entrata")]
    [SerializeField] private float entryDelay = 0.5f;

    private bool    _traveling        = false;
    private float   _distanceTraveled = 0f;
    private Vector3 _moveDir;

    private void Start()
    {
        _moveDir = direction == TravelDirection.LeftToRight ? Vector3.right : Vector3.left;

        if (SceneTransition.Instance != null)
            StartCoroutine(SceneTransition.Instance.FadeIn(fadeDuration));

        StartCoroutine(BeginTravel());
    }

    private IEnumerator BeginTravel()
    {
        yield return new WaitForSeconds(entryDelay);
        _traveling = true;
    }

    private void Update()
    {
        if (!_traveling || shipObject == null) return;

        float step = shipSpeed * Time.deltaTime;
        shipObject.transform.Translate(_moveDir * step, Space.World);
        _distanceTraveled += step;

        if (_distanceTraveled >= travelDistance)
        {
            _traveling = false;
            StartCoroutine(ExitTube());
        }
    }

    private IEnumerator ExitTube()
    {
        float elapsed = 0f;
        while (elapsed < 0.6f)
        {
            elapsed += Time.deltaTime;
            shipObject.transform.Translate(_moveDir * shipSpeed * 2f * Time.deltaTime, Space.World);
            yield return null;
        }

        if (SceneTransition.Instance != null)
            yield return StartCoroutine(SceneTransition.Instance.FadeOut(fadeDuration));

        SceneManager.LoadScene(nextSceneName);
    }
}