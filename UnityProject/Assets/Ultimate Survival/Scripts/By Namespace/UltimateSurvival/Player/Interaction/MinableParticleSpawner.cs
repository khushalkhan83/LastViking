using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinableParticleSpawner : MonoBehaviour
{
	[Header("Hit Particle Setup")]
	[SerializeField]
	private ParticleSystem mOnHitPrefab;

	[SerializeField]
	private Vector3 mOnHitLocalPosition;

	[SerializeField]
	[Range(0,100)]
	private float mOnHitChance;

	[Header("Depleted Particle Setup")]
	[SerializeField]
	private ParticleSystem mOnDepletionPrefab;

	[SerializeField]
	private Vector3 mOnDepletedLocalPosition;

	[SerializeField]
	[HideInInspector]
	private MinebleFractureObject mMinebleObject;

	private void OnEnable()
	{
		if (mMinebleObject == null)
			mMinebleObject = gameObject.GetComponent<MinebleFractureObject>();

		if (mMinebleObject != null)
		{
			mMinebleObject.Hit += OnMinableHit;
			mMinebleObject.Depleted += OnMinableDepleted;
		}
	}

	private void OnDisable()
	{
		if (mMinebleObject != null)
		{
			mMinebleObject.Hit -= OnMinableHit;
			mMinebleObject.Depleted -= OnMinableDepleted;	
		}
	}

	private void OnMinableDepleted(RaycastHit iRaycastHit, Ray iRay)
	{
		if (mOnDepletionPrefab == null)
			return;

		var _particleInstance = GameObject.Instantiate(mOnDepletionPrefab);

		_particleInstance.transform.parent = mMinebleObject.transform.parent;
		_particleInstance.transform.localPosition = mOnDepletedLocalPosition;
	}

	private void OnMinableHit(RaycastHit iRaycastHit, Ray iRay)
	{
		if (mOnHitPrefab == null)
			return;

		bool _randomCheck = mOnHitChance >= Random.Range(0f, 100f);

		if (_randomCheck == false)
			return;

		var _particleInstance = GameObject.Instantiate(mOnHitPrefab);

		_particleInstance.transform.parent = mMinebleObject.transform.parent;
		_particleInstance.transform.localPosition = mOnHitLocalPosition;
	}
}