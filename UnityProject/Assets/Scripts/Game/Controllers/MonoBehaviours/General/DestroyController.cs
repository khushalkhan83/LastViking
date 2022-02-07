using System;
using MarchingBytes;
using UnityEngine;

namespace Game.Controllers
{
    public class DestroyController : MonoBehaviour
    {
        public event Action<DestroyController> OnDestroyAction;

        public EnemyID EnemyID {get; private set;}
        public void SetEnemyID(EnemyID enemyID) => EnemyID = enemyID;

        private void OnDisable()
        {
            OnDestroyAction?.Invoke(this);
        }
    }
}
