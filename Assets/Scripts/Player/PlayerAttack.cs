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
    private float _jumpAttack_verRange;
    [SerializeField]
    private float _jumpAttack_horRange;
    [SerializeField]
    private float _jumpAttack_diveVelocity;
    [SerializeField]
    private float _jumpAttack_dmg;

    private Player _player;
    private PlayerMover _playerMove;
    private void Start()
    {
        _player = GetComponent<Player>();
        _playerMove = GetComponent<PlayerMover>();
    }
    private void FixedUpdate()
    {
        PlayerAttackInput();
    }
    private void PlayerAttackInput()
    {
        if (Input.GetKey(KeyCode.A) && Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + _normalAttack_delay; // 다음 발사 시간 갱신
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
            Debug.Log("투사체 발사!");
        }
    }
}
