using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttack : MonoBehaviour
{
    [SerializeField]
    private PlayerAttack _pAttack;
    public float _delay { get; set; }
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            _pAttack._enemy = collision.gameObject;
            BossHPManager.Instance.TakeDamage(30);
            //StartCoroutine(_pAttack.PlayerJumpAttackDelay());
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }
    }


}
