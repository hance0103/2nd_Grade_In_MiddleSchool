using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    private float _normalAttack_verRange;
    [SerializeField]
    private float _normalAttack_horRange;
    [SerializeField]
    private float _normalAttack_dmg;
    [SerializeField]
    private float _normalAttack_delay;

    [SerializeField]
    private float _airAttack_verRange;
    [SerializeField]
    private float _airAttack_horRange;
    [SerializeField]
    private float _airAttack_diveVelocity;
    [SerializeField]
    private float _airAttack_dmg;


    // Start is called before the first frame update
    void Start()
    {

    }
}
