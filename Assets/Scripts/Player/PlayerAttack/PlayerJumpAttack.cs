using System;
using UnityEngine;

public class PlayerJumpAttack : MonoBehaviour
{
    [SerializeField] private PlayerController controller;

    public bool isBossHit = false;
    //public float _delay { get; set; }
    // Start is called before the first frame update
    private void OnEnable()
    {
        isBossHit = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            // 보스 안치고 내려왔을 경우
            if (!isBossHit)
            {
                controller.CamShake();
                StartCoroutine(controller.PlayerJumpAttackAfterDelay());
            }
            // 보스 치고 내려왔을 경우
            else
            {
                
            }
        }
        if (collision.gameObject.CompareTag("Boss"))
        {
            if (!isBossHit)
            {
                isBossHit = true;
                Debug.Log("보스 타격");
                controller.CamShake();
                SoundManager.Instance.Play("PlayerSound/PlayerJumpAttackHit");
                BossHPManager.Instance.TakeDamage(controller._jumpAttack_dmg);

                StartCoroutine(controller.PlayerJumpAttackDelay());
            }

        }
    }

}
