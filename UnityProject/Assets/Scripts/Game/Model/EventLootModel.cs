using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

namespace Game.Models
{
    /* Refactor
     * string keys ?
     * probability
     * receive loot mechanics
     */ 

    public class EventLootModel : MonoBehaviour
    {
        Dictionary<string, int> customLoots;

        public event Action<string> onCustomLootRecived;

        public void RegisterCustomLoot(string item, int prob)
        {
            if (customLoots == null)
                customLoots = new Dictionary<string, int>();

            if (customLoots.ContainsKey(item))
            {
                customLoots[item] = prob;
            }
            else
            {
                customLoots.Add(item, prob);
            }
        }

        public void UnregisterCustomLoot(string item)
        {
            if (customLoots == null)
                customLoots = new Dictionary<string, int>();

            if (customLoots.ContainsKey(item))
                customLoots.Remove(item);
        }

        public bool HasCustomLoot(string item) => customLoots?.ContainsKey(item) ?? false;

        public bool GetLootFromCustom(string defKey,int relVal, out string lootKey)
        {
            if (customLoots == null)
            {
                lootKey = defKey;
                return false;
            }

            int sum = relVal;
            foreach(KeyValuePair<string,int> pair in customLoots)
            {
                sum += pair.Value;
            }
            int result = Random.Range(0, sum);

            foreach (KeyValuePair<string, int> pair in customLoots)
            {
                if (result <= pair.Value)
                {
                    lootKey = pair.Key;
                    return true;
                }
                else
                {
                    result -= pair.Value;
                }
            }

            lootKey = defKey;
            return false;
        }

        public void ReceiveLootFromCustom(string key) => onCustomLootRecived?.Invoke(key);
    }
}