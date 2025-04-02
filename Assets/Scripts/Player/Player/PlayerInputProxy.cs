using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputProxy : MonoBehaviour
{
    public bool JumpPressed { get; private set; }   // GetKeyDown
    public bool JumpHeld { get; private set; }      // GetKey
    public bool JumpReleased { get; private set; }  // GetKeyUp

    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    private PlayerInputAction inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
    }
    void Update()
    {
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Jump.started += ctx => JumpPressed = true;
        inputActions.Player.Jump.performed += ctx => JumpPressed = true;
        inputActions.Player.Jump.canceled += ctx => JumpPressed = true;

        inputActions.Player.Dash.performed += ctx  => DashPressed = true;

        inputActions.Player.Attack.started += ctx  => AttackPressed = true;
        inputActions.Player.Attack.canceled += ctx => AttackPressed = true;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void ResetInputs()
    {
        JumpPressed = false;
        DashPressed = false;
        AttackPressed = false;
    }

    // 👉 UI 버튼에서 호출할 함수
    public void OnJumpButtonDown()
    {
        JumpPressed = true;
        JumpHeld = true;
        Debug.Log("Jump 버튼 눌림");
    }

    public void OnJumpButtonUp()
    {
        JumpReleased = true;
        JumpHeld = false;
        Debug.Log("Jump 버튼 뗌");
    }

    public void ResetJumpFlags()
    {
        JumpPressed = false;
        JumpReleased = false;
        // JumpHeld는 누르고 있는 동안 유지
    }
    public void OnDashButton() => DashPressed = true;
    public void OnAttackButton() => AttackPressed = true;
}
