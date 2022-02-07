using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAtStart : MonoBehaviour
{

    #region Data
#pragma warning disable 0649

    [SerializeField] private GameObject[] m_Objects;

#pragma warning restore 0649
    #endregion


    private void Start()
    {
        foreach (var obj in m_Objects)
            obj.SetActive(false);
    }
}
