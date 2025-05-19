using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNormalAttack : MonoBehaviour
{
    private float _speed;
    private float _lifeTime;
    private float _damage;
    private bool _attackDirection;
    private SpriteRenderer sprite;
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
        sprite = GetComponent<SpriteRenderer>();
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
            sprite.flipX = true;
            transform.Translate(vec * _speed * Time.deltaTime);
        }
        
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Boss"))
        {
            //Debug.Log("보스 타격");
            BossHPManager.Instance.TakeDamage(_damage);
            Destroy(gameObject);
        }
        
    }
}
