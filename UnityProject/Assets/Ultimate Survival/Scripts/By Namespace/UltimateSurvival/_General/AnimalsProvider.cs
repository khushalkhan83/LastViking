using Core.Providers;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Providers_animals_default", menuName = "Providers/Animals", order = 0)]
public class AnimalsProvider : ProviderScriptable<AnimalID, Initable>
{
    [EnumNamedArray(typeof(AnimalID))]
    [SerializeField] private Initable[] _data;

    public override Initable this[AnimalID key] => _data[((int)(object)key - 1)];
}
