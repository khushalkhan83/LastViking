using Core;
using Core.Controllers;
using Game.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Controllers
{
    public class WideScreenScaleController : IWideScreenScaleController, IController
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        void IController.Enable()
        {
            var aspect = (float)Screen.width / Screen.height;

#if UNITY_EDITOR
            //var T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            // problem is below: there is no GetMainGameView metod
            //var GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            //var Res = GetMainGameView.Invoke(null, null);
            //var gameView = (UnityEditor.EditorWindow)Res;
            //var prop = T.GetType().GetProperty("currentGameViewSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //var gvsize = prop.GetValue(T, new object[0] { });
            //var gvSizeType = gvsize.GetType();
            //var height = (int)gvSizeType.GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });
            //var width = (int)gvSizeType.GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });

            //aspect = (float)width / height;
#endif

            if (aspect > 2.1f)
            {
                ViewsSystem.GetComponentInChildren<CanvasScaler>().referenceResolution = new Vector2(2100, 1080);
            }
            if (aspect > 2.3f)
            {
                ViewsSystem.GetComponentInChildren<CanvasScaler>().referenceResolution = new Vector2(2300, 1080);
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
        }
    }
}
