using UnityEngine;

namespace Game.Models
{
    [CreateAssetMenu(fileName = "SO_System_Models", menuName = "Variables/Systems/Models", order = 15)]
    public class ModelsSystemVariable : ScriptableObject
    {
        public ModelsSystem Value;
    }
}
