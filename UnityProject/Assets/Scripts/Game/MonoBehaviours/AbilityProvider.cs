using Core.Providers;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Providers_ability_default", menuName = "Providers/Ability", order = 0)]

public class AbilityProvider : ProviderScriptable<AbilityID, AbilityBase> {
    [EnumNamedArray(typeof(AbilityID))] 
    [SerializeField] private AbilityBase[] _data;

    public override AbilityBase this[AbilityID key] => _data[((int)(object)key - 1)];
}
