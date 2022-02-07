using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ActionsCollections.UI
{
    public class ActionsPanel : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private Transform _backButtonHolder;
        [SerializeField] private Transform _buttonPrefab;


        public void SetData(SectionBase section, UnityAction onBackButton)
        {
            RemoveAllButtons();
            AddActionsButtons(section.Actions);
            SetBackButton(onBackButton);

        }

        private void RemoveAllButtons()
        {
            foreach (Transform item in _content)
            {
                if (item == _backButtonHolder) continue;

                Destroy(item.gameObject);
            }
        }

        private void AddActionsButtons(List<ActionBase> actions)
        {
            foreach (var action in actions)
            {
                var newButton = Instantiate(_buttonPrefab, _content);
                newButton.gameObject.SetActive(true);
                newButton.GetComponentInChildren<Text>().text = action.OperationName;
                var buttonComponent = newButton.GetComponent<Button>();

                buttonComponent.onClick.RemoveAllListeners();
                buttonComponent.onClick.AddListener(action.DoAction);
            }
        }

        private void SetBackButton(UnityAction onBackButton)
        {
            var backButton = _backButtonHolder.GetComponentInChildren<Button>();
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => onBackButton?.Invoke());
        }
    }
}
