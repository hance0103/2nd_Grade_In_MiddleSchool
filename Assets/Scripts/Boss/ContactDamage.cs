using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField]
    private float _damage = 10f;
    [SerializeField]
    private float _damageInnterval = 0.5f;

    private float _lastDamageTime = -999f;

    private BoxCollider2D _collider;
    [SerializeField]
    private bool _isContacting = false;

    [SerializeField]
    private PlayerHPManager _hp;

    [SerializeField]
    private float _contactCounter = 0;
    void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (_isContacting)
        {
            _contactCounter += Time.deltaTime;
            if (_contactCounter >= _damageInnterval)
            {
                _contactCounter = 0;
                _hp.TakeDamage(_damage);
            }
        }
        else
        {
            _contactCounter = _damageInnterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerContact"))
        {
            _isContacting = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerContact"))
        {
            _isContacting = false;
        }
    }
}
