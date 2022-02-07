using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Decal : MonoBehaviour
{
	// Randomization Parameters.
	private const float PositionRange = 0.1f;
	private const float ScaleRange = 1.4f;
	private const float RotationRange = 10f;

	private const float MaxAlpha = 0.7f;
	private const float DepthOffset = 0.005f;
	private const float Duration = 6f;

	private static AnimationCurve AlphaCurve = new AnimationCurve(new Keyframe(0, MaxAlpha), new Keyframe(0.85f, MaxAlpha), new Keyframe(1, 0));

	[SerializeField]
	[HideInInspector]
	private bool mInitialized;

	private MeshRenderer mMeshRenderer;
	private MeshFilter mMeshFilter;
	private Mesh mMesh;
	private Vector3[] mOriginalVertices;
	private Vector3[] mOriginalNormals;
	private Vector3[] mBufferVertices;
	private Color[] mBufferColors;
	private bool mActive;
	private float mActiveTime;
	private float mAlpha;
	private Vector3 mOriginalScale;
	private int[] mOriginalTriangles;

	public bool markForForcedFinish { get; set; }

	public event Action<Decal> Finished;

	public GameObject associatedPrefab { get; private set; }

	private bool active
	{
		get
		{
			return mActive;
		}

		set
		{
			if (mActive == value)
				return;

			mActive = value;

			gameObject.SetActive(value);

			if (mActive == true)
			{
				mActiveTime = 0f;
				alpha = 1;
				markForForcedFinish = false;
			}
		}
	}

	private float alpha
	{
		get
		{
			return mAlpha;
		}

		set
		{
			if (mAlpha == value)
				return;

			mAlpha = value;

			mMeshRenderer.material.SetFloat("_Alpha", value);
		}
	}

	public static Decal Build(GameObject iPrefab)
	{
		var _instance = GameObject.Instantiate(iPrefab);
		var _decal = _instance.GetComponent<Decal>();

		_decal.associatedPrefab = iPrefab;

		return _decal;
	}

	public void Project(RaycastHit iHitInfo, Collider iCollider, float iDecalScale)
	{
		if (mInitialized == false)
		{
			Start();
		}

		// Position Object.
		transform.position = iHitInfo.point + (iHitInfo.normal * DepthOffset);
		transform.rotation = Quaternion.LookRotation(iHitInfo.normal);

		// Randomize.
		transform.Translate(new Vector3(Random.Range(-PositionRange, PositionRange), Random.Range(-PositionRange, PositionRange), 0));
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Random.Range(-RotationRange, RotationRange));
		transform.localScale = mOriginalScale * Random.Range(1f, ScaleRange) * iDecalScale;

		// Morph Mesh.
		MorphMesh(iCollider);

		active = true;
	}

	public void OnReturnToPool(GameObject iPoolRootGo)
	{
		transform.parent = iPoolRootGo.transform;
	}

	public void OnCalledFromPool(GameObject iCallerGo)
	{
		transform.parent = iCallerGo.transform;
	}

	private void Start()
	{
		if (mInitialized == false)
		{
			mMeshFilter = gameObject.GetComponent<MeshFilter>();
			mMeshRenderer = gameObject.GetComponent<MeshRenderer>();

			mOriginalScale = transform.localScale;

			mMesh = mMeshFilter.mesh;
			mOriginalVertices = mMesh.vertices;
			mOriginalNormals = mMesh.normals;
			mOriginalTriangles = mMesh.triangles;

			// Note: For some reason there is a possibility fort normals to be missing on mobile devices. In this case - populate normals with default direction.
			if (mOriginalNormals.Length != mOriginalVertices.Length)
			{
				mMesh.RecalculateNormals();
				mOriginalNormals = mMesh.normals;
			}

			mBufferVertices = new Vector3[mOriginalVertices.Length];
			mBufferColors = new Color[mOriginalVertices.Length];
			mInitialized = true;
		}
	}

	private void Update()
	{
		if (active)
		{
			mActiveTime += Time.deltaTime;

			float _progression = mActiveTime / Duration;

			alpha = AlphaCurve.Evaluate(_progression);

			if (_progression >= 1)
			{
				OnFinish();
			}

			if (markForForcedFinish)
			{
				OnFinish();
			}
		}
	}

	private void OnFinish()
	{
		active = false;

		DecalProvider.Return(this);

		Finished?.Invoke(this);

		markForForcedFinish = false;
	}

	private void MorphMesh(Collider iCollider)
	{
		HashSet<int> _missedIndex = new HashSet<int>();

		for (int _index = 0; _index < mOriginalVertices.Length; _index++)
		{
			var _transformedPosition = transform.TransformVector(mOriginalVertices[_index]);
			var _transformedNormal = transform.TransformDirection(mOriginalNormals[_index]);
			var _ray = new Ray(_transformedPosition + transform.position + (_transformedNormal * 0.5f), -_transformedNormal);

			RaycastHit _hitInfo;
			if (iCollider.Raycast(_ray, out _hitInfo, 1))
			{
				var _inversePosition = transform.InverseTransformVector(_hitInfo.point - transform.position);
				var _inverseNormal = transform.InverseTransformDirection(_hitInfo.normal);

				mBufferVertices[_index] = _inversePosition + (_inverseNormal * DepthOffset);
				mBufferColors[_index] = Color.white;
			}
			else
			{
				_missedIndex.Add(_index);

				mBufferVertices[_index] = Vector3.zero;
				mBufferColors[_index] = Color.black;
			}
		}

		mMesh.vertices = mBufferVertices;
		mMesh.colors = mBufferColors;

		mMesh.RecalculateBounds();
	}
}