using UnityEngine;

public class NoKeyPopUp : MonoBehaviour
{
    [SerializeField] private GameObject canvas;

    public void Show()
    {
        canvas.SetActive(true);
        PauseManager.RequestPause();
    }

    public void Close()
    {
        canvas.SetActive(false);
        PauseManager.ReleasePause();
    }

    private void Update()
    {
        if (!canvas.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Close();
    }
}