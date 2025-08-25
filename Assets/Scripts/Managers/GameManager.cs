using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{

    static GameManager s_inst;
    public static GameManager Inst
    {
        get
        {
            if (s_inst == null)
            {
                s_inst = FindObjectOfType<GameManager>();
                if (s_inst == null)
                {
                    GameObject go = new GameObject("@GameManager");
                    s_inst = go.AddComponent<GameManager>();
                }
            }
            return s_inst;
        }
    }

    InputManager _input = new InputManager();
    ResourceManager _resource = new ResourceManager();
    UIManager _ui_manager = new UIManager();
    SoundManager _sound = new SoundManager();
    SaveLoadManager _saveLoad = new SaveLoadManager();

    [SerializeField]
    private PlayerController _playerController;
    public PlayerController player
    {
        get
        {
            if (_playerController == null)
            {
                _playerController = FindObjectOfType<PlayerController>();
            }
            return _playerController;
        }
    }

    public static InputManager Input { get { return Inst._input; } }
    public static ResourceManager Resource { get { return Inst._resource; } }
    public static UIManager UI { get { return Inst._ui_manager; } }
    public static SoundManager Sound { get { return Inst._sound; } }
    public static SaveLoadManager SaveLoad { get { return Inst._saveLoad; } }
    public static bool isPlayerZoomOutAllowed = false;
    public static bool isFinishBossZoominAllowed = false;

    public static bool isStage1Cleared = false;
    public static bool isStage2Cleared = false;
    public static bool isStage3Cleared = false;

    private int nowStage = 0;
    public void SetNowStage(int stage)
    {
        nowStage = stage;
    }
    public int NowStage { get { return nowStage; } }

    private void Awake()
    {
        if (s_inst != null && s_inst != this)
        {
            Destroy(gameObject);
            return;
        }

        s_inst = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    private void Update()
    {
        _input.OnUpdate();
    }
}