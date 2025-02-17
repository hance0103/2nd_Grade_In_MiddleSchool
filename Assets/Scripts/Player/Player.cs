using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
public class Player : MonoBehaviour
{
    public PlayerStateMachine stateMachine = new();

    private void Start()
    {
        //stateMachine.ChangeState(new IdleState());
    }

    //public PlayerState playerState = PlayerState.Idle;

    [SerializeField]
    private int _playerHp;

    public Animator playerAni;
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
