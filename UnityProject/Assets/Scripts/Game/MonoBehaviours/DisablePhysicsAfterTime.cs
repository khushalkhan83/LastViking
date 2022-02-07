using System.Collections;
using UnityEngine;

public class DisablePhysicsAfterTime : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private float _disableDelay;
    [SerializeField] private Rigidbody _rigidbody;

#pragma warning restore 0649
    #endregion

    public float DisableDelay => _disableDelay;

    public Rigidbody Rigidbody => _rigidbody;

    private void OnEnable()
    {
        StartCoroutine(WaitForDisableGravity(DisableDelay));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator WaitForDisableGravity(float time)
    {
        yield return new WaitForSeconds(time);
        Rigidbody.isKinematic = true;
    }
}
