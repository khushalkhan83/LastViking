using CodeStage.AntiCheat.ObscuredTypes;
using Game.Audio;
using Game.Models;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(FracturedStructureBehaviour))]
public class MinebleFractureObject : MineableObject, IOutlineTarget
{
    #region Data
#pragma warning disable 0649

    [Tooltip("Time it takes for object to re-spawn. In seconds.")]
    [SerializeField] private ObscuredFloat _durationRespawn;

    [Tooltip("Scale resources reward on final collection hit. This parameter doe's not affect total ResourceValue. This is used to create accent on last hit.")]
    [SerializeField] private ObscuredFloat _completeGatherBonus;

    [Tooltip("Controls spawn of fracture pieces with dependent on current resource status. 1 Will update fracture model to closely represent current resource value. 0 Will result of no fracture spawning until the last resource is collected."
             + "Currently this is used to adjust fracture spawning to create approximately half of the fractures during collection, and another half at the moment when resource is depleted.")]
    [SerializeField] private ObscuredFloat _fractureRate;
    [SerializeField] private AudioID _audioID;
    [SerializeField] private Collider mWallCollider;
    [SerializeField] private WorldObjectID _spawnObjectID;

    List<Renderer> _renderers;
    [SerializeField] Vector3 _targetingShift;
    [SerializeField] private FracturedStructureBehaviour _fracturedStructure;
    [SerializeField] private long _timeSpawnTicks = 0;
    [Tooltip("Initialize by itsef")]
    [SerializeField] private bool _autoInit = true;
    [SerializeField] private UnityEvent onResourceValueDecreased;
    [SerializeField] private UnityEvent onResourceDepleted;

#pragma warning restore 0649
    #endregion

    public float DurationRespawn => _durationRespawn;
    public float CompleteGatherBonus => _completeGatherBonus;
    public float FractureRate => _fractureRate;
    public WorldObjectID SpawnObjectID => _spawnObjectID;
    public AudioID AudioID => _audioID;
    public long TimeSpawnTicks => _timeSpawnTicks;
    public bool AutoInit => _autoInit;

    public float mResourceValue;
    private Collider mCollider;
    private Coroutine spawnProcessCoroutine;

    public event Action<RaycastHit, Ray> Hit;
    public event Action<RaycastHit, Ray> Depleted;

    private AudioSystem AudioSystem => AudioSystem.Instance;
    private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
    private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;

    public float InitialResourceValue => Amount;

    public float ResourceValue
    {
        get
        {
            return mResourceValue;
        }
        set
        {
            if (value < mResourceValue)
            {
                StartRespawn();
                onResourceValueDecreased?.Invoke();
            }

            mResourceValue = value;

            // Enable collider only if object have resources available.
            mWallCollider.enabled = value > 0;
            mCollider.enabled = value > 0;
        }
    }

    public override void OnToolHit(Ray cameraRay, RaycastHit hitInfo, ExtractionSetting[] toolSettings)
    {
        var effectiveTool = toolSettings.FirstOrDefault(tool => tool.ToolID == RequiredToolPurpose);

        if (effectiveTool == null)
        {
            return;
        }

        var extractionModifier = ResourcesExtractionModel.GetModifier(effectiveTool.ToolID);
        var extractionRate = Mathf.Clamp(effectiveTool.ExtractionRate * extractionModifier, 0, ResourceValue);

        int oldResourceValue = Mathf.CeilToInt(ResourceValue);
        ResourceValue -= extractionRate;
        int newResourceValue = Mathf.CeilToInt(ResourceValue);

        int addResourcesCount = oldResourceValue - newResourceValue;
        
        if(addResourcesCount <= 0)
            return;

        bool notLastExtraction = ResourceValue > CompleteGatherBonus && ResourceValue > 0;

        if (notLastExtraction)
        {
            // Give player resources according to tool's extractionRate.
            DropLoot(addResourcesCount);
            Hit?.Invoke(hitInfo, cameraRay);
        }
        else
        {
            // Give player all remaining resources.
            addResourcesCount += Mathf.CeilToInt(ResourceValue);
            DropLoot(addResourcesCount, true);
            ResourceValue = 0;
            Depleted?.Invoke(hitInfo, cameraRay);
            onResourceDepleted?.Invoke();
        }

        UpdateFracture(cameraRay);
    }

    #region MonoBehaviour
    private void OnEnable()
    {
        if(AutoInit)
        {
            Init(InitialResourceValue, 0);
        }
       
    }

    public void Init(float resourceValue, long timeSpawnTicks)
    {
        if (_fracturedStructure == null)
            _fracturedStructure = gameObject.GetComponent<FracturedStructureBehaviour>();

        if (mCollider == null)
            mCollider = gameObject.GetComponent<Collider>();

        mResourceValue = resourceValue;
        _timeSpawnTicks = timeSpawnTicks;
        mWallCollider.enabled = mResourceValue > 0;
        mCollider.enabled = mResourceValue > 0;

        if(GameTimeModel.RealTimeNowTick >= TimeSpawnTicks)
        {
            OnRespawn();
        }
        else
        {
            UpdateFracture(new Ray(Vector3.zero, Vector3.forward));
            _fracturedStructure.UpdateDisaplayModeAtStart();
            SpawnProcess();
        }
    }

    private void OnDisable()
    {
        if(spawnProcessCoroutine != null)
        {
            StopCoroutine(spawnProcessCoroutine);
            spawnProcessCoroutine = null;
        }
    }

    #endregion

    private void OnRespawn()
    {
        ResourceValue = InitialResourceValue;
        _fracturedStructure.Reset();
    }

    /// <summary>
    /// Respawn starts as soon as player collects ANY resource value.
    /// </summary>
    private void StartRespawn()
    {
        _timeSpawnTicks = GameTimeModel.TicksRealNow + GameTimeModel.GetTicks(DurationRespawn);
        SpawnProcess();
    }

    private void UpdateFracture(Ray ray)
    {
        var normalizedResources = ResourceValue / (float)InitialResourceValue;

        if (normalizedResources <= 0)
        {
            // Resource is depleted, fracture all remaining pieces.
            _fracturedStructure.FractureAll(ray);

            AudioSystem.PlayOnce(AudioID);

            if (m_DestroyedObject)
            {
                Instantiate(m_DestroyedObject, transform.position + transform.TransformVector(OffsetObjectDestroyed), Quaternion.identity);
            }

            if (SpawnObjectID != WorldObjectID.None)
            {
                WorldObjectCreator.Create(SpawnObjectID, transform.position + new Vector3(-2, 0, -2) + transform.TransformVector(OffsetObjectDestroyed), Quaternion.identity);
            }

            return;
        }

        // Using mFractureRate treat fracture count as if we have fewer pieces.
        // Brake only those peaces while collecting the resource.
        var piecesToBreak = (int)(_fracturedStructure.totalPieces * FractureRate);
        var currentBroken = piecesToBreak - _fracturedStructure.totalBrokenPieces;
        var unbrokenPieces = piecesToBreak * normalizedResources;

        if (unbrokenPieces < currentBroken)
        {
            _fracturedStructure.FracturePiece(ray);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.TransformPoint(_targetingShift);
    }

    public List<Renderer> GetRenderers()
    {
        if (_fracturedStructure == null)
        {
            _fracturedStructure = gameObject.GetComponent<FracturedStructureBehaviour>();
        }
        
        if (_renderers == null)
            _fracturedStructure.GetRenderersList(SetNewRenderersList);

        return _renderers;
    }

    public event Action<IOutlineTarget> OnUpdateRendererList;

    void SetNewRenderersList(List<Renderer> newList)
    {
        _renderers = newList;
        OnUpdateRendererList?.Invoke(this);
    }

    public bool IsUseWeaponRange()
    {
        return true;
    }

    public int GetColor()
    {
        return 1;
    }

    private void SpawnProcess()
    {
        if(spawnProcessCoroutine != null)
        {
            StopCoroutine(spawnProcessCoroutine);
            spawnProcessCoroutine = null;
        }
        spawnProcessCoroutine = StartCoroutine(Spawn(GameTimeModel.GetSecondsTotal(_timeSpawnTicks - GameTimeModel.RealTimeNowTick)));
    }

    private IEnumerator Spawn(float remainingSeconds)
    {
        yield return new WaitForSecondsRealtime(remainingSeconds);
        OnRespawn();
    }
}
