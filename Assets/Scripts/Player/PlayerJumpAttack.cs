using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttack : MonoBehaviour
{
    private PlayerAttack _player;
    private float _jumpAttackDelayCount = 0;
    public float _delay { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<PlayerAttack>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss"))
        {
            _player._enemy = collision.gameObject;
        }
    }

    IEnumerator PlayerJumpAttackDelay()
    {
        _jumpAttackDelayCount = 0;
        Time.timeScale = 0;
        while (_jumpAttackDelayCount <= _jumpAttackDelayCount)
        {
            yield return null;
        }
        Time.timeScale = 1;
    }
}
