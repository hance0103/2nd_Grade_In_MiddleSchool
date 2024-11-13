using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private Player _player;

    [Header ("Move")]
    [SerializeField]
    private float _movSpeed;    //이동 속도
    [SerializeField]
    private float _maxMovSpeed; //최대 속도
    [SerializeField]
    private float _movAccel;    //가속도

    [Header ("Jump")]
    [SerializeField]
    private float _jumpSpeed;   // 기본 점프 속도
    [SerializeField]
    private float _maxJumpHeight;   // 최고 점프 높이
    private bool _isJumping;    // 점프 중인지
    private bool _canJump;      // 점프 가능한지
    private bool _isKeyHeld;    // 점프 키가 눌려있는지

    private float _initY;

    private PlayerInputDirection _direction;
    private PlayerLookingDirection _looking;

    void Start()
    {

        _player = GetComponent<Player>();
        _movSpeed = 0;
        _isJumping = false;
        _canJump = true;
        _direction = PlayerInputDirection.None;
        _looking = PlayerLookingDirection.Right;
        _initY = transform.position.y;

    }
    private void Update()
    {
        PlayerMoveInput();
        PlayerJumpInput();
    }
    // 업데이트에서 리지드바디를 사용하면 리지드바다는 60프레임마다 '강제'로 실행
    // 업데이트는 업데이트가 완료 될때마다 실행 0.15~0.17 될 수도 있다.
    // 픽스드 업데이트에서 물리 연산이 일어나도록 유니티는 세팅이 되어 있다.
    private void FixedUpdate()
    {

    }
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

    private void PlayerJumpInput()
    {

    }
    private void PlayerJump()
    {

    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        _canJump = true;
    //    }
    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground"))
    //    {
    //        _canJump = false;
    //    }
    //}
}