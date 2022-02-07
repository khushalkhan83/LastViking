using System;
using Core.Storage;
using UnityEngine;

namespace Game.Storage
{
    public class StorageMobile : StorageFiles
    {
        public override string RootPath => GetAndroidFriendlyFilesPath();
        private string GetAndroidFriendlyFilesPath()
        {
#if UNITY_ANDROID
            if(!string.IsNullOrEmpty(Application.persistentDataPath))
            {
                return Application.persistentDataPath;
            }
            else
            {
                AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject applicationContext = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaObject path = applicationContext.Call<AndroidJavaObject>("getFilesDir");
                string filesPath = path.Call<string>("getCanonicalPath");
                // use Debug.LogException to trigger Cloud Diagnostic report
                Debug.LogException(new Exception($"Application.persistentDataPath is null or empty: {filesPath} is used"));
                return filesPath;
            }
#endif
            return Application.persistentDataPath;
        }
    }
}