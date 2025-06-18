using UnityEngine;

public class PlayerJumpAttack : MonoBehaviour
{
    [SerializeField] private PlayerController controller;

    private bool isBossHit = false;
    public float _delay { get; set; }
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"트리거 엔터 : {collision.name}");
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
