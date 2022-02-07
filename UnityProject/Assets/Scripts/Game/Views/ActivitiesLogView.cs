using Core.Views;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Game.Views.SubViews;

namespace Game.Views
{
    public class ActivitiesLogView : ViewBase
    {
        
        #region Data
        #pragma warning disable 0649
        [SerializeField] private List<ActivityLogEnterence> activityLogEnterences;
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform contentRectTransform;

        [Header("Animation settings")]
        [SerializeField] private float contentPositionShift = -2000f;
        
        #pragma warning restore 0649
        #endregion

        public List<ActivityLogEnterence> ActivityLogEnterences => activityLogEnterences;

        public event Action OnCloseClick;

        [Button]
        public void ShowContentAnimation()
        {
            var temp = contentRectTransform.position;
            temp.x = contentPositionShift;
            contentRectTransform.position = temp;
        }

        public void PrepareEnoughViews(int count)
        {
            if(count > activityLogEnterences.Count)
            {
                var passes = count - activityLogEnterences.Count;

                var view = activityLogEnterences.FirstOrDefault();
                for (int i = 0; i < passes; i++)
                {
                    var newView = Instantiate(view,Vector3.zero,Quaternion.identity,view.transform.parent);
                    newView.transform.localPosition = Vector3.zero;
                    activityLogEnterences.Add(newView);
                }
            }
        }

        #region MonoBehaviour

        private void OnEnable()
        {
            closeButton.onClick.AddListener(() => OnCloseClick?.Invoke());
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveAllListeners();
        }
        #endregion
    }
}
