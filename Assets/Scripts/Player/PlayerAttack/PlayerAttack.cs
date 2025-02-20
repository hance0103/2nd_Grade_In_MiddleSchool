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
    [SerializeField]
    private float _jumpAttack_diveVelocity;
    [SerializeField]
    private float _jumpAttack_dmg;
    public float _jumpAttackHitDelay;
    [SerializeField]
    private GameObject _jumpAttackObject;

    private float _jumpAttackObjX;
    private float _jumpAttackObjY;

    private Player _player;
    private PlayerMover _playerMove;

    [SerializeField]
    private bool isKeyDown = false;



    public GameObject _enemy;

    private void Start()
    {
        _player = GetComponent<Player>();
        _playerMove = GetComponent<PlayerMover>();
        _jumpAttackObjX = _jumpAttackObject.transform.localPosition.x;
        _jumpAttackObjY = _jumpAttackObject.transform.localPosition.y;
    }
    private void Update()
    {


        PlayerAttackCancel();
    }
    private void PlayerAttackInput()
    {
        if (Input.GetKey(KeyCode.A) && Time.time >= _nextFireTime)
        {
            _player.playerAni.SetBool("IsNormalAttack", true);
            //_player.playerState = PlayerState.Attack;
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
        if (Input.GetKeyUp(KeyCode.A) )
        {
            //_player.playerAni.SetBool("IsNormalAttack", false);
            //_player.playerState = PlayerState.Idle;
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            _player.playerAni.SetBool("IsJumpAttack", true);
            _jumpAttackObject.SetActive(true);
            if (_playerMove._looking == PlayerLookingDirection.Right)
            {
                _jumpAttackObject.transform.localPosition = new Vector3(_jumpAttackObjX, _jumpAttackObjY, 0);
                _jumpAttackObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                _jumpAttackObject.transform.localPosition = new Vector3(-_jumpAttackObjX, _jumpAttackObjY, 0);
                _jumpAttackObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            //_player.playerState = PlayerState.JumpAttack;
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -_jumpAttack_diveVelocity);
            //gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;


        }
    }
    public IEnumerator PlayerJumpAttackDelay()
    {
        Debug.Log("시간 정지");
        Time.timeScale = 0.0f;
        yield return new WaitForSecondsRealtime(_jumpAttackHitDelay);
        Time.timeScale = 1;
        _jumpAttackObject.SetActive(false);
        Debug.Log("시간 복구");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 부딪힌 물체 데미지
    }
}
