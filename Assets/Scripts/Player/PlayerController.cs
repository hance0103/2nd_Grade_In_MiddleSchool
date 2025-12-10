using UnityEngine;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Threading;

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
    public float MoveInput => moveInput;
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
    
    
    [SerializeField] private float dashBeforeDelay = 0.1f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private float dashTime;

    [SerializeField] private float verticalDashDistance = 5f;
    [SerializeField] private float horizontalDashDistance = 5f;
    [SerializeField] private float diagonalDashX;
    [SerializeField] private float diagonalDashY;
    [SerializeField] private GameObject dashEffect;
    private float dashCooldownTimer = 0f;

    
    private Vector2 dashStartPos;
    private Vector2 dashVec;

    [SerializeField]
    private bool isJumpingDash;
    public bool isDownJumping {get;set;}
    public bool canDash { get; set; }

    public bool isFallingFromPlatform = false;

    private bool isDashing = false;

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
    public bool isAttacking = false;
    [SerializeField]
    private Vector2 _projectileStartPos;


    [Header("JumpAttack")]
    [SerializeField]
    private float _jumpAttack_diveVelocity;
    [SerializeField]
    public float _jumpAttack_dmg;
    [SerializeField]
    private float _jumpAttackBeforeDelay = 0.01f;
    public float _jumpAttackHitDelay;
    public float _jumpAttackAfterDelay;
    [SerializeField]
    private float _jumpAttackMinHeight = 1f;
    [SerializeField]
    private bool _canJumpAttack;
    [SerializeField]
    private GameObject _jumpAttackObject;

    private float _jumpAttackObjX;
    private float _jumpAttackObjY;


    public float camShakeDuration = 1f;
    public float camShakeMagnitude = 0.5f;

    [Header("PlayerHit")]
    [SerializeField]
    private float bindTimer = 1f;
    [SerializeField]
    private BoxCollider2D _hitCollider;
    private Vector2 _hitColliderOffset;

    public GameObject enemy;

    [Header("Controller")]
    // 컨트롤러 조작이 기본
    public bool isKeyboardControll = false;
    [SerializeField]
    private ControllerUI controller;
    public PlayerInputProxy input;

    public PlayerAnimation anim;
    private Animator animator;
    private SpriteRenderer characterSprite;
    public SpriteRenderer CharacterSprite => characterSprite;
    private BoxCollider2D collider2d;


    private Vector2 colliderOffset;

    private float colliderLeft;
    private float colliderRight;
    private float colliderBottom;
    [SerializeField]
    private LayerMask platformMask;
    [SerializeField]
    private LayerMask jumpAttackMask;
    [SerializeField]
    private bool _canPlayerControll = true;

    [Header("Player Invincible")]
    [SerializeField]
    private float _blinkDelay = 0.1f;
    [SerializeField]
    private bool _isInvincible = false;

    [Header("Player Hit")]
    public float playerHitShakeMagnitude = 0.2f;
    public float playerHitShakeDuration = 0.2f;

    Collider2D hitCol;


    private void Awake()
    {
        input = GetComponent<PlayerInputProxy>();
        characterSprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerAnimation>();
        animator = GetComponent<Animator>();
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
        
        dashEffect.SetActive(false);

        _hitColliderOffset = _hitCollider.offset;

        hitCol = null;
    }
    void Start()
    {
        _canPlayerControll = true;
    }
    private void Update()
    {
        if (!_canPlayerControll)
        {
            return;
        }

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

                looking = PlayerLookingDirection.Right;
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
                looking = PlayerLookingDirection.Left;
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
            if (direction != controller.GetDirection(controller.GetInput()))
            {
                
                // 컨트롤러 조작
                direction = controller.GetDirection(controller.GetInput());
                if (direction == PlayerInputDirection.Right ||
                    direction == PlayerInputDirection.UpRight ||
                    direction == PlayerInputDirection.DownRight)
                {
                    if (!isAttacking && !isDashing)
                    {
                        looking = PlayerLookingDirection.Right;
                        characterSprite.flipX = false;
                        collider2d.offset = colliderOffset;
                        _hitCollider.offset = _hitColliderOffset;
                    }
                    
                    moveInput = 1f;
                }
                else if (direction == PlayerInputDirection.Left ||
                        direction == PlayerInputDirection.UpLeft ||
                        direction == PlayerInputDirection.DownLeft)
                {
                    if (!isAttacking && !isDashing)
                    {
                        characterSprite.flipX = true;
                        looking = PlayerLookingDirection.Left;
                        collider2d.offset = new Vector2(-colliderOffset.x, colliderOffset.y);
                        _hitCollider.offset = new Vector2 (-_hitColliderOffset.x, _hitColliderOffset.y);
                    }
                    moveInput = -1f;
                }
                else
                {
                    moveInput = 0f;
                }
            }

            
        }
        ApplyMovement();
        input.ResetInputs();

    }
    /// <summary>
    /// 플랫폼 관련 레이캐스트들 처리는 FixedUpdate에서 처리
    /// </summary>
    private void FixedUpdate()
    {
        //RayCast
        colliderLeft = collider2d.bounds.min.x;
        colliderRight = collider2d.bounds.max.x;
        colliderBottom = collider2d.bounds.min.y;

        Vector2 rayVecLeft = new Vector2(colliderLeft, colliderBottom);
        Vector2 rayVecRight = new Vector2(colliderRight, colliderBottom);
        Vector2 rayDirection = Vector2.down;
        

        RaycastHit2D jumpAttackHitLeft = Physics2D.Raycast(rayVecLeft, rayDirection, _jumpAttackMinHeight, jumpAttackMask);
        RaycastHit2D jumpAttackHitRight = Physics2D.Raycast(rayVecRight, rayDirection, _jumpAttackMinHeight, jumpAttackMask);

        Debug.DrawLine(rayVecLeft, rayVecLeft + rayDirection * _jumpAttackMinHeight, Color.red);
        Debug.DrawLine(rayVecRight, rayVecRight + rayDirection * _jumpAttackMinHeight, Color.red);

        if (jumpAttackHitLeft.collider != null || jumpAttackHitRight.collider != null)
        {
            Collider2D jumnpHitCol = jumpAttackHitLeft.collider != null ? jumpAttackHitLeft.collider : jumpAttackHitRight.collider;
            _canJumpAttack = false;
        }
        else
        {
            _canJumpAttack = true;
        }



        // boxcast
        Vector2 boxCastSize = new Vector2(collider2d.bounds.size.x, 0.1f);
        Vector2 boxCastOrigin = new Vector2(collider2d.bounds.center.x, collider2d.bounds.min.y- boxCastSize.y * 0.5f);
        float boxCastDistance = 0.05f;

        RaycastHit2D hit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, boxCastDistance, platformMask);

        RaycastHit2D jumpInitHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, boxCastDistance, jumpAttackMask);
        if (jumpInitHit.collider != null)
        {
            hitCol = jumpInitHit.collider;
        }

        Color debugColor = hit.collider != null ? Color.green : Color.red;
        Vector2 topLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
        Vector2 topRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
        Vector2 bottomLeft = topLeft + Vector2.down * boxCastDistance;
        Vector2 bottomRight = topRight + Vector2.down * boxCastDistance;

        Debug.DrawLine(topLeft, topRight, debugColor);
        Debug.DrawLine(bottomLeft, bottomRight, debugColor);
        Debug.DrawLine(topLeft, bottomLeft, debugColor);
        Debug.DrawLine(topRight, bottomRight, debugColor);

        if (hit.collider != null)
        {
            if (!isOnPlatform && !isDownJumping && !isDashing)
            {
                canJump = true;
                isJumpingDash = false;
                isOnPlatform = true;
                isFallingFromPlatform = false;
                nowPlatform = hit.collider.gameObject;
                hit.collider.isTrigger = false;
                ChangeState(new IdleState(this));
            }
        }
        else
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

    public void PlayerStop()
    {
        SoundManager.Instance.StopLoopEffect();

        anim.PauseAnimation();
        _canPlayerControll = false;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }
    public void PlayerResume()
    {
        anim.ResumeAnimation();
        _canPlayerControll = true;
        rb.isKinematic = false;
    }
    public void PlayerDefeat()
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;

        ChangeState(new IdleState(this));
        _canPlayerControll = false;

        anim.PlayAnimation("Defeat");
    }
    public void ResumeAfterDefeat()
    {
        animator.updateMode = AnimatorUpdateMode.Normal;
        _canPlayerControll = true;
    }
    public void StartDash()
    {
        if (!canDash) return;


        isDashing = true;

        canDash = false;
        canJump = false;
        jumpTimer = 0f;
        dashStartPos = transform.position;


        dashBeforeVelocity = rb.velocity;
        dashBeforeVelocity.y = 0;
        
        // 방향계산 넣어야함
        dashVec = Vector2.zero;

        PlayerInputDirection dashDirection = controller.GetInputDirection();
        switch (dashDirection)
        {
            case PlayerInputDirection.Up:
                dashVec = new Vector2(0, verticalDashDistance);
                break;
            case PlayerInputDirection.Down:
                dashVec = new Vector2(0, -verticalDashDistance);
                break;
            case PlayerInputDirection.Right:
                dashVec = new Vector2(horizontalDashDistance, 0);
                break;
            case PlayerInputDirection.Left:
                dashVec = new Vector2(-horizontalDashDistance, 0);
                break;
            case PlayerInputDirection.UpRight:
                dashVec = new Vector2(diagonalDashX, diagonalDashY);
                break;
            case PlayerInputDirection.UpLeft:

                dashVec = new Vector2(-diagonalDashX, diagonalDashY);
                break;
            case PlayerInputDirection.DownRight:
                dashVec = new Vector2(diagonalDashX, -diagonalDashY);
                break;
            case PlayerInputDirection.DownLeft:
                dashVec = new Vector2(-diagonalDashX, -diagonalDashY);
                break;
            case PlayerInputDirection.None:
                switch (looking)
                {
                    case PlayerLookingDirection.Right:
                        dashVec = new Vector2(horizontalDashDistance, 0);
                        break;
                    case PlayerLookingDirection.Left:
                        dashVec = new Vector2(-horizontalDashDistance, 0);
                        break;
                }
                break;
        }

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;

        StartCoroutine(DashCoroutine(dashVec));
    }
    private IEnumerator DashCoroutine(Vector2 dashDirection)
    {
        bool wasOnPlatform = isOnPlatform;

        anim.PlayAnimation("Dash");
        SoundManager.Instance.Play("PlayerSound/PlayerDash");
        dashEffect.SetActive(true);

        if (dashDirection.x > 0)    // 오른쪽 방향으로 대시
        {
            dashEffect.transform.localPosition = new Vector2(-0.75f, 0);
            dashEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
            dashEffect.GetComponent<SpriteRenderer>().flipX = false;
            looking = PlayerLookingDirection.Right;
        }
        else if (dashDirection.x < 0) // 왼쪽 방향으로 대시
        {
            dashEffect.transform.localPosition = new Vector2(0.75f, 0);
            dashEffect.transform.rotation = Quaternion.Euler(0, 0, 0);
            dashEffect.GetComponent<SpriteRenderer>().flipX = true;
            looking = PlayerLookingDirection.Left;
        }
        else if (dashDirection.x == 0)  // 수직 방향 대시
        { 
            // 위로 대시
            if (dashDirection.y > 0)
            {
                if (looking == PlayerLookingDirection.Right)
                {
                    dashEffect.transform.localPosition = new Vector2(0.36f, -1.37f);
                    dashEffect.transform.rotation = Quaternion.Euler(0, 0, 90);
                    dashEffect.GetComponent<SpriteRenderer>().flipX = false;
                }
                else if (looking == PlayerLookingDirection.Left)
                {
                    dashEffect.transform.localPosition = new Vector2(-0.36f, -1.37f);
                    dashEffect.transform.rotation = Quaternion.Euler(0, 0, 90);
                    dashEffect.GetComponent<SpriteRenderer>().flipX = true;
                }

            }
            else
            {
                if (looking == PlayerLookingDirection.Right)
                {
                    dashEffect.transform.localPosition = new Vector2(0.53f, 1.27f);
                    dashEffect.transform.rotation = Quaternion.Euler(0, 0, -90);
                    dashEffect.GetComponent<SpriteRenderer>().flipX = false;
                }
                else if (looking == PlayerLookingDirection.Left)
                {
                    dashEffect.transform.localPosition = new Vector2(-0.53f, 1.27f);
                    dashEffect.transform.rotation = Quaternion.Euler(0, 0, -90);
                    dashEffect.GetComponent<SpriteRenderer>().flipX = true;
                }

            }
        }

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(dashBeforeDelay);

        Vector2 dashStartPos = rb.position;
        Vector2 dashEndPos = dashStartPos + dashDirection;

        Vector2 dashSpeed = dashDirection/dashDuration;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {

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

        yield return new WaitForFixedUpdate();

        if ((dashEndPos.y > dashStartPos.y) || stateMachine.GetPreviousState() is JumpState)
        {
            ChangeState(new JumpState(this));
        }
        else if(wasOnPlatform && rb.velocity.y < 0)
        {
            isJumpingDash = true;
            isJumping = true;
            canJump = false;
            ChangeState(new JumpState(this));
        }
        else
        {
            ChangeState(new IdleState(this));
        }

        isDashing = false;

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
    public void CamShake()
    {
        Camera.main.GetComponent<CameraShaker>().StartShake(camShakeDuration, camShakeMagnitude);
    }
    public void CamShake(float duration, float Magnitude)
    {
        Camera.main.GetComponent<CameraShaker>().StartShake(duration, Magnitude);
    }
    public void ChangeState(IPlayerState newState)
    {
        stateMachine.ChangeState(newState);
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
            Vector2 attackStartPos;
            if (attackDirection == PlayerLookingDirection.Right)
            {
                attackStartPos = _projectileStartPos;
                atttackDirection = true;
            }
            else
            {
                attackStartPos = new Vector2(-_projectileStartPos.x, _projectileStartPos.y);
                atttackDirection = false;
            }
            Vector3 shootPos = new Vector3(transform.position.x + attackStartPos.x, transform.position.y + attackStartPos.y, transform.position.z);

            GameObject instance = Instantiate(_normalAttackPrefab, shootPos, _attackPoint.rotation);

            //instance.transform.localPosition = attackStartPos;

            PlayerNormalAttack attack = instance.GetComponent<PlayerNormalAttack>();
            attack.AttackSetting(_normalAttackDmg, _normalAttackSpeed, _normalAttackRange, atttackDirection);

            int rand = Random.Range(1, 5);
            SoundManager.Instance.Play($"PlayerSound/PlayerNormalAttack{rand}");
        }
    }
    public void PlayerJumpAttackObjectDisable()
    {
        _jumpAttackObject.SetActive(false);
    }

    public void StartJumpAttack()
    {
        _jumpAttackObject.GetComponent<PlayerJumpAttack>().isBossHit = false;
        StartCoroutine(PlayerJumpAttackBeforeDelay());
    }
    private IEnumerator PlayerJumpAttackBeforeDelay()
    {
        PlayerStop();
        yield return new WaitForSeconds(_jumpAttackBeforeDelay);
        PlayerResume();
        PlayerJumpAttack();
    }
    private void PlayerJumpAttack()
    {
        SoundManager.Instance.Play("PlayerSound/PlayerJumpAttack");
        anim.PlayAnimation("JumpAttack");
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
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(_jumpAttackHitDelay);
        Time.timeScale = 1f;
    }
    public IEnumerator PlayerJumpAttackAfterDelay()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(_jumpAttackAfterDelay);
        Time.timeScale = 1f;

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
    public bool CanJumpAttack()
    {
        return _canJumpAttack;
    }
    public bool IsInvincible()
    {
        return _isInvincible;
    }
    public void ActivateInvincible()
    {
        //Debug.Log("무적");
        _isInvincible = true;
    }
    public void DeactivateInvincible()
    {
        characterSprite.color = Color.white;
        _isInvincible = false;
    }
    public IEnumerator InvincibleBlink()
    {
        while (_isInvincible)
        {
            characterSprite.color = new Color(70/255f,70/255f,70/255f);
            //sprite.color = Color.red;
            yield return new WaitForSeconds(_blinkDelay);
            characterSprite.color = Color.white;
            yield return new WaitForSeconds(_blinkDelay);
        }
    }
    public SpriteRenderer GetSpriteRenderer()
    {
        return characterSprite;
    }
    public void SetSpriteColor(Color color)
    {
        characterSprite.color = color;
    }
    public float GetBlinkDelay()
    {
        return _blinkDelay;
    }
    public PlayerInputDirection GetDirection()
    {
        return direction;
    }
    public void PlayerSlow(float time, float speedLimit, float accelLimit)
    {
        StartCoroutine(PlayerSlowCoroutine(time, speedLimit, accelLimit)); ;
    }
    private IEnumerator PlayerSlowCoroutine(float time, float speedLimit, float accelLimit)
    {
       

        float originMaxSpeed = maxSpeed;

        maxSpeed = speedLimit;
        nowSpeed = maxSpeed;


        yield return new WaitForSeconds(time);

        //Debug.Log($"원래속도 {originMaxSpeed}로 속도 복구");
        maxSpeed = originMaxSpeed;
    }

    public void OnStageEnd()
    {
        SoundManager.Instance.StopLoopEffect();
        moveInput = 0;
        _canPlayerControll = false;
        stateMachine.ChangeState(new IdleState(this));
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
            {
                ChangeState(new MoveState(this));
            }
            else
            {
                ChangeState(new IdleState(this));

            }
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }
} 