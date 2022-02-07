using System.Collections.Generic;
using System.Linq;
using Core.Providers;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Providers_enemies_default", menuName = "Providers/Enemies", order = 0)]
public class EnemiesProvider : ProviderScriptable<EnemyID, Initable> {
    [EnumNamedArray(typeof(EnemyID))]
    [SerializeField] private Initable[] _data;

    private List<EnemyID> ids = null;

    private List<EnemyID> Ids 
    {
        get
        {
            if(ids == null || ids.Count == 0) ids = Helpers.EnumsHelper.GetValues<EnemyID>().ToList();
            return ids;
        }
    }

    public override Initable this[EnemyID key] => _data[GetIndex(key)];

    private int GetIndex(EnemyID key)
    {
        var index = Ids.IndexOf(key) - 1; // -1 because of none key
        return index;
    }
 }
