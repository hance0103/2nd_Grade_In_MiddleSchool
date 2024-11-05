using UnityEngine;

[CreateAssetMenu(fileName = "SOTest", menuName = "Scriptable Object/SOTest", order = int.MaxValue)]

public class ScriptableObjectTest : ScriptableObject
{
    [SerializeField]
    private string patternName;
    public string PatternName { get { return patternName; } }
    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }
    [SerializeField]
    private float teleportRange;
    public float TeleportRange { get { return teleportRange; } }
    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }

}