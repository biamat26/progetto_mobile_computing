using UnityEngine;
using UnityEngine.UI;

public class BarraVitaCuori : MonoBehaviour
{
    [Header("Configurazione")]
    public int vitaMassima = 5;
    public int vitaAttuale;

    [Header("Sprite")]
    public Sprite cuoricinoPieno;
    public Sprite cuoricinoVuoto;

    [Header("Setup UI")]
    public Transform contenitoreCuori; // il parent con HorizontalLayoutGroup
    public GameObject prefabCuoricino; // un semplice GameObject con Image, da istanziare N volte

    private Image[] cuoriUI;

    void Awake()
    {
        vitaAttuale = vitaMassima;
        CreaCuori();
        AggiornaVisuale();
    }

    private void CreaCuori()
    {
        cuoriUI = new Image[vitaMassima];

        for (int i = 0; i < vitaMassima; i++)
        {
            GameObject nuovoCuore = Instantiate(prefabCuoricino, contenitoreCuori);
            cuoriUI[i] = nuovoCuore.GetComponent<Image>();
        }
    }

    public void ModificaVita(int quantita)
    {
        vitaAttuale = Mathf.Clamp(vitaAttuale + quantita, 0, vitaMassima);
        AggiornaVisuale();

        if (vitaAttuale <= 0)
        {
            // Gestisci qui la morte del player
            Debug.Log("Player morto!");
        }
    }

    public void SetVita(int nuovaVita)
    {
        vitaAttuale = Mathf.Clamp(nuovaVita, 0, vitaMassima);
        AggiornaVisuale();
    }

    private void AggiornaVisuale()
    {
        for (int i = 0; i < cuoriUI.Length; i++)
        {
            cuoriUI[i].sprite = (i < vitaAttuale) ? cuoricinoPieno : cuoricinoVuoto;
        }
    }
}