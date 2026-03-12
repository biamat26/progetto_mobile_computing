using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveDir;
    private float timer;
    private bool isMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            // Sceglie un tempo a caso tra 1.5 e 4 secondi per la prossima azione
            timer = Random.Range(1.5f, 4f);
            
            // Alterna tra stare fermo e muoversi
            isMoving = !isMoving; 

            if (isMoving)
            {
                // Sceglie una direzione a caso
                moveDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                // Aggiorna LastHorizontal/Vertical così l'animator sa dove guardare
                anim.SetFloat("LastHorizontal", moveDir.x);
                anim.SetFloat("LastVertical", moveDir.y);
            }
            else
            {
                moveDir = Vector2.zero;
            }
        }

        // Invia i dati all'Animator (ATTENZIONE alle maiuscole, devono essere uguali all'Animator)
        anim.SetFloat("Horizontal", moveDir.x);
        anim.SetFloat("Vertical", moveDir.y);
        
        // Questo risolve l'errore "Parameter Speed does not exist" 
        // (Assicurati di aver creato il parametro Float "Speed" nell'Animator!)
        anim.SetFloat("Speed", isMoving ? 1f : 0f); 
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }
}