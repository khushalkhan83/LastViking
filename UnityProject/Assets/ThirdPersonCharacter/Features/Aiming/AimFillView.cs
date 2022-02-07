using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ThirdPerson.RangedCombat.Misc
{
    public class AimFillView : MonoBehaviour
    {
        [SerializeField] Image fillImage = default;
        [SerializeField] Color fillingColor = Color.yellow;
        [SerializeField] Color filledColor = Color.green;

        public void SetFill(float fill)
        {
            fillImage.fillAmount = fill;
            fillImage.color = fill >= 1f ? filledColor : fillingColor;
        }
        
    }
}
