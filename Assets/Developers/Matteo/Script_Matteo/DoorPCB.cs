using System.Collections;
using UnityEngine;

/// <summary>
/// Porta PCB che si smaterializza in particelle di pixel.
///
/// Setup Unity:
///   1. Importa "door_disintegrate_spritesheet.png"
///      - Sprite Mode: Multiple
///      - Pixels Per Unit: 16
///      - Sprite Editor → Slice → Grid by Cell Size → W:64 H:96 → Apply
///      Ottieni 10 sprite: _0 (chiusa) ... _9 (sparita)
///
///   2. Crea GameObject "Door" con SpriteRenderer + BoxCollider2D
///   3. Attacca questo script
///   4. Trascina i 10 sprite nell'array Frames in ordine
///   5. Chiama door.Open() quando il player completa i task
/// </summary>
public class DoorPCB : MonoBehaviour
{
    [Header("Animazione")]
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float fps = 10f;

    private SpriteRenderer _sr;
    private Collider2D _col;
    private bool _animating = false;
    private bool _isOpen = false;

    private void Awake()
    {
        _sr  = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();

        if (frames != null && frames.Length > 0)
            _sr.sprite = frames[0];
    }

    public void Open()
    {
        if (_animating || _isOpen) return;
        StartCoroutine(Disintegrate());
    }

    private IEnumerator Disintegrate()
    {
        _animating = true;
        float delay = 1f / fps;

        // Piccolo flash cyan prima di smaterializzarsi
        _sr.color = new Color(0.5f, 1f, 1f, 1f);
        yield return new WaitForSeconds(0.08f);
        _sr.color = Color.white;
        yield return new WaitForSeconds(0.05f);

        // Riproduci i frame uno per uno
        foreach (var frame in frames)
        {
            _sr.sprite = frame;
            yield return new WaitForSeconds(delay);
        }

        // Porta sparita — disabilita collider e renderer
        _isOpen = true;
        _animating = false;

        if (_col != null) _col.enabled = false;
        _sr.enabled = false;
    }

    // Ripristina la porta (utile per reset scena)
    public void Reset()
    {
        if (frames != null && frames.Length > 0)
            _sr.sprite = frames[0];
        _sr.enabled = true;
        if (_col != null) _col.enabled = true;
        _sr.color = Color.white;
        _isOpen = false;
        _animating = false;
    }
}
