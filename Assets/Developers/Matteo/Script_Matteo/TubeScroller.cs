using UnityEngine;

/// <summary>
/// Scorre le pareti del tubo in loop creando l'illusione di movimento.
/// Attach a un GameObject "TubeScroller" nella scena.
///
/// Setup pareti:
///   - Crea due sprite lunghi orizzontali per top e bottom wall
///   - Devono essere abbastanza larghi da coprire lo schermo x2
///   - Assegna i due Transform nei campi wallTop e wallBottom
///   - La texture deve avere Wrap Mode = Repeat per lo scroll UV
/// </summary>
public class TubeScroller : MonoBehaviour
{
    [Header("Pareti")]
    [SerializeField] private Transform wallTop;
    [SerializeField] private Transform wallBottom;
    [SerializeField] private float scrollSpeed = 4f;

    [Header("Reset loop")]
    [Tooltip("Quando la parete si è spostata di questa distanza, torna all'origine")]
    [SerializeField] private float loopWidth = 20f;

    private Vector3 _topStart;
    private Vector3 _botStart;

    private void Start()
    {
        if (wallTop    != null) _topStart = wallTop.position;
        if (wallBottom != null) _botStart = wallBottom.position;
    }

    private void Update()
    {
        if (wallTop != null)
        {
            wallTop.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
            if (wallTop.position.x < _topStart.x - loopWidth)
                wallTop.position = _topStart;
        }

        if (wallBottom != null)
        {
            wallBottom.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
            if (wallBottom.position.x < _botStart.x - loopWidth)
                wallBottom.position = _botStart;
        }
    }
}
