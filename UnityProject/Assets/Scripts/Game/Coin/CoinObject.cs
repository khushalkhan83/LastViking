using UnityEngine;
using UnityEngine.AI;
using System.Linq;

namespace Coin
{
	public class CoinObject : MonoBehaviour
	{
		
		[Tooltip("Defines layers that will be used in raycast to determine coin position.")]
		[SerializeField] private LayerMask SampleLayerMask;
		[SerializeField] private CoinBodyAnimator Animator;
		[SerializeField] private float objectRadius = 0.125f;

		private Vector3 mAttachedPosition;

		/// <summary>
		/// Represents actual coin position, which is always static regardless of body animation.
		/// </summary>
		public Vector3 attachedPosition
		{
			get
			{
				return mAttachedPosition;
			}

			private set
			{
				mAttachedPosition = value;

				transform.position = value;
			}
		}


		public bool isCollected { get; private set; }

		public bool canBeCollected { get; private set; }
		public string FromName { get; private set; }

		public void Place(Vector3 iAnimateFromPosition, float randomizePosition, string fromName = "Other")
		{
			var positionRandom = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f)) * randomizePosition;
            var coinDestinationPosition = iAnimateFromPosition + positionRandom;

			isCollected = false;
			canBeCollected = false;
			FromName = fromName;

			GetBoundedDestination(iAnimateFromPosition,coinDestinationPosition,SampleLayerMask, out Vector3 boundedDestination);
			coinDestinationPosition = boundedDestination;

			float _terrainHeightAtPosition;
			GetFixedHight(coinDestinationPosition, SampleLayerMask, out _terrainHeightAtPosition);

			attachedPosition = new Vector3(coinDestinationPosition.x, _terrainHeightAtPosition + 0.5f, coinDestinationPosition.z);

			Animator.Initialize(attachedPosition);
			Animator.AnimateSpawn(iAnimateFromPosition, () => canBeCollected = true);

			Update();
		}

		public void StartCollect(Transform iCollector)
		{
			Animator.AnimateCollect(iCollector, () => isCollected = true);
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

		private bool GetFixedHight(Vector3 position, LayerMask layerMask, out float resultY)
		{
			var samplePosition = new Vector3(position.x, position.y + 1, position.z);
			var ray = new Ray(samplePosition, Vector3.down);

			var hits = Physics.RaycastAll(samplePosition,Vector3.down,100, layerMask).ToList();
			hits.Sort(
				delegate(RaycastHit hit1, RaycastHit hit2){
					return hit1.distance.CompareTo(hit2.distance);
				}
			);

			foreach (var hit in hits)
			{
				bool isEnemy = hit.transform.gameObject.GetComponent<NavMeshAgent>() != null;
				if(isEnemy) continue;

                resultY = hit.point.y;
                return true;
			}

            resultY = position.y;
			return false;
		}

		private void Update()
		{
			Animator.Update(Time.smoothDeltaTime);
		}
	}
}