using Core.Views;
using System;
using UnityEngine;

namespace Game.Views
{
    public class DebugTimeInteractView : ViewBase
    {
        //UI
        public event Action OnSwitchVisible;
        public void SwitchVisible() => OnSwitchVisible?.Invoke();

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
        private GameObject ProjectViewPrefab => EditorGameSettings.settings.DebugAssets.ProjectView;
        
        private void Start() 
        {
            if(EditorGameSettings.godModeOnStart)
            {
                SwitchVisible();
                SwitchVisible();
            }
            RefreshProjectView();
        }

        private bool projectViewEnabled;
        private GameObject instance;

        public void SwitchProjectView()
        {
            projectViewEnabled = !projectViewEnabled;
            RefreshProjectView();
        }


        private void RefreshProjectView()
        {
            if(projectViewEnabled)
            {
                instance = Instantiate(ProjectViewPrefab,this.transform);
            }
            else
            {
                Destroy(instance);
                instance = null;
            }
        }
    }
}
