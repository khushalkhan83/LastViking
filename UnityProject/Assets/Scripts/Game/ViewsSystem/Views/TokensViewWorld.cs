using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Views;
using Game.Models;
using UnityEngine.UI;
using TMPro;

namespace Game.Views
{
    public class TokensViewWorld : MonoBehaviour
    {
        [SerializeField]
        TextMeshPro _text;
        [SerializeField]
        SpriteRenderer _image;

        [SerializeField]
        Transform tokenShowPosition;

        [SerializeField]
        Transform arrow;

        Vector3 myPosition => transform.position;
        Vector3 tokenPosition => tokenShowPosition.position;

        Camera _playerCam;
        TokensModel _model;

        string _key = "";
        public string key => _key;
        float kSize => _model.pixelsInMeter;

        public void Show(string k, Camera playerCam,TokensModel _m)
        {
            _playerCam = playerCam;
            _model = _m;
            _key = k;
            name = "Token_" + k;

            _text.text = "";
            _image.sprite = _model.GetTokenSprite(key);

            //_text.GetComponent<MeshRenderer>().material.renderQueue = 5000;

            transform.position = _model.GetTokenPosition(key);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (_model!=null)
                UpdateToken();
        }

        float maxDistance => Mathf.Min(_playerCam.farClipPlane, _model.GetTokenMaxDistance(key));
        //[SerializeField]
        float borderAngleLerpVal, borderAngle;
        void UpdateToken()
        {
            Vector3 myPos = myPosition;
            Vector3 camPos = _playerCam.transform.position;

            Vector3 camDirection = _playerCam.transform.forward;
            Vector3 realDirection = myPos - camPos;

            float curAngle = Vector3.Angle(camDirection, realDirection);

            float borderVertical = _playerCam.fieldOfView / 2f - 2f;
            //float borderHorisontal = borderVertical * 1.5f;

            //Vector3 projectOnFace = Vector3.ProjectOnPlane(realDirection.normalized, _playerCam.transform.forward).normalized;

            //Debug.DrawRay(camPos, projectOnFace,Color.red);

            ////float minVal =   //Mathf.Cos(borderVertical * Mathf.Deg2Rad);

            //borderAngleLerpVal = 1f-Vector3.Project(projectOnFace, _playerCam.transform.up).magnitude;//(projectOnHorisont.magnitude - minVal) / (1f - minVal);

            borderAngle = borderVertical;//Mathf.Lerp(borderVertical, borderHorisontal, borderAngleLerpVal);

            if (curAngle < borderAngle)
            {
                tokenShowPosition.position = myPos;
                if (arrow.gameObject.activeSelf)
                    arrow.gameObject.SetActive(false);
            }
            else
            {
                Vector3 forwardDirection = camDirection * realDirection.magnitude;
                
                Vector3 newPosShift = Quaternion.RotateTowards(_playerCam.transform.rotation, Quaternion.LookRotation(realDirection, Vector3.up), borderAngle) * (Vector3.forward * realDirection.magnitude);
                tokenShowPosition.position = camPos + newPosShift;
                //Debug.DrawLine(camPos, camPos + forwardDirection, Color.white);
                //Debug.DrawLine(camPos, tokenShowPosition.position,Color.green);
                //Vector3.RotateTowards(forwardDirection, realDirection, borderAngle* Mathf.Deg2Rad, 0f) + camPos;

                if (!arrow.gameObject.activeSelf)
                    arrow.gameObject.SetActive(true);
                Vector3 planeVect = Vector3.ProjectOnPlane(newPosShift,camDirection);
                //Debug.DrawLine(camPos, camPos + planeVect,Color.cyan);

                float angleA = Vector3.Angle(planeVect, _playerCam.transform.up);
                if (Vector3.Dot(planeVect, _playerCam.transform.right) > 0f)
                    angleA = 360f - angleA;

                arrow.localRotation = Quaternion.Euler(0, 0, angleA);
                
            }

            Vector3 screenPos = _playerCam.WorldToScreenPoint(tokenPosition);
            float willScale = 0;
            Color willColor = Color.white;
            string willText = "";
            Color _nearColor = _model.GetTokenNearColor(key);
            Color _maxColor = _model.GetTokenMaxColor(key);             
            float _nearDistance = _model.GetTokenNearDistance(key);
            float _scaleNear = _model.GetTokenScaleNear(key);
            float _scaleMax = _model.GetTokenScaleMax(key);

            if (screenPos.z >= 0)
            {
                float dist = Vector3.Distance(tokenPosition, camPos);
                if (dist < maxDistance)
                {
                    if (isNearScreenCenter(screenPos))
                    {
                        willScale = 1f;
                        willColor = _nearColor;
                        willText = dist.ToString("f0") +"m";
                    }
                    else
                    {
                        float lerpVal = Mathf.Clamp01((dist - _nearDistance) / (maxDistance - _nearDistance));
                        willScale = Mathf.Lerp(_scaleNear, _scaleMax, lerpVal);
                        willColor = Color.Lerp(_nearColor, _maxColor, lerpVal);
                    }
                }
                else
                {
                    willScale = 0f;
                }
            }

            SetTokenData(willScale, willColor, willText);
        }

        Vector2 _centerOfScreen = Vector2.zero;
        bool _isCenterCalced = false;
        Vector2 centerOfScreen
        {
            get
            {
                if (!_isCenterCalced)
                {
                    _centerOfScreen = new Vector2(Screen.width / 2f, Screen.height / 2f);
                    _isCenterCalced = true;
                }
                return _centerOfScreen;

            }
        }

        bool isNearScreenCenter(Vector2 pos)
        {
            return (Vector2.SqrMagnitude(pos - centerOfScreen) < kSize*kSize);
        }

        void SetTokenData(float scale, Color color, string text)
        {
            Vector3 posOnCam1 = _playerCam.WorldToScreenPoint(tokenPosition);

            if (posOnCam1.z < 0)
            {
                tokenShowPosition.gameObject.SetActive(false);
            }
            else
            {
                _image.color = color;
                _text.color = color;
                if (!_text.text.Equals(text))
                    _text.text = text;

                tokenShowPosition.rotation = Quaternion.LookRotation(_playerCam.transform.forward, Vector3.up);
                Vector3 posOnCam2 = _playerCam.WorldToScreenPoint(tokenPosition + tokenShowPosition.up);
                float sc = (posOnCam2.y - posOnCam1.y) / kSize;
                tokenShowPosition.localScale = Vector3.one * (scale / sc);
                

                tokenShowPosition.gameObject.SetActive(true);
            }
        }
    }
}