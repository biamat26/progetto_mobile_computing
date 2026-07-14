using UnityEngine;
using System.Collections;

public class ComputerALU : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private GameObject popupConDocumento;
    [SerializeField] private GameObject popupSenzaDocumento;
    [SerializeField] private WaveManager waveManager;
    [Header("Documento con Password (fine ondate)")]
[SerializeField] private GameObject documentoConPassword; // il prefab/oggetto da far apparire
[SerializeField] private Transform puntoUscitaDocumento; // dove "vola fuori" dal computer

    [Header("Documento richiesto")]
    [SerializeField] private string documentoRichiesto = "DocumentoALU";

    private bool playerVicino = false;
    private bool eventoAvviato = false;

    void Update()
    {
        if (playerVicino && !eventoAvviato && Input.GetKeyDown(KeyCode.E))
        {
            Interagisci();
        }
    }

    private void Interagisci()
    {
        if (InventorySystem.Instance == null) return;

        int indiceDocumento = -1;
        for (int i = 0; i < 16; i++)
        {
            var item = InventorySystem.Instance.GetItem(i);
            if (item != null && item.name == documentoRichiesto)
            {
                indiceDocumento = i;
                break;
            }
        }

        if (indiceDocumento != -1)
        {
            InventorySystem.Instance.RemoveItem(indiceDocumento);
            eventoAvviato = true;

            Time.timeScale = 0f;
            if (popupConDocumento) popupConDocumento.SetActive(true);
        }
        else
        {
            Time.timeScale = 0f;
            if (popupSenzaDocumento) popupSenzaDocumento.SetActive(true);
        }
    }

    // Da collegare al bottone "Chiudi" del popup CON documento
    public void ChiudiPopupConDocumento()
    {
        Time.timeScale = 1f;
        if (popupConDocumento) popupConDocumento.SetActive(false);

        if (waveManager != null)
            waveManager.StartWaves();
        else
            Debug.LogWarning("WaveManager non assegnato in ComputerALU!");
    }

    // Da collegare al bottone "Chiudi" del popup SENZA documento
    public void ChiudiPopupSenzaDocumento()
    {
        Time.timeScale = 1f;
        if (popupSenzaDocumento) popupSenzaDocumento.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || eventoAvviato) return;
        playerVicino = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerVicino = false;
    }

    public void OnOndateCompletate()
{
    Debug.Log("Tutte le ondate completate! Faccio apparire il documento con la password.");

    if (documentoConPassword != null && puntoUscitaDocumento != null)
    {
        GameObject doc = Instantiate(documentoConPassword, puntoUscitaDocumento.position, Quaternion.identity);
        StartCoroutine(AnimaVoloDocumento(doc));
    }
}

private IEnumerator AnimaVoloDocumento(GameObject doc)
{
    Vector3 partenza = doc.transform.position;
    Vector3 arrivo = partenza + new Vector3(0, 1.5f, 0); // vola verso l'alto di 1.5 unità

    float durata = 0.8f;
    float t = 0f;

    while (t < durata)
    {
        t += Time.deltaTime;
        doc.transform.position = Vector3.Lerp(partenza, arrivo, t / durata);
        yield return null;
    }
}
}
