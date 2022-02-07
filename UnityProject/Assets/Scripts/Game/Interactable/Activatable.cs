using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public abstract class Activatable : MonoBehaviour
    {
        abstract public void OnActivate();
    }
}
