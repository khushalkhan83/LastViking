using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MarchingBytes;

public class PooledEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem myParticleSystem;
    [SerializeField] float lifeTime = 5f;
    private void OnEnable() 
    {
        myParticleSystem.Clear();
        myParticleSystem.Play();
        StartCoroutine(ReturnToPool());    
    }

    private void OnDisable() 
    {
        StopAllCoroutines();
    }

    private IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(lifeTime);
        EasyObjectPool.instance.ReturnObjectToPool(this.gameObject);
    }
}
