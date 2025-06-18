using UnityEngine;

public class IgnoreGrayscale : MonoBehaviour
{
    const string kTag = "NoGray";        // 원하는 태그 이름
    const string kLayer = "ColorOnly";   // 위에서 만든 레이어 이름

    void Awake()
    {
        if (CompareTag(kTag))
        {
            int layerIdx = LayerMask.NameToLayer(kLayer);
            SetLayerRecursively(gameObject, layerIdx);
        }
    }
    static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform t in obj.transform)
            SetLayerRecursively(t.gameObject, layer);
    }
}
