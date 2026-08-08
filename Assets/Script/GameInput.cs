using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance {get; private set; }
    
    private PlayerInputActions _playerInputActions;

    public event EventHandler OnPlayerAttack;
    public event EventHandler OnPlayerDashAttack;
    public event EventHandler OnPlayerDash;
    private void Awake()
    {
        Instance = this;
        
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Combat.Attack.performed += Attack_performed;
        _playerInputActions.Combat.SuperAttack.performed += DashAttack_performed;
        _playerInputActions.Player.Dash.performed += PlayerDash_Performed;
    }

    private void PlayerDash_Performed(InputAction.CallbackContext obj)
    {
        OnPlayerDash?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_performed(InputAction.CallbackContext context)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }
    private void DashAttack_performed(InputAction.CallbackContext context)
    {
        OnPlayerDashAttack?.Invoke(this, EventArgs.Empty);
    }
    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public void DisableMovement()
    {
        _playerInputActions.Disable();
    }
}
