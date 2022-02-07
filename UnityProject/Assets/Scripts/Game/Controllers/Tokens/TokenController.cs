using System;
using System.Collections.Generic;
using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Purchases;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class TokenController : IController, ITokenController
    {
        [Inject] public TokensModel TokensModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public FPManager FPManager { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public GameLateUpdateModel GameLateUpdateModel { get; private set; }
        [Inject] public WorldCameraModel WorldCameraModel { get; private set; }

        public void Disable()
        {
            TokensModel.onShow -= ShowHandler;
            TokensModel.onHide -= HideHandler;
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalHandler;
        }
        public void Enable()
        {
            TokensModel.onShow += ShowHandler;
            TokensModel.onHide += HideHandler;
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalHandler;
        }

        public void Start()
        {

        }

        void OnRevivalHandler()
        {
            //TokensModel.ReshowAll();
        }

        void ShowHandler(string k, Vector3 p)
        {
            //TokensViewWorld token = GetToken();
            //token.Show(k, FPManager.tokenCamera, TokensModel);
            //FPManager.tokenCamera.enabled = true;
            if (View == null)
            {
                OpenView();
            }
            View.AddToken(k, TokensModel, WorldCameraModel.WorldCamera);
        }

        void HideHandler(string k)
        {
            if (View != null)
                View.DeleteToken(k);
            if (TokensModel.tokensCount == 0)
            {
                CloseView();
            }

            //if (tokenLib == null)
            //    return;
            //TokensViewWorld t = null;
            //foreach (TokensViewWorld tvw in tokenLib)
            //{
            //    if (tvw.gameObject.activeSelf && tvw.key.Equals(k))
            //    {
            //        t = tvw;
            //        break;
            //    }
            //}

            //if (t != null)
            //    t.Hide();

            //if (!tokenLib[tokenLib.Count - 1].gameObject.activeSelf)
            //    FPManager.tokenCamera.enabled = false;
        }

        List<TokensViewWorld> tokenLib;
        public TokensViewWorld GetToken()
        {
            if (tokenLib == null)
            {
                tokenLib = new List<TokensViewWorld>();
                SpawnTokens(4);
            }

            TokensViewWorld selectedToken = null;
            for (int i = 0; i < tokenLib.Count; i++)
            {
                if (!tokenLib[i].gameObject.activeSelf)
                {
                    selectedToken = tokenLib[i];
                    break;
                }
            }

            if (selectedToken == null)
            {
                SpawnTokens(4);
                return GetToken();
            }
            else
            {
                tokenLib.Remove(selectedToken);
                tokenLib.Add(selectedToken);
                return selectedToken;
            }
        }

        void SpawnTokens(int n)
        {
            for (int i = 0; i < n; i++)
            {
                GameObject go = GameObject.Instantiate(TokensModel.prefab) as GameObject;
                go.SetActive(false);
                tokenLib.Add(go.GetComponent<TokensViewWorld>());
            }
        }

        public TokensView View { get; private set; }

        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<TokensView>(ViewConfigID.Tokens);
                View.OnHide += OnHideHandler;
                GameLateUpdateModel.OnLaterUpdate += UpdatePositionsUI;
            }
        }

        private void CloseView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            GameLateUpdateModel.OnLaterUpdate -= UpdatePositionsUI;
            View = null;
        }

        void UpdatePositionsUI()
        {
            //if (View != null)
            //{
            //    foreach (KeyValuePair<string, TokensModel.TokenData> pair in TokensModel.Tokens)
            //    {
            //        UpdateToken(pair.Key, pair.Value.position);
            //    }
            //}
        }

        //Vector3 ConvertPosition(Vector3 p)
        //{
        //    Vector3 nowPoint = WorldCameraModel.WorldCamera.WorldToScreenPoint(p);
        //    return nowPoint;
        //}

        //bool isNearScreenCenter(Vector2 pos)
        //{
        //    return (Vector2.SqrMagnitude(pos - centerOfScreen) < 1000f);
        //}

        //Vector2 _centerOfScreen = Vector2.zero;
        //bool _isCenterCalced = false;
        //Vector2 centerOfScreen
        //{
        //    get
        //    {
        //        if (!_isCenterCalced)
        //        {
        //            _centerOfScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
        //            _isCenterCalced = true;
        //        }
        //        return _centerOfScreen;
        //    }
        //}

        //void UpdateToken(string Key, Vector3 pos)
        //{
        //    Vector3 nowPos = ConvertPosition(pos);
        //    float willScale = 0;
        //    Color willColor = Color.white;
        //    string willText = "";

        //    if (nowPos.z >= 0)
        //    {
        //        float dist = Vector3.Distance(pos, WorldCameraModel.WorldCamera.transform.position);
        //        if (dist < TokensModel.maxDistance)
        //        {
        //            if (isNearScreenCenter(nowPos))
        //            {
        //                willScale = 1f;
        //                willColor = TokensModel.nearColor;
        //                willText = dist.ToString("f0");
        //            }
        //            else
        //            {
        //                float lerpVal = Mathf.Clamp01((dist - TokensModel.nearDistance) / (TokensModel.maxDistance - TokensModel.nearDistance));
        //                willScale = Mathf.Lerp(TokensModel.scaleNear, TokensModel.scaleMax, lerpVal);
        //                willColor = Color.Lerp(TokensModel.nearColor, TokensModel.maxColor, lerpVal);
        //            }
        //        }
        //        else
        //        {
        //            willScale = 0f;
        //        }
        //    }

        //    View.SetTokenData(Key, nowPos, willScale, willColor, willText);
        //}
    }
}