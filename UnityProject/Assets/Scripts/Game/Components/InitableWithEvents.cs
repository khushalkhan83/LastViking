using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InitableWithEvents : Initable
{
    [SerializeField] private UnityEvent _preEvents;
    public override void Init(bool setActiveOnInit = true)
    {
        _preEvents?.Invoke();
        base.Init(setActiveOnInit);
    }
}
