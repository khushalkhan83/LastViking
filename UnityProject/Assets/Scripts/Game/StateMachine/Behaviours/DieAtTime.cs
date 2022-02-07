using Core.StateMachine;
using Core.States.Parametrs;
using Game.Models;
using UnityEngine;

public class DieAtTime : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] StateProcessor _stateProcessor;
    [SerializeField] State _deathState;
    [SerializeField] EffectBase _deathEffect;
    [SerializeField] PositionParametr _hitPoint;
    [SerializeField] PositionParametr _hitDirection;
    [SerializeField] FloatParametr _hitForce;
    [SerializeField] float _hitForceValue;

    #endregion

    public StateProcessor StateProcessor => _stateProcessor;
    public State DeathState => _deathState;
    public EffectBase DeathEffect => _deathEffect;
    public PositionParametr HitDirection => _hitDirection;
    public FloatParametr HitForce => _hitForce;
    public float HitForceValue => _hitForceValue;

    private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;

    // [REPLACE]
    private SkeletonSpawnManager SkeletonSpawnManager => FindObjectOfType<SkeletonSpawnManager>();

    private StartNightInfoModel StartNightInfoModel => ModelsSystem.Instance._startNightInfoModel;
    private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

    public float DeathLowerTime { get; private set; }
    public float DeathUpperTime { get; private set; }

    private void OnEnable()
    {
        GameUpdateModel.OnUpdate += OnUpdate;
    }

    private void OnDisable()
    {
        GameUpdateModel.OnUpdate -= OnUpdate;
    }

    private void Start()
    {
        DeathUpperTime = SkeletonSpawnManager.Sessions[0].WavesSettings[0].SpawnTime;
        DeathLowerTime = StartNightInfoModel.TimeShowEnd;
    }

    private void OnUpdate()
    {
        if (GameTimeModel.Hours < DeathUpperTime && (GameTimeModel.Hours + GameTimeModel.Minutes / 60f) > DeathLowerTime)
        {
            if (StateProcessor.State != DeathState)
            {
                StateProcessor.TransitionTo(DeathState);
                DeathEffect.Apply();
                HitDirection.transform.position = Vector3.down; //? check this
                HitForce.SetValue(HitForceValue);
            }
        }
    }
}
