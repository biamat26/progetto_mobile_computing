using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 4; // coincide con il numero di cuoricini
    public GameoverUI gameOverUI;
    public HealthBar healthBar;
    public BarraVitaCuori barraVita;

    [Header("Audio Morte")]
    [Tooltip("Inserisci qui il suono o la musica del Game Over")]
    public AudioClip suonoMorte;
    [Tooltip("L'AudioSource attaccato al Player (se ne hai uno, altrimenti lascialo vuoto)")]
    public AudioSource playerAudioSource;

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
        _isDead = false;
        currentHP = maxHP;
        healthBar.SetMaxHealth(maxHP); 
        if (barraVita != null) {
            barraVita.gameObject.SetActive(true);
            barraVita.SetVita(currentHP);
        }
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();   

        if (gameOverUI == null)
        {
            gameOverUI = Object.FindFirstObjectByType<GameoverUI>();
        }
    }

    public void Heal(int amount)
    {
        if (_isDead) return;
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        healthBar.SetHealth(currentHP);
        if (barraVita != null) barraVita.SetVita(currentHP);
    }

    public void TakeDamage(int qt)
    {
        if (isInvulnerable || _isDead) return;   

        currentHP -= qt;
        healthBar.SetHealth(currentHP);
        if (barraVita != null) barraVita.SetVita(currentHP);
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
        healthBar.gameObject.SetActive(false);
        if (barraVita != null){
            barraVita.SetVita(0);
            barraVita.gameObject.SetActive(false); // <-- nasconde i cuoricini
        }
        if (_isDead) return;
        _isDead = true;

        // --- NUOVO: GESTIONE AUDIO DI MORTE ---
        
        // 1. Fermiamo lo script SceneAudioController per evitare che rimetta in play la musica
        SceneAudioController sceneAudio = Object.FindFirstObjectByType<SceneAudioController>();
        if (sceneAudio != null)
        {
            sceneAudio.StopAllCoroutines();
            sceneAudio.enabled = false;
        }

        // 2. Stoppiamo qualsiasi musica o suono attualmente in riproduzione nell'AudioManager globale
        if (AudioManager.instance != null)
        {
            AudioSource[] sorgenti = AudioManager.instance.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource s in sorgenti)
            {
                s.Stop();
            }
        }

        // 3. Facciamo partire il suono di Game Over
        if (suonoMorte != null)
        {
            if (playerAudioSource != null)
            {
                playerAudioSource.PlayOneShot(suonoMorte); // Usa l'AudioSource del player se l'hai assegnato
            }
            else
            {
                // Se non hai assegnato un AudioSource, Unity creerà un suono "volante" sulla telecamera
                AudioSource.PlayClipAtPoint(suonoMorte, Camera.main.transform.position, 1f); 
            }
        }
        // --------------------------------------

        // NUOVO: svuota l'inventario alla morte
        if (InventorySystem.Instance != null)
            InventorySystem.Instance.ClearInventory();

        Debug.Log($"[PLAYER DIE] chiamato! _isDead={_isDead}");
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

        // Disabilita completamente terminale e inventario
        if (TerminalManager.Istanza != null)
        {
            TerminalManager.Istanza.enabled = false;
        }

        InventoryToggle invToggle = Object.FindFirstObjectByType<InventoryToggle>();
        if (invToggle != null)
        {
            invToggle.enabled = false;
        }

        // Nascondi anche gli oggetti UI per sicurezza
        if (TerminalManager.Istanza != null && TerminalManager.Istanza.terminalRect != null)
        {
            TerminalManager.Istanza.terminalRect.gameObject.SetActive(false);
        }

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