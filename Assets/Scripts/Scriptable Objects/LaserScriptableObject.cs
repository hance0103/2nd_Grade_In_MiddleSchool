using UnityEngine;

[CreateAssetMenu(fileName = "LaserScriptableObject", menuName = "Scriptable Object/LaserScriptableObject", order = int.MaxValue)]

public class LaserScriptableObject : ScriptableObject
{
    //사용할 레이저 공격 이름
    [SerializeField]
    private string laserType;
    public string LaserType { get { return laserType; } }
    //데미지
    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } }
    //레이저 발사 위치 오프셋
    [SerializeField]
    private Vector3 laserOffset;
    public Vector3 LaserOffset { get { return laserOffset; } }
    //레이저속도
    [SerializeField]
    private float laserSpeed;
    public float LaserSpeed { get { return laserSpeed; } }
    //레이저 지속시간
    [SerializeField]
    private float laserDuration;
    public float LaserDuration { get { return laserDuration; } }

}