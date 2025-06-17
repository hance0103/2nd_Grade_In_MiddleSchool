using UnityEngine;

public class PlayerJumpAttack : MonoBehaviour
{
    [SerializeField] private PlayerController controller;

    private bool isBossHit = false;
    public float _delay { get; set; }
    // Start is called before the first frame update
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("콜리전 엔터");
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("트리거 엔터");
        if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Platform"))
        {
            if (!isBossHit)
            {
                
            }
            controller.CamShake();
            StartCoroutine(controller.PlayerJumpAttackAfterDelay());
            isBossHit = false;
        }
        if (collision.gameObject.CompareTag("Boss"))
        {
            //controller.CamShake();
            SoundManager.Instance.Play("PlayerSound/PlayerJumpAttackHit");
            BossHPManager.Instance.TakeDamage(controller._jumpAttack_dmg);
            isBossHit = true;
           StartCoroutine(controller.PlayerJumpAttackDelay());
        }
    }

}
