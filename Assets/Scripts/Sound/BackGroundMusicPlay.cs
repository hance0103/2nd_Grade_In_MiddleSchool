using UnityEngine;

public class BackgroundMusicDontDestroy : MonoBehaviour
{
    private static BackgroundMusicDontDestroy instance;

    void Awake()
    {
        // 중복된 오브젝트가 생성되지 않도록 싱글톤 패턴 적용
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 삭제되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 존재하는 인스턴스가 있다면 새 오브젝트 삭제
        }
    }
}