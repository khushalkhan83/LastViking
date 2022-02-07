using Core.Views;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using Game.Models;

  
namespace Game.Views
{
    public class TokensView : ViewBase
    {
        [SerializeField]
        GameObject prefab;

        [SerializeField]
        int cahceCount = 10;

        List<TokenSingle> cachedTokens;
        List<TokenSingle> activeTokens;

        public event Action onHideLast;

        RectTransform myRectTr;

        void Init()
        {
            myRectTr = GetComponent<RectTransform>();
            if (cachedTokens == null)
            {
                cachedTokens = new List<TokenSingle>();
                for(int i = 0;i< cahceCount;i++)
                {
                    GameObject newGO = Instantiate(prefab, transform) as GameObject;
                    cachedTokens.Add(newGO.GetComponent<TokenSingle>());
                }
                activeTokens = new List<TokenSingle>();
            }

            maxX = myRectTr.rect.width - horisontalSpacing;
            minX = horisontalSpacing;
            maxY = myRectTr.rect.height - vertivalSpacing;
            minY = vertivalSpacing;
            maxDx = (maxX - minX) / 2f;
            maxDy = (maxY - minY) / 2f;
            midX = (maxX + minX) / 2f;
            midY = (maxY + minY) / 2f;
        }

        public void AddToken(string key, TokensModel mod,Camera cam)
        {
            watchCamera = cam;
            model = mod;
            AddToken(key, model.GetTokenSprite(key));
        }

        public void AddToken(string key, Sprite token)
        {
            Init();

            TokenSingle newT = cachedTokens[0];
            cachedTokens.RemoveAt(0);
            activeTokens.Add(newT);

            newT.Init(key, token);
            newT.gameObject.SetActive(true);
        }

        public void DeleteToken(string key)
        {
            for(int i = 0;i<activeTokens.Count;i++)
            {
                TokenSingle ts = activeTokens[i];
                if (ts.gameObject.activeSelf)
                {
                    if (ts.key.Equals(key))
                    {
                        ts.gameObject.SetActive(false);
                        activeTokens.RemoveAt(i);
                        cachedTokens.Add(ts);
                    }
                }
            }

            CheckLast();
        }

        void CheckLast()
        {
            if(activeTokens.Count == 0)
                onHideLast?.Invoke();
        }

        void SetTokenData(string key, Vector2 newPos,float scale,Color color, string text,bool isArrow,Vector2 center)
        {
            foreach(TokenSingle ts in activeTokens)
            {
                if (ts.gameObject.activeSelf)
                {
                    ts.SetTokenData(key, newPos, scale, color, text,isArrow,center);
                }
            }
        }

        TokensModel model;
        Camera watchCamera;

        public void UpdateTokens()
        {
            foreach(KeyValuePair<string, TokensModel.TokenData> dat in model.Tokens)
            {
                UpdateToken(dat.Key, dat.Value.position);
            }
        }

        [SerializeField]
        float vertivalSpacing = 50;
        [SerializeField]
        float horisontalSpacing = 150;

        float maxX;
        float minX;
        float maxY;
        float minY;
        float maxDx;
        float maxDy;
        float midX;
        float midY;

        Vector2 centerPoint => new Vector2(midX, midY);

        Vector3 ConvertPosition(Vector3 p)
        {   
            Vector3 nowPoint = watchCamera.WorldToScreenPoint(p);
            float aspect = Screen.height / myRectTr.rect.height;
            nowPoint /= aspect;

            Vector3 planeV = new Vector3(nowPoint.x-myRectTr.rect.width/2f, nowPoint.y-myRectTr.rect.height/2f,0f);
            Vector3 planeV_Norm = planeV.normalized;
            float lerpValX = Mathf.Abs( planeV_Norm.x);
            float lerpValY = Mathf.Abs( planeV_Norm.y);

            bool willArrow = false;

            float willX = Mathf.Clamp(nowPoint.x, midX - maxDx * lerpValX, midX + maxDx * lerpValX);
            if (willX != nowPoint.x)
                willArrow = true;
            nowPoint.x = willX;

            float willY = Mathf.Clamp(nowPoint.y, midY - maxDy * lerpValY, midY + maxDy * lerpValY);
            if (willY != nowPoint.y)
                willArrow = true;
            nowPoint.y = willY;

            if (nowPoint.z < 0)
            {
                nowPoint.x = midX + maxDx * (nowPoint.x < midX ? lerpValX : (-lerpValX));
                nowPoint.y = midY + maxDy * (nowPoint.y < midY ? lerpValY : (-lerpValY));

                willArrow = true;
            }

            if (willArrow)
            {
                nowPoint.z = 0f;
            }
            return nowPoint;
        }

        bool isNearScreenCenter(Vector2 pos)
        {
            return (Vector2.SqrMagnitude(pos - centerOfScreen) < 1000f);
        }

        Vector2 _centerOfScreen = Vector2.zero;
        bool _isCenterCalced = false;
        Vector2 centerOfScreen
        {
            get
            {
                if (!_isCenterCalced)
                {
                    _centerOfScreen = myRectTr.rect.size / 2f;
                    _isCenterCalced = true;
                }
                return _centerOfScreen;
            }
        }

        void UpdateToken(string Key, Vector3 pos)
        {
            Vector3 nowPos = ConvertPosition(pos);
            float willScale = 0;
            Color willColor = Color.white;
            string willText = "";
            Color _nearColor = model.GetTokenNearColor(Key);
            Color _maxColor = model.GetTokenMaxColor(Key);
            float nearDistance = model.GetTokenNearDistance(Key);
            float maxDistance = model.GetTokenMaxDistance(Key);
            float scaleNear = model.GetTokenScaleNear(Key);
            float scaleMax = model.GetTokenScaleMax(Key);

            if (nowPos.z >= 0)
            {
                float dist = Vector3.Distance(pos, watchCamera.transform.position);
                if (dist < maxDistance)
                {
                    if (isNearScreenCenter(nowPos))
                    {
                        willScale = 1f;
                        willColor = _nearColor;
                        willText = dist.ToString("f0")+" m";
                    }
                    else
                    {
                        float lerpVal = Mathf.Clamp01((dist - nearDistance) / (maxDistance - nearDistance));
                        willScale = Mathf.Lerp(scaleNear, scaleMax, lerpVal);
                        willColor = Color.Lerp(_nearColor, _maxColor, lerpVal);
                    }
                }
                else
                {
                    willScale = 0f;
                }
            }
            SetTokenData(Key, nowPos, willScale, willColor, willText, nowPos.z == 0f, centerPoint);
        }
    }
}
