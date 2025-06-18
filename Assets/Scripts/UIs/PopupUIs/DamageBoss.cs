using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBoss : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Onclick()
    {
        BossHPManager.Instance.TakeDamage(100);
        Blink.instance.StartBlink();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
