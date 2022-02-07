namespace Coin
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;

	public static class CoinProvider
	{
		private static GameObject mPoolRootGO; 
		private static Stack<CoinObject> mPooledCoins = new Stack<CoinObject>(10);
		private static GameObject mEnvironmentRoot;

		// Note: Environment root can be destroyed with game restart. 
		private static GameObject environmentRoot
		{ 
			get
			{
				if (mEnvironmentRoot == null)
				{
					mEnvironmentRoot = new GameObject("Coins");
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
                    mPoolRootGO = new GameObject("Coin Pool");
					GameObject.DontDestroyOnLoad(mPoolRootGO);
				}

				return mPoolRootGO;
			}
		}

		public static CoinObject Get(GameObject iPrefab)
		{
			CoinObject _coin = null;

			if (mPooledCoins.Any())
			{
				_coin = mPooledCoins.Pop();
			}
            if (_coin == null)
            {
				_coin = CreateCoin(iPrefab);
			}

			_coin.transform.parent = environmentRoot.transform;
			_coin.gameObject.SetActive(true);

			return _coin;
		}
		
		public static void Return(CoinObject iCoin)
		{
			iCoin.gameObject.SetActive(false);
			iCoin.transform.parent = poolRoot.transform;

			mPooledCoins.Push(iCoin);
		}

		private static CoinObject CreateCoin(GameObject iPrefab)
		{
			var _coinGo = GameObject.Instantiate(iPrefab);
			var _coin = _coinGo.GetComponent<CoinObject>();

			return _coin;
		}
	}
}