using UnityEngine;

namespace Game.Objectives.Actions
{
    public enum TokenTarget
    {
        Tombs,
    }

    /* Make token id(category) 
     Make inheritance ? */
    public class ActivateTokenActionData : ActionBaseData
    {
        [SerializeField] private TokenTarget _tokenTarget;
        [SerializeField] private string _tokenName;
        [SerializeField] private bool _isOn;

        public TokenTarget TokenTarget => _tokenTarget;
        public string TokenName => _tokenName;
        public bool IsOn => _isOn;

        public override ActionID ActionID => ActionID.ActivateToken;
    }
}
