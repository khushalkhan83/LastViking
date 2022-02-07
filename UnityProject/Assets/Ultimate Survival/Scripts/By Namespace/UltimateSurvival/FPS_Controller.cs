using Game.Models;
using UnityEngine;

public class FPS_Controller : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    //[SerializeField] private GameTimeModel _gameTimeModel;
    //[SerializeField] private GameUpdateModel _gameUpdateModel;
    //[SerializeField] private StatisticsModel _statisticsModel;
    //[SerializeField] private float timeDelay = 0.6f;

#pragma warning restore 0649
    #endregion

    //public GameTimeModel GameTimeModel => _gameTimeModel;
    //public StatisticsModel StatisticsModel => _statisticsModel;
    //public GameUpdateModel GameUpdateModel => _gameUpdateModel;

    //string __text;
    //Rect __rect;
    //GUIStyle __style;

    //private float deltaSample;

    void Awake()
    {
        //__style = new GUIStyle();

        //__style.alignment = TextAnchor.LowerLeft;
        //__style.fontSize = Screen.height * 5 / 200;
        //__style.normal.textColor = Color.white;

        //__rect = new Rect(0, 0, Screen.width, Screen.height);
        //__text = string.Empty;
    }

    private void OnEnable()
    {
        //GameUpdateModel.OnUpdate += OnUpdate;
    }

    private void OnDisable()
    {
        //GameUpdateModel.OnUpdate -= OnUpdate;
    }

    private void OnUpdate()
    {
        //if (timeDelay >= 0)
        //{
        //    timeDelay -= Time.unscaledDeltaTime;
        //    return;
        //}
        //else
        //{
        //    deltaSample += (Time.unscaledDeltaTime - deltaSample) * 0.05f;

        //    if (Application.platform == RuntimePlatform.IPhonePlayer)
        //    {
        //        __text = $"iOS EA: v{Application.version} {1 / deltaSample:00}fps TD: {GameTimeModel.Days}; CD: {GameTimeModel.GetDays(GameTimeModel.Ticks - StatisticsModel.StartAliveTimeTicks)};";
        //    }
        //    else if (Application.platform == RuntimePlatform.Android)
        //    {
        //        __text = $"Android EA: v{Application.version} {1 / deltaSample:00}fps TD: {GameTimeModel.Days}; CD: {GameTimeModel.GetDays(GameTimeModel.Ticks - StatisticsModel.StartAliveTimeTicks)};";
        //    }
        //    else
        //    {
        //        __text = $"EA: v{Application.version} {1 / deltaSample:00}fps TD: {GameTimeModel.Days}; CD: {GameTimeModel.GetDays(GameTimeModel.Ticks - StatisticsModel.StartAliveTimeTicks)};";
        //    }
        //}
    }

    //void OnGUI() => GUI.Label(__rect, __text, __style);
}
