using UnityEngine;

public class ScanlinesOverlay : MonoBehaviour
{
    [SerializeField] private float lineSpacing = 4f;
    [SerializeField] private float lineAlpha = 0.15f;

    private Texture2D lineTex;

    void Awake()
    {
        lineTex = new Texture2D(1, 1);
        lineTex.SetPixel(0, 0, new Color(0, 0, 0, lineAlpha));
        lineTex.Apply();
    }

    void OnGUI()
    {
        for (float y = 0; y < Screen.height; y += lineSpacing * 2)
        {
            GUI.DrawTexture(new Rect(0, y, Screen.width, lineSpacing), lineTex);
        }
    }
}