using System;
using System.Collections.Generic;
using System.Linq;
using Core.Providers;
using NaughtyAttributes;
using UnityEngine;


// TODO: refactor (create abstract class). For example chain data provider
[CreateAssetMenu(fileName = "SO_Providers_chainData_frameRate_default", menuName = "Providers/ChainData/FrameRate", order = 0)]
public class FrameRateChainDataProvider : ScriptableObject {
    [SerializeField] private List<FrameRateData> data = new List<FrameRateData>();

    public FrameRateData MinFrameRateData => data[0];
    public FrameRateData MaxFrameRateData => data[data.Count - 1];

    public FrameRateData GetData(int vSyncCount)
    {
        var answer = data.Find(x => x.VSyncCount == vSyncCount);
        if(answer == null)
        {
            Debug.LogError("No data in provider");
        }
        return answer;
    }
    
    public FrameRateData GetNextData(int vSyncCount)
    {
        var index = GetIndexByCurrentValue(vSyncCount);
        var lastIndex = data.Count - 1;
        if (lastIndex <= index)
            return data[lastIndex];
        else
        {
            if(data[index].VSyncCount == vSyncCount)
                return data[index + 1];
            else
                return data[index];
        }
    }

    public FrameRateData GetPreviousData(int vSyncCount)
    {
        var index = GetIndexByCurrentValue(vSyncCount);
        if (index <= 0)
            return data[0];
        else
        {
            if(data[index].VSyncCount == vSyncCount)
                return data[index - 1];
            else return data[index];
        }
    }

    private int GetIndexByCurrentValue(int currentValue)
    {
        var nearest = data.OrderBy(x => Math.Abs((int)x.VSyncCount - currentValue)).First();
        var index = data.IndexOf(nearest);
        return index;
    }

    #region Testing
    
    [Space]
    [SerializeField] private int _testValue;

    [SerializeField] private int _testVSyncCount;
    [SerializeField] private string _testText;
    [ShowAssetPreview] [SerializeField] private Sprite _icon;
    [Button] void SetNextValue()
    {
        var answer = GetNextData(_testValue);
        _testValue = answer.VSyncCount;

        _testVSyncCount = answer.VSyncCount;
        _testText = answer.Text;
        _icon = answer.Icon;
    }
    [Button] void SetPreviousValue()
    {
        var answer = GetPreviousData(_testValue);
        _testValue = answer.VSyncCount;

        _testVSyncCount = answer.VSyncCount;
        _testText = answer.Text;
        _icon = answer.Icon;
    }
        
    #endregion
}

[System.Serializable]
public class FrameRateData
{
    public int FrameRate;
    public int VSyncCount;
    public string Text;
    public Sprite Icon;
}
