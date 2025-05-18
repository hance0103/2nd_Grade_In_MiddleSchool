using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField]
    private float shakeDuration = 1f;
    [SerializeField]
    private float shakeMagitude = 0.5f;

    Vector3 initPos;
    public void StartShake()
    {
        StartShake(shakeDuration, shakeMagitude);
    }
    public void StartShake(float duration, float magnitude)
    {
        initPos = transform.position;
        StartCoroutine(Shake(duration, magnitude));
    }
    IEnumerator Shake(float duration, float magnitude)

    {
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            transform.position = initPos + (Vector3)Random.insideUnitCircle * magnitude;
            elapsedTime += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = initPos;
    }
}
