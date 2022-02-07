using UnityEngine;


namespace Game.ThirdPerson.HUD
{
    public class HUDView : MonoBehaviour
    {
        #pragma warning disable 0649
        [SerializeField] private CanvasGroup canvas;
        [SerializeField] private GameObject aimButton;
        [SerializeField] private CanvasGroup extraCanvas;
        
        #pragma warning restore 0649
        public void Show(bool show)
        {
            var alpha = show ? 1 : 0;
            canvas.alpha = alpha;
            extraCanvas.alpha = alpha;

            canvas.interactable = show;
            extraCanvas.interactable = show;
        }
        public void ShowAimButton(bool show) => aimButton.SetActive(show);
    }
}