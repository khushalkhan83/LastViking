using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreloadSceneViewController : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private PreloadSceneView _view;
    
    [Header("Other dependencies")]
    [SerializeField] private PerformanceTester _performanceTester;
    [SerializeField] private GameObject _buttonsCanvas;

    private const string k_loadingSceneName = "LoadingScene";

    private void OnEnable()
    {
        _view.OnPlayButtonClicked += HandleOnPlayButtonClicked;
        _view.OnStartPerformanceTestButtonClicked += HandleOnStartPerformanceTestButtonClicked;
    }

    private void OnDisable()
    {
        _view.OnPlayButtonClicked += HandleOnPlayButtonClicked;
        _view.OnStartPerformanceTestButtonClicked += HandleOnStartPerformanceTestButtonClicked;
    }
    
    private void HandleOnPlayButtonClicked()
    {
        SceneManager.LoadScene(k_loadingSceneName);
    }

    private void HandleOnStartPerformanceTestButtonClicked()
    {
        _buttonsCanvas.SetActive(false);
        _performanceTester.StartTestPerformance();
    }
}
