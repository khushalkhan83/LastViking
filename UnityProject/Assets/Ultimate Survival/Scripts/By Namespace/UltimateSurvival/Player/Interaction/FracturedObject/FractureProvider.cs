using System.Collections.Generic;
using UnityEngine;

public static class FracturePackProvider
{
    private static Dictionary<GameObject, Stack<FracturePack>> FracturePoolDictionary { get; } = new Dictionary<GameObject, Stack<FracturePack>>();

    private static GameObject mPoolRootGo;
    private static Transform PoolRootGO
    {
        get
        {
            if (mPoolRootGo == null)
            {
                mPoolRootGo = new GameObject("Fracture Pool");
                Object.DontDestroyOnLoad(mPoolRootGo);
            }

            return mPoolRootGo.transform;
        }
    }

    public static FracturePack Get(GameObject prefab) => GetInstance(GetPool(prefab), prefab);

    public static void Return(FracturePack iFracturePack)
    {
        GetPool(iFracturePack.AssociatedPrefab).Push(iFracturePack);

        iFracturePack.OnReturnToPool(PoolRootGO);
    }

    public static void PreLoad(GameObject prefab, out int totalPieces)
    {
        var pool = GetPool(prefab);

        if (pool.Count == 0)
        {
            Return(new FracturePack(prefab));
        }

        totalPieces = pool.Peek().TotalPieces;
    }

    private static FracturePack GetInstance(Stack<FracturePack> pool, GameObject prefab)
    {
        if (pool.Count == 0)
        {
            return new FracturePack(prefab);
        }

        return pool.Pop();
    }

    private static Stack<FracturePack> GetPool(GameObject prefab)
    {
        if (!FracturePoolDictionary.ContainsKey(prefab))
        {
            var stack = new Stack<FracturePack>(3);
            FracturePoolDictionary.Add(prefab, stack);
            return stack;
        }

        return FracturePoolDictionary[prefab];
    }
}
