using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingZoneJumper : MonoBehaviour
{

    [SerializeField]
    FishingZone zone;

    [SerializeField]
    AnimationCurve jumpHeightCurve;
    [SerializeField]
    Animator jumpAnimator;
    [SerializeField]
    AnimationClip jumpAnim;

    [SerializeField]
    Transform fishRoot;
    // Start is called before the first frame update

    private void OnEnable()
    {
        zone.onSetFishVisible += FishVisible;
        fishRoot.gameObject.SetActive(false);
        FishVisible(zone.isFishHere);
    }

    private void OnDisable()
    {
        zone.onSetFishVisible -= FishVisible;
    }

    void FishVisible(bool isVisible)
    {
        isFishTime = isVisible;
        if (isFishTime && jumpRoutine == null)
            jumpRoutine = StartCoroutine(LiveJumps());
    }

    bool isFishTime;
    Coroutine jumpRoutine;

    IEnumerator LiveJumps()
    {
        while (isFishTime)
        {
            yield return new WaitForSeconds(Random.value);
            Vector3 jumpPos = transform.position + Quaternion.Euler(0,Random.value * 360,0) * Vector3.forward;
            float height = 0.3f + Random.value;
            yield return StartCoroutine(JumpAnimation(jumpPos, height));
        }
        jumpRoutine = null;
    }

    IEnumerator JumpAnimation(Vector3 startPos,float height)
    {
        fishRoot.rotation = Quaternion.Euler(0, Random.value * 360, 0);
        fishRoot.position = startPos;
        fishRoot.gameObject.SetActive(true);        
        float t = 0f;
        float T = 1f;
        jumpAnimator.speed = jumpAnim.length/T;
        while (t<=T)
        {
            t += Time.deltaTime;
            float lerpVal = Mathf.Clamp01(t / T);
            // (fishRoot.gameObject, lerpVal * jumpAnim.length);
            Vector3 dPos = startPos + Vector3.up * jumpHeightCurve.Evaluate(lerpVal);
            fishRoot.position = dPos;
            yield return null;
        }

        fishRoot.gameObject.SetActive(false);
    }
}
