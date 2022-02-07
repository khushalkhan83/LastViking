using CodeStage.AntiCheat.ObscuredTypes;
using Game.Controllers;
using Game.Models;
using System;
using System.Collections;
using UltimateSurvival;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkeletonSpawnManager : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private Transform _container;
    [SerializeField] private EnemiesProvider _enemiesProvider;
    [SerializeField] private ObscuredFloat _nightTimeOffset = 7;
    [SerializeField] private ObscuredFloat _sunsetTime = 22;

    [SerializeField] private SessionSettings[] _sessionSettings;

#pragma warning restore 0649
    #endregion

    [Serializable]
    public class WaveSettings
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private EnemyID _skeletonId;
        [SerializeField] private ObscuredFloat _delayBetweenSpawn;
        [SerializeField] private Transform[] _spawnPoints;
        [Range(0, 24)] [SerializeField] private float _spawnTime = 21;
        [SerializeField] private ObscuredInt _totalCountSpawn;
        [SerializeField] private ObscuredInt _fixedCountSpawn;
        [SerializeField] private ObscuredInt _killCountToSpawn;

        [SerializeField] private bool _isBoss;

#pragma warning restore 0649
        #endregion

        public void OnValidate()
        {
            if (_isBoss)
            {
                _totalCountSpawn = _fixedCountSpawn = _killCountToSpawn = 1;
                _skeletonId = EnemyID.skeleton_warrior;
            }
        }

        public EnemyID SkeletonId => _skeletonId;
        public float DelayBetweenSpawn => _delayBetweenSpawn;
        public float SpawnTime => _spawnTime;
        public int TotalCountSpawn => _totalCountSpawn;
        public int FixedCountSpawn => _fixedCountSpawn;
        public int KillCountToSpawn => _killCountToSpawn;
        public Transform[] SpawnPoints => _spawnPoints;
        public bool IsBoss => _isBoss;

    }

    [Serializable]
    public class SessionSettings
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private WaveSettings[] _wavesSettings;
        [SerializeField] private float _delayBeforeWave;

#pragma warning restore 0649
        #endregion

        public WaveSettings[] WavesSettings => _wavesSettings;
        public float DelayBeforeWave => _delayBeforeWave;

        public bool IsEmpty => WavesSettings.Length == 0;

        public bool TryGetWave(int id, out WaveSettings wave)
        {
            if (id >= 0 && id < WavesSettings.Length)
            {
                wave = WavesSettings[id];
                return true;
            }

            wave = default;
            return false;
        }

        public int GetTotalEnemies()
        {
            int count = 0;
            foreach (var w in WavesSettings) count += w.TotalCountSpawn;
            return count;
        }
    }

    #region DataProperties

    public Transform Container => _container;
    public SessionSettings[] Sessions => _sessionSettings;
    public SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;
    public GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
    public EnemiesProvider EnemiesProvider => _enemiesProvider;
    public float NightTimeOffset => _nightTimeOffset;
    public float SunsetTime => _sunsetTime;

    #endregion

    private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;

    public SessionSettings CurrentSession => Sessions[SessionId];
    public WaveSettings CurrentWave => CurrentSession.WavesSettings[WaveId];

    private bool IsLastWave => WaveId >= CurrentSession.WavesSettings.Length - 1;
    private bool IsHasShelter => SheltersModel.ShelterActive != ShelterModelID.None;

    public bool IsLastSession => SessionId >= Sessions.Length - 1;
    public bool IsEnableSpawn => IsHasShelter;

    private bool SetIsTimeToSpawn(float time) => GetLinearTime(GameTimeModel.EnviroTimeOfDay, NightTimeOffset) > GetLinearTime(time, NightTimeOffset);

    public bool IsSessionStarted { get; private set; }
    public int Nighid { private set; get; } // convert to session id ?
    public int SessionId { private set; get; } = -1;
    public int WaveId { private set; get; }
    public float SessionStart { private set; get; }

    public int KilledEnemiesPerSession { set; get; }
    public int KilledEnemies { set; get; }
    public int SpawnedWaveEnemies { set; get; }
    public int SpawnedSessionEnemies { set; get; }
    public int TotalSessionEnemies { get; private set; }

    public event Action<float> OnSpawnWaitStarted;
    public event Action OnSpawnWaitEnded;
    public event Action OnSessionStarted;
    public event Action OnWaveCleared;
    public event Action OnSessionCleared;
    public event Action OnEnableSpawn;
    public event Action OnDisableSpawn;


    private void OnValidate()
    {
        foreach (var s in Sessions) foreach (var w in s.WavesSettings) w.OnValidate();
    }

    private void OnDisable()
    {
        SheltersModel.OnBuy -= OnBuyShelter;
        GameUpdateModel.OnUpdate -= OnUpdateStartSpawn;
        GameUpdateModel.OnUpdate -= OnUpdateEndSpawn;
        SheltersModel.OnDeath -= OnDeathShelter;
    }

    private void Start()
    {
        GameUpdateModel.OnUpdate += OnUpdateStartSpawn;
        GameUpdateModel.OnUpdate += OnUpdateEndSpawn;

        if (IsHasShelter)
        {
            SheltersModel.OnDeath += OnDeathShelter;
        }
        else
        {
            SheltersModel.OnBuy += OnBuyShelter;
        }
    }

    private void OnBuyShelter(ShelterModel shelter)
    {
        SheltersModel.OnBuy -= OnBuyShelter;
        SheltersModel.OnDeath += OnDeathShelter;
        OnEnableSpawn?.Invoke();
    }

    private void OnDeathShelter(ShelterModel shelter)
    {
        SheltersModel.OnDeath -= OnDeathShelter;
        SheltersModel.OnBuy += OnBuyShelter;
        OnDisableSpawn?.Invoke();
    }

    public bool TryGetSession(out SessionSettings session) => TryGetSession(-1, out session);

    public bool TryGetSession(int offset, out SessionSettings session)
    {
        if (TryGetSesionId(offset, out var sessionId))
        {
            session = Sessions[sessionId];
            return true;
        }

        session = default;
        return false;
    }

    public bool TryGetNowSesionId(out int id) => TryGetSesionId(-1, out id);

    public bool TryGetSesionId(int offset, out int id)
    {
        if (!IsHasShelter)
        {
            id = default;
            return false;
        }

        id = GetNowSessionId() + offset;
        return ValidateSessionId(id);
    }

    private bool ValidateSessionId(int id) => id >= 0 && id < Sessions.Length;

    private int GetNowSessionId() => (int)(GameTimeModel.GetDays(GameTimeModel.EnviroTicks - GameTimeModel.GetHourTicks(NightTimeOffset)) - GameTimeModel.GetDays(SheltersModel.ShelterModel.StartAliveTimeTicks - GameTimeModel.GetHourTicks(NightTimeOffset)));

    int __sessionId;
    private float __timeLinear;
    private float __timeSpawnLinear;
    private SessionSettings __session;
    private WaveSettings __firstWave;

    private void OnUpdateStartSpawn()
    {
        if (!IsSessionStarted && TryGetNowSesionId(out __sessionId) && __sessionId > SessionId && SetIsTimeToSpawn(21f))
        {
            __session = Sessions[__sessionId];
            if (!__session.IsEmpty)
            {
                __timeLinear = GetLinearTime(GameTimeModel.EnviroTimeOfDay, NightTimeOffset);
                __firstWave = __session.WavesSettings[0];
                __timeSpawnLinear = GetLinearTime(__firstWave.SpawnTime, NightTimeOffset);
                if (__timeSpawnLinear <= __timeLinear)
                {
                    SessionId = __sessionId;
                    StartSpawnSession(__session);
                }
            }
        }
    }

    private void OnUpdateEndSpawn()
    {
        if (IsSessionStarted && !SetIsTimeToSpawn(SessionStart))
        {
            EndSpawnSession();
        }
    }

    private void StartSpawnSession(SessionSettings session)
    {
        OnSessionStarted?.Invoke();

        IsSessionStarted = true;
        WaveId = 0;
        SessionStart = session.WavesSettings[WaveId].SpawnTime;
        TotalSessionEnemies = session.GetTotalEnemies();
        StartSpawn(session.DelayBeforeWave, session.WavesSettings[WaveId].FixedCountSpawn);
    }

    private void EndSpawnSession()
    {
        OnSessionCleared?.Invoke();
        IsSessionStarted = false;

        StopAllCoroutines();
        KillAllSkeletons(false);

        KilledEnemies = 0;
        KilledEnemiesPerSession = 0;
        SpawnedWaveEnemies = 0;
        SpawnedSessionEnemies = 0;

        if (IsLastSession)
        {
            GameUpdateModel.OnUpdate -= OnUpdateStartSpawn;
        }
    }

    public float GetLinearTime(float t, float offset)
    {
        if (offset == 0)
        {
            return t;
        }

        if (offset > 0)
        {
            if (t < offset)
            {
                t += 24;
            }
        }
        else if (offset < 0)
        {
            if (t > 24 + offset)
            {
                t -= 24 + offset;
            }
        }

        return t - offset;
    }

    private void StartSpawn(float waitTime, int spawnCount) => StartCoroutine(DoSpawn(waitTime, spawnCount));

    private IEnumerator DoSpawn(float waitTime, int spawnCount)
    {
        OnSpawnWaitStarted?.Invoke(waitTime);
        yield return new WaitForSeconds(waitTime);
        OnSpawnWaitEnded?.Invoke();

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnObject(CurrentWave);
            SpawnedWaveEnemies++;
            SpawnedSessionEnemies++;

            yield return new WaitForSeconds(1.5f);
        }
    }

    private void SpawnObject(WaveSettings wave)
    {
        Initable instance = GetInitable(wave.SkeletonId);
        var point = wave.SpawnPoints[Random.Range(0, wave.SpawnPoints.Length)];

        instance.transform.SetParent(Container, false);
        instance.transform.position = point.position;
        instance.transform.rotation = point.rotation;
        instance.Init();

        if (wave.IsBoss)
        {
            OnBossSpawned();
        }

        DestroyController destroyController = instance.gameObject.AddComponent<DestroyController>();
        destroyController.OnDestroyAction += OnDestroyEnemyHandler;
    }

    private void OnBossSpawned() { }// => GameTimeModel.PauseTime();
    private void OnBossKilled() { }// => GameTimeModel.UnpauseTime();

    private bool HasEnemyDead(DestroyController destroyController) => destroyController.GetComponentInChildren<EnemyHealthModel>().IsDead;

    private void OnDestroyEnemyHandler(DestroyController destroyController)
    {
        destroyController.OnDestroyAction -= OnDestroyEnemyHandler;

        if (!HasEnemyDead(destroyController))
            return;

        KilledEnemies++;
        KilledEnemiesPerSession++;
        if (KilledEnemies >= CurrentWave.TotalCountSpawn)
        {
            WaveCleared();
        }
        else if (SpawnedWaveEnemies < CurrentWave.TotalCountSpawn)
        {
            if (KilledEnemies % CurrentWave.KillCountToSpawn == 0)
            {
                var count = Mathf.Min(CurrentWave.KillCountToSpawn, CurrentWave.TotalCountSpawn - SpawnedWaveEnemies);
                StartSpawn(0, count);
            }
        }
    }

    private void WaveCleared()
    {
        if (!IsLastWave)
        {
            OnWaveCleared?.Invoke();
        }
        else
        {
            IsSessionStarted = false;
            SpawnedSessionEnemies = 0;
            OnSessionCleared?.Invoke();
        }

        SpawnedWaveEnemies = 0;
        KilledEnemies = 0;

        if (CurrentWave.IsBoss)
        {
            OnBossKilled();
        }

        if (!IsLastWave)
        {
            NextWave();
        }
        else
        {
            SheltersModel.ProtectedFromSkeletons();
            KilledEnemiesPerSession = 0;
        }
    }

    private void NextWave()
    {
        ++WaveId;
        StartSpawn(CurrentWave.DelayBetweenSpawn, CurrentWave.FixedCountSpawn);
    }

    private Initable GetInitable(EnemyID enemyId) => Instantiate(EnemiesProvider[enemyId]);

    private void KillAllSkeletons(bool trackDeath) => StartCoroutine(DoKillAllSkeletons(trackDeath));

    private IEnumerator DoKillAllSkeletons(bool trackDeath)
    {
        Transform child;
        for (int i = 0; i < Container.childCount; i++)
        {
            child = Container.GetChild(i);
            if (!trackDeath)
            {
                child.GetComponent<DestroyController>().OnDestroyAction -= OnDestroyEnemyHandler;
            }
            Destroy(child.gameObject);
            i--;
            yield return null;
        }
    }
}
