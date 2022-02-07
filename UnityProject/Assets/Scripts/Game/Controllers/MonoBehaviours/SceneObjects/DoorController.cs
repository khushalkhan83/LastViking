using Game.Audio;
using Game.Models;
using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private float _openAngle = 90;
    [SerializeField] private float _moveDuration = 0.575f;
    [SerializeField] private Transform _door;

#pragma warning restore 0649
    #endregion

    public float OpenAngle => _openAngle;
    public float MoveDuration => _moveDuration;
    public Transform Door => _door;

    public bool IsTargetEnter { get; set; }
    public bool IsClose { get; private set; }
    public Quaternion OpenAngleQuaternion { get; private set; }
    public Quaternion CloseAngleQuaternion { get; private set; }
    public Quaternion OpenAngleInverseQuaternion { get; private set; }
    public AudioSystem AudioSystem => AudioSystem.Instance;

    private Coroutine _rotateDoorProcess;

    private float __angle;

    #region MonoBehaviour
    private void OnEnable()
    {
        CloseAngleQuaternion = Door.localRotation;
        IsClose = transform.localRotation == CloseAngleQuaternion;
        OpenAngleQuaternion = CloseAngleQuaternion * Quaternion.Euler(0, OpenAngle, 0);
        OpenAngleInverseQuaternion = CloseAngleQuaternion * Quaternion.Euler(0, -OpenAngle, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            __angle = Mathf.Abs(Vector3.Angle(transform.forward, other.transform.position - transform.position));

            if (IsClose)
            {
                if (__angle > 0 && __angle < 90)
                {
                    RotateDoor(OpenAngleQuaternion, AudioID.DoorOpen);
                }
                else if (__angle > 90 && __angle < 180)
                {
                    RotateDoor(OpenAngleInverseQuaternion, AudioID.DoorOpen);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            RotateDoor(CloseAngleQuaternion, AudioID.DoorClose);
        }
    }
    #endregion

    private void RotateDoor(Quaternion destination, AudioID audioID)
    {
        AudioSystem.PlayOnce(audioID, transform.position);
        if (_rotateDoorProcess != null)
        {
            StopCoroutine(_rotateDoorProcess);
        }
        _rotateDoorProcess = StartCoroutine(MoveDoorProcess(destination));
    }

    private float __remainingTime;
    private IEnumerator MoveDoorProcess(Quaternion destination)
    {
        __remainingTime = MoveDuration;

        while (__remainingTime > 0)
        {
            Door.localRotation = Quaternion.Slerp(destination, Door.localRotation, __remainingTime / MoveDuration);
            yield return null;
            __remainingTime -= Time.deltaTime;
        }

        Door.localRotation = destination;

        //

        IsClose = CloseAngleQuaternion == transform.localRotation;
    }
}
