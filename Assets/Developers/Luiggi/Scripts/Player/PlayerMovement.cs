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
    private bool isAttacking = false;

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
        // Dobbiamo cercare lo script dove sta effettivamente TakeDamage
        EnemyHealth eh = enemy.GetComponent<EnemyHealth>(); 
        
        if (eh != null)
        {
            Debug.Log("COLPITO: " + enemy.name + " - Script EnemyHealth trovato!");
            eh.TakeDamage(damage);
        }
        else 
        {
            // Se finisci qui, significa che il Virus ha il collider ma ti sei dimenticato
            // di trascinargli sopra lo script EnemyHealth nell'Inspector
            Debug.Log("Ho colpito " + enemy.name + " ma NON ha lo script EnemyHealth!");
        }
    }
    }

    private IEnumerator PerformAttack()
{
    isAttacking = true;

    // Spara l'animazione
    animator.SetTrigger(attack);

    // ASPETTA: qui il tempo deve essere quasi uguale alla durata della tua clip
    // Se la clip dura 0.5 secondi, aspetta 0.45
    yield return new WaitForSeconds(0.45f); 

    // RESET: puliamo il trigger a mano prima di finire, così Any State non lo rivede
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

            // Chiamiamo la funzione per spostare l'AttackPoint
            UpdateAttackPoint(movement.x, movement.y);

            if (movement.x < 0) spriteRenderer.flipX = true;
            else if (movement.x > 0) spriteRenderer.flipX = false;
        }

        if (InputManager.Attack && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

// Questa funzione sposta l'AttackPoint in base alla direzione
    private void UpdateAttackPoint(float x, float y)
    {
        if (attackPoint == null) return;

        // Definisci quanto deve essere distante dal centro del player
        float offset = 0.7f; 
        
        // Sposta l'AttackPoint localmente rispetto al Player
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
