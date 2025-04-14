using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using Unity.PlasticSCM.Editor.WebApi;

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
    [SerializeField] private float minChargeTime = 0.1f;
    [SerializeField] private float minJumpForce = 5f;  // 최소 점프 힘
    [SerializeField] private float maxJumpForce = 10f; // 최대 점프 힘
    [SerializeField] private float maxJumpHeight = 3f; // 최대 점프 높이
    [SerializeField] private float maxChargeTime = 0.5f; // 최대 점프 충전 시간

    private bool spaceReleased = false;
    public bool isJumping = false;
    private float jumpStartY;
    private float jumpTimer = 0f;
    public bool canJump;

    public bool isOnPlatform = false;
    public GameObject nowPlatform;

    [SerializeField]
    private bool isOnGround;

    [Header("Dash")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashBeforeDelay = 0.1f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashTime;

    [SerializeField] private float diagonalDashX;
    [SerializeField] private float diagonalDashY;
    [SerializeField] private GameObject dashEffect;
    private float dashCooldownTimer = 0f;
    private float dashEffectPosX;
    private float dashEffectPosY;

    
    private Vector2 dashStartPos;
    private Vector2 dashDirection;

    [SerializeField]
    private bool isJumpingDash;
    public bool isDownJumping {get;set;}
    public bool canDash { get; set; }

    public bool isFallingFromPlatform = false;

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
    public float _jumpAttack_dmg;
    public float _jumpAttackHitDelay;
    [SerializeField]
    private GameObject _jumpAttackObject;

    private float _jumpAttackObjX;
    private float _jumpAttackObjY;


    [Header("PlayerHit")]
    [SerializeField]
    private float bindTimer = 1f;

    public GameObject enemy;

    [Header("Controller")]
    // 컨트롤러 조작이 기본
    public bool isKeyboardControll = false;
    [SerializeField]
    private ControllerUI controller;
    public PlayerInputProxy input;

    public PlayerAnimation anim;
    private SpriteRenderer sprite;
    private BoxCollider2D collider2d;


    private Vector2 colliderOffset;

    private float colliderLeft;
    private float colliderRight;
    private float colliderBottom;
    [SerializeField]
    private LayerMask platformMask;

    
    private void Awake()
    {
        input = GetComponent<PlayerInputProxy>();
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerAnimation>();
        collider2d = GetComponent<BoxCollider2D>();
        stateMachine = new PlayerStateMachine();
        stateMachine.ChangeState(new IdleState(this));
        direction = PlayerInputDirection.None;
        looking = PlayerLookingDirection.Right;
        canDash = true;
        canJump = true;
        isDownJumping = false;
        isJumpingDash = false;
        isOnGround = true;
        colliderOffset = collider2d.offset;

        _jumpAttackObjX = _jumpAttackObject.transform.localPosition.x;
        _jumpAttackObjY = _jumpAttackObject.transform.localPosition.y;
        _jumpAttackObject.SetActive(false);

        dashEffectPosX = dashEffect.transform.localPosition.x;
        dashEffectPosY = dashEffect.transform.localPosition.y;
        dashEffect.SetActive(false);


    }
    void Start()
    {

    }
    private void Update()
    {
        direction = GetInputDirection();
        stateMachine.Update();

        if (isKeyboardControll)
        {
            //키보드 조작
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    direction = PlayerInputDirection.UpRight;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                direction = PlayerInputDirection.DownRight;
                }
                else
                {
                    direction = PlayerInputDirection.Right;
                }
                moveInput = 1f;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    direction = PlayerInputDirection.UpLeft;
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    direction = PlayerInputDirection.DownLeft;
                }
                else
                {
                    direction = PlayerInputDirection.Left;
                }
                moveInput = -1f;
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                direction = PlayerInputDirection.Up;
                moveInput = 0f;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                direction = PlayerInputDirection.Down;
                moveInput = 0f;
            }
            else
            {
                direction = PlayerInputDirection.None;
                moveInput = 0f;
            }
        }
        else
        {
            // 컨트롤러 조작
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
        }

        ApplyMovement();

        input.ResetInputs();


    }
    private void FixedUpdate()
    {
        //RayCast
        colliderLeft = collider2d.bounds.min.x;
        colliderRight = collider2d.bounds.max.x;
        colliderBottom = collider2d.bounds.min.y;

        Vector2 rayVecLeft = new Vector2(colliderLeft, colliderBottom);
        Vector2 rayVecRight = new Vector2(colliderRight, colliderBottom);
        Vector2 rayDirection = Vector2.down;
        float rayDistance = 0.1f;

        RaycastHit2D hitLeft = Physics2D.Raycast(rayVecLeft, rayDirection, rayDistance, platformMask);
        RaycastHit2D hitright = Physics2D.Raycast(rayVecRight, rayDirection, rayDistance, platformMask);

        Debug.DrawLine(rayVecLeft, rayVecLeft + rayDirection*rayDistance, Color.green);
        Debug.DrawLine(rayVecRight, rayVecRight + rayDirection*rayDistance, Color.green);

        if ((hitLeft.collider != null || hitright.collider != null))
        {
            Collider2D hitCol = hitLeft.collider != null ? hitLeft.collider : hitright.collider;
            if (!isOnPlatform && !isDownJumping)
            {
                Debug.Log("플랫폼 위에 올라감");
                canJump = true;
                isJumpingDash = false;
                isOnPlatform = true;
                isFallingFromPlatform = false;
                nowPlatform = hitCol.gameObject;
                hitCol.isTrigger = false;
                ChangeState(new IdleState(this));
            }
        }
        else if (hitLeft.collider == null && hitright.collider == null)
        {
            if (isOnPlatform && !isJumping && !(stateMachine.GetCurrentState() is DashState))
            {
                isFallingFromPlatform = true;
                ChangeState(new JumpState(this));
            }
                
            isOnPlatform = false;
            if (nowPlatform != null)
            {
                nowPlatform.GetComponent<BoxCollider2D>().isTrigger = true;
                nowPlatform = null;
            }
        }
    }
    private void ApplyMovement()
    {
        var currentState = stateMachine.GetCurrentState();
        if (!(currentState is MoveState || currentState is JumpState))
            return;

        if (moveInput != 0)
        {
            nowSpeed += moveAccel * Time.deltaTime;
            nowSpeed = Mathf.Min(nowSpeed, maxSpeed);
            sprite.flipX = moveInput < 0;

            if (moveInput > 0)
            {
                collider2d.offset = colliderOffset;
            }
            else if (moveInput < 0)
            {
                collider2d.offset = new Vector2(-colliderOffset.x, colliderOffset.y);
            }
        }
        else if (currentState is MoveState)
        {
            nowSpeed = 0;
        }

        transform.position += new Vector3(moveInput * nowSpeed * Time.deltaTime, 0, 0);
    }



    public void SetMoveInput(float input)
    {
        moveInput = input;
    }
    public float GetMoveInput()
    {
        return moveInput;
    }
    public PlayerLookingDirection GetLookingDirection()
    {
        return looking;
    }
    public void ResetMovement()
    {
        nowSpeed = 0;
        moveInput = 0;
    }

    public void StartJump()
    {
        // 점프 대시 이후 점프 초기화 안되게
        if (stateMachine.GetPreviousState() is DashState)
        {
            Debug.Log("대시 후 점프 불가");
            isJumpingDash = true;
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
        if (!isJumping || !canJump || GetJumpingDash()) return;

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
        rb.velocity = new Vector2(rb.velocity.x, 0f); // 기존 수직 속도 초기화
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse); // 힘을 순간적으로 가함
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
        if (!canDash) return;
        canDash = false;
        canJump = false;
        jumpTimer = 0f;
        dashStartPos = transform.position;


        dashBeforeVelocity = rb.velocity;
        dashBeforeVelocity.y = 0;
        
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
        anim.PlayAnimation("Dash");

        dashEffect.SetActive(true);
        if (looking == PlayerLookingDirection.Right)
        {
            dashEffect.transform.localPosition = new Vector2(dashEffectPosX, dashEffectPosY);
            dashEffect.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (looking == PlayerLookingDirection.Left)
        {
            dashEffect.transform.localPosition = new Vector2(-dashEffectPosX, -dashEffectPosY);
            dashEffect.GetComponent<SpriteRenderer>().flipX = true;
        }

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(dashBeforeDelay);

        Vector2 dashStartPos = rb.position;
        Vector2 dashEndPos = dashStartPos + direction;

        Vector2 dashSpeed = direction/dashDuration;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            Debug.Log(elapsed);
            elapsed += Time.fixedDeltaTime;
            Vector2 nextPos = rb.position + dashSpeed*Time.fixedDeltaTime;
            rb.MovePosition(nextPos);


            yield return new WaitForFixedUpdate();
        }

        rb.gravityScale = 4;
        rb.velocity = dashBeforeVelocity;

        dashEffect.SetActive(false);
        StartCoroutine(PlayerDashCoolDown());
        canJump = false;

        if ((dashEndPos.y > dashStartPos.y) || stateMachine.GetPreviousState() is JumpState)
            ChangeState(new JumpState(this));
        else
            ChangeState(new IdleState(this));


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

    public void PlayerNormalAttack(PlayerLookingDirection attackDirection)
    {
        if (Time.time >= _nextFireTime)
        {
            Shoot(attackDirection);
            _nextFireTime = Time.time + _normalAttack_delay;
        }
    }
    private void Shoot(PlayerLookingDirection attackDirection)
    {
        if (_normalAttackPrefab != null && _attackPoint != null)
        {
            bool atttackDirection;
            if (attackDirection == PlayerLookingDirection.Right)
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
    public float GetFallingVelocity()
    {
        return rb.velocity.y;
    }
    public bool GetJumpingDash()
    {
        return isJumpingDash;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            canJump = true;
            isDownJumping = false;
            isJumpingDash = false;
            isFallingFromPlatform = false;
            // 수직 속도만 초기화
            rb.velocity = new Vector2(rb.velocity.x, 0);

            if (moveInput != 0)
                ChangeState(new MoveState(this));
            else
                ChangeState(new IdleState(this));
        }
    }


    // void OnTriggerEnter2D(Collider2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Platform"))
    //     {

    //         if (rb.velocity.y < 0)
    //         {


    //             Debug.Log("플랫폼 위에 올라감");
    //             isOnPlatform = true;
    //             collision.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
    //             nowPlatform = collision.gameObject;
    //             ChangeState(new IdleState(this));
                
    //         }
    //     }

    // }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }
} 