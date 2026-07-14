using UnityEngine;
using UnityEngine.UI;

public class DocumentViewer : MonoBehaviour
{
    public static DocumentViewer Istanza;

    public Image immagine; // trascina qui la Image figlia
    
    // Variabile segreta per ricordare la dimensione normale degli altri documenti
    private Vector2 dimensioneOriginale; 

    void Awake()
    {
        Istanza = this;
        gameObject.SetActive(false); // nasconde tutta la canvas all'avvio
        
        // Salviamo la dimensione standard che hai impostato su Unity
        if (immagine != null)
        {
            dimensioneOriginale = immagine.rectTransform.sizeDelta;
        }
    }

    // ORA RICEVE TUTTO L'ITEM, NON SOLO L'IMMAGINE
    public void MostraImmagine(ItemData item) 
    {
        immagine.sprite = item.immagineDocumento;

        // --- GESTIONE GRANDEZZA IMMAGINE ---
        if (item.usaDimensioniSpeciali)
        {
            // Applica le dimensioni giganti del tuo documento speciale
            immagine.rectTransform.sizeDelta = item.dimensioniSpeciali;
        }
        else
        {
            // Per tutti gli altri, ripristina la dimensione normale!
            immagine.rectTransform.sizeDelta = dimensioneOriginale;
        }
        // -----------------------------------

        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Q))
            Chiudi();
    }

    public void Chiudi()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}