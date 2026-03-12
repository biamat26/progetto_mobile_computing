using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ================================================================
// MainMenuSceneBuilder.cs
// Posizione: Assets/Scripts/UI/
//
// COME USARLO (una volta sola!):
// 1. Apri la MainMenuScene
// 2. Crea un GameObject vuoto → chiamalo "SceneBuilder"
// 3. Attacca questo script
// 4. Premi PLAY → la UI viene creata automaticamente
// 5. Premi CTRL+S per salvare
// 6. Premi STOP ed elimina il GameObject "SceneBuilder"
// ================================================================

public class MainMenuSceneBuilder : MonoBehaviour
{
    void Start()
    {
        BuildMainMenuScene();
        Destroy(this);
    }

    void BuildMainMenuScene()
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
        GameObject bg = CreatePanel(canvasGO, "Background", new Color(0.08f, 0.08f, 0.12f));
        SetFullScreen(bg);

        // Striscia decorativa in alto (stile pixel art)
        GameObject topBar = CreatePanel(canvasGO, "TopBar", new Color(0.15f, 0.15f, 0.25f));
        SetRect(topBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -40), new Vector2(0, 80));

        // Striscia decorativa in basso
        GameObject botBar = CreatePanel(canvasGO, "BottomBar", new Color(0.15f, 0.15f, 0.25f));
        SetRect(botBar, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 40), new Vector2(0, 80));

        // ── TITOLO ──────────────────────────────────────────────
        GameObject title = CreateText(canvasGO, "TitleText", "⚔ PIXEL QUEST ⚔", 56, Color.white);
        SetRect(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -80), new Vector2(700, 80));

        // Sottotitolo
        GameObject subtitle = CreateText(canvasGO, "SubtitleText", "Un'avventura pixel art ti aspetta", 20, new Color(0.6f, 0.6f, 0.8f));
        SetRect(subtitle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -130), new Vector2(500, 40));

        // ── NOME UTENTE (in alto a destra) ──────────────────────
        GameObject txtBenvenuto = CreateText(canvasGO, "TxtBenvenuto", "Ciao, Giocatore!", 18, new Color(0.8f, 0.8f, 1f));
        SetRect(txtBenvenuto, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-160, -35), new Vector2(280, 40));
        txtBenvenuto.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineRight;

        // ── PANNELLO CENTRALE CON I BOTTONI ─────────────────────
        GameObject panelButtons = CreatePanel(canvasGO, "PanelButtons", new Color(0, 0, 0, 0));
        SetRect(panelButtons, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(340, 320));

        // Bottone GIOCA
        GameObject btnGioca = CreateButton(panelButtons, "BtnGioca", "▶  GIOCA", new Color(0.2f, 0.6f, 0.2f));
        SetRect(btnGioca, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(300, 65));
        AddOutline(btnGioca, new Color(0.4f, 1f, 0.4f));

        // Bottone IMPOSTAZIONI
        GameObject btnImpostazioni = CreateButton(panelButtons, "BtnImpostazioni", "⚙  IMPOSTAZIONI", new Color(0.2f, 0.3f, 0.6f));
        SetRect(btnImpostazioni, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20), new Vector2(300, 65));
        AddOutline(btnImpostazioni, new Color(0.4f, 0.6f, 1f));

        // Bottone ESCI
        GameObject btnEsci = CreateButton(panelButtons, "BtnEsci", "✕  ESCI", new Color(0.5f, 0.15f, 0.15f));
        SetRect(btnEsci, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -60), new Vector2(300, 65));
        AddOutline(btnEsci, new Color(1f, 0.3f, 0.3f));

        // ── PANNELLO IMPOSTAZIONI (overlay) ─────────────────────
        GameObject panelSettings = CreatePanel(canvasGO, "PanelSettings", new Color(0, 0, 0, 0.85f));
        SetFullScreen(panelSettings);

        // Box impostazioni
        GameObject settingsBox = CreatePanel(panelSettings, "SettingsBox", new Color(0.12f, 0.12f, 0.2f));
        SetRect(settingsBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(450, 420));
        AddOutline(settingsBox, new Color(0.4f, 0.4f, 0.8f));

        // Titolo impostazioni
        CreateText(settingsBox, "SettingsTitle", "⚙ IMPOSTAZIONI", 28, Color.white)
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 165);

        // Label Musica
        CreateText(settingsBox, "LabelMusica", "🎵 Volume Musica", 18, new Color(0.7f, 0.7f, 1f))
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(-60, 95);

        // Testo % musica
        GameObject txtMusica = CreateText(settingsBox, "TxtVolumeMusica", "100%", 18, Color.white);
        SetRect(txtMusica, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(155, 95), new Vector2(70, 30));

        // Slider Musica
        GameObject sliderMusica = CreateSlider(settingsBox, "SliderMusica");
        SetRect(sliderMusica, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 55), new Vector2(360, 30));

        // Label SFX
        CreateText(settingsBox, "LabelSFX", "🔊 Volume Effetti", 18, new Color(0.7f, 0.7f, 1f))
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(-55, 10);

        // Testo % SFX
        GameObject txtSFX = CreateText(settingsBox, "TxtVolumeSFX", "100%", 18, Color.white);
        SetRect(txtSFX, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(155, 10), new Vector2(70, 30));

        // Slider SFX
        GameObject sliderSFX = CreateSlider(settingsBox, "SliderSFX");
        SetRect(sliderSFX, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -30), new Vector2(360, 30));

        // Toggle Muto
        GameObject toggleMuto = CreateToggle(settingsBox, "ToggleMuto", "🔇 Disattiva tutto l'audio");
        SetRect(toggleMuto, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -85), new Vector2(300, 35));

        // Bottone Chiudi impostazioni
        GameObject btnChiudi = CreateButton(settingsBox, "BtnChiudiSettings", "✕  CHIUDI", new Color(0.4f, 0.1f, 0.1f));
        SetRect(btnChiudi, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -160), new Vector2(200, 50));

        panelSettings.SetActive(false); // Nascosto di default

        // ── COLLEGA MainMenuUI ───────────────────────────────────
        MainMenuUI mainMenuUI = canvasGO.AddComponent<MainMenuUI>();
        SetPrivateField(mainMenuUI, "btnGioca",         btnGioca.GetComponent<Button>());
        SetPrivateField(mainMenuUI, "btnImpostazioni",  btnImpostazioni.GetComponent<Button>());
        SetPrivateField(mainMenuUI, "btnEsci",          btnEsci.GetComponent<Button>());
        SetPrivateField(mainMenuUI, "panelSettings",    panelSettings);
        SetPrivateField(mainMenuUI, "txtBenvenuto",     txtBenvenuto.GetComponent<TMP_Text>());

        // ── COLLEGA SettingsUI ───────────────────────────────────
        SettingsUI settingsUI = settingsBox.AddComponent<SettingsUI>();
        SetPrivateField(settingsUI, "sliderMusica",     sliderMusica.GetComponent<Slider>());
        SetPrivateField(settingsUI, "txtVolumeMusica",  txtMusica.GetComponent<TMP_Text>());
        SetPrivateField(settingsUI, "sliderSFX",        sliderSFX.GetComponent<Slider>());
        SetPrivateField(settingsUI, "txtVolumeSFX",     txtSFX.GetComponent<TMP_Text>());
        SetPrivateField(settingsUI, "toggleMuto",       toggleMuto.GetComponent<Toggle>());
        SetPrivateField(settingsUI, "btnChiudi",        btnChiudi.GetComponent<Button>());

        Debug.Log("[MainMenuSceneBuilder] ✅ MainMenu creato! Salva con CTRL+S");
    }

    // ================================================================
    // HELPER — Elementi UI
    // ================================================================
    GameObject CreatePanel(GameObject parent, string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        Image img = go.AddComponent<Image>();
        img.color = color;
        return go;
    }

    GameObject CreateText(GameObject parent, string name, string text, int size, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        TMP_Text tmp = go.AddComponent<TextMeshProUGUI>();
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
        Image img = go.AddComponent<Image>();
        img.color = color;
        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.highlightedColor = Color.Lerp(color, Color.white, 0.25f);
        cb.pressedColor     = Color.Lerp(color, Color.black, 0.25f);
        btn.colors = cb;
        GameObject lblGO = CreateText(go, "Label", label, 20, Color.white);
        SetRect(lblGO, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        return go;
    }

    GameObject CreateSlider(GameObject parent, string name)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();

        // Background
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(go.transform, false);
        Image bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.2f);
        SetRect(bgGO, new Vector2(0, 0.25f), new Vector2(1, 0.75f), Vector2.zero, Vector2.zero);

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        fillArea.AddComponent<RectTransform>();
        SetRect(fillArea, new Vector2(0, 0.25f), new Vector2(1, 0.75f), new Vector2(-5, 0), new Vector2(-20, 0));

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.3f, 0.5f, 1f);
        SetRect(fill, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(10, 0));

        // Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        SetRect(handleArea, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-20, 0));
        handleArea.AddComponent<RectTransform>();

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        SetRect(handle, new Vector2(0, 0), new Vector2(0, 1), Vector2.zero, new Vector2(20, 0));

        Slider slider = go.AddComponent<Slider>();
        slider.fillRect   = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handleImg;
        slider.value = 1f;
        slider.minValue = 0f;
        slider.maxValue = 1f;

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
        SetRect(bgGO, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(12, 0), new Vector2(24, 24));

        GameObject checkGO = new GameObject("Checkmark");
        checkGO.transform.SetParent(bgGO.transform, false);
        Image checkImg = checkGO.AddComponent<Image>();
        checkImg.color = new Color(1f, 0.3f, 0.3f);
        SetRect(checkGO, Vector2.zero, Vector2.one, new Vector2(2, 2), new Vector2(-4, -4));

        GameObject lblGO = CreateText(go, "Label", label, 16, Color.white);
        SetRect(lblGO, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(32, 0), new Vector2(-32, 28));
        lblGO.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        toggle.targetGraphic = bgImg;
        toggle.graphic = checkImg;
        return go;
    }

    void AddOutline(GameObject go, Color color)
    {
        Outline o = go.AddComponent<Outline>();
        o.effectColor = color;
        o.effectDistance = new Vector2(2, 2);
    }

    void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        RectTransform rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
    }

    void SetFullScreen(GameObject go)
    {
        RectTransform rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null) field.SetValue(target, value);
        else Debug.LogWarning($"[MainMenuSceneBuilder] Campo non trovato: {fieldName}");
    }
}