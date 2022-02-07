using Game.Models;
using UnityEngine;

namespace UltimateSurvival
{
    public class DestroyableObject : MonoBehaviour
    {
        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());
    }
}
