using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CircleGlow : MonoBehaviour
{
	private MeshRenderer mRenderer;
	// Distance at witch glow will be offset from its parent. Used to make glow appear behind the object.
	private const float ParentOffset = 0.1f;

	private void Start()
	{
		
	}

	private void OnWillRenderObject()
	{
		transform.localPosition = Vector3.zero;
		transform.LookAt(Camera.current.transform.position);
		transform.Translate(Vector3.back * ParentOffset);
	}
}
