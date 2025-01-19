using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNormalAttack : MonoBehaviour
{
    private float _speed;
    private float _lifeTime;
    private float _damage;
    //true면 오른쪽 false면 왼쪽
    private bool _attackDirection;
    [Header("일반 공격 데미지")]
    [SerializeField] private float damamgevalue;
    public void AttackSetting(float damage, float speed, float lifeTime, bool direction)
    {
        _damage = damage;
        _speed = speed;
        _lifeTime = lifeTime;
        _attackDirection = direction;
    }
    void Start()
    {
        Destroy(gameObject, _lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (_attackDirection)
        {
            Vector3 vec = new Vector3(1, 0, 0);
            transform.Translate(vec * _speed * Time.deltaTime);
        }
        else
        {
            Vector3 vec = new Vector3(-1, 0, 0);
            transform.Translate(vec * _speed * Time.deltaTime);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Boss"))
        {
            // 싱글톤으로 구현된 보스 HP 감소 함수 호출
            BossHPManager.Instance.TakeDamage(10);
        }
        Destroy(gameObject);
    }
}
