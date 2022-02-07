using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class CoroutineModel : MonoBehaviour
    {
        private Dictionary<int, Coroutine> _coroutines = new Dictionary<int, Coroutine>();
        private int _baseCoroutineId = 0;

        public int InitCoroutine(IEnumerator coroutine)
        {
            _baseCoroutineId++;
            _coroutines[_baseCoroutineId] = StartCoroutine(coroutine);
            return _baseCoroutineId;
        }

        public void BreakeCoroutine(int coroutineId)
        {
            if (_coroutines.TryGetValue(coroutineId, out Coroutine coroutine))
            {
              
                _coroutines.Remove(coroutineId);

                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
        }

        public Coroutine GetCoroutine(int coroutineId) {
            if (_coroutines.TryGetValue(coroutineId, out Coroutine coroutine))
                return coroutine;
            else
                return null;
        }
    }
}
