using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public int hp = 100;


    // Update is called once per frame
    public void TakeDamage(int qt){
        
        hp -= qt;

        Debug.Log("Il player ha subito danno !");
    
        if(hp <= 0){
            Die();
        }
    }

    public void Die(){
        Debug.Log("Hai perso! GAME OVER");
    }

}
