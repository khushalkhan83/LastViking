using UnityEngine;
using UnityEngine.UI;

namespace Game.Views.SubViews
{
    public class ActivityLogEnterence: MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Text desciption;
        [SerializeField] private Image icon;
        [SerializeField] private Image textBackgroundImage;
        [SerializeField] private Image iconBackgroundImage;
        #pragma warning restore 0649
        #endregion

        public string Desciption {set => desciption.text = value;}
        public Sprite Icon {set => icon.sprite = value;}
        public float IconScale {set => icon.transform.localScale = Vector3.one * value;}
        public Color TextBacgroundColor {set => textBackgroundImage.color = value;}
        public Color IconBackgroundColor {set => iconBackgroundImage.color = value;}
    }
}
