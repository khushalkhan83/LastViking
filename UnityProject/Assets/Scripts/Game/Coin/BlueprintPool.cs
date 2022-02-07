namespace Coin
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public static class BlueprintsProvider
	{
		private static GameObject mPoolRootGO; 
		private static Stack<BlueprintObject> mPooledBlueprints = new Stack<BlueprintObject>(10);
		private static GameObject mEnvironmentRoot;

		// Note: Environment root can be destroyed with game restart. 
		private static GameObject environmentRoot
		{
			get
			{
				if (mEnvironmentRoot == null)
				{
					mEnvironmentRoot = new GameObject("Bleuprints");
				}

				return mEnvironmentRoot;
			}
		}

		private static GameObject poolRoot
		{
			get
			{
				if (mPoolRootGO == null)
				{
					mPoolRootGO = new GameObject("Blueprint Pool");
					GameObject.DontDestroyOnLoad(mPoolRootGO);
				}

				return mPoolRootGO;
			}
		}

		public static BlueprintObject Get(GameObject iPrefab)
		{
            BlueprintObject blueprint = null;

			if (mPooledBlueprints.Any())
			{
				blueprint = mPooledBlueprints.Pop();
			}
            if (blueprint == null)
            {
				blueprint = CreateBlueprint(iPrefab);
			}

            blueprint.transform.parent = environmentRoot.transform;
			blueprint.gameObject.SetActive(true);

			return blueprint;
		}
		
		public static void Return(BlueprintObject iBlueprint)
		{
			iBlueprint.gameObject.SetActive(false);
			iBlueprint.transform.parent = poolRoot.transform;

			mPooledBlueprints.Push(iBlueprint);
		}

		private static BlueprintObject CreateBlueprint(GameObject iPrefab)
		{
			var blueprintGo = GameObject.Instantiate(iPrefab);
			var blueprint = blueprintGo.GetComponent<BlueprintObject>();

			return blueprint;
		}
	}
}