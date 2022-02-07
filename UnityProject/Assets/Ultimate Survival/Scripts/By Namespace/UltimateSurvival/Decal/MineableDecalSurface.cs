using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class MineableDecalSurface : MonoBehaviour
{
	[SerializeField]
	private MinebleFractureObject mMinebleObject;

	[SerializeField]
	private GameObject[] mDecalPrefabs;

	[SerializeField]
	private float mDecalScale = 1f;

	private Collider mCollider;
	private List<Decal> mDecalList = new List<Decal>();

	private void OnEnable()
	{
		if (mMinebleObject == null)
		{
			UnityEngine.Debug.LogError("MineableDecalSurface: No MinableObject assigned!");
		}

		mCollider = gameObject.GetComponent<Collider>();

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

	private void OnMinableHit(RaycastHit iObj, Ray iRay)
	{
		CastDecal(iRay);
	}

	private void OnMinableDepleted(RaycastHit iObj, Ray iRay)
	{
		foreach (var _decal in mDecalList)
		{
			_decal.markForForcedFinish = true;
		}
	}

	private void CastDecal(Ray iRay)
	{
		if (mDecalPrefabs == null)
		{
			UnityEngine.Debug.LogError("MineableDecalSurface: Can't cast decal, no prefabs specified.");
			return;
		}

		RaycastHit _hitInfo;
		if (mCollider.Raycast(iRay, out _hitInfo, 100))
		{
			var _prefab = mDecalPrefabs[Random.Range(0, mDecalPrefabs.Length)];

			var _decal = DecalProvider.Get(_prefab, gameObject);

			_decal.Finished += OnDecalFinished;
			mDecalList.Add(_decal);

			_decal.Project(_hitInfo, mCollider, mDecalScale);
		}
	}

	private void OnDecalFinished(Decal iDecal)
	{
		mDecalList.Remove(iDecal);
	}
}