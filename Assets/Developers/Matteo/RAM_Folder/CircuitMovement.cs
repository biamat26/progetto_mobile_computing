using System.Collections;
using UnityEngine;

public class CircuitMovement : MonoBehaviour
{
    public float speed = 5f;
    public float changeDirectionTime = 0.5f; // Ogni quanti secondi curva

    private Vector3 currentDirection;

    void Start()
    {
        // Parte scegliendo una direzione a caso tra le 4 possibili (Su, Giù, Destra, Sinistra)
        ChooseRandom90DegreeDirection(true);
        StartCoroutine(MoveInCircuit());
    }

    void Update()
    {
        // Si muove dritto nella direzione attuale
        transform.Translate(currentDirection * speed * Time.deltaTime);
    }

    IEnumerator MoveInCircuit()
    {
        while (true)
        {
            yield return new WaitForSeconds(changeDirectionTime);
            
            // Quando scade il tempo, decide se svoltare a 90 gradi
            ChooseRandom90DegreeDirection(false);
        }
    }

    void ChooseRandom90DegreeDirection(bool isStart)
    {
        // Se stiamo andando in orizzontale, svoltiamo in verticale (e viceversa)
        if (isStart || currentDirection.x != 0) 
        {
            // Vai Su o Giù
            currentDirection = Random.value > 0.5f ? Vector3.up : Vector3.down;
        }
        else 
        {
            // Vai Destra o Sinistra
            currentDirection = Random.value > 0.5f ? Vector3.right : Vector3.left;
        }
    }
}