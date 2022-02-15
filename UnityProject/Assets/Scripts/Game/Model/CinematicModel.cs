using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.Models
{
    public class CinematicModel : MonoBehaviour
    {
        public event Action OnStartCinematic;
        public event Action OnEndCinematic;
        public event Action OnCinematicActiveStateChanged;

        public bool CanResetInEditor {get; private set;}
        public bool CinematicStarted {get; private set;}

        public void StartCinematic()
        {
            OnStartCinematic?.Invoke();
        }

        public void EndCinematic()
        {
            OnEndCinematic?.Invoke();
        }

        public void SetCanResetInEditor(bool value) => CanResetInEditor = value;

        public void ResetCameraInPlayMode()
        {
            if(!CanResetInEditor) return;
            #if UNITY_EDITOR
            bool editMode = !EditorApplication.isPlaying;
            if(editMode) return;
            EndCinematic();
            #endif
        }

        public void SetCinematicStarted(bool value)
        {
            CinematicStarted = value;
        }
    }
}
