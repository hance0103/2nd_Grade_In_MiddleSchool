using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpAttack : MonoBehaviour
{
    [SerializeField] private PlayerController controller;


    public float _delay { get; set; }
    // Start is called before the first frame update
    void Start()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Camera.main.GetComponent<CameraShaker>().StartShake(controller.camShakeDuration, controller.camShakeMagnitude);
        if (collision.gameObject.CompareTag("Boss"))
        {
            Debug.Log("점공 보스 타격");
           BossHPManager.Instance.TakeDamage(controller._jumpAttack_dmg);

           StartCoroutine(controller.PlayerJumpAttackDelay());
        }
    }

}
