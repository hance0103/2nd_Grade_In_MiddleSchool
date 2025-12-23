using UnityEngine;

public class BugTrap : MonoBehaviour
{
    [SerializeField] private Vector3 targetPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Transform playerTransform = other.transform;
        playerTransform.position = targetPosition;
    }
}