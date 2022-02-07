using Core.Storage;
using Game.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishingZone : MonoBehaviour
{
    [System.Serializable]
    public class Data: DataBase
    {
        public int _fishCount = 1;
        public long _timeForNextFish = 0;

        public void SetFishCount(int newVal)
        {
            _fishCount = newVal;
            ChangeData();
        }

        public void SetTimeForNextFish(long ticks)
        {
            _timeForNextFish = ticks;
            ChangeData();
        }
    }

    [SerializeField] Data _data;

    [SerializeField] int maxFishCount = 3;
    [SerializeField] float fishRespawnTime = 20f;

    private GameTimeModel timeModel => ModelsSystem.Instance._gameTimeModel;
    private StorageModel storageModel => ModelsSystem.Instance._storageModel;

    private void OnEnable()
    {
        storageModel.TryProcessing(_data);
        CheckFishRespawn();
        SetFishVisible(fishCount > 0);
    }

    void CheckFishRespawn()
    {   
        if (isOnSpawn)
            SaveSpawnTime();
        if (fishCount < maxFishCount)
            StartSpawn();
    }

    public int fishCount { get
        {
            return _data._fishCount;
        }
        set
        {
            _data.SetFishCount(value);
            SaveSpawnTime();
            CheckFishRespawn();
            SetFishVisible(fishCount > 0);
        }
    }

    public bool isFishHere => fishCount > 0;
    bool isOnSpawn => onSpawn != null;

    void SaveSpawnTime()
    {
        if (fishCount >= maxFishCount)
        {
            timeForNextSpawn = fishRespawnTime;
        }
        else
        {
            if (isOnSpawn)
            {
                // сохранить оставшееся
                timeForNextSpawn = fishRespawnTime - spawnTime;
            }
            else
            {
                timeForNextSpawn = fishRespawnTime;
            }
        }
    }

    float timeForNextSpawn
    {
        get
        {
            float retVal = timeModel.GetSecondsTotal(_data._timeForNextFish - timeModel.RealTimeNowTick);
            if (retVal < 0f)
                retVal = 0f;
            return retVal;
        }
        set
        {
            _data.SetTimeForNextFish(timeModel.RealTimeNowTick + timeModel.GetTicks(value));
        }
    }

    Coroutine onSpawn = null;
    float spawnTime = 0;
    void StartSpawn()
    {
        float T = timeForNextSpawn;

        if (onSpawn != null)
            StopCoroutine(onSpawn);

        onSpawn = StartCoroutine(FishSpawnRoutine(T));
    }

    float spawnAt = 0;
    IEnumerator FishSpawnRoutine(float T)
    {
        spawnAt = T;
        spawnTime = 0;
        while (spawnTime < T)
        {
            spawnTime += Time.deltaTime;
            yield return null;
        }
        spawnTime = 0;
        onSpawn = null;             
        fishCount++;
    }

    public event System.Action<bool> onSetFishVisible;

    //[SerializeField] GameObject fishIndicator;
    [SerializeField] Transform fishIndicator;
    List<ParticleSystem> fishParticles;
    void SetFishVisible(bool isVisible)
    {

        if (fishParticles == null)
        {
            fishParticles = new List<ParticleSystem>();
            fishParticles.AddRange( fishIndicator.GetComponentsInChildren<ParticleSystem>());
        }
        onSetFishVisible?.Invoke(isVisible);

        foreach (ParticleSystem ps in fishParticles)
        {
            var emission = ps.emission;
            emission.enabled = isVisible;
        }
    }
}
