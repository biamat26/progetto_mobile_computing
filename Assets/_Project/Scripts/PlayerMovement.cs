using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        // Prende il componente fisico all'inizio
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Legge WASD e le Frecce automaticamente:
        // A/Freccia Sinistra = -1 | D/Freccia Destra = 1
        // W/Freccia Su = 1 | S/Freccia Giù = -1
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        // Normalizza: evita che il quadrato corra come un pazzo in diagonale
        moveInput = moveInput.normalized;
    }

    void FixedUpdate()
    {
        // Muove il corpo fisico
        rb.linearVelocity = moveInput * moveSpeed;
    }
}