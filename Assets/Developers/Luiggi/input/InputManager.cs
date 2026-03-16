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
    public static bool Attack;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
    }

    private void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
        Attack = attackAction.triggered;
    }
    
}
