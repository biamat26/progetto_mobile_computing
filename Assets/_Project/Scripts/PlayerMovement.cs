using UnityEngine;

public class PlayerMovement : MonoBehaviour 
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        // Cerchiamo l'animator nel figlio (Skin_Renderer)
        anim = GetComponentInChildren<Animator>(); 
    }

    void Update() 
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput != Vector2.zero)
        {
            moveInput.Normalize();
            // Comunichiamo le direzioni all'Animator per il Blend Tree
            anim.SetFloat("Horizontal", moveInput.x);
            anim.SetFloat("Vertical", moveInput.y);
            anim.SetBool("isMoving", true);

            // Flip opzionale se non usi animazioni laterali separate
            if (moveInput.x != 0) {
                transform.localScale = new Vector3(moveInput.x > 0 ? 1 : -1, 1, 1);
            }
        }
        else
        {
            anim.SetBool("isMoving", false);
        }
    }

    void FixedUpdate() 
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}