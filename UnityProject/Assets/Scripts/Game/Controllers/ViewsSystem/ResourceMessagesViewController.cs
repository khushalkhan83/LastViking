using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    interface IMessageWithCounter
    {
        int Count { get; set; }
    }

    public class MessageGatheredData : IMessageWithCounter
    {
        public int ItemId { get; set; }
        public int Count { get; set; }
        public MessageAppendResourceView View { get; set; }
    }

    public class MessageAttenptData : IMessageWithCounter
    {
        public int ItemId { get; set; }
        public int Count { get; set; }
        public MessageAttentionResourceView View { get; set; }
    }

    public class MessageDestroyData : IMessageWithCounter
    {
        public int ItemId { get; set; }
        public int Count { get; set; }
        public MessageDestroyResourceView View { get; set; }
    }

    public class MessageCraftData : IMessageWithCounter
    {
        public int ItemId { get; set; }
        public int Count { get; set; }
        public MessageCraftResourceView View { get; set; }
    }

    public class FloatingTextData
    {
        public MessagesFloatingTextView View { get; set; }
        public Vector3 PositionBegin { get; set; }
        public Vector3 PositionEnd { get; set; }
        public float LifeTimeRemaining { get; set; }
    }

    public class MessageCoinData: IMessageWithCounter
    {
        public int Count { get; set; }
        public MessageAppendCoinView View { get; set; }
    }

    public class MessageBlueprintData: IMessageWithCounter
    {
        public int Count { get; set; }
        public MessageAppendBlueprintView View { get; set; }
    }

    public class ResourceMessagesViewController : ViewControllerBase<ResourceMessagesView>
    {
        [Inject] public ActionsLogModel ActionsLogModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected HashSet<FloatingTextData> FloatingTexts { get; } = new HashSet<FloatingTextData>();

        protected HashSet<MessageGatheredData> MessageGatheredData { get; } = new HashSet<MessageGatheredData>();
        protected HashSet<MessageAttenptData> MessageAttenptData { get; } = new HashSet<MessageAttenptData>();
        protected HashSet<MessageDestroyData> MessageDestroyData { get; } = new HashSet<MessageDestroyData>();
        protected HashSet<MessageCraftData> MessageCraftData { get; } = new HashSet<MessageCraftData>();
        protected HashSet<MessageCoinData> MessageCoinData { get; } = new HashSet<MessageCoinData>();
        protected HashSet<MessageBlueprintData> MessageBlueprintData { get; } = new HashSet<MessageBlueprintData>();


        private void Update()
        {
            UpdateFloatingTexts();
        }

        private const float FLOATING_TEXT_DURATION = 1;

        protected override void Show()
        {
            ActionsLogModel.AddListener<MessageInventoryAttentionData>(OnMessageHandler);
            ActionsLogModel.AddListener<MessageInventoryCraftedData>(OnMessageHandler);
            ActionsLogModel.AddListener<MessageInventoryDroppedData>(OnMessageHandler);
            ActionsLogModel.AddListener<MessageInventoryGatheredData>(OnMessageHandler);
            ActionsLogModel.AddListener<MessageInventoryGatheredBonusData>(OnMessageHandler);
            ActionsLogModel.AddListener<MessageAppendCoinData>(OnMessageHandler);
            ActionsLogModel.AddListener<MessageAppendBlueprintData>(OnMessageHandler);
        }

        protected override void Hide()
        {
            ActionsLogModel.RemoveListener<MessageInventoryAttentionData>(OnMessageHandler);
            ActionsLogModel.RemoveListener<MessageInventoryCraftedData>(OnMessageHandler);
            ActionsLogModel.RemoveListener<MessageInventoryDroppedData>(OnMessageHandler);
            ActionsLogModel.RemoveListener<MessageInventoryGatheredData>(OnMessageHandler);
            ActionsLogModel.RemoveListener<MessageInventoryGatheredBonusData>(OnMessageHandler);
            ActionsLogModel.RemoveListener<MessageAppendCoinData>(OnMessageHandler);
            ActionsLogModel.RemoveListener<MessageAppendBlueprintData>(OnMessageHandler);
        }

        private void OnMessageHandler(MessageAppendCoinData data)
        {
            var message = MessageCoinData.FirstOrDefault();

            bool queueIsEmpty = message == null;

            if(queueIsEmpty)
            {
                message = AddCoinMessage(data);
                ShowFirstMessage(message.View,data.CountCoins);
            }
            else
                HandleNextMessage(message, message.View, data.CountCoins);  
        }

        private void OnMessageHandler(MessageAppendBlueprintData data)
        {
            var message = MessageBlueprintData.FirstOrDefault();

            bool queueIsEmpty = message == null;

            if (queueIsEmpty)
            {
                message = AddBlueprintMessage(data);
                ShowFirstMessage(message.View, data.CountBlueprints);
            }
            else
                HandleNextMessage(message, message.View, data.CountBlueprints, showFloatingText: true);            
        }

        #region Generic Methods for interfaces

        private void ShowFirstMessage(IRessourceMessageView view, int count)
        {
            var isVissable = count > 1;

            string text = isVissable ? $"x{count}" : string.Empty;

            view.SetCountText(text);
            view.PlayShow();
        }

        void HandleNextMessage(IMessageWithCounter messageWithCounter, IRessourceMessageView view, int plusCount, bool showFloatingText = false)
        {
            messageWithCounter.Count += plusCount;

            view.PlayPulse();
            view.SetCountText($"x{messageWithCounter.Count}");
            view.SetIsVisibleCount(true);

            if(showFloatingText == false) return;

            var viewWithFloatingPoint = view as IHaveFloatingTextPosition;
            if(viewWithFloatingPoint == null) {Debug.LogError("Cant display message floating text here"); return;}

            AddFloatingTextPositive(plusCount, viewWithFloatingPoint.PositionFloatingText);
        }
            
        #endregion

        

        private MessageCoinData AddCoinMessage(MessageAppendCoinData data)
        {
            var view = ViewsSystem.Show<MessageAppendCoinView>(ViewConfigID.MessageAppendCoin, View.ContainerContent);
            view.OnEndHideAnimation += OnEndCoinViewHandler;

            var message = new MessageCoinData()
            {
                Count = data.CountCoins,
                View = view
            };

            MessageCoinData.Add(message);

            return message;
        }

        private MessageBlueprintData AddBlueprintMessage(MessageAppendBlueprintData data)
        {
            var view = ViewsSystem.Show<MessageAppendBlueprintView>(ViewConfigID.MessageAppendBlueprint, View.ContainerContent);
            view.OnEndHideAnimation += OnEndBlueprintViewHandler;

            var message = new MessageBlueprintData()
            {
                Count = data.CountBlueprints,
                View = view
            };

            MessageBlueprintData.Add(message);

            return message;
        }

        private void OnEndCoinViewHandler(MessageAppendCoinView view)
        {
            if (!view.IsTransition)
            {
                view.OnEndHideAnimation -= OnEndCoinViewHandler;

                var message = MessageCoinData.First(x => x.View == view);

                MessageCoinData.Remove(message);
                ViewsSystem.Hide(view);
            }
        }

        private void OnEndBlueprintViewHandler(MessageAppendBlueprintView view)
        {
            if (!view.IsTransition)
            {
                view.OnEndHideAnimation -= OnEndBlueprintViewHandler;

                var message = MessageBlueprintData.First(x => x.View == view);

                MessageBlueprintData.Remove(message);
                ViewsSystem.Hide(view);
            }
        }

        private void OnMessageHandler(MessageInventoryAttentionData data)
        {
            var message = MessageAttenptData.FirstOrDefault(x => x.ItemId == data.ItemData.Id);
            if (message != null)
            {
                message.View.PlayPulse();
            }
            else
            {
                message = AddAttentionMessage(data);
                message.View.SetIcon(data.ItemData.Icon);
                message.View.PlayShow();
            }
        }

        private MessageAttenptData AddAttentionMessage(MessageInventoryAttentionData data)
        {
            var view = ViewsSystem.Show<MessageAttentionResourceView>(ViewConfigID.MessageAttentionResource, View.ContainerContent);
            view.OnEndHideAnimation += OnEndAttentionViewHandler;

            var message = new MessageAttenptData()
            {
                ItemId = data.ItemData.Id,
                View = view
            };

            MessageAttenptData.Add(message);

            return message;
        }

        private void OnEndAttentionViewHandler(MessageAttentionResourceView view)
        {
            view.OnEndHideAnimation -= OnEndAttentionViewHandler;

            var message = MessageAttenptData.First(x => x.View == view);

            MessageAttenptData.Remove(message);
            ViewsSystem.Hide(view);
        }

        private void OnMessageHandler(MessageInventoryGatheredBonusData data)
        {
            var message = MessageGatheredData.FirstOrDefault(x => x.ItemId == data.ItemData.Id);
            if (message != null)
            {
                message.Count += data.CountItems;

                message.View.PlayPulseBonus();
                message.View.SetCountText($"x{message.Count}");
            }
            else
            {
                message = AddAppendedMessage(data);
                message.View.SetIcon(data.ItemData.Icon);
                if (message.Count > 1)
                {
                    message.View.SetCountText($"x{message.Count}");
                }
                else
                {
                    message.View.SetCountText(string.Empty);
                }
                message.View.PlayShowBonus();
            }
        }

        private void OnMessageHandler(MessageInventoryCraftedData data)
        {
            var message = MessageCraftData.FirstOrDefault(x => x.ItemId == data.ItemData.Id);
            if (message != null)
            {
                message.Count += data.CountItems;

                message.View.PlayPulse();
                message.View.SetCountText($"+{message.Count}");
                message.View.SetIsVisibleCount(true);
                AddFloatingTextPositive(data.CountItems, message.View.PositionFloatingText);
            }
            else
            {
                message = AddCraftedMessage(data);
                message.View.SetIcon(data.ItemData.Icon);

                var isVisibleCount = message.Count > 1;

                if (isVisibleCount)
                {
                    message.View.SetCountText($"+{message.Count}");
                }
                else
                {
                    message.View.SetCountText(string.Empty);
                }

                message.View.SetIsVisibleCount(isVisibleCount);
            }
        }

        private MessageCraftData AddCraftedMessage(MessageInventoryCraftedData data)
        {
            var view = ViewsSystem.Show<MessageCraftResourceView>(ViewConfigID.MessageCraftResource, View.ContainerContent);
            view.OnEndHideAnimation += OnEndCraftViewHandler;

            var message = new MessageCraftData()
            {
                Count = data.CountItems,
                ItemId = data.ItemData.Id,
                View = view
            };

            MessageCraftData.Add(message);

            return message;
        }

        private void OnEndCraftViewHandler(MessageCraftResourceView view)
        {
            if (!view.IsTransition)
            {
                view.OnEndHideAnimation -= OnEndCraftViewHandler;

                var message = MessageCraftData.First(x => x.View == view);

                MessageCraftData.Remove(message);
                ViewsSystem.Hide(view);
            }
        }

        private void OnMessageHandler(MessageInventoryDroppedData data)
        {
            var message = MessageDestroyData.FirstOrDefault(x => x.ItemId == data.ItemData.Id);
            if (message != null)
            {
                message.Count += data.CountItems;

                message.View.PlayPulse();
                message.View.SetCountText($"-{message.Count}");
                message.View.SetIsVisibleCount(true);
                AddFloatingTextNegative(data.CountItems, message.View.PositionFloatingText);
            }
            else
            {
                message = AddDestroyedMessage(data);
                message.View.SetIcon(data.ItemData.Icon);

                var isVisibleCount = message.Count > 1;

                if (isVisibleCount)
                {
                    message.View.SetCountText($"-{message.Count}");
                }
                else
                {
                    message.View.SetCountText(string.Empty);
                }

                message.View.SetIsVisibleCount(isVisibleCount);
            }
        }

        private MessageDestroyData AddDestroyedMessage(MessageInventoryDroppedData data)
        {
            var view = ViewsSystem.Show<MessageDestroyResourceView>(ViewConfigID.MessageDestroyResource, View.ContainerContent);
            view.OnEndHideAnimation += OnEndDestroyViewHandler;

            var message = new MessageDestroyData()
            {
                Count = data.CountItems,
                ItemId = data.ItemData.Id,
                View = view
            };

            MessageDestroyData.Add(message);

            return message;
        }

        private void OnEndDestroyViewHandler(MessageDestroyResourceView view)
        {
            if (!view.IsTransition)
            {
                view.OnEndHideAnimation -= OnEndDestroyViewHandler;

                var message = MessageDestroyData.First(x => x.View == view);

                MessageDestroyData.Remove(message);
                ViewsSystem.Hide(view);
            }
        }

        private void OnMessageHandler(MessageInventoryGatheredData data)
        {
            var message = MessageGatheredData.FirstOrDefault(x => x.ItemId == data.ItemData.Id);
            if (message != null)
            {
                message.Count += data.CountItems;

                message.View.PlayPulse();
                message.View.SetCountText($"x{message.Count}");
                message.View.SetIsVisibleCount(true);
                AddFloatingTextPositive(data.CountItems, message.View.PositionFloatingText);
            }
            else
            {
                message = AddAppendedMessage(data);
                message.View.SetIcon(data.ItemData.Icon);

                var isVisibleCount = message.Count > 1;

                if (isVisibleCount)
                {
                    message.View.SetCountText($"x{message.Count}");
                }
                else
                {
                    message.View.SetCountText(string.Empty);
                }

                message.View.SetIsVisibleCount(isVisibleCount);

                message.View.PlayShow();
            }
        }

        private MessageGatheredData AddAppendedMessage(MessageInventoryGatheredBonusData data)
        {
            var view = ViewsSystem.Show<MessageAppendResourceView>(ViewConfigID.MessageAppendResource, View.ContainerContent);
            view.OnEndHideAnimation += OnEndAppendViewHandler;

            var message = new MessageGatheredData()
            {
                Count = data.CountItems,
                ItemId = data.ItemData.Id,
                View = view
            };

            MessageGatheredData.Add(message);

            return message;
        }

        private MessageGatheredData AddAppendedMessage(MessageInventoryGatheredData data)
        {
            var view = ViewsSystem.Show<MessageAppendResourceView>(ViewConfigID.MessageAppendResource, View.ContainerContent);
            view.OnEndHideAnimation += OnEndAppendViewHandler;

            var message = new MessageGatheredData()
            {
                Count = data.CountItems,
                ItemId = data.ItemData.Id,
                View = view
            };

            MessageGatheredData.Add(message);

            return message;
        }

        private void UpdateFloatingTexts()
        {
            foreach (var item in FloatingTexts)
            {
                item.LifeTimeRemaining -= Time.deltaTime;
                item.View.SetPosition(Vector3.Lerp(item.PositionBegin, item.PositionEnd, item.View.MoveCurve.Evaluate(1 - item.LifeTimeRemaining / FLOATING_TEXT_DURATION)));
                item.View.SetAlpha(Mathf.Lerp(0, 1, item.View.AlphaCurve.Evaluate(1 - item.LifeTimeRemaining / FLOATING_TEXT_DURATION)));

                if (item.LifeTimeRemaining <= 0)
                {
                    ViewsSystem.Hide(item.View);
                    item.View = null;
                }
            }
            FloatingTexts.RemoveWhere(x => x.View == null);
        }

        private FloatingTextData AddFloatingTextPositive(int countItems, Vector3 position)
        {
            var view = ViewsSystem.Show<MessagesFloatingTextView>(ViewConfigID.MessagesFloatingText, View.ContainerFloatings);
            view.SetPosition(position);
            view.SetAlpha(1);
            view.SetText($"+{countItems}");

            var message = new FloatingTextData()
            {
                View = view,
                LifeTimeRemaining = FLOATING_TEXT_DURATION,
                PositionBegin = position,
                PositionEnd = position + new Vector3(4.5f, 10, 0)
            };

            FloatingTexts.Add(message);

            return message;
        }

        private FloatingTextData AddFloatingTextNegative(int countItems, Vector3 position)
        {
            var view = ViewsSystem.Show<MessagesFloatingTextView>(ViewConfigID.MessagesFloatingText, View.ContainerFloatings);
            view.SetPosition(position);
            view.SetAlpha(1);
            view.SetText($"-{countItems}");

            var message = new FloatingTextData()
            {
                View = view,
                LifeTimeRemaining = FLOATING_TEXT_DURATION,
                PositionBegin = position,
                PositionEnd = position + new Vector3(4.5f, 10, 0)
            };

            FloatingTexts.Add(message);

            return message;
        }

        private void OnEndAppendViewHandler(MessageAppendResourceView view)
        {
            if (!view.IsTransition)
            {
                view.OnEndHideAnimation -= OnEndAppendViewHandler;

                var message = MessageGatheredData.First(x => x.View == view);

                MessageGatheredData.Remove(message);
                ViewsSystem.Hide(view);
            }
        }
    }
}
