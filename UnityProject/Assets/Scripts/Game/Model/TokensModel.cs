using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace Game.Models
{
    public class TokensModel : MonoBehaviour
    { 
        public class TokenData
        {
            public int configIdx;
            Vector3 _position;
            public Vector3 position 
            { 
                get { return _position; }
                set { _position = value; }
            }
            public TokenData(int _configIdx, Vector3 _pos)
            {
                configIdx = _configIdx;
                _position = _pos;
            }
        }

        [System.Serializable]
        public class TokenConfigData
        {
            public Sprite sprite;
            public Color nearColor;
            public Color maxColor;
            public float nearDistance = 20f;
            public float maxDistance = 900f;
            public float scaleNear = 0.8f;
            public float scaleMax = 0.2f;
        }

        Dictionary<string, TokenData> tokens;

        [SerializeField]
        TokenConfigData[] tokenConfigs;
        
        [SerializeField,Tooltip("Prefab with TokensViewWorld Component")]
        GameObject _worldTokenPrefab;
        [SerializeField,Tooltip("TokenSize on screen in pixels")]
        float _pixelsInMeter = 100f;

        public event System.Action<string,Vector3> onShow;
        public event System.Action<string> onHide;

        public GameObject prefab => _worldTokenPrefab;
        public float pixelsInMeter => _pixelsInMeter;

        public void ShowToken(string key,int configIdx,Vector3 position)
        {
            if (tokens == null)
                tokens = new Dictionary<string, TokenData>();

            if (tokens.ContainsKey(key))
            {
                Debug.LogError("Token " + key + " exist");
            }
            else
            {
                //Debug.Log("Add Token "+key+" at "+position.ToString("f2"));

                tokens.Add(key, new TokenData(configIdx, position));
                onShow?.Invoke(key,position);
            }
        }

        public Sprite GetTokenSprite(string key) => tokenConfigs[tokens[key].configIdx].sprite;
        public Color GetTokenNearColor(string key) => tokenConfigs[tokens[key].configIdx].nearColor;
        public Color GetTokenMaxColor(string key) => tokenConfigs[tokens[key].configIdx].maxColor;
        public float GetTokenNearDistance(string key) => tokenConfigs[tokens[key].configIdx].nearDistance;
        public float GetTokenMaxDistance(string key) => tokenConfigs[tokens[key].configIdx].maxDistance;
        public float GetTokenScaleNear(string key) => tokenConfigs[tokens[key].configIdx].scaleNear;
        public float GetTokenScaleMax(string key) => tokenConfigs[tokens[key].configIdx].scaleMax;
        public Vector3 GetTokenPosition(string key) => tokens[key].position;

        public void HideToken(string key)
        {
            if (tokens == null)
                tokens = new Dictionary<string, TokenData>();

            if (tokens.ContainsKey(key))
            {
                tokens.Remove(key);
                onHide?.Invoke(key);
            }
        }

        public void UpdateToken(string key, Vector3 position)
        {
            if(tokens.TryGetValue(key, out var tokenData))
            {
                tokenData.position = position;
            }
        }

        public int tokensCount => tokens==null?0:tokens.Count;
        public Dictionary<string, TokenData> Tokens => tokens;

        public void HideAll()
        {
            if (tokens != null)
            {
                var array = Tokens.ToArray();
                foreach (var item in array)
                {
                    HideToken(item.Key);
                }
            }
        }

        public void ReshowAll()
        {
            if (tokens != null)
            {
                foreach (KeyValuePair<string, TokenData> pair in tokens)
                {
                    onShow?.Invoke(pair.Key, pair.Value.position);
                }
            }
        }
    }
}