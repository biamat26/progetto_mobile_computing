using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;


// Aggiungi ": MonoBehaviour" qui dietro
public class InputManager : MonoBehaviour 
{
    public static Vector2 Movement; 

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction attackAction;

    private PlayerHealth playerHealth;
    public static bool Attack;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void Update()
    {   
        if (playerHealth == null || playerHealth.isDead) return;
        Movement = moveAction.ReadValue<Vector2>();
        Attack = attackAction.triggered;
    }
    
}
