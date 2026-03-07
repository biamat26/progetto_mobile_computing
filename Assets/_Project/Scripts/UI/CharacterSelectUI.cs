using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ================================================================
// CharacterSelectUI.cs
// Posizione: Assets/Scripts/UI/
//
// SETUP IN UNITY:
// 1. Attacca questo script al Canvas della CharacterSelectScene
// 2. Crea tanti Button quanti sono i personaggi
// 3. Trascina i riferimenti negli slot dell'Inspector
// ================================================================

public class CharacterSelectUI : MonoBehaviour
{
    [Header("Personaggi disponibili")]
    [SerializeField] private Sprite[]   characterSprites;   // Sprite di ogni personaggio
    [SerializeField] private string[]   characterNames;     // Nomi dei personaggi
    [SerializeField] private Button[]   characterButtons;   // Bottoni di selezione

    [Header("Anteprima personaggio selezionato")]
    [SerializeField] private Image      previewImage;       // Immagine grande anteprima
    [SerializeField] private TMP_Text   previewName;        // Nome personaggio
    [SerializeField] private Animator   previewAnimator;    // Animazione idle (opzionale)

    [Header("Pulsanti")]
    [SerializeField] private Button     btnConferma;
    [SerializeField] private Button     btnIndietro;

    [Header("Colori selezione")]
    [SerializeField] private Color colorSelezionato = new Color(0.4f, 0.8f, 0.4f);
    [SerializeField] private Color colorNormale     = Color.white;

    private int selectedIndex = 0;

    private void Start()
    {
        // Collega ogni bottone personaggio
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i; // Cattura per la lambda
            characterButtons[i].onClick.AddListener(() => SelectCharacter(index));
        }

        btnConferma.onClick.AddListener(OnConfermaClicked);
        btnIndietro.onClick.AddListener(OnIndietroClicked);

        // Seleziona il personaggio che aveva scelto prima (se c'è)
        int savedIndex = UserSession.Instance?.SelectedCharacterIndex ?? 0;
        SelectCharacter(savedIndex);
    }

    private void SelectCharacter(int index)
    {
        if (index < 0 || index >= characterSprites.Length) return;

        AudioManager.Instance?.PlayClick();
        selectedIndex = index;

        // Aggiorna anteprima
        if (previewImage != null)  previewImage.sprite = characterSprites[index];
        if (previewName  != null)  previewName.text    = characterNames[index];

        // Evidenzia bottone selezionato
        for (int i = 0; i < characterButtons.Length; i++)
        {
            var img = characterButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == index) ? colorSelezionato : colorNormale;
        }
    }

    private void OnConfermaClicked()
    {
        AudioManager.Instance?.PlaySuccess();

        // Salva scelta nella sessione
        if (UserSession.Instance != null)
            UserSession.Instance.SelectedCharacterIndex = selectedIndex;

        Debug.Log($"[CharacterSelectUI] Personaggio scelto: {characterNames[selectedIndex]}");
        GameManager.Instance.GoToGame();
    }

    private void OnIndietroClicked()
    {
        AudioManager.Instance?.PlayClick();
        GameManager.Instance.GoToMainMenu();
    }
}