using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
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
    //[SerializeField]
    //private float _jumpAttack_verRange;
    //[SerializeField]
    //private float _jumpAttack_horRange;
    [SerializeField]
    private float _jumpAttack_diveVelocity;
    [SerializeField]
    private float _jumpAttack_dmg;
    [SerializeField]
    private float _jumpAttackHitDelay;
    private float _jumpAttackDelayCount;

    private Player _player;
    private PlayerMover _playerMove;

    [SerializeField]
    private bool isKeyDown = false;

    public GameObject _enemy;
    private void Start()
    {
        _player = GetComponent<Player>();
        _playerMove = GetComponent<PlayerMover>();
    }
    private void Update()
    {
        if (_player.playerState != PlayerState.Jump && _player.playerState != PlayerState.JumpAttack)
        {
            if (_player.playerState != PlayerState.Dash || _player.playerState != PlayerState.JumpAttack
                || _player.playerState != PlayerState.JumpAttack)
            {
                PlayerAttackInput();
            }

            PlayerAttackInput();
        }
        else if (_player.playerState == PlayerState.Jump)
        {
            if (!isKeyDown && _player.playerState != PlayerState.JumpAttack)
            {
                //점프공격

            }
            PlayerJumpAttack();
        }

        PlayerAttackCancel();
    }
    private void PlayerAttackInput()
    {
        if (Input.GetKey(KeyCode.A) && Time.time >= _nextFireTime)
        {
            _player.playerState = PlayerState.Attack;
            Shoot();
            _nextFireTime = Time.time + _normalAttack_delay; // 다음 발사 시간 갱신
            isKeyDown = true;
        }

    }
    private void PlayerNormalAttack()
    {
    }
    private void PlayerAttackCancel()
    {
        if (Input.GetKeyUp(KeyCode.A) && _player.playerState == PlayerState.Attack)
        {
            _player.playerState = PlayerState.Idle;
            isKeyDown = false;
        }
    }
    private void Shoot()
    {
        if (_normalAttackPrefab != null && _attackPoint != null)
        {
            bool atttackDirection;
            if (_playerMove._looking == PlayerLookingDirection.Right) 
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
    private void PlayerJumpAttack()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            _player.playerState = PlayerState.JumpAttack;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2 (0, -_jumpAttack_diveVelocity);
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;


        }
    }


}
