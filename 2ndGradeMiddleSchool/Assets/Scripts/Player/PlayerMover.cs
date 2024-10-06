using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField]
    private float _movSpeed;
    [SerializeField]
    private float _maxMovSpeed;
    [SerializeField]
    private float _movAccel;

    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _jumpTimeToApex;
    [SerializeField]
    private float _maxFallingSpeed;
    [SerializeField]
    private float _jumpHangGravityMult;

    [SerializeField]
    private float _dashDistance;
    [SerializeField]
    private float _dashDelay;

    private PlayerMoveDirection direction = PlayerMoveDirection.None;

    [SerializeField]
    private PlayerLookingDirection _looking = PlayerLookingDirection.None;

    void Start()
    {
        GameManager.Input.KeyAction -= OnKeyDown;
        GameManager.Input.KeyAction += OnKeyDown;
    }

    public void OnKeyDown()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction = PlayerMoveDirection.UpLeft;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                direction = PlayerMoveDirection.DownLeft;
            }
            else
                direction = PlayerMoveDirection.Left;
            _looking = PlayerLookingDirection.Left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                direction = PlayerMoveDirection.UpRight;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                direction = PlayerMoveDirection.DownRight;
            }
            else
                direction = PlayerMoveDirection.Right;
            _looking = PlayerLookingDirection.Right;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            direction = PlayerMoveDirection.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            direction = PlayerMoveDirection.Down;
        }

        if (direction != PlayerMoveDirection.None)
        {
            Debug.Log(direction.ToString());
            //Player.Inst.playerState = PlayerState.Move;
        }
            

        if (Input.GetKey(KeyCode.C))
        {
            Debug.Log("มกวม");
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            Player.Inst.playerState = PlayerState.None;
        }
    }
}