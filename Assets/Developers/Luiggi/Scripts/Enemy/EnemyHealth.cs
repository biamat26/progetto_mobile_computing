using UnityEngine;
using System.Collections;
public class EnemyHealth : MonoBehaviour
{
    public int maxHP = 50;
    public int currentHP;
    
    private Animator anim;
    private Rigidbody2D rb;
    private bool isDead = false;

    [SerializeField] private float invulnerabilityDuration = 0.5f;
    private bool isInvulnerable = false;
    void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
{
    if (isDead || isInvulnerable) return;

    currentHP -= damage;
    Debug.Log(gameObject.name + " HP: " + currentHP);

    if (currentHP <= 0)
        Die();
    else
        StartCoroutine(BecomeInvulnerable());
}

private IEnumerator BecomeInvulnerable()
{
    PlayHurtAnimation();
    isInvulnerable = true;
    yield return new WaitForSeconds(invulnerabilityDuration);
    isInvulnerable = false;
}

    private void PlayHurtAnimation(){
        if(anim!=null){
            // Passiamo le direzioni al Blend Tree del danno
            float lastH = anim.GetFloat("LastHorizontal");
            float lastV = anim.GetFloat("LastVertical");
            anim.SetFloat("Horizontal", lastH);
            anim.SetFloat("Vertical", lastV);

            // Lanciamo il trigger
            anim.SetTrigger("isHurt");
        }
    }

    private void Die()
    {
        isDead = true;

        // 1. Recupera l'ultima direzione dal nemico per il Blend Tree
        float lastH = anim.GetFloat("LastHorizontal");
        float lastV = anim.GetFloat("LastVertical");
        anim.SetFloat("Horizontal", lastH);
        anim.SetFloat("Vertical", lastV);

        // 2. Attiva l'animazione (visto che il tuo trigger si chiama isDead)
        anim.SetTrigger("isDead");

        // 3. Disabilita l'intelligenza artificiale (lo script Enemy)
        if (GetComponent<Enemy>()) GetComponent<Enemy>().enabled = false;

        // 4. Ferma la fisica
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        // 5. Togli il collider così non blocca il player da morto
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

        // 6. Distruggi il nemico solo dopo che l'animazione è finita (es. dopo 2 secondi)
        Destroy(gameObject, 2f);
    }
}