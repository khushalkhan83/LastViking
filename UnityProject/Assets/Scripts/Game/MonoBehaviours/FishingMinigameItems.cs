using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Models;

public class FishingMinigameItems : MonoBehaviour
{
    [SerializeField] FishingFloat fishingFloat;
    [SerializeField] FishingLine fishingLine;
    [SerializeField] FishingRodAnimations rodAnimator;

    public void SetFishingModel(FishingModel model)
    {
        fishingFloat.SetFishingModel(model);
        fishingLine.SetFishingModel(model);
        rodAnimator.SetFishingModel(model);
    }

    private void OnDestroy()
    {
        fishingFloat.UnSetFishingModel();
        fishingLine.UnSetFishingModel();
        rodAnimator.UnSetFishingModel();
    }
}
