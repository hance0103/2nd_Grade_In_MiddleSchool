using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

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

    [SerializeField] private float beforeSpeed = 0;
    private float moveInput = 0f;

    [Header("Jump")]
    [SerializeField] private float minChargeTime = 0.1f;
    [SerializeField] private float minJumpForce = 5f;  // 최소 점프 힘
    [SerializeField] private float maxJumpForce = 10f; // 최대 점프 힘
    [SerializeField] private float maxJumpHeight = 3f; // 최대 점프 높이
    [SerializeField] private float maxChargeTime = 0.5f; // 최대 점프 충전 시간

    private bool spaceReleased = false;
    private bool isJumping = false;
    private float jumpStartY;
    private float jumpTimer = 0f;
    private bool canJump;

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

    [Header("NormalAttack")]
    [SerializeField]
    private float _normalAttackDmg;        //데미지
    [SerializeField]
    private float _normalAttackSpeed;
    [SerializeField]
    private float _normalAttackRange;       //사거리
    [SerializeField]
    private float _normalAttack_delay;      //공격주기
    private float _nextFireTime = 0f;
    [SerializeField]
    private GameObject _normalAttackPrefab;
    [SerializeField]
    private Transform _attackPoint;

    [Header("JumpAttack")]
    [SerializeField]
    private float _jumpAttack_diveVelocity;
    [SerializeField]
    private float _jumpAttack_dmg;
    public float _jumpAttackHitDelay;
    [SerializeField]
    private GameObject _jumpAttackObject;

    private float _jumpAttackObjX;
    private float _jumpAttackObjY;


    [Header("PlayerHit")]
    [SerializeField]
    private float bindTimer = 1f;

    public GameObject enemy;

    [SerializeField]
    private ControllerUI controller;
    public PlayerInputProxy input;

    private SpriteRenderer sprite;
    private void Awake()
    {
        input = GetComponent<PlayerInputProxy>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        stateMachine = new PlayerStateMachine();
        stateMachine.ChangeState(new IdleState(this));
        direction = PlayerInputDirection.None;
        looking = PlayerLookingDirection.Right;
        canDash = true;
        canJump = true;

        _jumpAttackObjX = _jumpAttackObject.transform.localPosition.x;
        _jumpAttackObjY = _jumpAttackObject.transform.localPosition.y;
        _jumpAttackObject.SetActive(false);

    }
    void Start()
    {

    }
    private void Update()
    {
        direction = GetInputDirection();
        stateMachine.Update();

        

        direction = controller.GetDirection(controller.GetInput());
        if (direction == PlayerInputDirection.Right ||
            direction == PlayerInputDirection.UpRight || 
            direction == PlayerInputDirection.DownRight)
        {
            looking = PlayerLookingDirection.Right;
            moveInput = 1f;
        }
        else if (direction == PlayerInputDirection.Left ||
                direction == PlayerInputDirection.UpLeft || 
                direction == PlayerInputDirection.DownLeft)
        {
            looking = PlayerLookingDirection.Left;
            moveInput = -1f;
        }
        else
        {
            moveInput = 0f;
        }

        ApplyMovement();

        
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerBind();
        }
        // 누르고 있는 동안만 ChargeJump() 호출
        if (isButtonPressed)
        {
            ChargeJump();
        }

        input.ResetInputs();
    }
    private void ApplyMovement()
    {
        if (stateMachine.GetCurrentState() is DashState || stateMachine.GetCurrentState() is JumpAttackState) return;
        if (moveInput != 0)
        {
            if (beforeSpeed != 0)
            {
                Debug.Log("속도 복구");
                SetToBeforeSpeed();
            }
            nowSpeed += moveAccel * Time.deltaTime;
            nowSpeed = Mathf.Min(nowSpeed, maxSpeed);

            sprite.flipX = moveInput < 0;
        }
        else
        {
            nowSpeed = 0;
            beforeSpeed = 0;
        }

        Vector3 movement = new Vector3(moveInput * nowSpeed * Time.deltaTime, 0, 0);
        transform.position += movement;
    }

    public void SetMoveInput(float input)
    {
        moveInput = input;
    }
    public float GetMoveInput()
    {
        return moveInput;
    }
    public void ResetMovement()
    {
        nowSpeed = 0;
        moveInput = 0;
    }
    public void SaveBeforeSpeed()
    {
        beforeSpeed = nowSpeed;
        Debug.Log($"속도{beforeSpeed}저장");
    }
    public void SetToBeforeSpeed()
    {
        nowSpeed = beforeSpeed;
    }
    public void ResetBeforeSpeed()
    {
        beforeSpeed = 0;
    }

    public bool isButtonPressed = false;

    // public void OnDashButton() 
    // {
    //     stateMachine.ChangeState(new DashState(this));
    // }
    
    // public void OnJumpButtonDown() // PointerDown에 연결
    // {
    //     if (GetCurrentState() is JumpState) {
    //         return;
    //     }
    //     else
    //     {
    //         stateMachine.ChangeState(new JumpState(this));
    //         isButtonPressed = true;
    //         StartJump();
    //     }
    // }

    // public void OnJumpButtonUp() // PointerUp에 연결
    // {
    //     isButtonPressed = false;
    //     ReleaseJump();
    // }
    // public void OnAttackButton()
    // {
        
    //     if (GetCurrentState() is JumpState)
    //     {
    //         stateMachine.ChangeState(new JumpAttackState(this));
    //     }
    //     else
    //     {
    //         PlayerNormalAttack();
    //         Debug.Log("지상에서 공격하는 로직");
    //     }
    // }
    public void StartJump()
    {
        // 점프 대시 이후 점프 초기화 안되게
        if (stateMachine.GetPreviousState() is DashState)
        {
            Debug.Log("대시 후 점프 불가");
            isJumping = true;
            canJump = false;
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
        if (!isJumping || !canJump) return;

        jumpTimer += Time.deltaTime;

        if (transform.position.y - jumpStartY >= maxJumpHeight)
        {
            isJumping = false;
            return;
        }

        Jump(Mathf.Lerp(minJumpForce, maxJumpForce, jumpTimer / maxChargeTime));
    }

    public void ReleaseJump()
    {
        if (!isJumping) return;

        spaceReleased = true;


        Jump(jumpTimer < minChargeTime ? minJumpForce : rb.velocity.y);

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

            dashTime += Time.deltaTime;
            float t = dashTime / dashDuration;
            Vector2 newPosition = Vector2.Lerp(dashStartPos, dashEndPos, t);
            rb.MovePosition(newPosition);

        }
        rb.velocity = dashBeforeVelocity;
        rb.gravityScale = 4;

        if (dashEndPos.y > dashStartPos.y)
        {
            ChangeState(new JumpState(this));
        }
        else
        {
            stateMachine.RestorePreviousState();
        }
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

    public void PlayerNormalAttack()
    {
        if (Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + _normalAttack_delay;
        }
    }
    private void Shoot()
    {
        if (_normalAttackPrefab != null && _attackPoint != null)
        {
            bool atttackDirection;
            if (looking == PlayerLookingDirection.Right)
            {
                atttackDirection = true;
            }
            else
            {
                atttackDirection = false;
            }
            GameObject instance = Instantiate(_normalAttackPrefab, _attackPoint.position, _attackPoint.rotation);
            PlayerNormalAttack attack = instance.GetComponent<PlayerNormalAttack>();
            attack.AttackSetting(_normalAttackDmg, _normalAttackSpeed, _normalAttackRange, atttackDirection);
            Debug.Log("투사체 발사");
        }
    }

    public void PlayerJumpAttack()
    {
        _jumpAttackObject.SetActive(true);
        if (looking == PlayerLookingDirection.Right)
        {
            _jumpAttackObject.transform.localPosition = new Vector3(_jumpAttackObjX, _jumpAttackObjY, 0);
            _jumpAttackObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            _jumpAttackObject.transform.localPosition = new Vector3(-_jumpAttackObjX, _jumpAttackObjY, 0);
            _jumpAttackObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        rb.velocity = new Vector2(0, -_jumpAttack_diveVelocity);
    }
    public IEnumerator PlayerJumpAttackDelay()
    {
        Debug.Log("시간 정지");
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(_jumpAttackHitDelay);
        Time.timeScale = 1;
        Debug.Log("시간 복구");
    }
    public void EndJumpAttack()
    {
        _jumpAttackObject.SetActive(false);
    }
    public IPlayerState GetCurrentState()
    {
        return stateMachine.GetCurrentState();
    }
    public void RestorePreviousState()
    {
        stateMachine.RestorePreviousState();
    }
    //public bool IsGrounded()
    //{
    //    return rb.velocity.y == 0;
    //}

    public void PlayerBind()
    {
        StartCoroutine(PlayerStop());
    }
    IEnumerator PlayerStop()
    {
        Debug.Log("바인드");
        float bindCounter = 0f;
        rb.isKinematic = true;
        while (bindCounter <= bindTimer)
        {
            Debug.Log("바인드 진행중");
            moveInput = 0;
            rb.velocity = Vector2.zero;
            bindCounter += Time.deltaTime;
            yield return null;
        }
        Debug.Log("바인드 끝");
        rb.isKinematic = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            canJump = true;
            Debug.Log("착지");
            rb.velocity = Vector2.zero;
            ChangeState(new IdleState(this));
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {

        }
    }
}
