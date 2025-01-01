using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class ProjectileBehaviour : MonoBehaviour
{
    private float damage;
    private ObjectPool<GameObject> pool;
    private bool isReleased = false; // 반환 여부 확인용 플래그

    public void Initialize(float damage, ObjectPool<GameObject> pool)
    {
        this.damage = damage;
        this.pool = pool;
        isReleased = false; // 초기화 시 플래그 리셋
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReleased) return; // 이미 반환된 경우 무시

        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                Debug.Log($"Player hit! Damage: {damage}");
                // player.TakeDamage(damage); // 실제 데미지 적용 로직
            }
            ReleaseProjectile();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            ReleaseProjectile();
        }
    }

    private void ReleaseProjectile()
    {
        if (!isReleased) // 반환되지 않은 경우만 실행
        {
            isReleased = true;
            pool.Release(gameObject); // 풀에 반환
        }
    }

}