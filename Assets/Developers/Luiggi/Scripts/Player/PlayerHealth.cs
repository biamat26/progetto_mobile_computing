using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;

    private int currentHP;
    public float invulnerabilityDuration = 1.0f;
    private bool isInvulnerable = false;
    private SpriteRenderer sprite;
    public bool isDead = false;
    private Animator anim;

    void Start()
    {
        currentHP = maxHP;
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();   
    }

    public void TakeDamage(int qt)
    {
        if (isInvulnerable || isDead) return;   

        currentHP -= qt;
        Debug.Log($"Danno ricevuto! HP: {currentHP}");

        if (currentHP <= 0)
            Die();
        else
            StartCoroutine(BecomeInvulnerable());
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;

        for (float i = 0; i < invulnerabilityDuration; i += 0.2f)
        {
            sprite.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.1f);
            sprite.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        isInvulnerable = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("GAME OVER");

        // 1. IMPORTANTE: Recuperiamo l'ultima direzione dal movimento prima di fermarci
        float lastH = anim.GetFloat("LastHorizontal");
        // Forziamo il parametro Horizontal affinché il Blend Tree scelga il lato giusto
        anim.SetFloat("Horizontal", lastH);

        // 2. Disabilita lo script di movimento così non sovrascrive più i parametri
        PlayerMovement mov = GetComponent<PlayerMovement>();
        if (mov != null) mov.enabled = false;

        // 3. Resetta il flip (fondamentale se usi animazioni direzionali diverse)
        sprite.flipX = false; 

        // 4. Gestione Fisica
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

        // 5. Trigger Animazione
        anim.SetTrigger("Die");

        Invoke(nameof(RestartLevel), 2f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}