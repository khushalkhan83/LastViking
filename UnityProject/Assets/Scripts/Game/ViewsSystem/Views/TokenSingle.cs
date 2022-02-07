using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TokenSingle : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _text;
    [SerializeField]
    Image _image,_arrowImage;
    [SerializeField]
    RectTransform arrow;

    string _key = "";
    RectTransform myPos;


    public string key => _key;

    public void Init(string key, Sprite sprite)
    {
        _text.text = "";
        _image.sprite = sprite;
        _key = key;
        if (myPos == null)
            myPos = GetComponent<RectTransform>();
    }

    public void SetTokenData(string key, Vector2 newPos, float scale, Color color, string text,bool isArrow,Vector2 center)
    {
        if (key == _key)
        {
            myPos.anchoredPosition = newPos;
            myPos.localScale = Vector3.one * scale;
            _image.color = color;
            _text.text = text;
            _text.color = color;
            if (isArrow)
            {
                arrow.localRotation = PosToRotation(newPos, center);
                _arrowImage.color = color;
                arrow.gameObject.SetActive(true);
            }
            else
            {
                arrow.gameObject.SetActive(false);
            }
        
            myPos.ForceUpdateRectTransforms();
        }
    }

    Quaternion PosToRotation(Vector2 newPos,Vector2 center)
    {
        Vector2 rela = newPos - center;
        return Quaternion.FromToRotation(Vector3.up, rela);
    }
}
