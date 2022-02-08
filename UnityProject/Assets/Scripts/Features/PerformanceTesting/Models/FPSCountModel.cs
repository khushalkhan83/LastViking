using System;
using UnityEngine;

public class FPSCountModel : MonoBehaviour
{
    [SerializeField] private int _frameRange;
    
    private int[] _fpsBuffer;
    private int _fpsBufferIndex;

    public int AvarageFPS { get; private set; }

    public Action<int> OnAvarageFPSChanged;

    private void Update()
    {
        if(_fpsBuffer == null || _fpsBuffer.Length != _frameRange)
        {
            InitBuffer();
        }

        UpdateBuffer();
        CalculateFPS();
    }

    private void InitBuffer()
    {
        if (_frameRange <= 0)
        {
            _frameRange = 1;
        }

        _fpsBuffer = new int[_frameRange];
        _fpsBufferIndex = 0;
    }

    private void UpdateBuffer()
    {
        _fpsBuffer[_fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (_fpsBufferIndex >= _frameRange)
        {
            _fpsBufferIndex = 0;
        }
    }

    private void CalculateFPS()
    {
        int sum = 0;
        for (int i = 0; i < _frameRange; i++)
        {
            sum += _fpsBuffer[i];
        }

        AvarageFPS = sum / _frameRange;
        OnAvarageFPSChanged?.Invoke(AvarageFPS);
    }
}
