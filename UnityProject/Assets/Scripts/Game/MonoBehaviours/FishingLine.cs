using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Models;

[RequireComponent(typeof(LineRenderer))]
public class FishingLine : MonoBehaviour
{

    public void SetFishingModel(FishingModel newModel)
    {
        if (model != newModel)
        {
            if (_isSubscribed)
                UnSubscribe();

            model = newModel;
            Subscribe();
        }
    }

    public void UnSetFishingModel()
    {
        UnSubscribe();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
        Hide();
    }

    bool _isSubscribed = false;

    void Subscribe()
    {
        if (model!=null && !_isSubscribed)
        {
            model.OnThrowLineShow += OnThrowStart;
            model.OnStartFishing += OnStartFishingHandler;
            model.OnHookResult += OnHookResultHandler;
            model.OnPull += OnPullHandler;
            model.OnFisingSuccessful += OnFishingSuccessful;
            model.OnFishingFailTorn += OnFishigFailTorn;
            model.OnFishingFailEscape += OnFishingFailEscape;
            _isSubscribed = true;
        }
    }

    void UnSubscribe()
    {
        if (model != null && _isSubscribed)
        {
            model.OnThrowLineShow -= OnThrowStart;
            model.OnStartFishing -= OnStartFishingHandler;
            model.OnHookResult -= OnHookResultHandler;
            model.OnPull -= OnPullHandler;
            model.OnFisingSuccessful -= OnFishingSuccessful;
            model.OnFishingFailTorn -= OnFishigFailTorn;
            model.OnFishingFailEscape -= OnFishingFailEscape;
            _isSubscribed = false;
        }
    }

    LineRenderer line;
    FishingModel model;

    [SerializeField] int anglCount = 15;

    [SerializeField] Transform rodPoint;
    [SerializeField] Transform fishingFloatPoint;

    [SerializeField] Transform tangentRodView;
    [SerializeField] Transform tangentFloatView;
    [SerializeField] AnimationCurve dropCurve;
    [SerializeField, Range(0, 0.9f)] float dropPower = 0.6f;
    [SerializeField] float dropTime = 1.3f;

    Camera worldCamera;
    Camera weaponCamera;

    float tanglePower = 0;

    Vector3 RodPointPosition()
    {
        Vector3 point = rodPoint.position;
        point = weaponCamera.WorldToScreenPoint(point);
        point = worldCamera.ScreenToWorldPoint(point);
        
        return point;
    }

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        if (worldCamera == null)
        {
            PlayerCameras holder = transform.root.GetComponent<PlayerCameras>();
            worldCamera = holder.CameraWorld;
            weaponCamera = holder.CameraTools;
        }
        Hide();
    }

    void OnHookResultHandler(bool isOk)
    {
        if (isOk)
        {
            StopAllCoroutines();
            tanglePower = 0f;
        }
        else
        {
            Hide();
        }
    }

    void OnFishingSuccessful()
    {
        Hide();
    }

    void OnFishigFailTorn()
    {
        Hide();
    }

    void OnFishingFailEscape()
    {
        Hide();
    }

    void OnPullHandler(bool isOk)
    {
        tanglePower = 0;
    }

    void OnThrowStart(Vector3 initPoint)
    {
        Show();
        StartCoroutine(RopeDrop(-dropPower*2f, 0, model.throwTime));
    }

    void OnStartFishingHandler(bool isOk)
    {
        if (isOk)
        {
            Show();
            StartCoroutine(RopeDrop(0,dropPower,dropTime));
        }
        else
        {
            Hide();
        }
    }

    Vector3 wasPodPos = Vector3.zero;
    Vector3 wasFloatPos = Vector3.zero;
    float wasTension = -1f;
    private void Update()
    {
        if (line.enabled)
        {
            bool isNeedUpdate = false;
            if (wasFloatPos !=  fishingFloatPoint.position)
            {
                wasFloatPos = fishingFloatPoint.position;
                isNeedUpdate = true;
            }

            if (wasPodPos != RodPointPosition())
            {
                wasPodPos = RodPointPosition();
                isNeedUpdate = true;
            }

            if (wasTension != tanglePower)
            {
                wasTension = tanglePower;
                isNeedUpdate = true;
            }

            if (isNeedUpdate)
            {
                UpdateParams();
                BuildRope();
            }
        }
    }

    void Show()
    {
        RecalculateRope();
        line.enabled = true;
    }

    void Hide()
    {
        line.enabled = false;
    }
    #region RopeFunc

    IEnumerator RopeDrop(float from,float to,float T)
    {
        float t = 0f;

        while (t < T)
        {
            t += Time.deltaTime;
            float lerpVal = Mathf.Clamp01(t / T);
            lerpVal = dropCurve.Evaluate(lerpVal);
            tanglePower = Mathf.Lerp(from,to, lerpVal);
            yield return null;
        }
    }

    void BuildRope()
    {
        RecalculateRope();
    }

    Vector3 tangentFloat;
    Vector3 tangentRod;
   

    void RecalculateRope()
    {
        if (line != null && _isSubscribed)
        {
            List<Vector3> newLine = new List<Vector3>();
            newLine.Add(RodPointPosition());

            Vector3 A = RodPointPosition();
            Vector3 B = fishingFloatPoint.position;

            float step = 1f / anglCount;
            for (int i = 1;i<=anglCount;i++)
            {
                newLine.Add(GetPointPos(A,tangentRod,tangentFloat,B, i*step));
            }

            newLine.Add(fishingFloatPoint.position);
            line.positionCount = (newLine.Count);
            line.SetPositions(newLine.ToArray());
        }
    }

    float tangentLenght = 0.45f;
    void UpdateParams()
    {
        Vector3 distVect = fishingFloatPoint.position - RodPointPosition();
        float distance = distVect.magnitude;
        if (tanglePower >= 0)
        {
            tangentRod = Vector3.Lerp(distVect, Vector3.down, tanglePower).normalized * (distance * tangentLenght) + RodPointPosition();
            tangentFloat = Vector3.Lerp(-distVect, Vector3.down, tanglePower).normalized * (distance * tangentLenght) + fishingFloatPoint.position;
        }
        else
        {
            tangentRod = Vector3.Lerp(distVect, Vector3.up, -tanglePower).normalized * (distance * tangentLenght) + RodPointPosition();
            tangentFloat = Vector3.Lerp(-distVect, Vector3.up, -tanglePower).normalized * (distance * tangentLenght) + fishingFloatPoint.position;
        }

        tangentFloatView.position = tangentFloat;
        tangentRodView.position = tangentRod;
    }

    Vector3 GetPointPos(Vector3 A,Vector3 tangA,Vector3 tangB,Vector3 B,float t)
    {
        Vector3 A1 = Vector3.Lerp(A, tangA, t);
        Vector3 A2 = Vector3.Lerp(tangA, tangB, t);
        Vector3 A3 = Vector3.Lerp(tangB, B, t);

        Vector3 B1 = Vector3.Lerp(A1, A2, t);
        Vector3 B2 = Vector3.Lerp(A2, A3, t);

        Vector3 result = Vector3.Lerp(B1, B2, t);

        //Debug.DrawRay(result, Vector3.up,Color.black);

        return result;
    }
    #endregion
}
