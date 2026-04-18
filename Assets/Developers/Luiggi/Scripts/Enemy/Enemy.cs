using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{   
    [Header("Settings")]
    public Transform player;
    public float moveSpeed = 3f;
    public float engageDistance = 6f;
    public float attackRange = 0.8f;
    public float attackCoolDown = 1.5f;
    public int damage = 20;

    private float nextAttackTime = 0f;
    private Rigidbody2D rb2;
    private Animator animator;
    private Vector2 movement;
    private bool isAttacking = false;
    private EnemyHealth enemyHealth;
    private PlayerHealth playerHealth;
    private EnemyEngagementManager engagementManager;

    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
    private static readonly int LastVertical = Animator.StringToHash("LastVertical");
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        engagementManager = EnemyEngagementManager.Instance;
        rb2.constraints = RigidbodyConstraints2D.FreezeRotation;
        nextAttackTime = Time.time;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        // Cache del PlayerHealth al posto di cercarlo ogni frame
        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        // Morto = fermo
        if (enemyHealth != null && enemyHealth.currentHP <= 0) return;

        // Controlla isDead invece di SpriteRenderer.enabled
        if (player == null || playerHealth == null || playerHealth.isDead)
        {
            movement = Vector2.zero;
            UpdateAnimation(Vector2.zero);
            return;
        }

        // Mentre attacca, NESSUN calcolo di movimento
        if (isAttacking) return;

        bool canEngage = IsPlayerInsideEngageDistance();
        if (!canEngage)
        {
            movement = Vector2.zero;
            UpdateAnimation(Vector2.zero);
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        if (distance < attackRange && Time.time >= nextAttackTime)
        {
            // FIX: reset movimento PRIMA di entrare nella coroutine
            movement = Vector2.zero;
            rb2.linearVelocity = Vector2.zero;

            // FIX: imposta nextAttackTime PRIMA di lanciare la coroutine
            // così non può partire una seconda coroutine nel mezzo
            nextAttackTime = Time.time + attackCoolDown;
            StartCoroutine(PerformAttack());
        }
        else if (distance >= attackRange)
        {
            // Si muove SOLO se è fuori range — niente drifting residuo
            movement = direction;
        }
        else
        {
            // In range ma cooldown non ancora scaduto: aspetta fermo
            movement = Vector2.zero;
        }

        UpdateAnimation(movement);
    }

    void FixedUpdate()
    {
        if (isAttacking || movement == Vector2.zero)
            rb2.linearVelocity = Vector2.zero;
        else
            rb2.linearVelocity = movement * moveSpeed;
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;

        movement = Vector2.zero;
        rb2.linearVelocity = Vector2.zero;
        UpdateAnimation(Vector2.zero);

        animator.SetTrigger(AttackTrigger);

        yield return new WaitForSeconds(0.5f);

        animator.ResetTrigger(AttackTrigger);
        isAttacking = false;
    }

    // Chiamato dall'Animation Event
    public void HitPlayer()
    {
        if (playerHealth == null || playerHealth.isDead) return;
        if (!IsPlayerInsideEngageDistance()) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.5f)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Danno inflitto con successo dal Virus!");
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

    private bool IsPlayerInsideEngageDistance()
    {
        if (player == null) return false;

        if (engagementManager != null)
            return engagementManager.IsInsideEngageDistance(transform.position, player.position, engageDistance);

        float engageDistanceSafe = Mathf.Max(0f, engageDistance);
        return (player.position - transform.position).sqrMagnitude <= engageDistanceSafe * engageDistanceSafe;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, engageDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}