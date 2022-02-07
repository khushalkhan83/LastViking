using System;
using System.Collections;
using System.Collections.Generic;
using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class EnvironmentLoadingView : ViewBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private Image circle;
        private float step = 0.1f;


#pragma warning restore 0649
        #endregion

        private bool filled => circle.fillAmount >= 1;
        private bool empty => circle.fillAmount == 0;
        public void Update()
        {
            // controlled by Animator 
            
            // if(filled)
            //     circle.fillClockwise = false;
            
            // if(empty)
            //     circle.fillClockwise = true;
                

            // if(circle.fillClockwise)
            //     Increase();
            // else
            //     Decrease();

        }

        private void Increase() => circle.fillAmount += step;

        private void Decrease() => circle.fillAmount -= step;
    }
}
