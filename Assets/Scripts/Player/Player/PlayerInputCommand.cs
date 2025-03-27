using UnityEngine;

public class PlayerInputCommand : MonoBehaviour
{
    public bool JumpPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    public void OnJumpButtonPressed() => JumpPressed = true;
    public void OnDashButtonPressed() => DashPressed = true;
    public void OnAttackButtonPressed() => AttackPressed = true;

    public void ResetInputs()
    {
        JumpPressed = false;
        DashPressed = false;
        AttackPressed = false;
    }
}
