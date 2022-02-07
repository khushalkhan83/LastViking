using Coin;
using Game.Audio;
using Game.Purchases;
using Game.Views;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Models
{
    public class CoinObjectsModel : MonoBehaviour
    {
        private readonly List<CoinObject> mActiveCoins = new List<CoinObject>(20);

        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject CoinPrefab;
        [SerializeField] private float PickSqrDistance;

#pragma warning restore 0649
        #endregion
        private Transform _player;
        private Transform Player => _player ?? ModelsSystem.Instance._playerEventHandler.transform;

        private AudioSystem AudioSystem => AudioSystem.Instance;
        private ViewsSystem ViewsSystem => ViewsSystem.Instance;
        private ActionsLogModel ActionsLogModel => ModelsSystem.Instance._actionsLogModel;
        private PurchasesModel PurchasesModel => ModelsSystem.Instance._purchasesModel;

        public event Action<string> OnCollectCoin;

        public void SpawnAtPosition(int count, Vector3 position, Vector3 animateSpawnFromPosition, float randomizePosition = 0, string fromName = "Other")
        {
            for (int i = 0; i < count; i++)
            {
                SpawnCoin(position, animateSpawnFromPosition, randomizePosition, fromName);
            }
        }

        float __sqrDistance;
        private void Update()
        {
            var collectorPosition = new Vector2(Player.position.x, Player.position.z);

            foreach (var coin in mActiveCoins)
            {
                if (!coin.canBeCollected)
                    continue;

                __sqrDistance = (new Vector2(coin.attachedPosition.x, coin.attachedPosition.z) - collectorPosition).sqrMagnitude;

                if (__sqrDistance <= PickSqrDistance)
                {
                    coin.StartCollect(Player);
                }
            }

            for (int i = mActiveCoins.Count - 1; i >= 0 ; i--)
            {
                var coin = mActiveCoins[i];

                if (coin.isCollected)
                {
                    CollectCoin(coin);

                    CoinProvider.Return(coin);
                    mActiveCoins.RemoveAt(i);
                }
            }
        }

        private void OnPurchaseHandler(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                var audioIdentifier = CoinPrefab.GetComponent<AudioIdentifier>();
                if (audioIdentifier)
                {
                    AudioSystem.PlayOnce(audioIdentifier.AudioID[Random.Range(0, audioIdentifier.AudioID.Length)]);
                }

                ActionsLogModel.SendMessage(new MessageAppendCoinData(1));
            }
        }

        private void CollectCoin(CoinObject iCoin)
        {
            PurchasesModel.Purchase(PurchaseID.CoinOne, OnPurchaseHandler);
            OnCollectCoin?.Invoke(iCoin.FromName);
        }

        private void SpawnCoin(Vector3 position, Vector3 animateFromPosition, float randomizePosition, string fromName)
        {
            var coin = CoinProvider.Get(CoinPrefab);
            coin.Place(animateFromPosition, randomizePosition, fromName);

            mActiveCoins.Add(coin);
        }
    }
}