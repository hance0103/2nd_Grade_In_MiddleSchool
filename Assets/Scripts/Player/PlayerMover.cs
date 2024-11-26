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

    [Header ("Jump")]
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

    [Header("Dash")]
    [SerializeField]
    private float _dashDistance;
    [SerializeField]
    private float _dashCoolDown;
    private float _dashTime = 0;
    private bool _canDash = true;

    [SerializeField]
    private PlayerInputDirection _direction = PlayerInputDirection.None;
    [SerializeField]
    private PlayerLookingDirection _looking = PlayerLookingDirection.Right;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GetComponent<Player>();
        _rigid = GetComponent<Rigidbody2D>();

    }
    private void Update()
    {
        PlayerMoveInput();
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

            if (!_isJumping)
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
            if (!_isJumping)
                _player.playerState = PlayerState.Move;
            _looking = PlayerLookingDirection.Right;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            _direction = PlayerInputDirection.Up;
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            _direction = PlayerInputDirection.Down;
        }
        PlayerMoveVec();
        if (!Input.GetKey(KeyCode.RightArrow) &&
            !Input.GetKey(KeyCode.LeftArrow) &&
            !_isJumping)
        {
            _player.playerState = PlayerState.Idle;
            _movSpeed = 0;
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
            _isJumping = false;
        }

        if (!_canJump && _rigid.velocity.y < 0)
        {
            _rigid.gravityScale = 5f;
        }
        else
        {
            _rigid.gravityScale = 4f;
        }
    }
    private void PlayerDash()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("dash");
            //대시
        }
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
        else if(!_canJump && _rigid.velocity.y < -1)
        {
            _spriteRenderer.sprite = Resources.Load<Sprite>($"Sprites/jump_{left_or_right}_02");
        }
        if(_canJump)
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
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            _canJump = false;
        }
    }
}