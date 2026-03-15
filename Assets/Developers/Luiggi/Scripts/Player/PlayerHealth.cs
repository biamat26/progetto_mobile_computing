using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int hp = 100;
    public float invulnerabilityDuration = 1.0f;
    private bool isInvulnerable = false;
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int qt)
    {
        if (isInvulnerable) return;

        hp -= qt;
        Debug.Log($"Danno ricevuto! HP: {hp}");

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(BecomeInvulnerable());
        }
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
        Debug.Log("GAME OVER");

        sprite.enabled = false;
        if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;
        
        // FIX: Ferma il movimento fisico alla morte
        if (GetComponent<Rigidbody2D>()) GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

        Invoke("RestartLevel", 2f);
    }

    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}