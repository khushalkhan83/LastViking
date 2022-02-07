using UnityEngine;

public class FollowToTarget : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private Transform _target;

#pragma warning restore 0649
    #endregion

    public Transform Target => _target;

    void LateUpdate()
    {
        transform.position = Target.position;
    }
}
