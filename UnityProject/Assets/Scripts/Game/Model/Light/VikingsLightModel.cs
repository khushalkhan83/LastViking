using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Game.Models
{
    public class VikingsLightModel : MonoBehaviour
    {
        [SerializeField] private PlayableDirector playableDirector = default;

        public GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;
        public GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        private void OnEnable() 
        {
            GameUpdateModel.OnUpdate += OnUpdate;    
        }

        private void OnDisable() 
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            float normalizedTime = (float)playableDirector.duration * (float)GameTimeModel.EnviroTimeOfDayTicks / GameTimeModel.DayDurationTicks;
            playableDirector.time = normalizedTime;
            playableDirector.Evaluate();
        }
    }
}
