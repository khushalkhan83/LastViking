using Game.Models;
using System.Collections;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

public class ManualDoorOpener : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private Transform _doorRoot;
    [SerializeField] private Transform _defenceRoot;
    [SerializeField] private float _openAngle = 90;
    [SerializeField] private float _openSpeed = 300;

#pragma warning restore 0649
    #endregion

    public Transform DoorRoot => _doorRoot;
    public Transform DefenceRoot => _defenceRoot;
    public float OpenAngle => _openAngle;
    public float OpenSpeed => _openSpeed;

    private bool IsInFront(Transform tfm) => DefenceRoot.InverseTransformPoint(tfm.position).z > 0;

    private Quaternion forwardRotation;
    private Quaternion backardRotation;
    private Quaternion initRotation;
    private bool isOpened = false;

    private void OnEnable()
    {
        forwardRotation = Quaternion.Euler(0, _openAngle, 0);
        backardRotation = Quaternion.Euler(0, -_openAngle, 0);
        initRotation = Quaternion.identity;
    }

    public void InteractDoor(Transform opener)
    {
        if (isOpened)
        {
            Close();
        }
        else
        {
            if (IsInFront(opener))
            {
                OpenBackward();
            }
            else
            {
                OpenForward();
            }
        }

        isOpened = !isOpened;
    }

    private void OpenForward() => RotateDoor(forwardRotation);
    private void OpenBackward() => RotateDoor(backardRotation);
    private void Close() => RotateDoor(initRotation);

    private void RotateDoor(Quaternion targetRot)
    {
        StopAllCoroutines();
        StartCoroutine(RotateDoorProcess(targetRot));
    }

    private IEnumerator RotateDoorProcess(Quaternion targetRot)
    {
        while (!Mathf.Approximately(DoorRoot.eulerAngles.y, targetRot.eulerAngles.y))
        {
            DoorRoot.localRotation = Quaternion.RotateTowards(DoorRoot.localRotation, targetRot, OpenSpeed * Time.deltaTime);
            yield return null;
        }
    }
}

