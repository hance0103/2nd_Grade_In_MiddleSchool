using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    private Rigidbody2D rb;

    [SerializeField] IPlayerState nowState;

    [Header("Move")]
    [SerializeField] private float nowSpeed = 0f;
    [SerializeField] private float maxSpeed = 5f;         // 최고 속도
    [SerializeField] private float moveAccel;

    private float moveInput = 0f;

    [Header("Jump")]
    [SerializeField] private float minJumpForce = 5f;  // 최소 점프 힘
    [SerializeField] private float maxJumpForce = 10f; // 최대 점프 힘
    [SerializeField] private float maxJumpHeight = 3f; // 최대 점프 높이
    [SerializeField] private float maxChargeTime = 0.5f; // 최대 점프 충전 시간

    private bool spaceReleased = false;
    private bool isJumping = false;
    private float jumpStartY;
    private float jumpTimer = 0f;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashBeforeDelay = 0.1f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashTime;

    [SerializeField] private float diagonalDashX;
    [SerializeField] private float diagonalDashY;

    private float dashCooldownTimer = 0f;


    public bool canDash { get; set; }
    private Vector2 dashStartPos;
    private Vector2 dashDirection;

    private Vector2 dashBeforeVelocity;

    [Header("Direction")]
    [SerializeField] private PlayerInputDirection direction;
    [SerializeField] private PlayerLookingDirection looking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new PlayerStateMachine();
        stateMachine.ChangeState(new IdleState(this));
        direction = PlayerInputDirection.None;
        looking = PlayerLookingDirection.Right;
        canDash = true;

        
    }

    private void Update()
    {
        direction = GetInputDirection();
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void ApplyMovement()
    {
        if (stateMachine.GetCurrentState() is DashState) return;
        if (moveInput != 0)
        {
            nowSpeed += moveAccel * Time.deltaTime;
            nowSpeed = Mathf.Min(nowSpeed, maxSpeed);
        }
        else
        {
            nowSpeed = 0;
        }
        Vector3 movement = new Vector3(moveInput * nowSpeed * Time.deltaTime, 0, 0);
        transform.position += movement;
    }

    public void SetMoveInput(float input)
    {
        moveInput = input;
    }

    public void StartJump()
    {
        if (stateMachine.GetPreviousState() is DashState)
        {
            isJumping = true;
            return;
        }

        if (!isJumping)
        {
            isJumping = true;
            spaceReleased = false;
            jumpStartY = transform.position.y;
            jumpTimer = 0f;
            Jump(minJumpForce);
        }
    }

    public void ChargeJump()
    {
        if (!isJumping) return;

        jumpTimer += Time.deltaTime;

        if (transform.position.y - jumpStartY >= maxJumpHeight)
        {
            isJumping = false;
            return;
        }

        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, jumpTimer / maxChargeTime);
        Jump(jumpForce);
    }

    public void ReleaseJump()
    {
        if (!isJumping) return;

        spaceReleased = true;
        isJumping = false;
    }

    public void Jump(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    public void ResetJump()
    {
        isJumping = false;
        spaceReleased = false;
    }

    public void StopMovement()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void StartDash()
    {
        canDash = false;
        dashStartPos = transform.position;

        dashBeforeVelocity = rb.velocity;

        // 방향계산 넣어야함
        dashDirection = Vector2.zero;
        switch (direction)
        {
            case PlayerInputDirection.Up:
                dashDirection = new Vector2(0, dashDistance);
                break;
            case PlayerInputDirection.Down:
                dashDirection = new Vector2(0, -dashDistance);
                break;
            case PlayerInputDirection.Right:
                dashDirection = new Vector2(dashDistance, 0);
                break;
            case PlayerInputDirection.Left:
                dashDirection = new Vector2(-dashDistance, 0);
                break;
            case PlayerInputDirection.UpRight:
                dashDirection = new Vector2(diagonalDashX, diagonalDashY);
                break;
            case PlayerInputDirection.UpLeft:
                dashDirection = new Vector2(-diagonalDashX, diagonalDashY);
                break;
            case PlayerInputDirection.DownRight:
                dashDirection = new Vector2(diagonalDashX, -diagonalDashY);
                break;
            case PlayerInputDirection.DownLeft:
                dashDirection = new Vector2(-diagonalDashX, -diagonalDashY);
                break;
            case PlayerInputDirection.None:
                switch (looking)
                {
                    case PlayerLookingDirection.Right:
                        dashDirection = new Vector2(dashDistance, 0);
                        break;
                    case PlayerLookingDirection.Left:
                        dashDirection = new Vector2(-dashDistance, 0);
                        break;
                }
                break;
        }

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        StartCoroutine(DashCoroutine(dashDirection));
    }
    private IEnumerator DashCoroutine(Vector2 direction)
    {
        float dashBeforeDelayCounter = 0f;
        while (dashBeforeDelayCounter <= dashBeforeDelay)
        {
            rb.velocity = Vector2.zero;
            dashBeforeDelayCounter += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(Dash(direction));
        StartCoroutine(PlayerDashCoolDown());
    }
    private IEnumerator Dash(Vector2 direction)
    {
        Debug.Log("대시 시작");

        Vector2 dashStartPos = rb.position;
        Vector2 dashEndPos = rb.position + direction;
        dashTime = 0f;
        Debug.Log(dashStartPos);
        Debug.Log(dashEndPos);


        while (dashTime < dashDuration)
        {
            yield return new WaitForFixedUpdate();

            Debug.Log("대시 진행중");

            dashTime += Time.deltaTime;
            float t = dashTime / dashDuration;
            Vector2 newPosition = Vector2.Lerp(dashStartPos, dashEndPos, t);
            rb.MovePosition(newPosition);

        }
        Debug.Log("대시 끝");
        rb.velocity = dashBeforeVelocity;
        rb.gravityScale = 4;
        stateMachine.RestorePreviousState();
    }
    private IEnumerator PlayerDashCoolDown()
    {
        canDash = false;
        dashCooldownTimer = 0f;
        while (dashCooldownTimer <= dashCooldown)
        {
            dashCooldownTimer += Time.deltaTime;
            yield return null;
        }
        canDash = true;
    }

    public void ChangeState(IPlayerState newState)
    {
        stateMachine.ChangeState(newState);
    }
    public PlayerInputDirection GetInputDirection()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0)
            return PlayerInputDirection.None;
        if (horizontal > 0)
        {
            looking = PlayerLookingDirection.Right;
            if (vertical > 0) return PlayerInputDirection.UpRight;
            else if (vertical < 0) return PlayerInputDirection.DownRight;
            else return PlayerInputDirection.Right;
        }
        else if (horizontal < 0)
        {
            looking = PlayerLookingDirection.Left;
            if (vertical > 0) return PlayerInputDirection.UpLeft;
            else if (vertical < 0) return PlayerInputDirection.DownLeft;
            else return PlayerInputDirection.Left;
        }


        if (vertical > 0) return PlayerInputDirection.Up;
        if (vertical < 0) return PlayerInputDirection.Down;

        return PlayerInputDirection.None;
    }
    public IPlayerState GetCurrentState()
    {
        return stateMachine.GetCurrentState();
    }
    public void RestorePreviousState()
    {
        stateMachine.RestorePreviousState();
    }
    public bool IsGrounded()
    {
        return rb.velocity.y == 0;
    }
}
