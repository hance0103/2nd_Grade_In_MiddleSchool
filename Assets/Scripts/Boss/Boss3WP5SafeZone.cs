using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss3WP5SafeZone : MonoBehaviour
{
    BoxCollider2D boxCol;
    public Collider2D targetCol;
    void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        targetCol = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCol != null && IsFullyInside(boxCol, targetCol))
        {
            Debug.Log("완전히 포함됨");
        }
    }

    private bool IsFullyInside(BoxCollider2D container, Collider2D target)
    {
        Bounds containerBounds = container.bounds;
        Bounds targetBounds = target.bounds;

        Vector3[] corners = new Vector3[8]
        {
        new Vector3(targetBounds.min.x, targetBounds.min.y),
        new Vector3(targetBounds.min.x, targetBounds.max.y),
        new Vector3(targetBounds.max.x, targetBounds.min.y),
        new Vector3(targetBounds.max.x, targetBounds.max.y),
        new Vector3(targetBounds.center.x, targetBounds.min.y),
        new Vector3(targetBounds.center.x, targetBounds.max.y),
        new Vector3(targetBounds.min.x, targetBounds.center.y),
        new Vector3(targetBounds.max.x, targetBounds.center.y),
        };

        foreach (var point in corners)
        {
            if (!containerBounds.Contains(point))
                return false;
        }

        return true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어 콜라이더가 부딪혔을때
        if (collision != null && collision.CompareTag("PlayerContact"))
        {
            Debug.Log("플레이어 입갤");
            targetCol = collision;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("PlayerContact"))
        {
            Debug.Log("플레이어 퇴갤");
            targetCol = null;
        }
    }
}
