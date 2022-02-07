using Game.Models;
using UnityEngine;

namespace Game.Spawners.AutoRespawn
{
    public abstract class WorldObjectCreatorBase: MonoBehaviour
    {
        public abstract bool TryCreateInstance(Vector3 position, Quaternion rotation, Vector3 localScale, DataProcessing dataProcessing, Transform transform, out WorldObjectModel model);
    }
}