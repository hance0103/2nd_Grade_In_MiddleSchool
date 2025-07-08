using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBoss : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnclickBoss()
    {
        BossHPManager.Instance.TakeDamage(100);
    }
    public void OnclickPlayer()
    {
        PlayerHPManager.Instance.TakeDamage(100);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
