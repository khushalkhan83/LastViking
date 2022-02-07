using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class TokenTargetDisabler : MonoBehaviour
    {
        [SerializeField] private TokenTarget tokenTarget;

        public TokenTarget TokenTarget => tokenTarget;

        public void DisableTokenTarget()
        {
            if(tokenTarget.enabled)
                tokenTarget.enabled = false;
        }
        public void EnableTokenTarget()
        {
            if(!tokenTarget.enabled)
                tokenTarget.enabled = true;
        }
    }
}
