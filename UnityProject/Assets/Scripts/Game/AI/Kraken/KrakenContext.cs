using Game.Models;
using UnityEngine;


namespace Game.AI.Behaviours.Kraken
{
    public class KrakenContext : MonoBehaviour
    {
        private FirstKrakenModel FirstKrakenModel => ModelsSystem.Instance._firstKrakenModel;

        private IKrakenConfigurable[] configurables = default;

        #region MonoBehaviour

        private void Awake()
        {
            configurables = GetComponentsInChildren<IKrakenConfigurable>(true);
        }
        #endregion

        // called by InitableWithEvents
        public void Configurate()
        {
            if(configurables == null) return;

            var config = FirstKrakenModel.Config;
            if(config == null) return;

            foreach (var configurable in configurables)
            {
                configurable.Configurate(config);
            }
        }
    }
}
