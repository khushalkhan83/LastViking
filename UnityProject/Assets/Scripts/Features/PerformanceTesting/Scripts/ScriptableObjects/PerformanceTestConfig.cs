using UnityEngine;

[CreateAssetMenu(fileName = "SO_PerformanceConfig", menuName = "Performance/Config", order = 0)]
public class PerformanceTestConfig : ScriptableObject
{
    [SerializeField] private int _skipToQuest;
    [SerializeField] private float _measureAtSpotTime;
    [SerializeField] private float _waitAfterMoveToAnotherSpotTime;
    [SerializeField] private bool _takeScreenshots;

    public int SkipToQuest => _skipToQuest;
    public float MeasureAtSpotTime => _measureAtSpotTime;
    public float WaitAfterMoveToAnotherSpotTime => _waitAfterMoveToAnotherSpotTime;
    public bool TakeScreenshots => _takeScreenshots;
}
