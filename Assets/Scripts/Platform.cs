using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Platform : MonoBehaviour
{
    public bool iscontacting = false;
    BoxCollider2D col;
    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }
    private void Update()
    {
        if (!iscontacting)
        {
            col.isTrigger = true;
        }
        
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        col.isTrigger = false;
        iscontacting = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            iscontacting = false;
        }
    }
    
}
