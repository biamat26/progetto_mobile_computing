using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

// ================================================================
// MainMenuBuilderEditor.cs
// Posizione: Assets/Editor/  ← IMPORTANTE: deve stare in Editor!
//
// COME USARLO:
// 1. Metti questo file in Assets/Editor/
// 2. In Unity vai nel menu in cima: Tools → Costruisci MainMenu
// 3. La UI viene creata senza premere Play!
// 4. Salva con CMD+S
// ================================================================

public class MainMenuBuilderEditor : MonoBehaviour
{
    [MenuItem("Tools/Costruisci MainMenu")]
    static void BuildMainMenu()
    {
        // Pulisce la scena dagli oggetti UI vecchi
        var oldCanvas = GameObject.Find("Canvas");
        if (oldCanvas != null) DestroyImmediate(oldCanvas);
        var oldES = GameObject.Find("EventSystem");
        if (oldES != null) DestroyImmediate(oldES);

        // ── CANVAS ──────────────────────────────────────────────
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        canvasGO.AddComponent<GraphicRaycaster>();

        // EventSystem
        GameObject esGO = new GameObject("EventSystem");
        esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
        esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // ── SFONDO ──────────────────────────────────────────────
        GameObject bg = CreatePanel(canvasGO, "Background", new Color(0.08f, 0.08f, 0.12f));
        SetFullScreen(bg);

        GameObject topBar = CreatePanel(canvasGO, "TopBar", new Color(0.15f, 0.15f, 0.25f));
        SetRect(topBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -40), new Vector2(0, 80));

        GameObject botBar = CreatePanel(canvasGO, "BottomBar", new Color(0.15f, 0.15f, 0.25f));
        SetRect(botBar, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 40), new Vector2(0, 80));

        // ── TITOLO ──────────────────────────────────────────────
        GameObject title = CreateText(canvasGO, "TitleText", "PIXEL QUEST", 56, Color.white);
        SetRect(title, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -80), new Vector2(700, 80));

        GameObject subtitle = CreateText(canvasGO, "SubtitleText", "Un'avventura pixel art ti aspetta", 20, new Color(0.6f, 0.6f, 0.8f));
        SetRect(subtitle, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -130), new Vector2(500, 40));

        // ── NOME UTENTE ──────────────────────────────────────────
        GameObject txtBenvenuto = CreateText(canvasGO, "TxtBenvenuto", "Ciao, Giocatore!", 18, new Color(0.8f, 0.8f, 1f));
        SetRect(txtBenvenuto, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-160, -35), new Vector2(280, 40));
        txtBenvenuto.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineRight;

        // ── BOTTONI ──────────────────────────────────────────────
        GameObject panelButtons = CreatePanel(canvasGO, "PanelButtons", new Color(0, 0, 0, 0));
        SetRect(panelButtons, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(340, 320));

        GameObject btnGioca = CreateButton(panelButtons, "BtnGioca", "GIOCA", new Color(0.2f, 0.6f, 0.2f));
        SetRect(btnGioca, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(300, 65));

        GameObject btnImpostazioni = CreateButton(panelButtons, "BtnImpostazioni", "IMPOSTAZIONI", new Color(0.2f, 0.3f, 0.6f));
        SetRect(btnImpostazioni, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20), new Vector2(300, 65));

        GameObject btnEsci = CreateButton(panelButtons, "BtnEsci", "ESCI", new Color(0.5f, 0.15f, 0.15f));
        SetRect(btnEsci, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -60), new Vector2(300, 65));

        // ── PANNELLO IMPOSTAZIONI ────────────────────────────────
        GameObject panelSettings = CreatePanel(canvasGO, "PanelSettings", new Color(0, 0, 0, 0.85f));
        SetFullScreen(panelSettings);

        GameObject settingsBox = CreatePanel(panelSettings, "SettingsBox", new Color(0.12f, 0.12f, 0.2f));
        SetRect(settingsBox, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(450, 420));

        CreateText(settingsBox, "SettingsTitle", "IMPOSTAZIONI", 28, Color.white)
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 165);

        CreateText(settingsBox, "LabelMusica", "Volume Musica", 18, new Color(0.7f, 0.7f, 1f))
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(-60, 95);

        GameObject txtMusica = CreateText(settingsBox, "TxtVolumeMusica", "100%", 18, Color.white);
        SetRect(txtMusica, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(155, 95), new Vector2(70, 30));

        GameObject sliderMusica = CreateSlider(settingsBox, "SliderMusica");
        SetRect(sliderMusica, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 55), new Vector2(360, 30));

        CreateText(settingsBox, "LabelSFX", "Volume Effetti", 18, new Color(0.7f, 0.7f, 1f))
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(-55, 10);

        GameObject txtSFX = CreateText(settingsBox, "TxtVolumeSFX", "100%", 18, Color.white);
        SetRect(txtSFX, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(155, 10), new Vector2(70, 30));

        GameObject sliderSFX = CreateSlider(settingsBox, "SliderSFX");
        SetRect(sliderSFX, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -30), new Vector2(360, 30));

        GameObject toggleMuto = CreateToggle(settingsBox, "ToggleMuto", "Disattiva audio");
        SetRect(toggleMuto, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -85), new Vector2(300, 35));

        GameObject btnChiudi = CreateButton(settingsBox, "BtnChiudiSettings", "CHIUDI", new Color(0.4f, 0.1f, 0.1f));
        SetRect(btnChiudi, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -160), new Vector2(200, 50));

        // NASCONDE il pannello impostazioni
        panelSettings.SetActive(false);

        // ── COLLEGA MainMenuUI ───────────────────────────────────
        MainMenuUI mainMenuUI = canvasGO.AddComponent<MainMenuUI>();
        Assign(mainMenuUI, "btnGioca",        btnGioca.GetComponent<Button>());
        Assign(mainMenuUI, "btnImpostazioni", btnImpostazioni.GetComponent<Button>());
        Assign(mainMenuUI, "btnEsci",         btnEsci.GetComponent<Button>());
        Assign(mainMenuUI, "panelSettings",   panelSettings);
        Assign(mainMenuUI, "txtBenvenuto",    txtBenvenuto.GetComponent<TMP_Text>());

        // ── COLLEGA SettingsUI ───────────────────────────────────
        SettingsUI settingsUI = settingsBox.AddComponent<SettingsUI>();
        Assign(settingsUI, "sliderMusica",    sliderMusica.GetComponent<Slider>());
        Assign(settingsUI, "txtVolumeMusica", txtMusica.GetComponent<TMP_Text>());
        Assign(settingsUI, "sliderSFX",       sliderSFX.GetComponent<Slider>());
        Assign(settingsUI, "txtVolumeSFX",    txtSFX.GetComponent<TMP_Text>());
        Assign(settingsUI, "toggleMuto",      toggleMuto.GetComponent<Toggle>());
        Assign(settingsUI, "btnChiudi",       btnChiudi.GetComponent<Button>());

        // Registra per undo e salva
        Undo.RegisterCreatedObjectUndo(canvasGO, "Crea MainMenu UI");
        Undo.RegisterCreatedObjectUndo(esGO, "Crea EventSystem");

        Debug.Log("[MainMenuBuilderEditor] UI creata! Salva con CMD+S");
    }

    // ================================================================
    // HELPER
    // ================================================================
    static GameObject CreatePanel(GameObject parent, string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = color;
        return go;
    }

    static GameObject CreateText(GameObject parent, string name, string text, int size, Color color)
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

    static GameObject CreateButton(GameObject parent, string name, string label, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = color;
        go.AddComponent<Button>();
        GameObject lbl = CreateText(go, "Label", label, 20, Color.white);
        SetRect(lbl, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        return go;
    }

    static GameObject CreateSlider(GameObject parent, string name)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();

        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(go.transform, false);
        bgGO.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.2f);
        SetRect(bgGO, new Vector2(0, 0.25f), new Vector2(1, 0.75f), Vector2.zero, Vector2.zero);

        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        fillArea.AddComponent<RectTransform>();
        SetRect(fillArea, new Vector2(0, 0.25f), new Vector2(1, 0.75f), new Vector2(-5, 0), new Vector2(-20, 0));

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        fill.AddComponent<Image>().color = new Color(0.3f, 0.5f, 1f);
        SetRect(fill, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(10, 0));

        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        handleArea.AddComponent<RectTransform>();
        SetRect(handleArea, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-20, 0));

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        SetRect(handle, new Vector2(0, 0), new Vector2(0, 1), Vector2.zero, new Vector2(20, 0));

        Slider slider = go.AddComponent<Slider>();
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handleImg;
        slider.value = 1f;

        return go;
    }

    static GameObject CreateToggle(GameObject parent, string name, string label)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.AddComponent<RectTransform>();
        Toggle toggle = go.AddComponent<Toggle>();

        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(go.transform, false);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.2f, 0.2f, 0.3f);
        SetRect(bgGO, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(12, 0), new Vector2(24, 24));

        GameObject checkGO = new GameObject("Checkmark");
        checkGO.transform.SetParent(bgGO.transform, false);
        var checkImg = checkGO.AddComponent<Image>();
        checkImg.color = new Color(1f, 0.3f, 0.3f);
        SetRect(checkGO, Vector2.zero, Vector2.one, new Vector2(2, 2), new Vector2(-4, -4));

        GameObject lblGO = CreateText(go, "Label", label, 16, Color.white);
        SetRect(lblGO, new Vector2(0, 0.5f), new Vector2(1, 0.5f), new Vector2(32, 0), new Vector2(-32, 28));
        lblGO.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.MidlineLeft;

        toggle.targetGraphic = bgImg;
        toggle.graphic = checkImg;
        return go;
    }

    static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
    }

    static void SetFullScreen(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static void Assign(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null) field.SetValue(target, value);
        else Debug.LogWarning($"Campo non trovato: {fieldName}");
    }
}