using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RainBehavior : MonoBehaviour
{
    private float damage; // 투사체 데미지
    private ObjectPool<GameObject> pool; // Object Pool 참조
    private bool isReleased = false; // 반환 여부 확인용 플래그
    private int delProjWallLayer; // DelProjWall 레이어 캐싱


    public void Initialize(float damage, ObjectPool<GameObject> pool)
    {
        this.damage = damage;
        this.pool = pool;
        isReleased = false; // 반환 상태 초기화
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
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                SoundManager.Instance.EffectSoundOn("21");
                Debug.Log($"플레이어 피격! 데미지: {damage}");
                PlayerHPManager.Instance.TakeDamage(damage); // 실제 데미지 적용 로직
            }
            ReleaseProjectile();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                 collision.gameObject.layer == delProjWallLayer) // Ground 또는 DelProjWall과 충돌
        {
            ReleaseProjectile();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("RainGround")) // 보스 2 비 투사체
        {
            SoundManager.Instance.EffectSoundOn("25-2");
        }
    }

    // 화면 밖으로 나갔을 때 반환
    private void OnBecameInvisible()
    {
        ReleaseProjectile();
    }

    private void ReleaseProjectile()
    {
        if (!isReleased && pool != null && gameObject != null) // 반환되지 않은 경우만 실행
        {
            Debug.Log($"프로젝트 반환: {gameObject.name}");
            isReleased = true; // 반환 상태 설정
            gameObject.SetActive(false); // 비활성화 추가
            pool.Release(gameObject); // Object Pool로 반환
            //if (!gameObject.activeInHierarchy)
            //{
            //    Destroy(gameObject);
            //}
        }
    }
    private void OnDisable()
    {
        isReleased = false; // 플래그 리셋
    }
}
