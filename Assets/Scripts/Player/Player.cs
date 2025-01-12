using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerState playerState = PlayerState.Idle;

    [SerializeField]
    private int _playerHp;

    // 플레이어 피격시
    public void Onhit(int dmg)
    {
        _playerHp -= dmg;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
}
