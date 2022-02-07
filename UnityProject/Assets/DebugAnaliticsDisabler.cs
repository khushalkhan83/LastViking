using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugAnaliticsDisabler : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToDisable = new List<GameObject>();
    private void Awake()
    {
        if(Debug.isDebugBuild && !EditorGameSettings.Instance.enableAnalitics)
        {
            objectsToDisable.ForEach(x => x.SetActive(false));
        }
    }
}
