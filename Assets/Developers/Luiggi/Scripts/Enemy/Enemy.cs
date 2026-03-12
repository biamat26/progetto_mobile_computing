using UnityEngine;

public class Enemy : MonoBehaviour
{   
    public Transform player;

    public float moveSpeed = 3f;

    private float attackRange = 1.5f; 

    public float attackCoolDown = 1.5f;

    public float nextAttackTime = 0f;

    public int virus_hp = 50;

    public int damage = 30;

    private const string horizontal = "Horizontal";

    private const string vertical = "Vertical"; 

    private const string lastHorizontal = "LastHorizontal";

    private const string lastVertical = "LastVertical"; 
    public Vector2 movement;

    public Rigidbody2D rb2;

    private Animator animator;


    void Start(){
        rb2 = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void Update(){
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position,player.position);

        //se distanza è minore del range in cui posso attaccare allora attacco
        if(distance < attackRange){
            movement = Vector2.zero;
            rb2.linearVelocity = Vector2.zero;

            if(Time.time >= nextAttackTime){
                Attack();
                nextAttackTime = Time.time + attackCoolDown;
            }
        }
        else{
            movement = direction;
            rb2.linearVelocity = movement * moveSpeed;
        }

        animator.SetFloat(horizontal,movement.x);
        animator.SetFloat(vertical,movement.y);

        if(movement != Vector2.zero){
            animator.SetFloat(lastHorizontal,movement.x);
            animator.SetFloat(lastVertical,movement.y);
        }
    }
    void Attack(){
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        animator.SetTrigger("Attack");
        if(playerHealth!=null){
            playerHealth.TakeDamage(damage);
            Debug.Log("Ti ho infettato!");
        }
    }
}