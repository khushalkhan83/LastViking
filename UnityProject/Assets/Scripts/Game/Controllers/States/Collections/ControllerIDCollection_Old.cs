using UnityEngine;
using RoboRyanTron.SearchableEnum;
using NaughtyAttributes;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Game.Controllers
{
    namespace Controllers.States
    {
        // [CreateAssetMenu(fileName = "ControllerIDCollection", menuName = "Controllers/States/ControllerIDCollection", order = 0)]
        // public class ControllerIDCollection : ScriptableObject 
        // {
        //     [SearchableEnum]
        //     [SerializeField] private List<ControllerID> controllerIDs;

        //     [ResizableTextArea]
        //     [SerializeField] private string input;

        //     [Button] void Test()
        //     {
        //         var lines = StringToLines(input);

        //         controllerIDs.Clear();
                
        //         foreach (var line in lines)
        //         {
        //             if(line.Contains("//")) continue;

        //             ControllerID id = (ControllerID)Enum.Parse(typeof(ControllerID), line);
        //             controllerIDs.Add(id);
        //         }
        //     }
            

        //     private List<string> StringToLines(string input)
        //     {
        //         string[] lines = input.Split(
        //             new[] { Environment.NewLine },
        //             StringSplitOptions.None
        //         );
        //         return lines.ToList();
        //     }
        // }
    }
}
