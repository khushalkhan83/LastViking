using CodeStage.AntiCheat.ObscuredTypes;
using Coin;
using Game.Audio;
using Game.Purchases;
using Game.Views;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Models
{
    public class BlueprintObjectsModel : MonoBehaviour
    {
        private readonly List<BlueprintObject> mActiveBlueprints = new List<BlueprintObject>(20);

        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject BluePrintPrefab;
        [SerializeField] private float PickSqrDistance;
        [SerializeField] private ObscuredInt _adjustValue;
        [SerializeField] private ObscuredInt _countToDrop;

#pragma warning restore 0649
        #endregion
        public int CountToDrop => _countToDrop;

        private PurchasesModel mPurchasesModel;

        private Transform _player;
        private Transform Player => _player ?? ModelsSystem.Instance._playerEventHandler.transform;

        private AudioSystem AudioSystem => AudioSystem.Instance;
        private ViewsSystem ViewsSystem => ViewsSystem.Instance;
        private ActionsLogModel ActionsLogModel => ModelsSystem.Instance._actionsLogModel;
        private BluePrintsModel BluePrintsModel => ModelsSystem.Instance._bluePrintsModel;
        private PurchasesModel PurchasesModel => ModelsSystem.Instance._purchasesModel;

        public void SpawnAtPosition(int count, Vector3 position, Vector3 animateSpawnFromPosition, float randomizePosition = 0)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnBluePrint(position, animateSpawnFromPosition, randomizePosition);
            }
        }

        float __sqrDistance;
        private void Update()
        {
            var collectorPosition = new Vector2(Player.position.x, Player.position.z);

            foreach (var blueprint in mActiveBlueprints)
            {
                if (!blueprint.canBeCollected)
                    continue;

                __sqrDistance = (new Vector2(blueprint.attachedPosition.x, blueprint.attachedPosition.z) - collectorPosition).sqrMagnitude;

                if (__sqrDistance <= PickSqrDistance)
                {
                    blueprint.StartCollect(Player);
                }
            }

            for (int i = mActiveBlueprints.Count - 1; i >= 0 ; i--)
            {
                var blueprint = mActiveBlueprints[i];

                if (blueprint.isCollected)
                {
                    CollectBlueprint(blueprint);

                    BlueprintsProvider.Return(blueprint);
                    mActiveBlueprints.RemoveAt(i);
                }
            }
        }

        private void CollectBlueprint(BlueprintObject iCoin)
        {
            BluePrintsModel.Adjust(_adjustValue);
            var audioIdentifier = BluePrintPrefab.GetComponent<AudioIdentifier>();
            if (audioIdentifier)
            {
                AudioSystem.PlayOnce(audioIdentifier.AudioID[Random.Range(0, audioIdentifier.AudioID.Length)]);
            }
            ActionsLogModel.SendMessage(new MessageAppendBlueprintData(_adjustValue));
        }

        private void SpawnBluePrint(Vector3 position, Vector3 animateFromPosition, float randomizePosition)
        {
            var blueprint = BlueprintsProvider.Get(BluePrintPrefab);
            blueprint.Place(animateFromPosition, randomizePosition);

            mActiveBlueprints.Add(blueprint);
        }
    }
}