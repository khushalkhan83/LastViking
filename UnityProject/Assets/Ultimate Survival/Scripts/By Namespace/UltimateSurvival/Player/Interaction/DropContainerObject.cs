using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coin;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.AI;

namespace UltimateSurvival
{
	public class DropContainerObject : MonoBehaviour
	{
		[Tooltip("Defines layers that will be used in raycast to determine container position.")]
		[SerializeField] private LayerMask SampleLayerMask;
		[SerializeField] private CoinBodyAnimator Animator;
		[SerializeField] private WorldObjectModel worldObjectModel;
		[SerializeField] private LootObject lootObject;
		[SerializeField] private float objectRadius = 0.125f;

		private Vector3 boxPosition;

		/// <summary>
		/// Represents actual coin position, which is always static regardless of body animation.
		/// </summary>
		public Vector3 containerPosition
		{
			get
			{
				return boxPosition;
			}

			private set
			{
				boxPosition = value;

				transform.position = value;
			}
		}

		private void OnEnable() 
		{
			lootObject.OnClose += OnCloseHandler;
		}

		private void OnDisable() 
		{
			lootObject.OnClose -= OnCloseHandler;
		}

		public void Place(Vector3 iAnimateFromPosition, float randomizePosition)
		{
			var positionRandom = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * randomizePosition;
			var destinationPosition = iAnimateFromPosition + positionRandom;

			GetBoundedDestination(iAnimateFromPosition,destinationPosition,SampleLayerMask, out Vector3 boundedDestination);
			destinationPosition = boundedDestination;

			containerPosition = new Vector3(destinationPosition.x, destinationPosition.y + 1f, destinationPosition.z);
			worldObjectModel.SetPosition(containerPosition);

			Animator.Initialize(containerPosition);
			Animator.AnimateSpawn(iAnimateFromPosition, null);

			Update();
		}

		private bool GetBoundedDestination(Vector3 position, Vector3 destionation, LayerMask layerMask, out Vector3 boundedDestination)
		{
			var direction = destionation - position;
			var ray = new Ray(position, direction);

			if (Physics.Raycast(ray, out var hitInfo, direction.magnitude, layerMask))
			{
				boundedDestination = new Vector3(GetFixedDestination(position.x,hitInfo.point.x),
												hitInfo.point.y,
												GetFixedDestination(position.z,hitInfo.point.z));
				return true;
			}

			boundedDestination = destionation;
			return false;

			float GetFixedDestination(float positionValue, float destinationValue)
			{
				bool decreaseValue = destinationValue > positionValue;

				if(decreaseValue) 
					destinationValue -= objectRadius;
				else 
					destinationValue += objectRadius;
				
				return destinationValue;
			}
		}

		private void Update()
		{
			Animator.Update(Time.smoothDeltaTime);
		}

		private void OnCloseHandler()
		{
			if (lootObject.IsEmpty)
			{
				worldObjectModel.Delete();
				Destroy(this.gameObject);
			}
		}
	}
}
