using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initable : MonoBehaviour, IInitable
{
    public virtual void Init(bool setActiveOnInit = true)
    {
        gameObject.SetActive(setActiveOnInit);
    }
}
