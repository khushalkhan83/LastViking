using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using Game.Providers;
using UnityEngine;

namespace Game.Controllers
{
    public class EventTokensController : IEventTokensController, IController
    {
        [Inject] public TokensModel TokensModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public EventTokensModel EventTokensModel { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel {get; private set;}
        [Inject] public TombsModel TombsModel {get; private set;}

        public void Enable()
        {
            PlayerDeathModel.OnRevival += OnPlayerRevival;
            PlayerDeathModel.OnRevivalPrelim += OnPlayerRevival;
            PlayerHealthModel.OnDeath += OnPlayerDeath;
            PlayerScenesModel.OnEnvironmentChange += OnEvnrimentChanged;
        }

        public void Start()
        {
            if (!PlayerHealthModel.IsDead)
            {
                WorldObjectsModel.OnAdd.AddListener(EventTokensModel.TombId, OnAddTomb);
                WorldObjectsModel.OnRemove.AddListener(EventTokensModel.TombId, OnRemoveTomb);
                ResetTombState();
            }
        }

        public void Disable()
        {
            PlayerHealthModel.OnDeath -= OnPlayerDeath;
            PlayerDeathModel.OnRevival -= OnPlayerRevival;
            PlayerDeathModel.OnRevivalPrelim -= OnPlayerRevival;
            PlayerScenesModel.OnEnvironmentChange -= OnEvnrimentChanged;

            WorldObjectsModel.OnAdd.RemoveListener(EventTokensModel.TombId, OnAddTomb);
            WorldObjectsModel.OnRemove.RemoveListener(EventTokensModel.TombId, OnRemoveTomb);
        }

        private void OnEvnrimentChanged()
        {
           ResetTombState();
        }

        private void OnPlayerDeath()
        {
            TokensModel.HideAll();

            WorldObjectsModel.OnAdd.RemoveListener(EventTokensModel.TombId, OnAddTomb);
            WorldObjectsModel.OnRemove.RemoveListener(EventTokensModel.TombId, OnRemoveTomb);
        }

        private void OnPlayerRevival()
        {
            WorldObjectsModel.OnAdd.AddListener(EventTokensModel.TombId, OnAddTomb);
            WorldObjectsModel.OnRemove.AddListener(EventTokensModel.TombId, OnRemoveTomb);

            ResetTombState();
        }

        private void OnAddTomb(WorldObjectModel model)
        {
            ActivateTombToken(model);
            ResetTombState();
        }

        private void OnRemoveTomb(WorldObjectModel model)
        {
            DeactivateTombToken();
            ActivateLastTombToken();
        }

        private void ResetTombState()
        {
            DeactivateTombToken();

            ActivateLastTombToken();
        }

        private void ActivateLastTombToken()
        {
            if (WorldObjectsModel.SaveableObjectModels.TryGetValue(EventTokensModel.TombId, out var models) &&
                models.Count > 0)
            {
                ActivateTombToken(models[models.Count - 1]);
            }
        }

        private void ActivateTombToken(WorldObjectModel model) => TokensModel.ShowToken(EventTokensModel.TombPrefix, TombsModel.TokenConfigId, model.Position + Vector3.up * 1f);
        private void DeactivateTombToken() => TokensModel.HideToken(EventTokensModel.TombPrefix);

    }
}