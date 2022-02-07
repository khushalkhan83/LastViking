using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FracturePieceBehaviour : MonoBehaviour
{
	private const float SleepTimeRendomization = 1f;

	public event Action<bool> BrokenStateChange;

	[SerializeField]
	private float mMaxSimulationDuration = 3f;

	[SerializeField]
	private float mDisappearanceDuration = 4f;

	[SerializeField]
	private DisappearanceBehaviour mDisappearanceBehaviour;

	[SerializeField]
	private float mPushForce;

	[SerializeField]
	private float mRandomAngularForce;

	[SerializeField]
	private Material mFadeMaterialVariant;

	// Initialization assumed to happen once when object structure is assembled in editor.
	[SerializeField]
	private bool mInitialized;

	[SerializeField]
	private MeshCollider mCollider;

	[SerializeField]
	private Rigidbody mRigidbody;

	[SerializeField]
	private Vector3 mInitialScale;

	[SerializeField]
	private Vector3 mInitialRotation;

	[SerializeField]
	private Vector3 mInitialPosition;

	[SerializeField]
	private Material mInitialMaterial;

    [SerializeField]
    private MeshRenderer mRenderer;

	private State mState;
	private float mStateTime;
	private Vector3 mStateStartPosition;
	private bool mMaterialFadeMode;
	private float mSleepDuration;

	public enum State
	{
		None = 0,
		Undisturbed,
		BrokenSimulating,
		BrokenSleeping,
		BrokenDisappearing,
		Hidden,
	}

	public enum DisappearanceBehaviour
	{
		ScaleOut = 0,
		Fade,
	}

	public enum BreakBehaviour
	{
		None,
		Push,
	}

	public State state
	{
		get
		{
			return mState;
		}

		set
		{
			if (mState == value)
				return;

			var _lastState = mState;

			mState = value;

			OnStateChange(value, _lastState);

			mStateTime = 0;
			mStateStartPosition = transform.localPosition;//change
		}
	}

	private bool materialFadeMode
	{
		set
		{
			if (mMaterialFadeMode == value)
				return;

			mMaterialFadeMode = value;

			if (value == true && mFadeMaterialVariant == null)
			{
				UnityEngine.Debug.LogError("FracturePieceBehaviour: Fade behaviour require mFadeMaterialVariant to be assigned.");
				return;
			}

			mRenderer.material = value ? mFadeMaterialVariant : mInitialMaterial;

			if (value == true)
			{
				mFadeMaterialVariant.SetColor("_Color", Color.white);
			}
		}
	}

	public void Initialize()
	{
		// Get and Setup Members.
		// Mesh.
		var _meshFilter = gameObject.GetComponent<MeshFilter>();

		if (_meshFilter == null || _meshFilter.sharedMesh == null)
		{
			UnityEngine.Debug.LogError("FracturePieceBehaviour: Unable to locate mesh.");
			return;
		}

		var _mesh = _meshFilter.sharedMesh;

		// Collider.
		mCollider = gameObject.GetComponent<MeshCollider>();

		if (mCollider == null)
		{
			mCollider = gameObject.AddComponent<MeshCollider>();
		}

		mCollider.convex = true;
		mCollider.sharedMesh = _mesh;

		// Rigidbody.
		mRigidbody = gameObject.GetComponent<Rigidbody>();

		if (mRigidbody == null)
		{
			mRigidbody = gameObject.AddComponent<Rigidbody>();
		}

		mRigidbody.maxDepenetrationVelocity = 2.01f;

		// Renderer.
		mRenderer = gameObject.GetComponent<MeshRenderer>();

		// Various.
		mInitialScale = transform.localScale;
		mInitialRotation = transform.localEulerAngles;
        mInitialPosition = transform.localPosition;

		mInitialMaterial = mRenderer.sharedMaterial;

		mInitialized = true;
	}

	public bool Raycast(Ray iRay, out RaycastHit oRaycastHit)
	{
		oRaycastHit = default(RaycastHit);

		if (state != State.Undisturbed)
			return false;

		return mCollider.Raycast(iRay, out oRaycastHit, 100);
	}

	public void Break(Ray iRay)
	{	
		state = FracturePieceBehaviour.State.BrokenSimulating;

		if (Mathf.Abs(mPushForce) > 0)
		{
			mRigidbody.AddForce(iRay.direction * mPushForce, ForceMode.Impulse);
		}

		if (Mathf.Abs(mRandomAngularForce) > 0)
		{
			mRigidbody.AddTorque(
				new Vector3(
					UnityEngine.Random.Range(-1f, 1f),
					UnityEngine.Random.Range(-1f, 1f),
					UnityEngine.Random.Range(-1f, 1f)) * mRandomAngularForce,
				ForceMode.Impulse);
		}
	}

	public void Reset()
	{
		state = State.Undisturbed;

		mRigidbody.maxDepenetrationVelocity = 0.01f;
		mRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
	}
	
	private void Update()
	{
		mStateTime += Time.unscaledDeltaTime;

		if (state == State.BrokenSimulating)
		{
			if (mStateTime >= mMaxSimulationDuration || mRigidbody.IsSleeping())
			{
				state = State.BrokenSleeping;
			}
		}
		else if (state == State.BrokenSleeping)
		{
			if (mStateTime >= mSleepDuration)
			{
				state = State.BrokenDisappearing;
			}
		}
		else if (state == State.BrokenDisappearing)
		{
			float _disappearingProgress = mStateTime / mDisappearanceDuration;

			AnimateDisappearing(_disappearingProgress);

			if (mStateTime >= mDisappearanceDuration)
			{
				state = State.Hidden;
			}
		} 
	}

	private void OnStateChange(State iState, State iLastState)
	{
		// Note: generally assumed not happen during play.
		if (mInitialized == false)
			Initialize();

        mRenderer.enabled = iState != State.Hidden;

		// Rigidbody Control.
		mRigidbody.detectCollisions = iState == State.BrokenSimulating;
		mRigidbody.useGravity = iState == State.BrokenSimulating;
		mRigidbody.velocity = Vector3.zero;
		mRigidbody.angularVelocity = Vector3.zero;

		// Transform.
		transform.localScale = mInitialScale;

		if (iState == State.Hidden || iState == State.Undisturbed)
		{
			transform.localPosition = mInitialPosition;
			transform.localEulerAngles = mInitialRotation;
		}

		// Notify broken state change.
		var _nowBroken = iState != State.None && iState != State.Undisturbed;
		var _wasBroken = iLastState != State.None && iLastState != State.Undisturbed;

		if (_nowBroken != _wasBroken)
		{
			BrokenStateChange?.Invoke(_nowBroken);
		}

		// Note: Randomize Disappearance animation start for better visual effect.
		if (iState == State.BrokenSleeping)
			mSleepDuration = UnityEngine.Random.Range(0, SleepTimeRendomization);

		// Change material rendering mode if it is required by Disappearance Behaviour.
		materialFadeMode = state == State.BrokenDisappearing &&
		                   mDisappearanceBehaviour == DisappearanceBehaviour.Fade;
	}

	private void AnimateDisappearing(float iTime)
	{
		float _animatedValue = iTime * iTime;

		if (mDisappearanceBehaviour == DisappearanceBehaviour.ScaleOut)
		{
			transform.localScale = Vector3.Lerp(mInitialScale, mInitialScale * 0.1f, _animatedValue);

			var _positionToLerp = new Vector3(mStateStartPosition.x, transform.parent.position.y - 1f, mStateStartPosition.z);
			transform.position = Vector3.Lerp(mStateStartPosition, _positionToLerp, _animatedValue);
		}
		else if (mDisappearanceBehaviour == DisappearanceBehaviour.Fade)
		{
			mRenderer.material.SetColor("_Color", new Color(1,1,1, 1 - _animatedValue));
		}
	}
}