using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cinemachine;
using Game.Models;
using Gamekit3D;
using NaughtyAttributes;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PerformanceTester : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PerformanceTestConfig _config;
    [SerializeField] private Slider _slider;

    [Header("Object not to destroy when test is started")]
    [SerializeField] private List<GameObject> dontDestroyOnLoadObjects;

    [Header("Other dependencies")]
    [SerializeField] private FPSCountModel _fpsCountModel;
    [SerializeField] private PointsProvider _pointsProvider;
    [SerializeField] private SendMailModel _sendEmailModel;
    [SerializeField] private QuestsModel _questsModel;

    private int SkipToQuest => _config.SkipToQuest;
    private float MeasureAtSpotTime => _config.MeasureAtSpotTime;
    private float WaitAfterMoveToAnotherSpotTime => _config.WaitAfterMoveToAnotherSpotTime;
    private bool TakeScreenshots => _config.TakeScreenshots;

    private QualityModel QualityModel = null;
    private QuestsModel QuestsModel = null;
    private Transform _playerTransform = null;

    private CameraSettings _cameraSettings;

    private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

    private List<QualityID> _qualities;

    private readonly string k_txtExtension = ".txt";
    private string _date
    {
        get
        {
            string result = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

            result = result.Replace("/", "-");
            result = result.Replace(" ", "_");
            result = result.Replace(":", "-");

            return result;
        }
    }
    private string _reportName => $"PerformanceReport_{SystemInfo.deviceModel}_{_date}";
    private string _directoryPath => Application.persistentDataPath + "/" + _reportName;
    private DirectoryInfo _directoryInfo;
    private string _result;

    public void StartTestPerformance()
    {
        HandleSliderValue();

        SetupDontDestroyOnLoadObjects();

        EditorGameSettings.IsPerformanceTest = true;

        SceneManager.sceneLoaded += HandleOnSceneLoaded;
        SceneManager.LoadSceneAsync("LoadingScene");
    }

    private void HandleSliderValue()
    {
        int value = (int)_slider.value;

        switch (value)
        {
            case 1:
                _qualities = new List<QualityID>()
                {
                    QualityID.Low
                };
                break;

            case 2:
                _qualities = new List<QualityID>()
                {
                    QualityID.Medium,
                };
                break;

            case 3:
                _qualities = new List<QualityID>()
                {
                    QualityID.High
                };
                break;

            case 4:
                _qualities = new List<QualityID>()
                {
                    QualityID.Low, QualityID.Medium, QualityID.High
                };
                break;
        }
    }

    private void HandleOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        PlayerScenesModel playerScenesModel = null;

        if (scene.name == "CoreScene")
        {
            playerScenesModel = FindObjectOfType<PlayerScenesModel>();
            playerScenesModel.OnEnvironmentLoaded += HandleOnEnvironmentLoaded;
        }

        void HandleOnEnvironmentLoaded()
        {
            SceneManager.sceneLoaded -= HandleOnSceneLoaded;
            playerScenesModel.OnEnvironmentLoaded -= HandleOnEnvironmentLoaded;

            StartCoroutine(StartProcess());
        }
    }

    private IEnumerator StartProcess()
    {
        int iterations = 0;
        int fps = 0;

        _cameraSettings = FindObjectOfType<CameraSettings>();//.GetComponentInChildren<CinemachineBrain>();
        _playerTransform = FindObjectOfType<PlayerEventHandler>().transform;
        _cameraSettings.enabled = false;//.lookAt = _playerTransform;
        QualityModel = FindObjectOfType<QualityModel>();

        var quest = _questsModel.GetQuest(SkipToQuest);
        _questsModel.ActivateQuest(quest);

        StringBuilder sb = new StringBuilder();
        _directoryInfo = Directory.CreateDirectory(_directoryPath);

        foreach (var quality in _qualities)
        {
            string text = "Quality: " + quality;
            Debug.Log(text);
            sb.AppendLine(text);
            QualityModel.SetQuality(quality);

            yield return StartCoroutine(MovePlayerToSpotsProcess(quality.ToString()));

            text = string.Empty;
            sb.AppendLine(text);
        }

        string result = sb.ToString();
        _result = result;
        string filePath = _directoryInfo.FullName + "/" + _reportName + k_txtExtension;

        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(result);
        sw.Close();

        // System.Diagnostics.Process.Start(_directoryInfo.FullName);

        _sendEmailModel.SendEmail(_reportName, "PERFORMANCE RESULTS\n", result);

        SceneManager.LoadScene("PreloadScene");
        EditorGameSettings.IsPerformanceTest = false;

        IEnumerator MovePlayerToSpotsProcess(string quality)
        {
            string text = string.Empty;
            int pointIndex = _pointsProvider.Count;
            for (int i = 0; i < pointIndex; i++)
            {
                MovePlayerToNextSpot();
                yield return new WaitForSeconds(WaitAfterMoveToAnotherSpotTime);
                
                _cameraSettings.lookAt=_playerTransform;

                if (TakeScreenshots)
                    ScreenCapture.CaptureScreenshot(_directoryInfo.FullName + $"/Point{i}_{quality}.png");

                _fpsCountModel.OnAvarageFPSChanged += HandleOnAvarageFPSChanged;
                yield return new WaitForSeconds(MeasureAtSpotTime);
                _fpsCountModel.OnAvarageFPSChanged -= HandleOnAvarageFPSChanged;

                int avarageFPS = fps / iterations;
                text = $"\tPoint {i}: FPS = {avarageFPS}, RenderTime = {1000 / avarageFPS}";
                sb.AppendLine(text);

                ResetMeasurements();
            }
        }

        void HandleOnAvarageFPSChanged(int value)
        {
            iterations += 1;
            fps += value;
        }

        void ResetMeasurements()
        {
            iterations = 0;
            fps = 0;
        }
    }

    public void SendDataAgainIfPossible()
    {
        _sendEmailModel.SendEmail(_reportName, "PERFORMANCE RESULTS\n", _result);
    }

    private int iterations;
    private int fps;

    private void MovePlayerToNextSpot()
    {
        var point = _pointsProvider.GetPoint();

        _cameraSettings.transform.rotation = point.rotation;
        _cameraSettings.lookAt=point;

        _playerTransform.position = point.position;
        _playerTransform.rotation = point.rotation;
    }

    private void SetupDontDestroyOnLoadObjects()
    {
        if (!dontDestroyOnLoadObjects.Contains(gameObject))
            dontDestroyOnLoadObjects.Add(gameObject);

        dontDestroyOnLoadObjects.Distinct().ToList().ForEach(x => DontDestroyOnLoad(x));
    }
}
