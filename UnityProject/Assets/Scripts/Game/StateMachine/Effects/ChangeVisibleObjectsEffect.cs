using Core.StateMachine;
using UnityEngine;

namespace Game.StateMachine.Effects
{
    public class ChangeVisibleObjectsEffect : EffectBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject[] _objects;

#pragma warning restore 0649
        #endregion

        public GameObject[] Objects => _objects;

        public override void Apply()
        {
            foreach (var obj in Objects)
            {
                obj.SetActive(!obj.activeSelf);
            }
        }
    }
}
