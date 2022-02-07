using System.Collections;
using System.Collections.Generic;
using Coin;
using UnityEngine;

namespace Game.QuestSystem.Map.Extra
{
    public class QuestItemObjectAnimation : MonoBehaviour
    {
		[SerializeField] private CoinBodyAnimator Animator;
		
		private void Update()
		{
			Animator.Update(Time.smoothDeltaTime);
		}
    }
}
