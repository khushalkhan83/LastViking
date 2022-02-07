using Core.Controllers;
using Game.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanguageDropdownController : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public Color DisableColor;
    public Color ActiveColor;

    public void OnPointerExit(PointerEventData eventData)
    {
        var toggle = GetComponent<Toggle>();
        ColorBlock cb = toggle.colors;
        if (toggle.isOn)
        {
                
        }
        else
        {
            cb.normalColor = DisableColor;
            cb.highlightedColor = DisableColor;
            cb.pressedColor = DisableColor;
            cb.disabledColor = DisableColor;
        }
            
        toggle.colors = cb;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var toggle = GetComponent<Toggle>();
        ColorBlock cb = toggle.colors;
        if (toggle.isOn)
        {

        }
        else
        {
            cb.normalColor = DisableColor;
            cb.highlightedColor = ActiveColor;
            cb.pressedColor = DisableColor;
            cb.disabledColor = DisableColor;
        }

        toggle.colors = cb;
    }
}
