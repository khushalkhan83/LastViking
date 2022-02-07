using UnityEngine;

namespace Game.Views
{
    [CreateAssetMenu(fileName = "SO_System_Views", menuName = "Variables/Systems/Views", order = 15)]
    public class ViewsSystemVariable : ScriptableObject
    {
        public ViewsSystem Value;
    }
}
