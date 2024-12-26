using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMover : MonoBehaviour
{
    private Player _player;
    private Rigidbody2D _rigid;
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private string left_or_right;
    [Header("Move")]
    [SerializeField]
    private float _movSpeed = 0f;    //이동 속도
    [SerializeField]
    private float _maxMovSpeed; //최대 속도
    [SerializeField]
    private float _movAccel;    //가속도

    [Header("Jump")]
    [SerializeField]
    private float _maxJumpForce;   // 최대 점프
    [SerializeField]
    private float _minJumpForce;   // 최소 점프 
    [SerializeField]
    private float _maxChargeTime;  // 최대 점프 충전 시간

    [SerializeField]
    private float _jumpTimer = 0f;          // 점프 키 누르는 시간
    [SerializeField]
    private bool _canJump = true;           // 점프 가능한지
    [SerializeField]
    private bool _isJumping = false;        // 점프 중인지
    [SerializeField]
    private float normalGravityScale;       // 일반 중력스케일
    [SerializeField]
    private float fallingGravityScale;      // 낙하 중력스케일

    [Header("Dash")]
    [SerializeField]
    private float _dashDistance;        //대쉬 거리
    [SerializeField]
    private float _diagonalDashX;
    [SerializeField]
    private float _diagonalDashY;
    [SerializeField]
    private float _dashDuration;        //대쉬 지속시간
    private bool _isDashing = false;    //대쉬 하고있는지
    private float _dashTime = 0;        //대쉬를 얼마나 했는지 시간
    [SerializeField]
    private float _dashGravityScale;
    [SerializeField]
    private float _dashGravityScaleTime;

    // 쿨타임 계산 관련 변수
    [SerializeField]
    private float _dashCoolDown;    //대쉬 쿨타임
    private bool _canDash = true;
    private float _dashCooldownTimer = 0f;

    [SerializeField]
    private PlayerInputDirection _direction = PlayerInputDirection.None;
    public PlayerLookingDirection _looking = PlayerLookingDirection.Right;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GetComponent<Player>();
        _rigid = GetComponent<Rigidbody2D>();

    }
    private void Update()
    {
        if (_player.playerState != PlayerState.Dash)
        {
            PlayerMoveInput();
        }
        PlayerJump();
        PlayerDash();
        PlayerAni();
    }
    // 업데이트에서 리지드바디를 사용하면 리지드바다는 60프레임마다 '강제'로 실행
    // 업데이트는 업데이트가 완료 될때마다 실행 0.15~0.17 될 수도 있다.
    // 픽스드 업데이트에서 물리 연산이 일어나도록 유니티는 세팅이 되어 있다.
    private void PlayerMoveInput()
    {
        // 왼쪽 이동 관리
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.UpArrow))  // 왼쪽 위 방향키 입력
            {
                _direction = PlayerInputDirection.UpLeft;
            }
            else if (Input.GetKey(KeyCode.DownArrow))   // 왼쪽 아래 방향키 입력
            {
                _direction = PlayerInputDirection.DownLeft;
            }
            else // 왼쪽 방향키만 입력
            {
                _direction = PlayerInputDirection.Left;
            }

            if (_player.playerState != PlayerState.Jump)
                _player.playerState = PlayerState.Move;
            _looking = PlayerLookingDirection.Left;
        }
        // 오른쪽 이동 관리
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.UpArrow)) // 오른쪽 위 방향키 입력
            {
                _direction = PlayerInputDirection.UpRight;
            }
            else if (Input.GetKey(KeyCode.DownArrow)) // 오른쪽 아래 방향키 입력
            {
                _direction = PlayerInputDirection.DownRight;
            }
            else //오른쪽 방향키만 입력
            {
                _direction = PlayerInputDirection.Right;
            }
            if (_player.playerState != PlayerState.Jump)
                _player.playerState = PlayerState.Move;
            _looking = PlayerLookingDirection.Right;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            _direction = PlayerInputDirection.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            _direction = PlayerInputDirection.Down;
        }
        PlayerMoveVec();
        if (!Input.GetKey(KeyCode.RightArrow) &&
            !Input.GetKey(KeyCode.LeftArrow) &&
            !_isJumping)
        {
            if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
            {
                _direction = PlayerInputDirection.None;
            }
            _movSpeed = 0;
            if (_player.playerState != PlayerState.Jump)
            {
                _player.playerState = PlayerState.Idle;
            }
            
        }


    }
    private void PlayerMoveVec()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0)
        {
            _movSpeed += _movAccel * Time.deltaTime;
            _movSpeed = Mathf.Min(_movSpeed, _maxMovSpeed);
        }

        Vector3 movement = new Vector3(moveInput * _movSpeed * Time.deltaTime, 0, 0);
        transform.position += movement;
    }
    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _canJump)
        {
            _isJumping = true;
            _jumpTimer = 0f;
        }
        if (Input.GetKey(KeyCode.Space) && _isJumping)
        {
            _jumpTimer += Time.deltaTime;
            float jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, _jumpTimer / _maxChargeTime);
            _rigid.velocity = new Vector2(_rigid.velocity.x, jumpForce);
            if (_jumpTimer >= _maxChargeTime)
            {
                _isJumping = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space) && _isJumping)
        {
            if (_jumpTimer < 0.1f)
            {
                _jumpTimer = 0.1f;
                float jumpForce = Mathf.Lerp(_minJumpForce, _maxJumpForce, _jumpTimer / _maxChargeTime);
                _rigid.velocity = new Vector2(_rigid.velocity.x, jumpForce);
                Debug.Log("_jumpTimer < 0.1f");
            }
            _isJumping = false;
        }

        //if (!_canJump && _rigid.velocity.y < 0)
        //{
        //    _rigid.gravityScale = fallingGravityScale;
        //}
        //else
        //{
        //    _rigid.gravityScale = normalGravityScale;
        //}
    }
    private void PlayerDash()
    {
        if (Input.GetKeyDown(KeyCode.C) && _canDash)
        {
            Debug.Log("dash");
            StartCoroutine(Dash());
            StartCoroutine(PlayerDashCoolDown());
        }
    }
    private IEnumerator Dash()
    {
        Debug.Log("대시 시작");
        _rigid.gravityScale = 0;
        PlayerState beforeState = _player.playerState;
        _player.playerState = PlayerState.Dash;
        _isDashing = true;

        Vector2 _dashDirection = Vector2.zero;
        switch (_direction)
        {
            case PlayerInputDirection.Up:
                _dashDirection = new Vector2(0, _dashDistance);
                break;
            case PlayerInputDirection.Down:
                _dashDirection = new Vector2(0, -_dashDistance);
                break;
            case PlayerInputDirection.Right:
                _dashDirection = new Vector2(_dashDistance, 0);
                _rigid.gravityScale = 4;
                break;
            case PlayerInputDirection.Left:
                _dashDirection = new Vector2(-_dashDistance, 0);
                _rigid.gravityScale = 4;
                break;
            case PlayerInputDirection.UpRight:
                _dashDirection = new Vector2(_diagonalDashX, _diagonalDashY);
                break;
            case PlayerInputDirection.UpLeft:
                _dashDirection = new Vector2(-_diagonalDashX, _diagonalDashY);
                break;
            case PlayerInputDirection.DownRight:
                _dashDirection = new Vector2(_diagonalDashX, -_diagonalDashY);
                break;
            case PlayerInputDirection.DownLeft:
                _dashDirection = new Vector2(-_diagonalDashX, -_diagonalDashY);
                break;
            case PlayerInputDirection.None:
                switch (_looking)
                {
                    case PlayerLookingDirection.Right:
                        _dashDirection = new Vector2(_dashDistance, 0);
                        break;
                    case PlayerLookingDirection.Left:
                        _dashDirection = new Vector2(-_dashDistance, 0);
                        break;
                }
                break;
        }

        Vector2 dashStartPos = _rigid.position;
        Vector2 dashEndPos = _rigid.position + _dashDirection;
        _dashTime = 0f;
        Debug.Log(dashStartPos);
        Debug.Log(dashEndPos);
        //_rigid.position = dashEndPos;
        while (_dashTime < _dashDuration)
        {
            Debug.Log("대시 진행중");

            // 이쪽 수정
            _dashTime += Time.deltaTime;
            float t = _dashTime / _dashDuration;
            Vector2 newPosition = Vector2.Lerp(dashStartPos, dashEndPos, t);
            _rigid.MovePosition(newPosition);
            yield return null;
        }
        Debug.Log("대시 끝");
        _isDashing = false;
        if (beforeState == PlayerState.Jump)
        {
            _player.playerState = PlayerState.Jump;
        }
        else
        {
            _player.playerState = PlayerState.Idle;
        }
        
        _rigid.velocity = Vector2.zero;
        StartCoroutine(DashGravity());
    }
    private IEnumerator DashGravity()
    {
        _rigid.gravityScale = _dashGravityScale;
        float dashGravityTimeCounter = 0f;
        while(dashGravityTimeCounter <= _dashGravityScaleTime)
        {
            dashGravityTimeCounter += Time.deltaTime;
            yield return null;
        }
        _rigid.gravityScale = fallingGravityScale;
    }
    private IEnumerator PlayerDashCoolDown()
    {
        _canDash = false;
        _dashCooldownTimer = 0f;
        while (_dashCooldownTimer <= _dashCoolDown)
        {
            _dashCooldownTimer += Time.deltaTime;
            yield return null;
        }
        _canDash = true;
    }
    /// <summary>
    /// 점프 스프라이트 교체를 위해서 임시로 작성한 함수.
    /// 나중에 애니메이션 작업할때 지우고 새로 만들기
    /// </summary>
    private void PlayerAni()
    {
        if (!_canJump && _rigid.velocity.y > 0)
        {
            _spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/jump_{left_or_right}_01");

        }
        else if (!_canJump && _rigid.velocity.y < -1)
        {
            _spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/jump_{left_or_right}_02");

        }
        if (_canJump)
        {
            _spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/Idle_01");

        }
    }


    //나중에 발 부분에 콜라이더 하나 더 만들어서
    //벽에 부딪혔을때 점프 초기화 안되도록 고쳐야함
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            _canJump = true;
            _player.playerState = PlayerState.Idle;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            _canJump = false;
            _player.playerState = PlayerState.Jump;
        }
    }
}