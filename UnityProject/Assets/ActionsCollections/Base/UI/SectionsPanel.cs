using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ActionsCollections.UI
{
    public class SectionsPanel : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private Transform _buttonPrefab;

        public void SetData(List<SectionBase> sections, UnityAction<SectionBase> onSectionSelected)
        {
            RemoveAllButtons();
            AddActionsButtons(sections, onSectionSelected);
        }

        private void RemoveAllButtons()
        {
            foreach (Transform item in _content)
            {
                Destroy(item.gameObject);
            }
        }

        private void AddActionsButtons(List<SectionBase> sections, UnityAction<SectionBase> onSectionSelected)
        {
            foreach (var section in sections)
            {
                var newButton = Instantiate(_buttonPrefab, _content);
                newButton.gameObject.SetActive(true);
                newButton.GetComponentInChildren<Text>().text = section.SectionName;
                var buttonComponent = newButton.GetComponent<Button>();

                buttonComponent.onClick.RemoveAllListeners();
                buttonComponent.onClick.AddListener(() =>
                {
                    onSectionSelected(section);
                });
            }
        }
    }
}
