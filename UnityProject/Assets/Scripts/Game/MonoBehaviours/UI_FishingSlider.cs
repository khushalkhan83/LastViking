using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FishingSlider : MonoBehaviour
{
    [SerializeField]
    float minAngle, maxAngle;
    [SerializeField]
    RectTransform indicator;

    public void SetTension(float t)
    {
        indicator.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(minAngle, maxAngle, t));
    }
}
