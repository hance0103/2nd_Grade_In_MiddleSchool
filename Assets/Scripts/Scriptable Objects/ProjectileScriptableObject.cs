using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileScriptableObject", menuName = "Scriptable Object/ProjectileScriptableObject", order = int.MaxValue)]

public class ProjectileScriptableObject : ScriptableObject
{
   
    //데미지
    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } }
    //투사체속도
    [SerializeField]
    private float projectileSpeed;
    public float ProjectileSpeed { get { return projectileSpeed; } }
    // 발사체 크기
    [SerializeField]
    private Vector3 projectileScale;
    public Vector3 ProjectileScale { get { return projectileScale; } }

    // 발사 속도 (초당 발사체 수)
    [SerializeField]
    private float fireRate;
    public float FireRate { get { return fireRate; } }

}