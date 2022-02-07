namespace Coin
{
	using System;
	using UnityEngine;
	using Random = UnityEngine.Random;

	[Serializable]
	public class CoinBodyAnimator
	{
		private const AnimationState DefaultAnimationState = AnimationState.Floating;

		[SerializeField]
		private GameObject AnimationTargetGO;

		[SerializeField]
		private float SpawnAnimationDuration;

		[SerializeField]
		private float CollectAnimationDuration;

		[SerializeField]
		private float AngularDrag;

		[SerializeField]
		private AnimationCurve SpawnAnimationY;

		[SerializeField]
		private AnimationCurve CollectAnimationLerp;

		[SerializeField]
		private float CollectionPointYAddition;

		private AnimationState mAnimationState;					
		private float mRandomAnimationOffset;
		private float mStateTime;
		private Vector3 mSpawnAnimationPosition;
		private Vector3 mAttachedPosition;
		private float mAngularInertia = 1;
		private Func<bool> mOnAnimationCompleteCallback;
		private Transform mCollectAnimationCollectorTarget;
		private Vector3 mCollectAnimationInitialPosition;

		private enum AnimationState
		{
			None = 0,
			Spawning,
			Floating,
			Collected,
		}

		private AnimationState animationState
		{
			get
			{
				return mAnimationState;
			}
			set
			{
				if (mAnimationState == value)
					return;

				mAnimationState = value;

				mStateTime = 0;

				// Trigger callback.
				mOnAnimationCompleteCallback?.Invoke();
				mOnAnimationCompleteCallback = null;
			}
		}

		public void Initialize(Vector3 iPosition)
		{
			mRandomAnimationOffset = Random.Range(0, 1f);
			mAttachedPosition = iPosition;
			mAngularInertia = 0;

			mOnAnimationCompleteCallback = null;
			animationState = DefaultAnimationState;
		}

		public void Update(float iDeltaTime)
		{
			mStateTime += iDeltaTime;

			UpdateSpin(iDeltaTime);

			bool _completed = false;

			switch (animationState)
			{
				case AnimationState.Spawning:
					UpdateSpawnAnimation(out _completed);
					break;

				case AnimationState.Floating:
					UpdateFloatingAnimation(iDeltaTime);
					break;

				case AnimationState.Collected:
					UpdateCollectAnimation(out _completed);
					break;
			}

			if (_completed == true || animationState == AnimationState.None)
			{
				animationState = DefaultAnimationState;
			}
		}

		public void AnimateSpawn(Vector3 iAnimateFromPosition, Func<bool> iOnAnimationCompleteCallback)
		{
			mSpawnAnimationPosition = iAnimateFromPosition;

			mAngularInertia += 10;

			animationState = AnimationState.Spawning;

			// Note: Must be set after animation switch.
			mOnAnimationCompleteCallback = iOnAnimationCompleteCallback;
		}

		public void AnimateCollect(Transform iTarget, Func<bool> iOnAnimationCompleteCallback)
		{
			mCollectAnimationCollectorTarget = iTarget;
			mCollectAnimationInitialPosition = AnimationTargetGO.transform.position;

			animationState = AnimationState.Collected;

			// Note: Must be set after animation switch.
			mOnAnimationCompleteCallback = iOnAnimationCompleteCallback;
        }

		private void UpdateSpin(float iDeltaTime)
		{
			mAngularInertia = Mathf.MoveTowards(mAngularInertia, 1, iDeltaTime * AngularDrag);

			AnimationTargetGO.transform.eulerAngles += new Vector3(0, (Time.deltaTime * 100 * mAngularInertia) + (mRandomAnimationOffset), 0);
		}

		private void UpdateFloatingAnimation(float iDeltaTime)
		{
			float _floatAnimation = Mathf.Sin((Time.time + mRandomAnimationOffset) * 3) * 0.04f;

			// Smooth position, to help with transition from different animation states.
			float _currentYPosition = AnimationTargetGO.transform.localPosition.y;
			float _newYPosition = Mathf.MoveTowards(_currentYPosition, _floatAnimation, iDeltaTime * 0.1f); 

			AnimationTargetGO.transform.localPosition = Vector3.up * _newYPosition;
		}

		private void UpdateSpawnAnimation(out bool oCompleted)
		{
			float _animationTime = mStateTime / (SpawnAnimationDuration + mRandomAnimationOffset * 0.3f);

			// Animate object movement from spawned position to actual coin position.
			var _baseMovement = Vector3.Lerp(mSpawnAnimationPosition, mAttachedPosition, _animationTime);

			_baseMovement += Vector3.up * SpawnAnimationY.Evaluate(_animationTime) * (1 + mRandomAnimationOffset * 0.3f);

			AnimationTargetGO.transform.position = _baseMovement;

			oCompleted = _animationTime >= 1;
		}

		private void UpdateCollectAnimation(out bool oCompleted)
		{
			if (mCollectAnimationCollectorTarget == null)
			{
				UnityEngine.Debug.LogError("CoinBodyAnimator: mCollectAnimationTarget is missing during collect animation. This should not happen.");

				// Force animation completion.
				oCompleted = true;

				return;
			}

			float _animationTime = mStateTime / (CollectAnimationDuration + (mRandomAnimationOffset * 0.1f));

			var _baseMovement = Vector3.Lerp(mCollectAnimationInitialPosition, mCollectAnimationCollectorTarget.position + (Vector3.up * CollectionPointYAddition), CollectAnimationLerp.Evaluate(_animationTime));

			AnimationTargetGO.transform.position = _baseMovement;

			oCompleted = _animationTime >= 1;
		}
	}
}