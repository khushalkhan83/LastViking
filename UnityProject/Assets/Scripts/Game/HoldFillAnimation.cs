using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldFillAnimation : MonoBehaviour
{
    [SerializeField] private Hold hold = default;
    [SerializeField] private Image targetImage = default;

    private void OnEnable() 
    {
        OnProgressChaged(hold.Progress);
        hold.OnProgressChanged += OnProgressChaged; 
    }

    private void OnDisable() 
    {
        hold.OnProgressChanged -= OnProgressChaged; 
    }

    private void OnProgressChaged(float progress)
    {
        targetImage.fillAmount = progress;
    }
}
