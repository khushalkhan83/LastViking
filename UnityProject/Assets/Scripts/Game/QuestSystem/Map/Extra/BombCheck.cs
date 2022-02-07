using System;
using System.Collections;
using Game.Interactables;
using Game.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Game.QuestSystem.Map.Extra
{
    public class BombCheck : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private UnityEvent onCheckPassed;
        #pragma warning restore 0649
        #endregion

        private BombDestroyEnter BombDestroyEnter => FindObjectOfType<BombDestroyEnter>();

        public void Check()
        {
            StartCoroutine(DoAfterSeconds(3,() => {
                if(BombDestroyEnter.IsOpen)
                {
                    onCheckPassed?.Invoke();
                }
            }));
            
        }

        IEnumerator DoAfterSeconds(float seconds, Action action)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
            yield break;
        }
    }
}