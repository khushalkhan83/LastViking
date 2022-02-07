using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using Helpers;
using System.Linq;

namespace Game.Views
{
    public class ViewsProvider : Provider<ViewID, Component> 
    {
#if UNITY_EDITOR

        [Button]
        public void CheckProvider()
        {
            foreach (var d in _data)
            {
                if(d == null)
                {
                    EditorUtility.DisplayDialog("Внимание", "В провайдере есть недостающие данные", "Ok");
                    return;
                }
            }

            var allEnumsExeptNone =  Helpers.EnumsHelper.GetValues<ViewID>().Count() - 1;

            if(_data.Length != allEnumsExeptNone)
            {
                EditorUtility.DisplayDialog("Внимание", "Количество данных и id не совпадает", "Ok");
                    return;
            }
        }

        
        public void ExpandAndAddComponent(Component component)
        {
            var oldData = _data;
            _data = new Component[_data.Length+1];

            for (int i = 0; i < oldData.Length; i++)
            {
                _data[i] = oldData[i];
            }

            _data[_data.Length - 1] = component;
        }
        
        // Testing
        [Button] void ExpandValues() => ExpandAndAddComponent(null);
        // [SerializeField] private Component cmp;
        // [Button] void TestExpandAndAddComponent() => ExpandValues(cmp);

#endif

    }
}
