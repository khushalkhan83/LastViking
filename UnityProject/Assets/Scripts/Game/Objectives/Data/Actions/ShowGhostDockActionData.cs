using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Objectives.Actions
{
    public class ShowGhostDockActionData : ActionBaseData
    {
        [SerializeField] private bool _show;

        public bool Show => _show;

        public override ActionID ActionID => ActionID.ShowGhostDock;
    }
}
