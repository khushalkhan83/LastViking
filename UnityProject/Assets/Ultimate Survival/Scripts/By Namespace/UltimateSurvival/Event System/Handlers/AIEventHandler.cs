using UnityEngine;

namespace UltimateSurvival
{
	/// <summary>
	/// 
	/// </summary>
	public class AIEventHandler : EntityEventHandler 
	{
		/// <summary></summary>
		public ChangedValue<bool> IsHungry = new ChangedValue<bool>(false); 

		/// <summary></summary>
		public ChangedValue<float> LastFedTime = new ChangedValue<float>(0f);

		/// <summary></summary>
		public Activity Patrol = new Activity();

		/// <summary></summary>
		public Activity Chase = new Activity();

		/// <summary></summary>
		public Activity Attack = new Activity();

		/// <summary></summary>
		public Activity RunAway = new Activity();
	}
}
