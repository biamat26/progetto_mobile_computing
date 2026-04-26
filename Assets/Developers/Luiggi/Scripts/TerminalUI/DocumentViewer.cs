using UnityEngine;
using UnityEngine.UI;

public class DocumentViewer : MonoBehaviour
{
    public static DocumentViewer Istanza;

    public Image immagine; // trascina qui la Image figlia

    void Awake()
    {
        Istanza = this;
        gameObject.SetActive(false); // nasconde tutta la canvas all'avvio
    }

    public void MostraImmagine(Sprite sprite)
    {
        immagine.sprite = sprite;
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Chiudi();
    }

    public void Chiudi()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}