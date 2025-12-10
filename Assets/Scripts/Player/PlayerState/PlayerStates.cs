using UnityEditor.AddressableAssets.Build;
using UnityEngine;

public class IdleState : IPlayerState
{
    private PlayerController player;

    public IdleState(PlayerController player) { this.player = player; }
    private PlayerInputProxy input;
    public void Enter()
    {
        player.anim.PlayAnimation("Idle");
        input = player.input;
        player.canJump = true;

    }

    public void Update()
    {

        float moveInput = player.GetMoveInput();
        if (player.isOnPlatform)
        {




            // 플랫폼 위에 있을때
            if (player.GetDirection() == PlayerInputDirection.Down ||
                player.GetDirection() == PlayerInputDirection.DownRight ||
                player.GetDirection() == PlayerInputDirection.DownLeft)
            {
                Debug.Log("아래방향");
                if (Input.GetKeyDown(KeyCode.Space) || input.JumpPressed)
                {
                    player.isDownJumping = true;
                    player.isOnPlatform = false;
                    player.nowPlatform.GetComponent<BoxCollider2D>().isTrigger = true;
                    player.isJumping = true;
                    player.ChangeState(new JumpState(player));
                    return;
                }
            }
        }

        if (moveInput != 0) player.ChangeState(new MoveState(player));
        else if (Input.GetKeyDown(KeyCode.Space) || input.JumpPressed) player.ChangeState(new JumpState(player));
        else if ((Input.GetKeyDown(KeyCode.LeftShift) || input.DashPressed) && player.canDash) player.ChangeState(new DashState(player));
        else if (Input.GetKeyDown(KeyCode.A) || input.AttackPressed) player.ChangeState(new AttackState(player));
    }

    public void Exit()
    {

    }
    public override string ToString() => "Idle";
}

public class MoveState : IPlayerState
{
    private PlayerController player;
    public MoveState(PlayerController player) { this.player = player; }

    public void Enter()
    {
        if (player.MoveInput > 0)
        {
            player.CharacterSprite.flipX = false;
        }
        else
        {
            player.CharacterSprite.flipX = true;
        }
        player.anim.PlayAnimation("Move");
        SoundManager.Instance.Play("PlayerSound/PlayerMove", Sound.LoopEffect);
    }

    public void Update()
    {
        if (!player.anim.animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
        {
            player.anim.PlayAnimation("Move");
        }
        
        float moveInput = player.GetMoveInput();
        var input = player.input;

        player.SetMoveInput(moveInput);

        if (!SoundManager.Instance.isLoopEffectPlaying())
        {
            SoundManager.Instance.Play("PlayerSound/PlayerMove", Sound.LoopEffect);
        }

        if (player.isOnPlatform)
        {

            Debug.Log(player.GetDirection());
            // 플랫폼 위에 있을때
            if (player.GetDirection() == PlayerInputDirection.Down ||
                player.GetDirection() == PlayerInputDirection.DownRight ||
                player.GetDirection() == PlayerInputDirection.DownLeft)
            {

                if (Input.GetKeyDown(KeyCode.Space) || input.JumpPressed)
                {
                    player.isDownJumping = true;
                    player.isOnPlatform = false;
                    player.nowPlatform.GetComponent<BoxCollider2D>().isTrigger = true;
                    player.isJumping = true;
                    player.ChangeState(new JumpState(player));
                    return;
                }
            }
        }

        if (moveInput == 0) player.ChangeState(new IdleState(player));
        else if (Input.GetKeyDown(KeyCode.Space) || input.JumpPressed) player.ChangeState(new JumpState(player));
        else if (Input.GetKeyDown(KeyCode.A) || input.AttackPressed) player.ChangeState(new AttackState(player));
        else if ((Input.GetKeyDown(KeyCode.LeftShift) || input.DashPressed)&& player.canDash) player.ChangeState(new DashState(player));
    }

    public void Exit()
    {

        SoundManager.Instance.StopLoopEffect();


    }

    public override string ToString() => "Move";
}

public class JumpState : IPlayerState
{
    private PlayerController player;

    public JumpState(PlayerController player) { this.player = player; }

    public void Enter()
    {

        if (player.isDownJumping || player.isFallingFromPlatform ||player.GetJumpingDash())
        {
            return;
        }
            
            
        player.StartJump();
        player.anim.PlayAnimation("JumpUp");
        SoundManager.Instance.Play("PlayerSound/PlayerJump");
    }

    public void Update()
    {
        var input = player.input;

        if (player.MoveInput != 0)
        {
            if (player.MoveInput > 0)
            {
                player.CharacterSprite.flipX = false;
            }
            else
            {
                player.CharacterSprite.flipX = true;
            }
        }
        
        if (player.GetFallingVelocity() < 0)
        {
            player.anim.PlayAnimation("JumpDown");
        }

        if ((Input.GetKeyDown(KeyCode.A) || input.AttackPressed) && player.CanJumpAttack())
        {
            player.ChangeState(new JumpAttackState(player));
            return;
        }
        if ((Input.GetKeyDown(KeyCode.LeftShift) || input.DashPressed) && player.canDash)
        {
            player.ChangeState(new DashState(player));
            return;
        }

        if (player.isDownJumping || player.GetJumpingDash() || player.isFallingFromPlatform)
            return;




        if (player.GetCurrentState() is DashState) return;


        if (Input.GetKey(KeyCode.Space) || input.JumpHeld)
        {
            player.ChargeJump();
        }

        else if (Input.GetKeyUp(KeyCode.Space) || input.JumpReleased)
        {
            player.ReleaseJump();
        }

        float moveInput = player.GetMoveInput();
        player.SetMoveInput(moveInput);

    }

    public void Exit()
    {
        //Debug.Log("Jump 상태 종료");
        
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
        player.StartDash();
    }

    public void Update()
    {

    }

    public void Exit()
    {
    }
    public override string ToString() => "Dash";
}

public class AttackState : IPlayerState
{
    private PlayerController player;
    private PlayerLookingDirection attackDirection;
    public AttackState(PlayerController player) { this.player = player; }
    private PlayerInputProxy input;
    public void Enter()
    {
        player.anim.PlayAnimationCrossFade("NormalAttack");
        player.isAttacking = true;
        input = player.input;
        input.OnAttackButtonDown();
    }

    public void Update()
    {


        if (Input.GetKeyDown(KeyCode.LeftShift) || input.DashPressed) player.ChangeState(new DashState(player));



        if (Input.GetKeyUp(KeyCode.A) || input.AttackReleased)
        {

            if (player.MoveInput != 0)
            {
                player.ChangeState(new MoveState(player));
            }
            else
                player.ChangeState(new IdleState(player));
        }



        player.PlayerNormalAttack(player.GetLookingDirection());

    }

    public void Exit()
    {
        var input = player.input;
        player.isAttacking = false;
        input.ResetAttackFlags();
    }
    public override string ToString() => "Attack";
}

public class JumpAttackState : IPlayerState
{
    private PlayerController player;


    public JumpAttackState(PlayerController player) { this.player = player; }

    public void Enter()
    {
        player.StartJumpAttack();
    }

    public void Update()
    {

    }

    public void Exit()
    {
        Time.timeScale = 1f;
        player.EndJumpAttack();
        //player.ResetMovement();
        player.PlayerJumpAttackObjectDisable();
    }
    public override string ToString() => "JumpAttack";
}