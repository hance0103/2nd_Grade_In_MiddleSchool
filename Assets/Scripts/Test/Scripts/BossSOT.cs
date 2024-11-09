using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSOT : MonoBehaviour
{
    [SerializeField]
    private BossScriptableObject bossScriptableObjectData;
    public BossScriptableObject BossScriptableObject { set { bossScriptableObjectData = value; } }

    public void BossDataLog()
    {
        Debug.Log("패턴 이름 :: " + bossScriptableObjectData.PatternName);
        Debug.Log("공격력 :: " + bossScriptableObjectData.Damage);
        Debug.Log("텔포 범위 :: " + bossScriptableObjectData.TeleportRange);
        Debug.Log("이동속도 :: " + bossScriptableObjectData.MoveSpeed);
    }

}
