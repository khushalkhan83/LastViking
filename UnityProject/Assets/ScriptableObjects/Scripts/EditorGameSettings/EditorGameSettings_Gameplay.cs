using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public partial class EditorGameSettings
{
    [TabGroup("Gameplay")]
    [Tooltip("Выбранная сцена загрузиться на старте игры.")]
    [SerializeField] private bool resetKrakenOnStart;
    [TabGroup("Gameplay")]
    [Tooltip("Игнор цены в RequiredItemsObject.")]
    [SerializeField] private bool ignoreItemsPrice;
    [TabGroup("Gameplay")] 
    [SerializeField] private bool breakWeaponImmediately;
    [TabGroup("Gameplay")]
    [SerializeField] private bool useDebugEnemiesForBaseProtection;

    public bool ResetKrakenOnStart => Debug.isDebugBuild ? resetKrakenOnStart : false;
    public bool IgnoreItemsPrice
    {
        get{ return Debug.isDebugBuild ? ignoreItemsPrice : false;}
        set{ ignoreItemsPrice = value;}
    }
    public bool UseDebugEnemiesForBaseProtection => Debug.isDebugBuild ? useDebugEnemiesForBaseProtection : false;
   

    public bool BreakWeaponImmediately => Debug.isDebugBuild ? breakWeaponImmediately: false;

    #if UNITY_EDITOR

    [TabGroup("Gameplay")] [Button] void CoreRestart() => EditorApplication.ExecuteMenuItem("EditorTools/RestartCore");

    #endif
}