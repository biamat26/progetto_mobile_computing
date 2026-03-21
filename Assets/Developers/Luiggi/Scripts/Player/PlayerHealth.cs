using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public GameoverUI gameOverUI;

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

        float lastH = anim.GetFloat("LastHorizontal");
        anim.SetFloat("Horizontal", lastH);

        PlayerMovement mov = GetComponent<PlayerMovement>();
        if (mov != null) mov.enabled = false;

        sprite.flipX = false; 

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

        anim.SetTrigger("Die");

        if (gameOverUI != null)
            StartCoroutine(ShowGameOverDelayed(gameOverUI));
        else
            Debug.LogError("gameOverUI non assegnato nell'Inspector!");
    }

    private IEnumerator ShowGameOverDelayed(GameoverUI ui)
    {
        Debug.Log("ShowGameOverDelayed avviato, aspetto 1.2s...");
        yield return new WaitForSeconds(1.2f);
        Debug.Log("Chiamo ui.Show()...");
        ui.Show();
    }
}