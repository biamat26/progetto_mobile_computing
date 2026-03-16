using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    public int maxHP=50;
    public int currentHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     currentHP = maxHP;   
    }

    public void TakeDamage(int damage){
        currentHP-=damage;
        Debug.Log(gameObject.name + " HP: " + currentHP);

        if(currentHP <= 0){
            Die();
        }
    }

    // Update is called once per frame
    private void Die(){
        Destroy(gameObject);
    }
}
