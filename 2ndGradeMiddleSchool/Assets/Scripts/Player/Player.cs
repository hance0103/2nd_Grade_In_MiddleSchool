using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    static Player s_inst;
    public static Player Inst { get { return s_inst; } }

    public PlayerState playerState = PlayerState.None;

}
