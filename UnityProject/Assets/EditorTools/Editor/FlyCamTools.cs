using DebugActions;
using Extensions;
using Game.Views;
using UnityEditor;
using UnityEngine;

namespace CustomeEditorTools
{
    public static class FlyCamTools
    {
        private static DebugOptionsActionFlyCamera flyCameraAction;

        // used to reset static variables when when using fast reload for scenes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            flyCameraAction = null;
        }

        [MenuItem("EditorTools/Camera/Switch FlyCam _F11")] // F11 to use
        private static void Switch_FlyCam()
        {
            if (!EditorInPlayMode())
            {
                EditorUtility.DisplayDialog("Error", "Shortcuts work only in play mode", "Ok");
                return;
            }

            if(flyCameraAction == null)
                flyCameraAction = new DebugOptionsActionFlyCamera("name",0.1f);
            
            flyCameraAction.DoAction();
        }

        private static bool EditorInPlayMode()
        {
            return EditorApplication.isPlaying;
        }
    }
}