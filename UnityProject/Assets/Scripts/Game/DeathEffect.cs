using Game.Models;
using MarchingBytes;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] string effectPoolName = "DeathEffect";
    [SerializeField] Vector3 shiftPosition = Vector3.up;
    [SerializeField] private EnemyHealthModel EnemyHealthModel;

    private void OnEnable()
    {
        EnemyHealthModel.OnDeath +=GetDeathEffect;
    }

    private void OnDisable()
    {
        //EasyObjectPool.instance.GetObjectFromPool(effectPoolName, transform.position + shiftPosition, Quaternion.identity, null);
    }

    void GetDeathEffect()
    {
        EasyObjectPool.instance.GetObjectFromPool(effectPoolName, transform.position + shiftPosition, Quaternion.identity, null);
    }
}
