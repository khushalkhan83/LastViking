using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Hold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public UnityEvent OnHold;
    public Action<float> OnProgressChanged;

    [SerializeField] float _holdDelay = 0.4f;

    private float _holdTime = 0;
    private bool _isHolding = false;
    private int _pointerId = -100;
    private float _progress = 0;
    
    public float Progress
    {
        get{return _progress;}
        set{
            if(_progress != value)
            {
                _progress = value;
                if(_progress > 1f) _progress = 1f;
                if(_progress < 0) _progress = 0;
                OnProgressChanged?.Invoke(_progress);
            }
        }
    }
    
    private void OnEnable() {
        Progress = 0;
    }

    private void Update() {
        if (_isHolding) 
        {
            Progress = _holdTime / _holdDelay;
            if (_holdTime > _holdDelay)
            {
                OnHold?.Invoke();
                _holdTime = 0;
                _isHolding = false;
                _pointerId = -100;
                Progress = 0;
            }
            else
            {
                _holdTime += Time.deltaTime;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pointerId = eventData.pointerId;
        _holdTime = 0;
        _isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_pointerId == eventData.pointerId)
        {
            _isHolding = false;
            Progress = 0;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_pointerId == eventData.pointerId)
        {
            _isHolding = false;
            Progress = 0;
        }
    }

    private void OnApplicationPause(bool pause) {
        if (pause) {
            _isHolding = false;
            Progress = 0;
        }
    }

    private void OnApplicationFocus(bool focus) {
        if (!focus) {
            _isHolding = false;
            Progress = 0;
        }
    }
}
