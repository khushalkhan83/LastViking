using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DecalProvider
{
	private static readonly Dictionary<GameObject, Stack<Decal>> DecalPoolDictionary = new Dictionary<GameObject, Stack<Decal>>();
	private static GameObject mPoolRootGo;

	private static GameObject poolRootGO
	{
		get
		{
			if (mPoolRootGo == null)
			{
				mPoolRootGo = new GameObject("Decal Pool");
				GameObject.DontDestroyOnLoad(mPoolRootGo);
			}

			return mPoolRootGo;
		}
	}

	public static Decal Get(GameObject iPrefab, GameObject iCallerGO)
	{
		var _pool = GetPool(iPrefab);
		var _decal = GetInstance(_pool, iPrefab);

		_decal.OnCalledFromPool(iCallerGO);

		return _decal;
	}

	public static void Return(Decal iDecal)
	{
		var _pool = GetPool(iDecal.associatedPrefab);
		_pool.Push(iDecal);

		iDecal.OnReturnToPool(poolRootGO);
	}

	private static Stack<Decal> GetPool(GameObject iPrefab)
	{
		if (DecalPoolDictionary.ContainsKey(iPrefab) == false)
		{
			DecalPoolDictionary.Add(iPrefab, new Stack<Decal>(3));
		}

		return DecalPoolDictionary[iPrefab];
	}

	private static Decal GetInstance(Stack<Decal> iPool, GameObject iPrefab)
	{
		if (iPool.Any() == false)
		{
			var _decal = Decal.Build(iPrefab);

			return _decal;
		}

		return iPool.Pop();
	}
}