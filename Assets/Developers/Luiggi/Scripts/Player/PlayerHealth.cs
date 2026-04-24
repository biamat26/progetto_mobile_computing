using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    public GameoverUI gameOverUI;
    public HealthBar healthBar;

    // L'ho messo public così lo vedi nell'Inspector e capisci se il danno funziona
    public int currentHP; 
    public float invulnerabilityDuration = 1.0f;
    private bool isInvulnerable = false;
    private SpriteRenderer sprite;
    private bool _isDead = false;
    public bool isDead => _isDead; // gli altri script possono leggerlo
    private Animator anim;

void Awake() 
    {
        _isDead = false; // ora non è serializzato, parte sempre false
        currentHP = maxHP;
        healthBar.SetMaxHealth(maxHP); 
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();   

        // Se non lo hai assegnato a mano, cercalo nella scena
    if (gameOverUI == null)
    {
        gameOverUI = Object.FindFirstObjectByType<GameoverUI>();
    }
    }

    public void TakeDamage(int qt)
    {
        if (isInvulnerable || _isDead) return;   

        currentHP -= qt;
        healthBar.SetHealth(currentHP);
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
            if (sprite != null) sprite.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.1f);
            if (sprite != null) sprite.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        isInvulnerable = false;
    }

    public void Die()
    {
        healthBar.SetHealth(0);
        if (_isDead) return;
        _isDead = true;
        Debug.Log($"[PLAYER DIE] chiamato! _isDead={_isDead}");
    // stack trace per capire chi lo chiama
        Debug.Log(System.Environment.StackTrace);
        Debug.Log("GAME OVER");

        // --- LO SCUDO ANTI CRASH CHE MANCAVA ---
        if (anim != null) 
        {
            float lastH = anim.GetFloat("LastHorizontal");
            anim.SetFloat("Horizontal", lastH);
            anim.SetTrigger("Die");
        } 
        else 
        {
            Debug.LogWarning("Sono morto ma l'Animator non c'è, quindi salto l'animazione per NON FAR CRASHARE IL GIOCO!");
        }
        // ---------------------------------------

        PlayerMovement mov = GetComponent<PlayerMovement>();
        if (mov != null) mov.enabled = false;

        if (sprite != null) sprite.flipX = false; 

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }

        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

        if (gameOverUI != null)
            StartCoroutine(ShowGameOverDelayed(gameOverUI));
        else
            Debug.LogWarning("gameOverUI non assegnato nell'Inspector!");
    }

    private IEnumerator ShowGameOverDelayed(GameoverUI ui)
    {
        Debug.Log("ShowGameOverDelayed avviato, aspetto 1.2s...");
        yield return new WaitForSeconds(1.2f);
        Debug.Log("Chiamo ui.Show()...");
        ui.Show();
    }
}