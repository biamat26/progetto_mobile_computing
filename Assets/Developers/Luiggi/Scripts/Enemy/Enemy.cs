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

    // NUOVA VARIABILE: Serve per dire al radar cosa considerare "Ostacolo/Muro"
    [Header("Pathfinding")]
    public LayerMask obstacleLayer; 

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

        if (player != null)
            playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (enemyHealth != null && enemyHealth.currentHP <= 0) return;

        if (player == null || playerHealth == null || playerHealth.isDead)
        {
            movement = Vector2.zero;
            UpdateAnimation(Vector2.zero);
            return;
        }

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
            movement = Vector2.zero;
            rb2.linearVelocity = Vector2.zero;
            nextAttackTime = Time.time + attackCoolDown;
            StartCoroutine(PerformAttack());
        }
        else if (distance >= attackRange)
        {
            // --- SISTEMA ANTI-INCASTRO (RADAR) ---
            // Creiamo un cerchio invisibile davanti al nemico per rilevare i muri
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.4f, direction, 0.5f, obstacleLayer);

            if (hit.collider != null)
            {
                // Se stiamo per sbattere, calcoliamo la direzione parallela al muro (tangente)
                Vector2 slideDirection = new Vector2(-hit.normal.y, hit.normal.x);

                // Assicuriamoci di scivolare verso il player e non di allontanarci
                if (Vector2.Dot(slideDirection, direction) < 0)
                {
                    slideDirection = -slideDirection;
                }

                // Sostituiamo la direzione verso il player con la direzione lungo il muro
                direction = slideDirection.normalized;
            }

            movement = direction;
        }
        else
        {
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

    public void HitPlayer()
    {
        if (playerHealth == null || playerHealth.isDead) return;
        if (!IsPlayerInsideEngageDistance()) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange + 0.5f)
        {
            playerHealth.TakeDamage(damage);
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