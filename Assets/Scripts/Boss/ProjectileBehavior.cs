using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class ProjectileBehaviour : MonoBehaviour
{
    private float damage;
    private ObjectPool<GameObject> pool;

    public void Initialize(float damage, ObjectPool<GameObject> pool)
    {
        this.damage = damage;
        this.pool = pool;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                //player.TakeDamage(damage);
            }
            // 풀에 반환
            pool.Release(gameObject);
        }
    }
}