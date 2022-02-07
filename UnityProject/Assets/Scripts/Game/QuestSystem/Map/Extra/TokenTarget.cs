using System;
using System.Collections;
using Game.Models;
using NaughtyAttributes;
using UnityEngine;
using Extensions;

namespace Game.QuestSystem.Map.Extra
{
    public class TokenTarget : MonoBehaviour
    {
        private TokensModel TokensModel => ModelsSystem.Instance._tokensModel;
        private CoroutineModel CoroutineModel => ModelsSystem.Instance._coroutineModel;
        private PlayerDeathModel PlayerDeathModel => ModelsSystem.Instance._playerDeathModel;
        
        #region Data
        #pragma warning disable 0649
        [SerializeField] private int tokenConfigId;
        [SerializeField] private bool repaint = false;
        [SerializeField] private bool itself = true;

        [HideIf("itself")]
        [SerializeField] private Transform target;
        
        #pragma warning restore 0649
        #endregion

        private static int tockenCounter = 1;

        private int corutineID;
        private int tockenIndex;
        private string tockenID;
        private Transform Target => itself ? transform : target;
        private Transform NotNullTarget
        {
            get
            {
                var t = Target;
                if(t == null) return transform;
                return t;
            }
        }
        public int TokenConfigId 
        {
            get{return tokenConfigId;}
            set{tokenConfigId = value;}
        }
        public bool Repaint
        {
            get{return repaint;}
            set{repaint = value;}
        }


        #region MonoBehaviour
        private void OnEnable()
        {
            PlayerDeathModel.OnRevival += OnPlayerRevival;
            PlayerDeathModel.OnRevivalPrelim += OnPlayerRevival;

            tockenIndex = tockenCounter++;
            tockenID = "Token" + tockenIndex;
            Show();
            if(repaint)
                corutineID = CoroutineModel.InitCoroutine(RepaintTocken());
        }

        private void OnDisable()
        {
            PlayerDeathModel.OnRevival -= OnPlayerRevival;
            PlayerDeathModel.OnRevivalPrelim -= OnPlayerRevival;
            
            if(repaint)
                CoroutineModel.CheckNull()?.BreakeCoroutine(corutineID);
            Hide();
        }
        #endregion

        private void Show() => TokensModel.ShowToken(tockenID, tokenConfigId, NotNullTarget.position);

        private void Hide() => TokensModel?.HideToken(tockenID);

        private IEnumerator RepaintTocken()
        {
            while(true)
            {
                yield return null;
                TokensModel.UpdateToken(tockenID, NotNullTarget.position);
            }
        }

        private void OnPlayerRevival()
        {
            Hide();
            Show();
        }
    }
}