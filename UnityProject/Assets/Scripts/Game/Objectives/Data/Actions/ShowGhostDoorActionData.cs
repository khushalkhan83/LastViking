using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Objectives.Actions
{
    [CreateAssetMenu(fileName = "ShowGhostDoorActionData", menuName = "ScriptableObjects/ShowGhostDoorActionData", order = 1)]
    public class ShowGhostDoorActionData : ActionBaseData
    {
        [SerializeField] private bool _show;

        public bool Show => _show;

        public override ActionID ActionID => ActionID.ShowGhostDoor;
    }
}
