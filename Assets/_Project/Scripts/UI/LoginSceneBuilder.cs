using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

// ================================================================
// LoginSceneBuilder.cs
// Posizione: Assets/Scripts/UI/
//
// COME USARLO (una volta sola!):
// 1. Crea un GameObject vuoto nella LoginScene → chiamalo "SceneBuilder"
// 2. Attacca questo script
// 3. Premi PLAY → tutta la UI viene creata automaticamente
// 4. Salva la scena (CMD+S)
// 5. Premi STOP ed elimina il GameObject "SceneBuilder"
// ================================================================

public class LoginSceneBuilder : MonoBehaviour
{
    void Start()
    {
        BuildLoginScene();
        Destroy(this);
    }

    void BuildLoginScene()
    {
        // ── CANVAS ──────────────────────────────────────────────
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        canvasGO.AddComponent<GraphicRaycaster>();

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
        GameObject title = CreateText(canvasGO, "TitleText", "⚔ PIXEL QUEST ⚔", 48, Color.white);
        SetRect(title, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 200), new Vector2(500, 70));

        // ── PANEL LOGIN ─────────────────────────────────────────
        GameObject panelLogin = CreatePanel(canvasGO, "PanelLogin", new Color(0.15f, 0.15f, 0.2f));
        SetRect(panelLogin, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(380, 380));
        AddOutline(panelLogin, new Color(0.4f, 0.4f, 0.8f));

        CreateText(panelLogin, "LabelLogin", "ACCEDI", 24, new Color(0.6f, 0.6f, 1f))
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 155);

        GameObject emailInput = CreateInputField(panelLogin, "EmailInput", "Email...");
        SetRect(emailInput, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 70), new Vector2(320, 45));

        GameObject passInput = CreateInputField(panelLogin, "PasswordInput", "Password...", isPassword: true);
        SetRect(passInput, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 10), new Vector2(320, 45));

        GameObject rememberToggle = CreateToggle(panelLogin, "RememberMe", "Ricordami");
        SetRect(rememberToggle, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-60, -40), new Vector2(180, 30));

        GameObject btnLogin = CreateButton(panelLogin, "BtnLogin", "ACCEDI", new Color(0.3f, 0.3f, 0.9f));
        SetRect(btnLogin, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -95), new Vector2(320, 50));

        GameObject btnGoReg = CreateButton(panelLogin, "BtnGoToRegister", "Non hai un account? Registrati", new Color(0.2f, 0.2f, 0.3f));
        SetRect(btnGoReg, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -155), new Vector2(320, 35));

        GameObject loginError = CreateText(panelLogin, "LoginError", "", 14, new Color(1f, 0.3f, 0.3f));
        SetRect(loginError, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -195), new Vector2(320, 30));
        loginError.SetActive(false);

        // ── PANEL REGISTRAZIONE ─────────────────────────────────
        GameObject panelReg = CreatePanel(canvasGO, "PanelRegister", new Color(0.15f, 0.15f, 0.2f));
        SetRect(panelReg, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(380, 430));
        AddOutline(panelReg, new Color(0.4f, 0.8f, 0.4f));
        panelReg.SetActive(false);

        CreateText(panelReg, "LabelRegister", "REGISTRATI", 24, new Color(0.4f, 1f, 0.4f))
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 180);

        GameObject regEmail = CreateInputField(panelReg, "RegEmailInput", "Email...");
        SetRect(regEmail, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(320, 45));

        GameObject regPass = CreateInputField(panelReg, "RegPasswordInput", "Password...", isPassword: true);
        SetRect(regPass, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40), new Vector2(320, 45));

        GameObject regPassConfirm = CreateInputField(panelReg, "RegPasswordConfirmInput", "Conferma password...", isPassword: true);
        SetRect(regPassConfirm, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(320, 45));

        GameObject btnReg = CreateButton(panelReg, "BtnRegister", "CREA ACCOUNT", new Color(0.2f, 0.7f, 0.3f));
        SetRect(btnReg, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -90), new Vector2(320, 50));

        GameObject btnGoLogin = CreateButton(panelReg, "BtnGoToLogin", "Hai già un account? Accedi", new Color(0.2f, 0.2f, 0.3f));
        SetRect(btnGoLogin, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -155), new Vector2(320, 35));

        GameObject regError = CreateText(panelReg, "RegisterError", "", 14, new Color(1f, 0.3f, 0.3f));
        SetRect(regError, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -200), new Vector2(320, 30));
        regError.SetActive(false);

        // ── LOADING PANEL ───────────────────────────────────────
        GameObject loadingPanel = CreatePanel(canvasGO, "LoadingPanel", new Color(0, 0, 0, 0.7f));
        SetFullScreen(loadingPanel);
        CreateText(loadingPanel, "LoadingText", "Caricamento...", 30, Color.white)
            .GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        loadingPanel.SetActive(false);

        // ── COLLEGA LoginUI ─────────────────────────────────────
        LoginUI loginUI = canvasGO.AddComponent<LoginUI>();
        Assign(loginUI, "panelLogin",              panelLogin);
        Assign(loginUI, "loginEmailInput",         emailInput.GetComponent<TMP_InputField>());
        Assign(loginUI, "loginPasswordInput",      passInput.GetComponent<TMP_InputField>());
        Assign(loginUI, "btnLogin",                btnLogin.GetComponent<Button>());
        Assign(loginUI, "btnGoToRegister",         btnGoReg.GetComponent<Button>());
        Assign(loginUI, "loginErrorText",          loginError.GetComponent<TMP_Text>());
        Assign(loginUI, "rememberMeToggle",        rememberToggle.GetComponent<Toggle>());
        Assign(loginUI, "panelRegister",           panelReg);
        Assign(loginUI, "regEmailInput",           regEmail.GetComponent<TMP_InputField>());
        Assign(loginUI, "regPasswordInput",        regPass.GetComponent<TMP_InputField>());
        Assign(loginUI, "regPasswordConfirmInput", regPassConfirm.GetComponent<TMP_InputField>());
        Assign(loginUI, "btnRegister",             btnReg.GetComponent<Button>());
        Assign(loginUI, "btnGoToLogin",            btnGoLogin.GetComponent<Button>());
        Assign(loginUI, "registerErrorText",       regError.GetComponent<TMP_Text>());
        Assign(loginUI, "loadingPanel",            loadingPanel);

        Debug.Log("[LoginSceneBuilder] ✅ UI creata! Salva con CMD+S");
    }

    // ================================================================
    // HELPER — Elementi UI
    // ================================================================
    GameObject CreatePanel(GameObject parent, string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = color;
        return go;
    }

    GameObject CreateText(GameObject parent, string name, string text, int size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        return go;
    }

    GameObject CreateButton(GameObject parent, string name, string label, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = color;
        go.AddComponent<Button>();
        GameObject lbl = CreateText(go, "Label", label, 16, Color.white);
        SetRect(lbl, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        return go;
    }

    GameObject CreateInputField(GameObject parent, string name, string placeholder, bool isPassword = false)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f);

        TMP_InputField input = go.AddComponent<TMP_InputField>();

        GameObject phGO = CreateText(go, "Placeholder", placeholder, 16, new Color(0.5f, 0.5f, 0.5f));
        SetRect(phGO, Vector2.zero, Vector2.one, new Vector2(10, 0), new Vector2(-10, 0));

        GameObject txtGO = CreateText(go, "Text", "", 16, Color.white);
        SetRect(txtGO, Vector2.zero, Vector2.one, new Vector2(10, 0), new Vector2(-10, 0));

        input.placeholder = phGO.GetComponent<TMP_Text>();
        input.textComponent = txtGO.GetComponent<TMP_Text>();
        if (isPassword) input.contentType = TMP_InputField.ContentType.Password;

        return go;
    }

    GameObject CreateToggle(GameObject parent, string name, string label)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        Toggle toggle = go.AddComponent<Toggle>();

        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(go.transform, false);
        Image bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.3f);
        SetRect(bgGO, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(10, 0), new Vector2(20, 20));

        GameObject checkGO = new GameObject("Checkmark");
        checkGO.transform.SetParent(bgGO.transform, false);
        Image checkImg = checkGO.AddComponent<Image>();
        checkImg.color = new Color(0.4f, 0.8f, 0.4f);
        SetRect(checkGO, Vector2.zero, Vector2.one, new Vector2(2, 2), new Vector2(-4, -4));

        GameObject lblGO = CreateText(go, "Label", label, 14, Color.white);
        SetRect(lblGO, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(30, 0), new Vector2(0, 20));
        lblGO.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        toggle.targetGraphic = bgImg;
        toggle.graphic = checkImg;
        return go;
    }

    void AddOutline(GameObject go, Color color)
    {
        var o = go.AddComponent<Outline>();
        o.effectColor = color;
        o.effectDistance = new Vector2(2, 2);
    }

    void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
    }

    void SetFullScreen(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // Unico metodo per assegnare i campi privati (niente duplicati!)
    void Assign(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName,
            BindingFlags.NonPublic | BindingFlags.Instance);
        if (field != null) field.SetValue(target, value);
        else Debug.LogWarning($"[LoginSceneBuilder] Campo non trovato: {fieldName}");
    }
}