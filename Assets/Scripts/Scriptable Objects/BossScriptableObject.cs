using UnityEngine;

[CreateAssetMenu(fileName = "BossScriptableObject", menuName = "Scriptable Object/BossScriptableObject", order = int.MaxValue)]

public class BossScriptableObject : ScriptableObject
{
    //패턴이름
    [SerializeField]
    private string patternName;
    public string PatternName { get { return patternName; } }

    [Header("공격")]
    [Space(10f)]
    //데미지
    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } }
    //공격전 딜레이
    [SerializeField]
    private float beforeattackDelay;
    public float BeforeAttackDelay { get { return beforeattackDelay; } }
    //공격범위
    [SerializeField]
    private float attackRange;
    public float AttackRange { get { return attackRange; } }
    //공격후 넘어가는 시간
    [SerializeField]
    private float afterAttackDelay;
    public float AfterAttackDelay { get { return afterAttackDelay; } }

    [Header("텔레포트")]
    [Space (10f)]
    //텔포 오프셋 (플리이어로부터 거리)
    [SerializeField]
    private Vector3 teleportOffset;
    public Vector3 TeleportOffset { get { return teleportOffset; } }
    //텔레포트 대기시간
    [SerializeField]
    private float teleportWaitTime;
    public float TeleportWaitTime { get { return teleportWaitTime; } }
}