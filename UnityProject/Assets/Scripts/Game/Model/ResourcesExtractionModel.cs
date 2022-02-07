using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using static UltimateSurvival.FPTool;

namespace Game.Models
{
    public class ResourcesExtractionModel : MonoBehaviour
    {
        private const int defaultModifier = 1;
        private Dictionary<ToolPurpose,int> modifierByToolPurpose = new Dictionary<FPTool.ToolPurpose, int>();
        public void SetModifier(ToolPurpose toolPurpose, int modifier)
        {
            if(modifierByToolPurpose.ContainsKey(toolPurpose))
            {
                modifierByToolPurpose[toolPurpose] = modifier;
            }
            else
            {
                modifierByToolPurpose.Add(toolPurpose,modifier);
            }
        }

        public int GetModifier(ToolPurpose toolPurpose)
        {
            if(modifierByToolPurpose.TryGetValue(toolPurpose, out int value))
                return value;
            else
                return defaultModifier;
        }

        public void RemoveModifier(ToolPurpose toolPurpose)
        {
            if(modifierByToolPurpose.ContainsKey(toolPurpose))
                modifierByToolPurpose.Remove(toolPurpose);
        }
    }
}
