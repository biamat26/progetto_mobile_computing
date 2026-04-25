using UnityEngine;

/// <summary>
/// Mettilo sul GameObject dei computer.
/// Quando il player preme E vicino al computer, apre il WirePuzzleManager.
/// </summary>
public class ComputerInteraction : MonoBehaviour
{
    [Header("Riferimenti")]
    [SerializeField] private WirePuzzleManager puzzleManager;
    [SerializeField] private GameObject tooltipInteragisci;

    private bool playerVicino = false;

    void Awake()
    {
        if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
    }

    void Update()
    {
        if (playerVicino && Input.GetKeyDown(KeyCode.E))
        {
            if (!puzzleManager.IsSolved())
                puzzleManager.OpenPuzzle();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerVicino = true;
        if (tooltipInteragisci && !puzzleManager.IsSolved())
            tooltipInteragisci.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerVicino = false;
        if (tooltipInteragisci) tooltipInteragisci.SetActive(false);
    }
}
