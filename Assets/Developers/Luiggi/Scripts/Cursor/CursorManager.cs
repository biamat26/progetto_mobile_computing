using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        // Nasconde la noiosa freccetta bianca di Windows/Mac
        Cursor.visible = false; 
    }

    private void Update()
    {
        // Se il mouse è collegato, sposta l'immagine alla sua posizione
        if (Mouse.current != null)
        {
            rectTransform.position = Mouse.current.position.ReadValue();
        }
    }
}