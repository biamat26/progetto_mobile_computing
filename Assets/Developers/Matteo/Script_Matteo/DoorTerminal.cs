using System.Collections;
using UnityEngine;
using TMPro;

public class DoorTerminal : MonoBehaviour
{
    [Header("Porta")]
    [SerializeField] private Sprite[] disintegrateFrames;
    [SerializeField] private float    animFps = 8f;

    [Header("UI Prompt")]
    [SerializeField] private GameObject promptPanel;

    [Header("UI Terminale")]
    [SerializeField] private GameObject      terminalPanel;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI inputText;
    [SerializeField] private TextMeshProUGUI keypadText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Password")]
    [SerializeField] private string correctPassword = "11092001";
    [SerializeField] private int    maxDigits = 8;

    [Header("Movimento player")]
    [SerializeField] private MonoBehaviour playerMovementScript;

    private bool   _playerInRange  = false;
    private bool   _terminalOpen   = false;
    private bool   _doorOpen       = false;
    private bool   _animating      = false;
    private string _input          = "";
    private bool   _cursorVisible  = true;
    private float  _cursorTimer    = 0f;

    private SpriteRenderer _sr;
    private Collider2D     _col;
    private Vector3        _originalScale; // ← salva la scala originale

    private void Awake()
    {
        _sr  = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();

        // Salva la scala originale al momento dell'avvio
        _originalScale = transform.localScale;

        if (disintegrateFrames != null && disintegrateFrames.Length > 0)
            _sr.sprite = disintegrateFrames[0];

        promptPanel?.SetActive(false);
        terminalPanel?.SetActive(false);
        UpdateKeypadDisplay();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_doorOpen)
        {
            _playerInRange = true;
            promptPanel?.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            promptPanel?.SetActive(false);
            if (_terminalOpen) CloseTerminal();
        }
    }

    private void Update()
    {
        if (_doorOpen || _animating) return;

        if (_playerInRange && !_terminalOpen && Input.GetKeyDown(KeyCode.E))
            OpenTerminal();

        if (!_terminalOpen) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CloseTerminal();
            return;
        }

        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                if (_input.Length < maxDigits)
                {
                    _input += i.ToString();
                    UpdateInputDisplay();
                    CheckPassword();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace) && _input.Length > 0)
        {
            _input = _input.Substring(0, _input.Length - 1);
            UpdateInputDisplay();
            if (statusText) statusText.text = "";
        }

        _cursorTimer += Time.deltaTime;
        if (_cursorTimer >= 0.5f)
        {
            _cursorTimer   = 0f;
            _cursorVisible = !_cursorVisible;
            UpdateInputDisplay();
        }
    }

    private void OpenTerminal()
    {
        _terminalOpen = true;
        _input        = "";
        promptPanel?.SetActive(false);
        terminalPanel?.SetActive(true);
        if (playerMovementScript) playerMovementScript.enabled = false;

        if (headerText) headerText.text = "> SISTEMA DI SICUREZZA v2.4\n> Inserisci codice di accesso:";
        if (statusText) statusText.text = "";
        UpdateKeypadDisplay();
        UpdateInputDisplay();
    }

    private void CloseTerminal()
    {
        _terminalOpen = false;
        _input        = "";
        terminalPanel?.SetActive(false);
        if (playerMovementScript) playerMovementScript.enabled = true;
        if (_playerInRange) promptPanel?.SetActive(true);
    }

    private void UpdateInputDisplay()
    {
        if (inputText == null) return;
        inputText.text = "> " + _input + (_cursorVisible ? "_" : " ");
    }

    private void UpdateKeypadDisplay()
    {
        if (keypadText == null) return;
        keypadText.text =
            "  [ 1 ] [ 2 ] [ 3 ]\n" +
            "  [ 4 ] [ 5 ] [ 6 ]\n" +
            "  [ 7 ] [ 8 ] [ 9 ]\n" +
            "        [ 0 ]      \n\n" +
            "  [BACKSPACE] cancella\n" +
            "  [ Z ] esci";
    }

    private void CheckPassword()
    {
        if (_input.Length < maxDigits) return;

        if (_input == correctPassword)
        {
            if (statusText)
            {
                statusText.text  = "> ACCESSO CONSENTITO\n> Apertura in corso...";
                statusText.color = new Color(0f, 1f, 0.5f);
            }
            StartCoroutine(GrantAccess());
        }
        else
        {
            if (statusText)
            {
                statusText.text  = "> CODICE ERRATO. Riprova.";
                statusText.color = new Color(1f, 0.2f, 0.2f);
            }
            StartCoroutine(ClearInputAfterDelay(1f));
        }
    }

    private IEnumerator ClearInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _input = "";
        if (statusText) statusText.text = "";
        UpdateInputDisplay();
    }

    private IEnumerator GrantAccess()
    {
        yield return new WaitForSeconds(1.2f);
        CloseTerminal();
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(DisintegrateDoor());
    }

    private IEnumerator DisintegrateDoor()
    {
        _animating = true;
        _doorOpen  = true;

        // Flash cyan
        _sr.color = new Color(0.5f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.08f);
        _sr.color = Color.white;

        float delay = 1f / animFps;
        foreach (var frame in disintegrateFrames)
        {
            _sr.sprite = frame;
            // Mantieni sempre la scala originale
            transform.localScale = _originalScale;
            yield return new WaitForSeconds(delay);
        }

        if (_col != null) _col.enabled = false;
        _sr.enabled = false;
        _animating  = false;
    }
}