using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour 
{
    public static Vector2 Movement; 
    public static bool Attack;
    public static bool ToggleInventory;
    public static bool Interact;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction inventoryAction;
    private InputAction interactAction;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
        inventoryAction = playerInput.actions["Inventory"];
        interactAction = playerInput.actions["Interact"];
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void Update()
    {   
        if (playerHealth == null || playerHealth.isDead)
        {
            Movement = Vector2.zero;
            Attack = false;
            ToggleInventory = false;
            Interact = false;
            return;
        }

        Movement = moveAction.ReadValue<Vector2>();
        Attack = attackAction.triggered;
        ToggleInventory = inventoryAction.triggered;
        Interact = interactAction.triggered;

        if (Movement != Vector2.zero || Attack || ToggleInventory)
        {
            if (TerminalManager.Istanza != null)
                TerminalManager.Istanza.ResetProgressoGiocatore();
        }
    }
}