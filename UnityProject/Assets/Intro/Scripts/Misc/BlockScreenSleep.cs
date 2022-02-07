using UnityEngine;

namespace Game.Misc
{
    public class BlockScreenSleep : MonoBehaviour
    {
        private void OnEnable() 
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        private void OnDisable() 
        {
            Screen.sleepTimeout = SleepTimeout.SystemSetting;
        }
    }
}