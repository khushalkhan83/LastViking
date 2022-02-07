using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSwithcer : MonoBehaviour
{
    public void Switch(GameObject target) => target.SetActive(!target.activeSelf);
}
