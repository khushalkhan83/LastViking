using System;
using UnityEngine;
using UnityEngine.UI;

public class PreloadSceneView : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _startPerformanceTestButton;

    public event Action OnPlayButtonClicked;
    public void PlayButtonClick() => OnPlayButtonClicked?.Invoke();

    public event Action OnStartPerformanceTestButtonClicked;
    public void StartPerformanceTestButtonClick() => OnStartPerformanceTestButtonClicked?.Invoke();
}
