using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class ColumnEditor : MonoBehaviour
{
    public GameObject cShaft;
    public GameObject cShaftBottom;
    public GameObject cShaftTop;
    public List<GameObject> cParts;
    
    void OnEnable()
    {
        Debug.Log("Awake");
        getAllParts();
    }

    public void getAllParts()
    {
        Debug.Log("Function");
        cShaft = this.transform.Find("").gameObject;

        foreach(Transform child in transform)
        {
            Debug.Log("Adding Child");
            cParts.Add(child.gameObject);
        }
    }
}
