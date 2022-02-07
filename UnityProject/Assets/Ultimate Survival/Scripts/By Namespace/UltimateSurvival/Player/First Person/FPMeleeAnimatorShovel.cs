using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class FPMeleeAnimatorShovel : FPMeleeAnimator
    {
        protected override void Awake()
        {
            base.Awake();

            if (FPObject is FPToolShovel)
            {
                FPToolShovel toolDig = FPObject as FPToolShovel;

                toolDig.Dig.AddListener(On_Dig);
            }
            else
                Debug.LogError("The animator is of type FPMeleeAnimatorShovel, but no FPToolShovel script found on this game object!", this);
        }

        private void On_Dig()
        {
            Animator.SetTrigger("Dig");
        }
    }
}
