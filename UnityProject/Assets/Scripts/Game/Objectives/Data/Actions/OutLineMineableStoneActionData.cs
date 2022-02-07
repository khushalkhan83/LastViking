using UnityEngine;

namespace Game.Objectives.Actions
{
    public enum OutLineTarget
    {
        None = 0,
        StoneMineables = 1,
        Coconuts = 2,
        BlueBarrels = 3,
        BagPickups = 4,
        Banana = 5,
    }

    public class OutLineMineableStoneActionData : ActionBaseData
    {
        [SerializeField] private OutLineTarget _outLineTarget;
        [SerializeField] private bool _isSelection;

        public OutLineTarget OutLineTarget => _outLineTarget;
        public bool IsSelection => _isSelection;

        public override ActionID ActionID => ActionID.OutLineMineableStone;
    }
}
