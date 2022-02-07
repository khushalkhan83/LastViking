using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;
using UltimateSurvival;
using System.Collections.Generic;

namespace Game.Controllers
{
    public abstract class TokenMineableControllerBase : IController
    {
        [Inject] public TokensModel TokensModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public MinebleElementsModel MinebleElementsModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        protected virtual int TokenConfigId => 0;
        protected abstract string TockenID {get;}
        protected abstract OutLineMinableObjectID outLineMinableObjectID {get;}
        protected virtual Vector3 ShiftTokenPosition => Vector3.up;

        protected MinebleElement currentTokenTarget = null;
        protected float closestSqrDistance;
        protected float sqrDistance;

        public virtual void Enable()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        public virtual void Start()
        {
            
        }

        public virtual void Disable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            Hide();
        }


        protected void OnUpdate()
        {
            UpdateToken();
        }

        private void UpdateToken()
        {
            if (MinebleElementsModel.OutlineObjects.TryGetValue(outLineMinableObjectID, out List<MinebleElement> elements))
            {
                MinebleElement closestTarget = null;
                closestSqrDistance = float.MaxValue;
                foreach (var element in elements)
                {
                    sqrDistance = (PlayerEventHandler.transform.position - element.transform.position).sqrMagnitude;
                    if (sqrDistance < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistance;
                        closestTarget = element;
                    }
                }

                if (currentTokenTarget != closestTarget)
                {
                    currentTokenTarget = closestTarget;
                    Hide();
                    if (currentTokenTarget != null)
                        Show(currentTokenTarget.transform.position);
                }
            }
        }

        protected virtual void Show(Vector3 position) => TokensModel.ShowToken(TockenID, TokenConfigId, position + ShiftTokenPosition);

        protected virtual void Hide() => TokensModel?.HideToken(TockenID);
    }
}
