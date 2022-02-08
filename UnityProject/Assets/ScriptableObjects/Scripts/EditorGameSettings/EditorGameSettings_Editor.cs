using System;
using Helpers;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public partial class EditorGameSettings
{
    [TabGroup("Editor")]
    [Tooltip("Если вкл. То при старте игры включен god mod")]
    public bool godModeOnStart;

    [TabGroup("Editor")]
    // [ReadOnly]
    [Tooltip("Если галочка включена, то при сбросе сейвов, игра не будет запускать туториал в начале. /n Совет: Лучше всего работает при сбросе сейвов (EditorTools/Clear Save Files или Cntrl + Shift + G). Не работает сейчас")]
    public bool ignoreTutorial;

    [TabGroup("Editor")]
    [DisableIf("EditorInPlayMode")]
    [Tooltip("Если галочка вкл, то сейвы сейвятся. n/ Совет: можно играть до определенного момента. Потом выключить запись сейвов и переигрывать один момент много раз (полезно для тестирования). Лучше сначала выключить игру, выключить галочку и запустить левел заново.")]
    public bool saveProgress;

    [TabGroup("Editor")]
    [Tooltip("Останавливает время когда мы рефокусируемся (например клацаем в инспекторе).")]
    [SerializeField] private bool _pauseGameOnRefocus = true;

    [TabGroup("Editor")]
    [InfoBox("Эксперементальная фича.", InfoMessageType.Error)]
    [Tooltip("Эксперементальная фича. Отключает в SECTR LateUpdate который обновляет каждый кадр membership у child-ов")]
    [SerializeField] private bool fixSectorPerfomance = false;

    [TabGroup("Editor")]
    [Tooltip("Включить автовыбор активной стадии в квестах")]
    public bool autoSelectQuestStageInEditor = false;

    [TabGroup("Editor")]
    [Tooltip("Добавляет debug предметы в инвентарь игрока на старте игры ")]
    [SerializeField] private bool _addDebugItemsOnGameStart;
    [TabGroup("Editor")]
    [Tooltip("Выставляет environment сцену в качестве главной, эмулируя освещения подобное тому что мы увидим в билде.")]
    [SerializeField] private bool _emulateSceneLight;
    [TabGroup("Editor")]
    [Tooltip("Включить отображения квестовых тригеров.")]
    [SerializeField] private bool _showQuestTriggers;
    [TabGroup("Editor")]
    [SerializeField] private bool _showControllersInitLogs;
    [TabGroup("Editor")]
    [SerializeField] private bool _showNullCoordinatesSpawnPopup = true;
    [TabGroup("Editor")]
    [SerializeField] private bool _blockPopupsOnStart = false;

    [TabGroup("Editor")]
    [SerializeField] private bool _isPerformanceTest = false;

    public bool FixSectorPerfomance
#if UNITY_EDITOR
        => fixSectorPerfomance;
#else
        => false;
#endif
    public bool PauseGameOnRefocus
#if !UNITY_EDITOR
        =>    true;
#else
        => _pauseGameOnRefocus;
#endif

#if UNITY_EDITOR

    [TabGroup("Editor")] [Button] void OpenGameSavesWindow() => EditorApplication.ExecuteMenuItem("EditorTools/Windows/GameSaveWindow");

#endif

    public bool EmulateSceneLight => ApplicationHelper.IsEditorApplication() ? _emulateSceneLight : false;


    public bool AddDebugItemsOnGameStart => Debug.isDebugBuild ? _addDebugItemsOnGameStart : false;

    public Action<bool> OnShowQuestTriggers;
    public bool ShowQuestTriggers
    {
        get
        {
            return Debug.isDebugBuild ? _showQuestTriggers : false;
        }
        set
        {
            _showQuestTriggers = value;
            OnShowQuestTriggers?.Invoke(_showQuestTriggers);
        }
    }

    public bool ShowControllersSystemLogs => Debug.isDebugBuild ? _showControllersInitLogs : false;
    public bool ShowNullCoordinatesSpawnPopup => Debug.isDebugBuild ? _showNullCoordinatesSpawnPopup : false;
    public bool BlockPopupsOnStart => Debug.isDebugBuild ? _blockPopupsOnStart : false;
    public bool IsPerformanceTest
    {
        get => _isPerformanceTest;
        set => _isPerformanceTest = value;
    }
}