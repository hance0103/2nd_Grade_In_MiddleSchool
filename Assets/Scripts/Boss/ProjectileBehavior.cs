using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField]
    private float damage; // 투사체 데미지
    private ObjectPool<GameObject> pool; // Object Pool 참조
    private bool isReleased = false; // 반환 여부 확인용 플래그
    private int delProjWallLayer; // DelProjWall 레이어 캐싱

    
    private BossProjectileEffect effect;

    public void Initialize(float damage, ObjectPool<GameObject> pool, BossProjectileEffect effect = null)
    {
        this.damage = damage;
        this.pool = pool;
        isReleased = false; // 반환 상태 초기화

        this.effect = effect;

    }

    private void Awake()
    {
        delProjWallLayer = LayerMask.NameToLayer("DelProjWall");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReleased) return; // 이미 반환된 경우 무시


        if (collision.CompareTag("Player")) // 플레이어와 충돌
        {   
            if (GameManager.Inst.player.IsInvincible())
            {
                return;
            }
            // Player 컴포넌트 체크 없이 바로 데미지 적용
            SoundManager.Instance.EffectSoundOn("21");
            //Debug.Log($"{damage}데미지 받아야함");
            PlayerHPManager.Instance.TakeDamage(damage); // 플레이어 HP 직접 감소

            if (effect.effectActive)
            {
                Debug.Log($"지속시간 {effect.time} 최대속도 {effect.maxSpeed}");

                GameManager.Inst.player.PlayerSlow(effect.time, effect.maxSpeed, effect.accel);
                
            }


            ReleaseProjectile();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                 collision.gameObject.layer == delProjWallLayer) // Ground 또는 DelProjWall과 충돌
        {
            Debug.Log("회수");
            ReleaseProjectile();
        }
    }

    // 화면 밖으로 나갔을 때 반환
    private void OnBecameInvisible()
    {
        ReleaseProjectile();
    }

    public void ReleaseProjectile()
    {
        if (!isReleased && pool != null && gameObject != null) // 반환되지 않은 경우만 실행
        {
            //Debug.Log($"프로젝트 반환: {gameObject.name}");
            isReleased = true; // 반환 상태 설정
            gameObject.SetActive(false); // 비활성화 추가
            pool.Release(gameObject); // Object Pool로 반환

            //if (!gameObject.activeInHierarchy)
            //{
            //    Debug.Log("destroy");
            //    Destroy(gameObject);
            //}
        }
    }
    private void OnDisable()
    {
        isReleased = false; // 플래그 리셋
    }
}