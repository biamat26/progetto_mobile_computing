using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ================================================================
// LoginSceneBuilder.cs
// Crea AUTOMATICAMENTE tutta la UI della LoginScene
//
// COME USARLO (una volta sola!):
// 1. Crea un GameObject vuoto nella LoginScene → chiamalo "SceneBuilder"
// 2. Attacca questo script
// 3. Premi PLAY → tutta la UI viene creata automaticamente
// 4. Salva la scena (CTRL+S)
// 5. Rimuovi questo script (non serve più)
// ================================================================

public class LoginSceneBuilder : MonoBehaviour
{
    void Start()
    {
        BuildLoginScene();
        Destroy(this); // Si auto-rimuove dopo aver creato tutto
    }

    void BuildLoginScene()
    {
        // ── CANVAS ──────────────────────────────────────────────
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Aggiunge EventSystem se non esiste
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // ── SFONDO ──────────────────────────────────────────────
        GameObject bg = CreatePanel(canvasGO, "Background", new Color(0.1f, 0.1f, 0.15f));
        SetFullScreen(bg);

        // Titolo gioco
        GameObject title = CreateText(canvasGO, "TitleText", "⚔ PIXEL QUEST", 48, Color.white);
        SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 120), new Vector2(400, 70));

        // ── PANEL LOGIN ─────────────────────────────────────────
        GameObject panelLogin = CreatePanel(canvasGO, "PanelLogin", new Color(0.15f, 0.15f, 0.2f));
        SetRect(panelLogin, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(380, 380));
        AddOutline(panelLogin, new Color(0.4f, 0.4f, 0.8f));

        // Titolo pannello login
        CreateText(panelLogin, "LabelLogin", "ACCEDI", 24, new Color(0.6f, 0.6f, 1f))
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 155);

        // Email input
        GameObject emailInput = CreateInputField(panelLogin, "EmailInput", "Email...");
        SetRect(emailInput, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 70), new Vector2(320, 45));

        // Password input
        GameObject passInput = CreateInputField(panelLogin, "PasswordInput", "Password...", isPassword: true);
        SetRect(passInput, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 10), new Vector2(320, 45));

        // Toggle "Ricordami"
        GameObject rememberToggle = CreateToggle(panelLogin, "RememberMe", "Ricordami");
        SetRect(rememberToggle, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-60, -40), new Vector2(180, 30));

        // Bottone Login
        GameObject btnLogin = CreateButton(panelLogin, "BtnLogin", "ACCEDI", new Color(0.3f, 0.3f, 0.9f));
        SetRect(btnLogin, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -95), new Vector2(320, 50));

        // Bottone → vai a registrazione
        GameObject btnGoReg = CreateButton(panelLogin, "BtnGoToRegister", "Non hai un account? Registrati", new Color(0.2f, 0.2f, 0.3f));
        SetRect(btnGoReg, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -155), new Vector2(320, 35));

        // Testo errore login
        GameObject loginError = CreateText(panelLogin, "LoginError", "", 14, new Color(1f, 0.3f, 0.3f));
        SetRect(loginError, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -200), new Vector2(320, 30));
        loginError.SetActive(false);

        // ── PANEL REGISTRAZIONE ─────────────────────────────────
        GameObject panelReg = CreatePanel(canvasGO, "PanelRegister", new Color(0.15f, 0.15f, 0.2f));
        SetRect(panelReg, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(380, 430));
        AddOutline(panelReg, new Color(0.4f, 0.8f, 0.4f));
        panelReg.SetActive(false); // Nascosto all'inizio

        // Titolo pannello registrazione
        CreateText(panelReg, "LabelRegister", "REGISTRATI", 24, new Color(0.4f, 1f, 0.4f))
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 180);

        // Email reg
        GameObject regEmail = CreateInputField(panelReg, "RegEmailInput", "Email...");
        SetRect(regEmail, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(320, 45));

        // Password reg
        GameObject regPass = CreateInputField(panelReg, "RegPasswordInput", "Password...", isPassword: true);
        SetRect(regPass, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40), new Vector2(320, 45));

        // Conferma password
        GameObject regPassConfirm = CreateInputField(panelReg, "RegPasswordConfirmInput", "Conferma password...", isPassword: true);
        SetRect(regPassConfirm, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(320, 45));

        // Bottone Registrati
        GameObject btnReg = CreateButton(panelReg, "BtnRegister", "CREA ACCOUNT", new Color(0.2f, 0.7f, 0.3f));
        SetRect(btnReg, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -90), new Vector2(320, 50));

        // Bottone → torna al login
        GameObject btnGoLogin = CreateButton(panelReg, "BtnGoToLogin", "Hai già un account? Accedi", new Color(0.2f, 0.2f, 0.3f));
        SetRect(btnGoLogin, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -155), new Vector2(320, 35));

        // Testo errore registrazione
        GameObject regError = CreateText(panelReg, "RegisterError", "", 14, new Color(1f, 0.3f, 0.3f));
        SetRect(regError, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -205), new Vector2(320, 30));
        regError.SetActive(false);

        // ── LOADING PANEL ───────────────────────────────────────
        GameObject loadingPanel = CreatePanel(canvasGO, "LoadingPanel", new Color(0, 0, 0, 0.7f));
        SetFullScreen(loadingPanel);
        CreateText(loadingPanel, "LoadingText", "Caricamento...", 30, Color.white)
            .GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        loadingPanel.SetActive(false);

        // ── COLLEGA LoginUI ─────────────────────────────────────
        LoginUI loginUI = canvasGO.AddComponent<LoginUI>();

        // Usa reflection per assegnare i campi privati serializzati
        SetPrivateField(loginUI, "panelLogin",               panelLogin);
        SetPrivateField(loginUI, "loginEmailInput",          emailInput.GetComponent<TMP_InputField>());
        SetPrivateField(loginUI, "loginPasswordInput",       passInput.GetComponent<TMP_InputField>());
        SetPrivateField(loginUI, "btnLogin",                 btnLogin.GetComponent<Button>());
        SetPrivateField(loginUI, "btnGoToRegister",          btnGoReg.GetComponent<Button>());
        SetPrivateField(loginUI, "loginErrorText",           loginError.GetComponent<TMP_Text>());
        SetPrivateField(loginUI, "rememberMeToggle",         rememberToggle.GetComponent<Toggle>());
        SetPrivateField(loginUI, "panelRegister",            panelReg);
        SetPrivateField(loginUI, "regEmailInput",            regEmail.GetComponent<TMP_InputField>());
        SetPrivateField(loginUI, "regPasswordInput",         regPass.GetComponent<TMP_InputField>());
        SetPrivateField(loginUI, "regPasswordConfirmInput",  regPassConfirm.GetComponent<TMP_InputField>());
        SetPrivateField(loginUI, "btnRegister",              btnReg.GetComponent<Button>());
        SetPrivateField(loginUI, "btnGoToLogin",             btnGoLogin.GetComponent<Button>());
        SetPrivateField(loginUI, "registerErrorText",        regError.GetComponent<TMP_Text>());
        SetPrivateField(loginUI, "loadingPanel",             loadingPanel);

        Debug.Log("[LoginSceneBuilder] ✅ UI creata con successo! Salva la scena con CTRL+S");
    }

    // ================================================================
    // HELPER — Crea elementi UI
    // ================================================================

    GameObject CreatePanel(GameObject parent, string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        Image img = go.AddComponent<Image>();
        img.color = color;
        go.AddComponent<RectTransform>();
        return go;
    }

    GameObject CreateText(GameObject parent, string name, string text, int size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    GameObject CreateInputField(GameObject parent, string name, string placeholder, bool isPassword = false)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);

        Image bg = go.AddComponent<Image>();
        bg.color = new Color(0.08f, 0.08f, 0.12f);

        TMP_InputField input = go.AddComponent<TMP_InputField>();

        // Placeholder
        GameObject phGO = CreateText(go, "Placeholder", placeholder, 16, new Color(0.5f, 0.5f, 0.5f));
        SetRect(phGO, Vector2.zero, Vector2.one, new Vector2(10, 0), new Vector2(-10, 0));

        // Testo
        GameObject txtGO = CreateText(go, "Text", "", 16, Color.white);
        SetRect(txtGO, Vector2.zero, Vector2.one, new Vector2(10, 0), new Vector2(-10, 0));

        input.placeholder = phGO.GetComponent<TMP_Text>();
        input.textComponent = txtGO.GetComponent<TMP_Text>();

        if (isPassword)
            input.contentType = TMP_InputField.ContentType.Password;

        return go;
    }

    GameObject CreateButton(GameObject parent, string name, string label, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);

        Image img = go.AddComponent<Image>();
        img.color = color;

        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = Color.Lerp(color, Color.white, 0.3f);
        cb.pressedColor     = Color.Lerp(color, Color.black, 0.3f);
        btn.colors = cb;

        GameObject txtGO = CreateText(go, "Label", label, 16, Color.white);
        SetRect(txtGO, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

        return go;
    }

    GameObject CreateToggle(GameObject parent, string name, string label)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);

        Toggle toggle = go.AddComponent<Toggle>();

        // Background
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(go.transform, false);
        Image bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.3f);
        SetRect(bgGO, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(10, 0), new Vector2(20, 20));

        // Checkmark
        GameObject checkGO = new GameObject("Checkmark");
        checkGO.transform.SetParent(bgGO.transform, false);
        Image checkImg = checkGO.AddComponent<Image>();
        checkImg.color = new Color(0.4f, 0.8f, 0.4f);
        SetRect(checkGO, Vector2.zero, Vector2.one, new Vector2(2, 2), new Vector2(-2, -2));

        // Label
        GameObject lblGO = CreateText(go, "Label", label, 14, Color.white);
        SetRect(lblGO, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(30, 0), new Vector2(0, 20));
        lblGO.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        toggle.targetGraphic = bgImg;
        toggle.graphic = checkImg;

        return go;
    }

    void AddOutline(GameObject go, Color color)
    {
        Outline outline = go.AddComponent<Outline>();
        outline.effectColor = color;
        outline.effectDistance = new Vector2(2, 2);
    }

    // ================================================================
    // HELPER — RectTransform
    // ================================================================

    void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
    }

    void SetFullScreen(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // ================================================================
    // HELPER — Assegna campi privati via Reflection
    // ================================================================
    void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
            field.SetValue(target, value);
        else
            Debug.LogWarning($"[LoginSceneBuilder] Campo non trovato: {fieldName}");
    }
}