using UnityEngine;

public class BurloneTrigger : MonoBehaviour
{
    [Header("Riferimento al Dialogo")]
    [Tooltip("Trascina qui l'oggetto che contiene lo script BurloneDialogue")]
    public BurloneDialogue scriptDialogo;

    [Header("UI Interazione")]
    [Tooltip("Trascina qui l'oggetto della scritta 'Interagisci' (es. un testo nel Canvas)")]
    public GameObject scrittaInteragisci;

    private bool giocatoreVicino = false;

    void Start()
    {
        // Appena parte il gioco, ci assicuriamo che la scritta sia nascosta
        if (scrittaInteragisci != null)
        {
            scrittaInteragisci.SetActive(false);
        }
    }

    void Update()
    {
        // Se il giocatore è vicino e preme il tasto E
        if (giocatoreVicino && Input.GetKeyDown(KeyCode.E))
        {
            if (scriptDialogo != null)
            {
                scriptDialogo.AvviaDialogo();
                
                giocatoreVicino = false; 

                // Mentre parlano, nascondiamo la scritta "interagisci"
                if (scrittaInteragisci != null)
                {
                    scrittaInteragisci.SetActive(false);
                }
            }
        }
    }

    // Quando il giocatore ENTRA nell'area
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            giocatoreVicino = true;
            Debug.Log("Trigger attivato! Il giocatore è vicino."); // <--- AGGIUNGI QUESTO
            
            if (scrittaInteragisci != null)
            {
                scrittaInteragisci.SetActive(true);
            }
        }
    }

    // Quando il giocatore ESCE dall'area
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            giocatoreVicino = false;
            
            // Spegne la scritta!
            if (scrittaInteragisci != null)
            {
                scrittaInteragisci.SetActive(false);
            }
        }
    }
}