using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMover : MonoBehaviour
{
    //TODO
    // 하향점프 만들어야함

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

    private float _dashTime = 0;        //대쉬를 얼마나 했는지 시간
    [SerializeField]
    private float _dashGravityScale;
    [SerializeField]
    private float _dashGravityScaleTime;
    [SerializeField]
    private float _dashBeforeDelay;

    // 쿨타임 계산 관련 변수
    [SerializeField]
    private float _dashCoolDown;    //대쉬 쿨타임
    private bool _canDash = true;
    private float _dashCooldownTimer = 0f;

    [SerializeField]
    private PlayerInputDirection _direction = PlayerInputDirection.None;
    public PlayerLookingDirection _looking = PlayerLookingDirection.Right;

    public bool canChangeLookingDirection = false;

    private SpriteRenderer _sprite;

    ////나중에 발 부분에 콜라이더 하나 더 만들어서
    ////벽에 부딪혔을때 점프 초기화 안되도록 고쳐야함
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
    //    {
    //        _player.playerAni.SetBool("Jump", false);
    //        _player.playerAni.SetBool("IsJumpAttack", false);
    //        _player.playerAni.SetBool("IsJumpDown", false);
    //        _canJump = true;
    //        _player.playerState = PlayerState.Idle;
    //        _rigid.gravityScale = normalGravityScale;
    //    }
    //}
    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
    //    {
    //        _canJump = false;
    //        _player.playerState = PlayerState.Jump;
    //    }
    //}
}