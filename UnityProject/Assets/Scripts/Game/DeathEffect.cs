using System.Collections;
using System.Collections.Generic;
using MarchingBytes;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] string effectPoolName = "DeathEffect";
    [SerializeField] Vector3 shiftPosition = Vector3.up;
    private void OnDisable() 
    {
        EasyObjectPool.instance.GetObjectFromPool(effectPoolName, transform.position + shiftPosition, Quaternion.identity, null);
    }
}
