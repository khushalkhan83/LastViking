using Game.Audio;
using UnityEngine;

public class CursorIdentifier : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private CursorID _cursorID;

#pragma warning restore 0649
    #endregion

    public CursorID CursorID => _cursorID;
}
