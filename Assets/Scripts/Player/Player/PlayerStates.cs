using UnityEngine;

public class IdleState : IPlayerState
{
    private PlayerController player;

    public IdleState(PlayerController player) { this.player = player; }

    public void Enter()
    { 
        Debug.Log("Idle 상태 시작");
    }

    public void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        var input = player.inputCommand;

        if (moveInput != 0) player.ChangeState(new MoveState(player));
        else if (Input.GetKeyDown(KeyCode.Space)) player.ChangeState(new JumpState(player));
        else if (Input.GetKeyDown(KeyCode.LeftShift) && player.canDash) player.ChangeState(new DashState(player));
        else if (Input.GetKeyDown(KeyCode.A)) player.ChangeState(new AttackState(player));
    }

    public void Exit() => Debug.Log("Idle 상태 종료");
    public override string ToString() => "Idle";
}

public class MoveState : IPlayerState
{
    private PlayerController player;
    public MoveState(PlayerController player) { this.player = player; }

    public void Enter()
    {
        player.SetToBeforeSpeed();
        Debug.Log("Move 상태 시작");
    }

    public void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        player.SetMoveInput(moveInput);

        if (moveInput == 0) player.ChangeState(new IdleState(player));
        else if (Input.GetKeyDown(KeyCode.Space)) player.ChangeState(new JumpState(player));
        else if (Input.GetKeyDown(KeyCode.A)) player.ChangeState(new AttackState(player));
        else if (Input.GetKeyDown(KeyCode.LeftShift) && player.canDash) player.ChangeState(new DashState(player));
    }

    public void Exit()
    {
        player.SaveBeforeSpeed();
        Debug.Log("Move 상태 종료");
    }

    public override string ToString() => "Move";
}

public class JumpState : IPlayerState
{
    private PlayerController player;

    public JumpState(PlayerController player) { this.player = player; }

    public void Enter()
    {
        Debug.Log("Jump 상태 시작");

        player.StartJump();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && player.canDash)
        {
            player.ChangeState(new DashState(player));
            return;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            player.ChangeState(new JumpAttackState(player));
            return;
        }
        if (player.GetCurrentState() is DashState) return;


        if (Input.GetKey(KeyCode.Space))
        {
            player.ChargeJump();
        }

        else if (Input.GetKeyUp(KeyCode.Space))
        {
            player.ReleaseJump();
        }

        float moveInput = Input.GetAxisRaw("Horizontal");
        player.SetMoveInput(moveInput);

        //if (player.IsGrounded())
        //{
        //    player.ChangeState(new IdleState(player));
        //}


    }

    public void Exit()
    {
        Debug.Log("Jump 상태 종료");
        player.ResetJump();
    }

    public override string ToString() => "Jump";
}


public class DashState : IPlayerState
{
    private PlayerController player;
    public DashState(PlayerController player) { this.player = player; }

    public void Enter()
    {
        Debug.Log("Dash 상태 시작");
        player.StartDash();
    }

    public void Update()
    {

    }

    public void Exit()
    {
        Debug.Log("Dash 상태 종료");
    }
    public override string ToString() => "Dash";
}

public class AttackState : IPlayerState
{
    private PlayerController player;

    public AttackState(PlayerController player) { this.player = player; }

    public void Enter()
    {
        Debug.Log("Attack 상태 시작");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) player.ChangeState(new JumpState(player));
        else if (Input.GetKeyDown(KeyCode.LeftShift)) player.ChangeState(new DashState(player));

        if (Input.GetKeyUp(KeyCode.A)) player.ChangeState(new IdleState(player));

        player.PlayerNormalAttack();

    }

    public void Exit() => Debug.Log("Attack 상태 종료");
    public override string ToString() => "Attack";
}

public class JumpAttackState : IPlayerState
{
    private PlayerController player;


    public JumpAttackState(PlayerController player) { this.player = player; }

    public void Enter()
    {
        Debug.Log("JumpAttack 상태 시작");
        player.PlayerJumpAttack();
    }

    public void Update()
    {

    }

    public void Exit()
    {
        Debug.Log("JumpAttack 상태 종료");
        player.EndJumpAttack();
        player.ResetMovement();
    }
    public override string ToString() => "JumpAttack";
}
