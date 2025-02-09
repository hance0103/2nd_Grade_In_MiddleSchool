using UnityEngine;

[CreateAssetMenu(fileName = "LaserScriptableObject", menuName = "Scriptable Object/LaserScriptableObject", order = int.MaxValue)]

public class LaserScriptableObject : ScriptableObject
{
    // 사용할 레이저 공격 이름
    [SerializeField] private string laserType;
    public string LaserType { get { return laserType; } }

    // 데미지
    [SerializeField] private float damage;
    public float Damage { get { return damage; } }

    // 레이저속도
    [SerializeField] private float laserSpeed;
    public float LaserSpeed { get { return laserSpeed; } }

    // 레이저 지속시간
    [SerializeField] private float laserDuration;
    public float LaserDuration { get { return laserDuration; } }
    // 레이저 스프라이트 지정
    [SerializeField] private string laserSprite;
    public string LaserSprite { get { return laserSprite; } }

    [Header("추가 설정을 위한 변수들")]
    [SerializeField] private float laserWidth = 0.2f;
    public float LaserWidth { get { return laserWidth; } }
    
    [SerializeField] private Color laserColor = Color.red;
    public Color LaserColor { get { return laserColor; } }

    [SerializeField] private float maxDistance = 100f;
    public float MaxDistance { get { return maxDistance; } }

    [SerializeField] private LayerMask targetLayer;
    public LayerMask TargetLayer { get { return targetLayer; } }


    [Header("스테이지 2 전용 레이저 타이밍 설정")]
    [SerializeField] private float laserFollowDuration = 2f; // 레이저가 플레이어를 따라다니는 시간
    public float LaserFollowDuration { get { return laserFollowDuration; } }

    [SerializeField] private float laserLockDuration = 1f;   // 레이저가 고정되는 시간
    public float LaserLockDuration { get { return laserLockDuration; } }

}