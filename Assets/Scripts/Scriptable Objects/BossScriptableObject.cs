using UnityEngine;

[CreateAssetMenu(fileName = "BossScriptableObject", menuName = "Scriptable Object/BossScriptableObject", order = int.MaxValue)]

public class BossScriptableObject : ScriptableObject
{
    //패턴이름
    [SerializeField]
    private string patternName;
    public string PatternName { get { return patternName; } }
    //데미지
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }
    //텔포 오프셋 (플리이어로부터 거리)               //Vector로 받아야하는지 float로 받아야하는지 모르겠어요
    //[SerializeField]
    //private float teleportOffset;
    //public float TeleportOffset { get { return teleportOffset; } }
    [SerializeField]
    private Vector3 teleportOffset;
    public Vector3 TeleportOffset { get { return teleportOffset; } }

    //이동속도
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }
    //공격딜레이
    [SerializeField]
    private int attackDelay;
    public int AttackDelay { get { return attackDelay; } }
    
}