using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterBox : MonoBehaviour
{
    void Awake() { DontDestroyOnLoad(gameObject); }
}