using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearManager : MonoBehaviour
{
    [Header("스테이지 가림막 오브젝트")]
    [SerializeField] private GameObject stage2Blocker;
    [SerializeField] private GameObject stage2BlockerSprite;
    [SerializeField] private GameObject stage3Blocker;
    [SerializeField] private GameObject stage3BlockerSprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake() => ApplyStageLocks();
    public void ResetAllClears()
    {
        PlayerPrefs.SetInt("Stage1Clear", 0);
        PlayerPrefs.SetInt("Stage2Clear", 0);
        PlayerPrefs.SetInt("Stage3Clear", 0);
        PlayerPrefs.Save();
        ApplyStageLocks();
    }
    private void ApplyStageLocks()
    {

        // 1스테이지가 클리어되면 Stage1Clear == 1
        bool stage1Cleared = PlayerPrefs.GetInt("Stage1Clear", 0) == 1;
        if (stage2Blocker) stage2Blocker.SetActive(!stage1Cleared);
        if (stage2BlockerSprite) stage2BlockerSprite.SetActive(!stage1Cleared);

        // 2스테이지가 클리어되면 Stage2Clear == 1
        bool stage2Cleared = PlayerPrefs.GetInt("Stage2Clear", 0) == 1;
        if (stage3Blocker) stage3Blocker.SetActive(!stage2Cleared);
        if (stage3BlockerSprite) stage3BlockerSprite.SetActive(!stage2Cleared);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
