using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Models
{
    public class EventSystemModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private EventSystem _eventSystem;

        #pragma warning restore 0649
        #endregion


        public event Action<GameObject, GameObject> OnSelectionChanged;
        public EventSystem EventSystem => _eventSystem;

        public void SelectionChanged(GameObject newSelection, GameObject oldSelection) => OnSelectionChanged?.Invoke(newSelection,oldSelection);

        public void SetSelectedGameObject(GameObject gameObject) { } //_eventSystem.SetSelectedGameObject(gameObject);

        public PointerEventData GetPointerData() => new PointerEventData(_eventSystem);
    }
}
