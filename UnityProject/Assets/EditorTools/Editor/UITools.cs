using cakeslice;
using Extensions;
using Game.Views;
using UnityEditor;
using UnityEngine;

namespace CustomeEditorTools
{
    public static class UITools
    {
        private static bool visiable = true;
        private const string k_IngameDebugConsole = "IngameDebugConsole";

        [MenuItem("EditorTools/UI/Switch UI Visiable _F12")] // F12 to use
        private static void Switch_UI_Visiable()
        {
            if (!EditorInPlayMode())
            {
                EditorUtility.DisplayDialog("Error", "Shortcuts work only in play mode", "Ok");
                return;
            }

            visiable = !visiable;
            Set_UI_Visiable(visiable);
        }

        private static void Set_UI_Visiable(bool visiable)
        {
            GetMainUICamera().enabled = visiable;
            GetIngameDebugConsole().CheckNull()?.SetActive(visiable);

            var oultine = GetOutlineEffect();
            if(oultine != null)
            {
                oultine.enabled = visiable;
            }
        }

        private static Camera GetMainUICamera() => ViewsSystem.Instance.GetComponent<Camera>();
        private static GameObject GetIngameDebugConsole() => GameObject.Find(k_IngameDebugConsole);
        private static OutlineEffect GetOutlineEffect()  => GameObject.FindObjectOfType<OutlineEffect>();

        private static bool EditorInPlayMode()
        {
            return EditorApplication.isPlaying;
        }
    }
}