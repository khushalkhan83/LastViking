using System.Collections.Generic;
using Game.Models;
using UnityEngine;
using Extensions;
using Game.Controllers;
using System;
using System.Collections;

namespace Game.QuestSystem.Map.Extra.Kraken
{
    public class KrakenSpawnManager : MonoBehaviour
    {   
        private FirstKrakenModel FirstKrakenModel => ModelsSystem.Instance._firstKrakenModel;
        private QuestNotificationsModel QuestNotificationsModel => ModelsSystem.Instance._questNotificationsModel;

        [SerializeField] private  List<AnimalPointSpawner> _krakenSpawnPoints = default;
        private bool _krakenIsSpawned;


        #region MonoBehaviour
        private void OnEnable()
        {
            if(FirstKrakenModel.Active)
                MainLogic();
            else
                FirstKrakenModel.OnKrakenActivate += MainLogic;
        }

        private void OnDisable()
        {
            FirstKrakenModel.OnKrakenActivate -= MainLogic;

            if(_krakenIsSpawned)
            {
                QuestNotificationsModel.RemoveTargetEnemyHealth();
                
                var kraken = GetComponentInChildren<Initable>();
                if(kraken != null)
                    Destroy(kraken.gameObject);
            }
            _krakenIsSpawned = false;

            _krakenSpawnPoints.ForEach(x => x.enabled = false);
        }
        #endregion

        private void MainLogic()
        {
            FirstKrakenModel.OnKrakenActivate -= MainLogic;
            
            if(_krakenIsSpawned) return;

            // do this after frame because QuestNotificationsModel.SetTargetEnemyHealth must be called after main stage message is shown (QuestNotificationsMode.Show())
            DoActionAfterFrame(() => {
                var spawner = _krakenSpawnPoints.RandomElement();
                spawner.enabled = true;
                _krakenIsSpawned = true;

                var kraken = GetComponentInChildren<Initable>();
                var krakenHealth = kraken.GetComponentInChildren<IHealth>();
                
                QuestNotificationsModel.SetTargetEnemyHealth(krakenHealth);
            });
        }

        private void DoActionAfterFrame(Action action) => StartCoroutine(CDoActionAfterFrame(action));

        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator CDoActionAfterFrame( Action action)
        {
            yield return null;
            action?.Invoke();
        }
    }
}
