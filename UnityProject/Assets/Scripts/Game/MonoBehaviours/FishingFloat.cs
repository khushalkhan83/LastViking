using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Models;
using Core;

public class FishingFloat : MonoBehaviour
{
    [SerializeField] Transform visualObject;
    [SerializeField] AnimationCurve biteOn, biteOff;
    [SerializeField] Vector3 divePosition;
    [SerializeField] float animationTimeBite = 0.3f;
    [SerializeField] float animationTimeRelease = 3f;

    [SerializeField] GameObject startFishingEffect,dragEffect;

    Camera worldCamera;
    Camera weaponCamera;

    FishingModel fishingModel;

    public Transform FishHealthPivot => dragEffect.transform;
    
    public void SetFishingModel(FishingModel mm)
    {
        if (fishingModel != mm)
        {
            if (fishingModel != null)
                Describe(false);

            fishingModel = mm;
            if (gameObject.activeInHierarchy)
                Describe(true);
        }
    }

    public void UnSetFishingModel()
    {
        DescribeInternal();
    }

    bool isDescribed = false;

    private void OnEnable()
    {
        startFishingEffect.SetActive(false);
        if (worldCamera == null)
        {
            PlayerCameras holder = transform.root.GetComponent<PlayerCameras>();
            worldCamera = holder.CameraWorld;
            weaponCamera = holder.CameraTools;
        }

        Hide();
        Describe(true);
    }

    private void OnDisable()
    {
        Describe(false);
    }

    void Describe(bool isD)
    {
        if (isD && !isDescribed && fishingModel != null)
        {
            SubscribeInternal();
        }

        if (!isD && isDescribed && fishingModel != null)
        {
            DescribeInternal();
        }
    }

    void SubscribeInternal()
    {
        fishingModel.OnThrowLineShow += ThrowShow;
        fishingModel.OnBiting += ShowBiting;
        fishingModel.OnStartFishing += StartFishing;
        fishingModel.OnHookResult += OnHookResult;
    }
    void DescribeInternal()
    {
        fishingModel.OnThrowLineShow -= ThrowShow;
        fishingModel.OnBiting -= ShowBiting;
        fishingModel.OnStartFishing -= StartFishing;
        fishingModel.OnHookResult -= OnHookResult;
    }


    void ThrowShow(Vector3 initPoint)
    {
        Show();
        StartCoroutine(MovePosition(initPoint));
    }

    Vector3 FishingPoint()
    {
        Vector3 point = fishingModel.fishingPoint;

        return point;
        point = worldCamera.WorldToScreenPoint(point);
        point = weaponCamera.ScreenToWorldPoint(point);
        return point;
    }

    IEnumerator MovePosition(Vector3 pointFrom)
    {
        Vector3 pointTo = FishingPoint();
        float distance = Vector3.Distance(pointFrom, pointTo);
        Vector3 tangent = (pointFrom + pointTo) / 2f + Vector3.up * distance / 3f;
        float t = 0f;
        float T = 0.9f;

        while(t<=T)
        {
            t += Time.deltaTime;
            float lerpVal = Mathf.Clamp01(t / T);
            visualObject.position = NURB(pointFrom, tangent, pointTo, lerpVal);
            yield return null;
        }
    }

    Vector3 NURB(Vector3 from, Vector3 mid,Vector3 to,float t)
    {
        Vector3 A = Vector3.Lerp(from, mid, t);
        Vector3 B = Vector3.Lerp(mid, to, t);
        Vector3 retVal = Vector3.Lerp(A, B, t);
        return retVal;
    }

    void StartFishing(bool isOk)
    {
        if (isOk)
        {
            Show();
            startFishingEffect.SetActive(false);
            startFishingEffect.SetActive(true);
            visualObject.localPosition = divePosition;
            ShowBiting(false);
        }
        else
            Hide();
    }

    void ShowBiting(bool isBiting)
    {
        if (isBiting)
        {  
            StartCoroutine(AnimrBite(visualObject.localPosition, divePosition, biteOn, animationTimeBite));
        }
        else
        {
            StartCoroutine(AnimrBite(visualObject.localPosition, Vector3.zero, biteOff, animationTimeRelease));
        }
    }

    IEnumerator AnimrBite(Vector3 from,Vector3 to,AnimationCurve curve,float time)
    {
        float t = 0;
        while(t<time && fishingModel.state == FishingModel.FishingState.biting)
        {
            t += Time.deltaTime;
            float lerpVal = Mathf.Clamp01(t / time);
            lerpVal = curve.Evaluate(lerpVal);
            visualObject.localPosition = Vector3.LerpUnclamped(from, to, lerpVal);
            yield return null;
        }
    }

    IEnumerator PullMoveAnimation()
    {
        yield return null;
        dragEffect.SetActive(true);
        while (fishingModel.state == FishingModel.FishingState.pulling)
        {
            Vector3 newPos = fishingModel.fishingPoint;
            newPos.x += Random.value * 2f - 1f;
            newPos.z += Random.value * 2f - 1f;
            float T = 0.5f + Random.value;
            yield return StartCoroutine(MoveToPoint(newPos,T));
        }
        dragEffect.SetActive(false);
    }

    IEnumerator MoveToPoint(Vector3 point,float T)
    {
        float t = 0;
        Vector3 initPoint = visualObject.position;
        while(t<=T && fishingModel.state == FishingModel.FishingState.pulling)
        {
            t += Time.deltaTime;
            float lerpVal = Mathf.Clamp01(t / T);
            Vector3 newPos = Vector3.Lerp(initPoint, point, lerpVal);
            visualObject.position = newPos;
            dragEffect.transform.position = newPos;
            yield return null;
        }
    }

    void OnHookResult(bool isOk)
    {
        Hide();
        if (isOk)
        {
            StartCoroutine(PullMoveAnimation());
        }
    }

    void Show()
    {
        transform.position = FishingPoint();
        visualObject.localPosition = Vector3.zero;
        visualObject.gameObject.SetActive(true);
    }

    void Hide()
    {
        StopAllCoroutines();
        visualObject.localPosition = Vector3.zero;
        visualObject.gameObject.SetActive(false);
    }
}
