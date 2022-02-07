using UnityEngine;

// General (int, flaot, string etc)
namespace SO_Variables_General
{
    [CreateAssetMenu(fileName = "SO_variable_f_Varibale", menuName = "Variables/General/Float", order = 5)]
    public class FloatVariable : ScriptableObject {
        public float Value;
    }
}
