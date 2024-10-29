using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    private Player _player;

    // 이동 관련 변수
    [SerializeField]
    private float _movSpeed;    //이동 속도
    [SerializeField]
    private float _maxMovSpeed; //최대 속도
    [SerializeField]
    private float _movAccel;    //가속도

    // 점프 관련 변수
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
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            // 키가 눌렸을 때
            if (Input.GetKeyDown(key))
            {
                Debug.Log("Pressed Key: " + key);
            }
        }
    }
    private void FixedUpdate()
    {
        PlayerMoveInput();
        PlayerJumpInput();
    }
    private void PlayerMoveInput()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _direction = PlayerInputDirection.UpLeft;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                _direction = PlayerInputDirection.DownLeft;
            }
            else
            {
                _direction = PlayerInputDirection.Left;
            }

            if (!_isJumping)
                _player.playerState = PlayerState.Move;
            _looking = PlayerLookingDirection.Left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _direction = PlayerInputDirection.UpRight;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                _direction = PlayerInputDirection.DownRight;
            }
            else
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
        float moveInput = Input.GetAxis("Horizontal");
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