using UnityEngine;
using System.Collections;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager _instance;

    public static CoroutineManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CoroutineManager");
                _instance = obj.AddComponent<CoroutineManager>();
                DontDestroyOnLoad(obj); // 매니저 오브젝트가 씬 전환 시 파괴되지 않음
            }
            return _instance;
        }
    }

    public Coroutine StartCoroutineExternally(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }
}