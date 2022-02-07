using Game.Models;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Extensions;

public class _TestCutsceneLoader : MonoBehaviour
{
    [SerializeField] private bool _debugMode;
    [SerializeField] private ColliderTriggerModel _colliderTriggerModelLoader;
    [SerializeField] private ColliderTriggerModel _colliderTriggerModelUnloader;
    private CutsceneKrakenModel CutsceneKrakenModel => ModelsSystem.Instance._cutsceneKrakenModel;
    private LayerMask ActivatorLayer => CutsceneKrakenModel.ActivatorLayer;

    public _TestCutsceneManager TestCutsceneManager => FindObjectOfType<_TestCutsceneManager>();

    private bool hasEntered = false;
    private bool cutsceneUnloaded;

    #region MonoBehaviour
    private void OnEnable()
    {
        _colliderTriggerModelLoader.OnEnteredTrigger += OnEnteredTrigger;
        _colliderTriggerModelUnloader.OnExitedTrigger += OnExitedTrigger;
    }

    private void OnDisable()
    {
        _colliderTriggerModelLoader.OnEnteredTrigger -= OnEnteredTrigger;
        _colliderTriggerModelUnloader.OnExitedTrigger -= OnExitedTrigger;

        if(hasEntered && !cutsceneUnloaded)
        {
            UnloadCutscene();
            hasEntered = false;
        }
    }

    #endregion

    private void OnEnteredTrigger(Collider other)
    {
        if (!CheckTriggerCondition(other)) return;
        if (hasEntered) return;
        hasEntered = true;
        EnterZone();
    }

    private void OnExitedTrigger(Collider other)
    {
        if (!CheckTriggerCondition(other)) return;
        if(!hasEntered) return;
        hasEntered = false;

        ExitZone();
    }
        
    private bool CheckTriggerCondition(Collider other) => other.gameObject.InsideLayerMask(ActivatorLayer);

    private void EnterZone() => StartCoroutine(LoadCutsceneProcess());
    private void ExitZone() => UnloadCutscene();

    private IEnumerator LoadCutsceneProcess()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(CutsceneKrakenModel.SceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        TestCutsceneManager.OnCutsceneEnded += OnCutsceneEnded;

        cutsceneUnloaded = false;
    }

    private void UnloadCutscene()
    {
        SceneManager.UnloadSceneAsync(CutsceneKrakenModel.SceneName);
        TestCutsceneManager.OnCutsceneEnded -= OnCutsceneEnded;

        cutsceneUnloaded = true;
    }

    private void OnCutsceneEnded()
    {
        TestCutsceneManager.OnCutsceneEnded -= OnCutsceneEnded;
        _colliderTriggerModelLoader.gameObject.SetActive(false);

        CutsceneKrakenModel.SetShown();
        SceneManager.UnloadSceneAsync(CutsceneKrakenModel.SceneName);
    }
}
