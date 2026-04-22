using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoorTerminal : MonoBehaviour
{
    [Header("Porta")]
    [SerializeField] private Sprite[]   disintegrateFrames;
    [SerializeField] private float      animFps = 8f;
    [SerializeField] private GameObject doorObject;

    [Header("Prompt [E]")]
    [SerializeField] private GameObject promptPanel;

    [Header("UI Terminale")]
    [SerializeField] private GameObject      terminalPanel;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI inputDisplay;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Bottoni 0-9")]
    [SerializeField] private Button btn1, btn2, btn3;
    [SerializeField] private Button btn4, btn5, btn6;
    [SerializeField] private Button btn7, btn8, btn9;
    [SerializeField] private Button btn0;

    [Header("Bottoni azione")]
    [SerializeField] private Button btnDel;
    [SerializeField] private Button btnEsc;

    [Header("Colori UI")]
    [SerializeField] private Color successColor = new Color(0f, 1f, 0.53f, 1f);
    [SerializeField] private Color errorColor   = new Color(1f, 0.27f, 0.27f, 1f);
    [SerializeField] private Color grayColor    = new Color(1f, 1f, 1f, 0.4f);

    [Header("Password")]
    [SerializeField] private string correctPassword = "11092001";
    [SerializeField] private int    maxDigits = 8;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerMovementScript;

    private bool   _playerInRange = false;
    private bool   _terminalOpen  = false;
    private bool   _doorOpen      = false;
    private bool   _animating     = false;
    private bool   _inputLocked   = false;
    private string _input         = "";
    private bool   _cursorVisible = true;
    private float  _cursorTimer   = 0f;

    private SpriteRenderer _sr;
    private Collider2D     _col;
    private Vector3        _originalScale;

    private void Awake()
    {
        if (doorObject != null)
        {
            _sr            = doorObject.GetComponent<SpriteRenderer>();
            _col           = doorObject.GetComponent<Collider2D>();
            _originalScale = doorObject.transform.localScale;
            if (disintegrateFrames != null && disintegrateFrames.Length > 0)
                _sr.sprite = disintegrateFrames[0];
        }

        promptPanel?.SetActive(false);
        terminalPanel?.SetActive(false);

        BindKey(btn1,"1"); BindKey(btn2,"2"); BindKey(btn3,"3");
        BindKey(btn4,"4"); BindKey(btn5,"5"); BindKey(btn6,"6");
        BindKey(btn7,"7"); BindKey(btn8,"8"); BindKey(btn9,"9");
        BindKey(btn0,"0");
        btnDel?.onClick.AddListener(PressDelete);
        btnEsc?.onClick.AddListener(PressEsc);
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

        if (!_inputLocked)
        {
            for (int i = 0; i <= 9; i++)
                if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
                    PressKey(i.ToString());
            if (Input.GetKeyDown(KeyCode.Backspace)) PressDelete();
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Z)) PressEsc();

        _cursorTimer += Time.deltaTime;
        if (_cursorTimer >= 0.5f)
        {
            _cursorTimer   = 0f;
            _cursorVisible = !_cursorVisible;
            UpdateDisplay();
        }
    }

    private void OpenTerminal()
    {
        _terminalOpen = true;
        _inputLocked  = false;
        _input        = "";
        promptPanel?.SetActive(false);
        terminalPanel?.SetActive(true);
        if (playerMovementScript) playerMovementScript.enabled = false;
        if (headerText) headerText.text = "> SISTEMA DI SICUREZZA v2.4";
        if (statusText) statusText.text = "";
        UpdateDisplay();
    }

    private void CloseTerminal()
    {
        _terminalOpen = false;
        _inputLocked  = false;
        _input        = "";
        terminalPanel?.SetActive(false);
        if (playerMovementScript) playerMovementScript.enabled = true;
        if (_playerInRange) promptPanel?.SetActive(true);
    }

    private void BindKey(Button btn, string digit)
    {
        if (btn != null) btn.onClick.AddListener(() => PressKey(digit));
    }

    private void PressKey(string digit)
    {
        if (_inputLocked || _input.Length >= maxDigits) return;
        _input += digit;
        if (statusText) statusText.text = "";
        UpdateDisplay();
        if (_input.Length == maxDigits) CheckPassword();
    }

    private void PressDelete()
    {
        if (_inputLocked || _input.Length == 0) return;
        _input = _input.Substring(0, _input.Length - 1);
        if (statusText) statusText.text = "";
        UpdateDisplay();
    }

    private void PressEsc()
    {
        if (statusText) { statusText.color = grayColor; statusText.text = "> Uscita dal terminale..."; }
        StartCoroutine(DelayedClose(0.6f));
    }

    private void UpdateDisplay()
    {
        if (inputDisplay == null) return;
        inputDisplay.text = "> " + new string('*', _input.Length) + (_cursorVisible ? "_" : " ");
    }

    private void CheckPassword()
    {
        if (_input == correctPassword)
        {
            _inputLocked = true;
            if (statusText) { statusText.color = successColor; statusText.text = "> ACCESSO CONSENTITO\n> Apertura in corso..."; }
            StartCoroutine(GrantAccess());
        }
        else
        {
            if (statusText) { statusText.color = errorColor; statusText.text = "> CODICE ERRATO. Riprova."; }
            StartCoroutine(ClearAfterDelay(1.2f));
        }
    }

    private IEnumerator ClearAfterDelay(float t)
    {
        yield return new WaitForSeconds(t);
        _input = "";
        if (statusText) statusText.text = "";
        UpdateDisplay();
    }

    private IEnumerator DelayedClose(float t)
    {
        yield return new WaitForSeconds(t);
        CloseTerminal();
    }

    private IEnumerator GrantAccess()
    {
        yield return new WaitForSeconds(1.5f);
        CloseTerminal();
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(DisintegrateDoor());
    }

    private IEnumerator DisintegrateDoor()
    {
        if (_sr == null) yield break;
        _animating = true;
        _doorOpen  = true;

        _sr.color = new Color(0.5f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.08f);
        _sr.color = Color.white;

        float delay = 1f / animFps;
        foreach (var frame in disintegrateFrames)
        {
            _sr.sprite = frame;
            doorObject.transform.localScale = _originalScale;
            yield return new WaitForSeconds(delay);
        }

        if (_col != null) _col.enabled = false;
        _sr.enabled = false;
        _animating  = false;
    }
}