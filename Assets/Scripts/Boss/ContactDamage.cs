using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField]
    private float _damage = 10f;

    [SerializeField]
    private bool _isContacting = false;

    [SerializeField]
    private PlayerHPManager _hp;

    void Start()
    {
    }
    private void Update()
    {
        if (_isContacting)
        {
            _hp.TakeDamage(_damage);
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
