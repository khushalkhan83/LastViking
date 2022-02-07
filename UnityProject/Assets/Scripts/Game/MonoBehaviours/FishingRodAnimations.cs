using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Models;
using UltimateSurvival;

public class FishingRodAnimations : MonoBehaviour
{
    [SerializeField]
    GameObject lineObj, fishObj, floatObj;

    [SerializeField]
    GameObject[] bottleObjs;

    [SerializeField]
    Transform startLinePoint;

    [SerializeField]
    PlayerBehaviour player;

    Animator animator;

    FishingModel _model;
    FishingModel model
    {
        get
        {
            if (_model == null)
            {

            }
            return _model;
        }
    }
    public void SetFishingModel(FishingModel newModel)
    {
        if (model != newModel)
        {
            if (_isSubscribed)
                UnSubscribe();

            _model = newModel;
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
        lineObj.SetActive(true);
        fishObj.SetActive(false);
        SetActiveBottle(false);
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    void SetActiveBottle(bool on)
    {
        foreach (var item in bottleObjs)
        {
            item.SetActive(on);
        }
    }

    bool _isSubscribed = false;

    void Subscribe()
    {
        if (model != null && !_isSubscribed)
        {
            //model.OnStateChange += OnUpdateState;
            model.OnThrowStarted += OnThrowAnimationStart;
            model.OnStartFishing += OnStartFishingHandler;
            model.OnHookResult += OnHookResultHandler;
            model.OnTension += OnTensionHandler;
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
            //model.OnStateChange -= OnUpdateState;
            model.OnThrowStarted -= OnThrowAnimationStart;
            model.OnStartFishing -= OnStartFishingHandler;
            model.OnHookResult -= OnHookResultHandler;
            model.OnTension -= OnTensionHandler;
            //model.OnPull -= OnPullHandler;
            model.OnFisingSuccessful -= OnFishingSuccessful;
            model.OnFishingFailTorn -= OnFishigFailTorn;
            model.OnFishingFailEscape -= OnFishingFailEscape;
            _isSubscribed = false;
        }
    }

    [SerializeField]
    string baseLayerName = "Base", throwStgateName = "Throw", idleFishingName = "Idle_Fishing",pullName="pull",missName="Miss",getFishName="Get_Fish", getBottleName = "Get_Bottle", holsterName = "Holster",moveXName="moveX",moveZName="moveZ";

    int throwHash, idleFishingHash, pullHash, missHash, getFishHash, getBottleHash, holsterHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        string prefix = baseLayerName + ".";
        throwHash = Animator.StringToHash(prefix + throwStgateName);
        idleFishingHash = Animator.StringToHash(prefix + idleFishingName);
        pullHash = Animator.StringToHash(prefix + pullName);
        missHash = Animator.StringToHash(prefix + missName);
        getFishHash = Animator.StringToHash(prefix + getFishName);
        getBottleHash = Animator.StringToHash(prefix + getBottleName);
        holsterHash = Animator.StringToHash(prefix + holsterName);
        //StartCoroutine(UpdateIdleMove());
        wasPoint = watchPoint.position;
    }

    void OnFishigFailTorn()
    {
        PlayMiss();
    }

    void OnFishingFailEscape()
    {
        PlayMiss();
    }

    void OnThrowAnimationStart()
    {
        fishObj.SetActive(false);
        SetActiveBottle(false);
        animator.Play(throwHash);
        StartCoroutine(WaitThrow(model.throwTime));
    }

    void ShowLine()
    {
        model.ThrowShowLine(startLinePoint.position);
        lineObj.SetActive(false);
        fishObj.SetActive(false);
        SetActiveBottle(false);
    }

    IEnumerator WaitThrow(float t)
    {
        yield return new WaitForSeconds(t);
        model.ThrowAnimationEnd();
    }


    void OnHookResultHandler(bool isOK)
    {
        if (isOK)
            animator.CrossFadeInFixedTime(pullHash, 0.5f);
        else
            PlayMiss();
    }

    void OnStartFishingHandler(bool isOk)
    {
        if (isOk)
            animator.CrossFadeInFixedTime(idleFishingHash, 0.3f);
        else
            PlayMiss();
    }

    bool _isOnTension = false;

    void PlayMiss()
    {
        lineObj.SetActive(true);
        animator.Play(missHash);
        wasPoint = watchPoint.position;
    }

    void OnFishingSuccessful()
    {
        lineObj.SetActive(true);
        fishObj.SetActive(model.IsFishGiven);
        SetActiveBottle(!model.IsFishGiven);
        int state = model.IsFishGiven ? getFishHash : getBottleHash;
        animator.CrossFadeInFixedTime(state, 0.5f);
    }

    void ReciveFish()
    {
        model.FishingEnd();
        SetActiveBottle(false);
        wasPoint = watchPoint.position;
    }

    void OnTensionHandler(float val)
    {
        animator.SetFloat("PullPower", val);
    }

    [SerializeField]
    Transform watchPoint,showObj;
    Vector3 wasPoint;

    //[SerializeField]
    Vector3 force, toCenterForce, dumpingForce,  velocity, position;

    [SerializeField]
    float dumpingPow = 0.3f, forcePow = 6f, toCenterPow = 0.6f, veloPow = 100f;

    void Update()
    {
        if (model!=null && model.state == FishingModel.FishingState.none)
        {
            if (!player.Player.IsGrounded.Value)
                animator.Play("Jump");
            //wasPoint = watchPoint.position;
            //while(true)
            //{
            //    yield return null;

            Vector3 nowPoint = watchPoint.position;
            force = (nowPoint - wasPoint) * forcePow;
            wasPoint = nowPoint;

            toCenterForce = -position * toCenterPow;

            dumpingForce = velocity * (-dumpingPow);

            Vector3 addVelocity = force + dumpingForce + toCenterForce;
            addVelocity *= veloPow * Time.deltaTime;

            velocity += addVelocity;

            position += velocity * Time.deltaTime;

            Vector3 global = watchPoint.InverseTransformDirection(position);
            showObj.localPosition = global;

            float gx = Mathf.Abs(global.x);
            float gy = Mathf.Abs(global.y);

            gx = gx / (gx + 0.8f);
            gy = gy / (gy + 0.8f);

            if (global.x > 0)
                gx *= -1f;

            if (global.y < 0)
                gy *= -1f;

            animator.SetFloat(moveXName, gx);
            animator.SetFloat(moveZName, gy);
        }
    }
}
