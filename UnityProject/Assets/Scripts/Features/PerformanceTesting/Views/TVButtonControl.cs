using UnityEngine;
using UnityEngine.EventSystems;

public class TVButtonControl : MonoBehaviour
{
    private void Awake()
    {
        EventSystem.current.SetSelectedGameObject(this.gameObject);
    }
}
