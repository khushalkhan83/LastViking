using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

namespace Encounters
{
    public class SpecialEncounterSpawnPointProvider: MonoBehaviour
    {
        private List<ISpawnPointProvider> sokets = new List<ISpawnPointProvider>();
        private List<ISpawnPointProvider> busy = new List<ISpawnPointProvider>();

        #region MonoBehaviour
        private void Awake()
        {
            sokets = GetComponentsInChildren<ISpawnPointProvider>().ToList();
        }
        #endregion
        
        public bool TryGetSoket(out ISpawnPointProvider spawnPointProvider, out int providerIndex)
        {
            List<ISpawnPointProvider> freeSokets = GetFreeSokets();
            providerIndex = 0;

            for (int i = 0; i < freeSokets.Count; i++)
            {
                var soket = freeSokets.RandomElement();
                bool noSpawnPoint = !soket.TryGetValidSpawnPoint(out Vector3 spawnPoint);

                if (noSpawnPoint)
                {
                    freeSokets.Remove(soket);
                }
                else
                {
                    spawnPointProvider = soket;
                    providerIndex = i;
                    return true;
                }
            }

            spawnPointProvider = null;
            return false;
        }

        public bool TryGetSoketByIndex(int providerIndex, out ISpawnPointProvider spawnPointProvider)
        {
            spawnPointProvider = null;

            if(!sokets.IndexOutOfRange(providerIndex))
            {
                var targetSoket = sokets[providerIndex];
                if(busy.Contains(targetSoket))
                {
                    return false;
                }
                else
                {
                    spawnPointProvider = targetSoket;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void SetBusy(ISpawnPointProvider provider) => busy.Add(provider);
        public void UnSetBusy(ISpawnPointProvider provider) => busy.Remove(provider);

        private List<ISpawnPointProvider> GetFreeSokets()
        {
            var freeSokets = new List<ISpawnPointProvider>();

            foreach (var soket in sokets)
            {
                if (busy.Contains(soket))
                    continue;

                freeSokets.Add(soket);
            }

            return freeSokets;
        }
    }
}