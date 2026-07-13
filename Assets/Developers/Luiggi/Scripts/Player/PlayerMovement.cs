using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public int damage = 40;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public float attackCooldown = 0.5f; // <-- NUOVO
    private bool isAttacking = false;
    private float nextAttackTime = 0f; // <-- NUOVO

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip swordAttackSound;

    private const string attack = "Attack";
    private const string horizontal = "Horizontal";
    private const string vertical = "Vertical";
    private const string lastHorizontal = "LastHorizontal";
    private const string lastVertical = "LastVertical";

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DealDamage()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        
        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth eh = enemy.GetComponent<EnemyHealth>(); 
            
            if (eh != null)
            {
                Debug.Log("COLPITO: " + enemy.name + " - Script EnemyHealth trovato!");
                eh.TakeDamage(damage);
            }
            else 
            {
                Debug.Log("Ho colpito " + enemy.name + " ma NON ha lo script EnemyHealth!");
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        if (audioSource != null && swordAttackSound != null)
        {
            audioSource.PlayOneShot(swordAttackSound);
        }

        animator.SetTrigger(attack);

        yield return new WaitForSeconds(0.45f); 

        animator.ResetTrigger(attack);
        
        isAttacking = false;
    }

    private void Update()
    {
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        movement.Set(InputManager.Movement.x, InputManager.Movement.y);
        rb.linearVelocity = movement * moveSpeed;

        animator.SetFloat(horizontal, movement.x);
        animator.SetFloat(vertical, movement.y);

        if (movement != Vector2.zero)
        {
            animator.SetFloat(lastHorizontal, movement.x);
            animator.SetFloat(lastVertical, movement.y);

            UpdateAttackPoint(movement.x, movement.y);

            if (movement.x < 0) spriteRenderer.flipX = true;
            else if (movement.x > 0) spriteRenderer.flipX = false;
        }

        // MODIFICATO: aggiunto controllo cooldown
        if (InputManager.Attack && !isAttacking && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(PerformAttack());
        }
    }

    private void UpdateAttackPoint(float x, float y)
    {
        if (attackPoint == null) return;

        float offset = 0.7f; 
        attackPoint.localPosition = new Vector3(x * offset, y * offset, 0);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}