using Game.Controllers;
using Game.Models;
using UnityEngine;

public class IntroActivationController : ActivationObjectsController
{
    [SerializeField] private QualityModel qualityModel;
    protected override void ChangeActivationObjects(QualityConfig qualityConfig)
    {

    }

    protected override void InitActivationObjects(QualityConfig qualityConfig)
    {
        qualityModel.SetQuality(qualityConfig.QuaityID);
    }

    private void OnEnable()
    {
        qualityModel.OnChangeQuality += OnChangeQuality;
    }

    private void Start()
    {
        InitActivationObjects(qualityModel.QualityConfig);
    }

    private void OnDisable()
    {
        qualityModel.OnChangeQuality -= OnChangeQuality;
    }

    private void OnChangeQuality()
    {
        ChangeActivationObjects(qualityModel.QualityConfig);
    }
}
