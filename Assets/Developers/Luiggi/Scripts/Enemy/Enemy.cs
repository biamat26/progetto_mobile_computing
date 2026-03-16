using UnityEngine;

public class Enemy : MonoBehaviour
{   
    [Header("Settings")]
    public Transform player;
    public float moveSpeed = 3f;
    public float attackRange = 1.8f; // Prova ad alzarlo un po' se ti sembra ancora lento
    public float attackCoolDown = 1.5f;
    public int damage = 20;

    private float nextAttackTime = 0f;
    private Rigidbody2D rb2;
    private Animator animator;
    private Vector2 movement;

    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
    private static readonly int LastVertical = Animator.StringToHash("LastVertical");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Blocca la rotazione (Z) e prepara l'attacco subito
        rb2.constraints = RigidbodyConstraints2D.FreezeRotation;
        nextAttackTime = Time.time; 
    }

    void Update()
    {
        if (player == null || !player.GetComponent<SpriteRenderer>().enabled) 
        {
            movement = Vector2.zero;
            rb2.linearVelocity = Vector2.zero; // Si ferma se il player muore
            UpdateAnimation(Vector2.zero);
            return;
        }

        float distance = Mathf.Abs(transform.position.x - player.position.x);
        Vector2 direction = (player.position - transform.position).normalized;

        if (distance < attackRange)
        {
            movement = Vector2.zero;
            rb2.linearVelocity = Vector2.zero; // FIX: Si inchioda sul posto per attaccare

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCoolDown;
            }
        }
        else
        {
            movement = direction;
        }

        UpdateAnimation(movement);
    }

    void FixedUpdate()
    {
        // Applica velocità solo se non siamo fermi ad attaccare
        if (movement != Vector2.zero) {
            rb2.linearVelocity = movement * moveSpeed;
        } else {
            rb2.linearVelocity = Vector2.zero;
        }
    }

    void UpdateAnimation(Vector2 dir)
    {
        animator.SetFloat(Horizontal, dir.x);
        animator.SetFloat(Vertical, dir.y);
        if (dir != Vector2.zero)
        {
            animator.SetFloat(LastHorizontal, dir.x);
            animator.SetFloat(LastVertical, dir.y);
        }
    }

    void Attack()
    {
        animator.SetTrigger(AttackTrigger);
    }

    // FUNZIONE PER ANIMATION EVENT (Mettila nel frame dell'attacco!)
    public void HitPlayer()
    {
        if (player == null) return;

        float distance = Mathf.Abs(transform.position.x - player.position.x);

        if (distance <= attackRange + 0.5f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Danno inflitto con successo!");
            }
        }
    }
}