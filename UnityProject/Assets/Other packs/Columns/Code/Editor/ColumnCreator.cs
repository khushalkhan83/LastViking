using UnityEngine;
using System.Collections;
using UnityEditor;

public class ColumnCreator : MonoBehaviour
{
    [Header("Default Column Properties")]
    public GameObject defaultShaft;
    public GameObject defaultBase;
    public GameObject defaultCap;
    public GameObject defaultNeck;
    [Range(0.0f, 1.0f)]
    public float neckPercentage;

    //Settings for Column
    [Header("Column Settings")]
    public float cHeightInFeet = 10.0f;
    [Range(-1.0f, 1.0f)]
    public float cBaseSize = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float cCapSize = 0.0f;

    //Declare Cap and Base locations
    [HideInInspector]
    public Transform shaftBottom;
    [HideInInspector]
    public Transform shaftTop;

    [Header("Create Column")]
    public int numberOfColumns;

    public void SpawnDefaultColumn()
    {
        //Instantiate();
        //Debug.Log("Column Spawned");

        //Parent Under Master Node
        GameObject currentColumnParent = new GameObject();
        currentColumnParent.name = "Column_Master";


        if (defaultShaft != null)
        {
            //Spawn Default Objects
            GameObject currentColumn = (GameObject)Instantiate(defaultShaft, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            //Rename Cloned objects
            currentColumn.name = defaultShaft.name;
            currentColumn.transform.parent = currentColumnParent.transform;
            shaftBottom = currentColumn.transform.Find("Master/Bottom");
            shaftTop = currentColumn.transform.Find("Master/Top");

            //Set Values for taper
            Vector3 cBaseScale = shaftBottom.transform.localScale;
            Vector3 cCapScale = shaftBottom.transform.localScale;
            shaftBottom.transform.localScale = new Vector3(cBaseScale.x + cBaseSize, cBaseScale.y + cBaseSize, cBaseScale.z + cBaseSize);
            shaftTop.transform.localScale = new Vector3(cCapScale.x + cCapSize, cCapScale.y + cCapSize, cCapScale.z + cCapSize);
            shaftTop.transform.localPosition = new Vector3(0.0f, cHeightInFeet, 0.0f);
        }
        else
        {
            Debug.LogWarning("You require a shaft.");
        }

        //Base
        if (defaultBase != null)
        {
            GameObject currentBase = (GameObject)Instantiate(defaultBase, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            currentBase.name = "Base";//defaultBase.name;
            currentBase.transform.parent = shaftBottom;
            currentBase.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            currentBase.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        }

        //Cap
        if (defaultCap != null)
        {
            GameObject currentCap = (GameObject)Instantiate(defaultCap, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            currentCap.name = "Cap";//defaultCap.name;
            currentCap.transform.parent = shaftTop;
            //Reset Transform to Zero and add orientation rotation
            currentCap.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
            currentCap.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }


        //Neck
        if (defaultNeck != null)
        {
            GameObject currentNeck = (GameObject)Instantiate(defaultNeck, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            currentNeck.name = "Neck";
            currentNeck.transform.parent = currentColumnParent.transform;
            //Get 75% of location on column
            float neckLocation = (shaftBottom.transform.localPosition.y + shaftTop.transform.localPosition.y) * neckPercentage;
            currentNeck.transform.localPosition = new Vector3(0.0f, neckLocation, 0.0f);

            //Get average of cap and base Scale
            Vector3 cBS = shaftBottom.transform.localScale;
            Vector3 cCS = shaftTop.transform.localScale;
            Vector3 cNAverage = (cBS + cCS) / 2;
            currentNeck.transform.localScale = cNAverage;
        }

        //Add Column Editor Script
        currentColumnParent.AddComponent<ColumnEditor>();

        //Select Column
        Selection.activeGameObject = currentColumnParent;
    }
}
