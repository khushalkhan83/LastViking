using UnityEditor;
using UnityEngine;
using Extensions;

public static class TimeTools
{
    [MenuItem("EditorTools/Time:normal _F7")] // F7 to use
    private static void Normal()
    {
        if(IsEditMode)
        {
            EditorUtility.DisplayDialog("Error", ErorrMessage, "Ok");
            return;
        }

        Time.timeScale = 1;
    }
    [MenuItem("EditorTools/Time:faster _F8")] // F8 to use
    private static void Faster()
    {
        if(IsEditMode)
        {
            EditorUtility.DisplayDialog("Error", ErorrMessage, "Ok");
            return;
        }

        Time.timeScale++;
    }

    private static bool IsEditMode => !EditorApplication.isPlaying;
    private static string ErorrMessage => "Данная команда работает только в PlayMode";
}
