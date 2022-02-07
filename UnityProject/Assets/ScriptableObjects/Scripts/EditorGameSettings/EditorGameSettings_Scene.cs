using System.Collections.Generic;
using CustomeEditorTools.EditorGameSettingsData;
using Game.Models;
using RoboRyanTron.SearchableEnum;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public partial class EditorGameSettings
{
    [TabGroup("Scene")]
    [Tooltip("Выбранная сцена загрузиться на старте игры.")]
    public bool startInSelectedEnvironment;
    [TabGroup("Scene")]
    [ShowIf("startInSelectedEnvironment")]
    [SearchableEnum]
    public EnvironmentSceneID debugStartEnvironment;

    [TabGroup("Scene")]
    [DisableIf("EditorInPlayMode")]
    [Tooltip("Експерементальная фича. Позволяет работать с несколькими открытыми сценами (например Core и Island/Waterfall) в открытом состоянии. После выхода с Play Mode порядок сцен востанавливается. [Documentation Placeholder]")]
    public bool multiScenePlayMode;

    [TabGroup("Scene")]
    [InfoBox("Experimental.", InfoMessageType.Warning)]
    [Tooltip("Позволяет старт овать с Core или Loading scene при нажатии на Play кнопку. Не открывает рабочую сцену после использования! Experimental.")]
    [DisableIf("EditorInPlayMode")]
    public bool customePlayMode;

    [TabGroup("Scene")]
    [ShowIf("customePlayMode")]
    [DisableIf("EditorInPlayMode")]
    public CustomePlayModeStartPoint playModeStartPoint;

    #if UNITY_EDITOR

    [TabGroup("Scene")] [Button] void RestartCore() => EditorApplication.ExecuteMenuItem("EditorTools/RestartCore");
    [TabGroup("Scene")] [Button] void EmulateRestartGam() => EditorApplication.ExecuteMenuItem("EditorTools/EmulateRestartGame");
    [TabGroup("Scene")] [Button] void SaveGame() => EditorApplication.ExecuteMenuItem("EditorTools/Save game");
    [TabGroup("Scene")] [Button] void OpenEnvironmentGenerator() => EditorApplication.ExecuteMenuItem("EditorTools/Windows/CodeGeneratorWindow_2.0");

    #endif

    [TabGroup("Scene")]
    public ScenesProfile scenesProfile_Default;


    [HideInInspector]
    public List<string> unloadedScenesBeforeStartPlayMode = new List<string>();
    [HideInInspector]
    public string activeSceneBefourePlayMode;
}