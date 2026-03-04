using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb;

    private void Awake(){
        rb = GetComponent<Rigidbody2D>();

    }

    private void Update(){
        movement.Set(InputManager.Movement.x,InputManager.Movement.y);

        rb.linearVelocity = movement * moveSpeed;
    }

}
