using Game.Controllers;
using Game.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NotificationData
{
    public PriorityID PriorityID { get; }
    public ViewConfigID ViewConfigID { get; }
    public IDataViewController DataViewController { get; }

    public NotificationData(PriorityID priorityID, ViewConfigID viewConfigID, IDataViewController dataViewController)
    {
        PriorityID = priorityID;
        ViewConfigID = viewConfigID;
        DataViewController = dataViewController;
    }
}

public enum NotificationStateID : byte
{
    None = 0,
    First = 1,
    Queue = 2,
    Cache = 3,
}

public enum PriorityID : byte
{
    None = 0,
    Tutorial = 1,
    EventObjectives = 1,
    TierObjectives = 2,
    Wave = 2,
}

public class NotificationContainerViewModel : MonoBehaviour
{
    public event Action OnShow;
    public event Action OnHide;

    public NotificationData DataCurrent { get; private set; }
    public NotificationStateID StateIDCurrent { get; private set; }

    public NotificationStateID StateIDNext { get; private set; }
    public NotificationStateID StateIDLast { get; private set; }

    public bool IsHasCurrent => DataCurrent != null;

    public Dictionary<PriorityID, Queue<NotificationData>> Queue { get; } = new Dictionary<PriorityID, Queue<NotificationData>>();
    public Dictionary<PriorityID, Stack<NotificationData>> Stack { get; } = new Dictionary<PriorityID, Stack<NotificationData>>();

    public void Show(PriorityID priorityID, ViewConfigID viewConfigID, IDataViewController dataViewController)
    {
        var data = new NotificationData(priorityID, viewConfigID, dataViewController);

        if (IsHasCurrent)
        {
            if (priorityID > DataCurrent.PriorityID)
            {
                AddToStack(DataCurrent);

                StateIDNext = NotificationStateID.Queue;
                OnHide?.Invoke();

                StateIDLast = StateIDCurrent;
                DataCurrent = data;
                StateIDCurrent = StateIDNext;
                StateIDNext = NotificationStateID.None;
                OnShow?.Invoke();
            }
            else
            {
                AddToQueue(data);
            }
        }
        else
        {
            StateIDLast = StateIDCurrent;
            DataCurrent = data;
            StateIDCurrent = NotificationStateID.First;
            StateIDNext = NotificationStateID.None;
            OnShow?.Invoke();
        }
    }

    public void EndCurrent()
    {
        if (TryGetNext(out var nextData))
        {
            StateIDNext = nextData.StateID;
            OnHide?.Invoke();

            StateIDLast = StateIDCurrent;
            DataCurrent = nextData.Data;
            StateIDCurrent = StateIDNext;
            StateIDNext = NotificationStateID.None;
            OnShow?.Invoke();
        }
        else
        {
            StateIDNext = NotificationStateID.None;
            OnHide?.Invoke();

            StateIDLast = StateIDCurrent;
            DataCurrent = default;
            StateIDCurrent = NotificationStateID.None;
        }
    }

    public void AddToStack(NotificationData data) => GetStack(data).Push(data);
    public void AddToQueue(NotificationData data) => GetQueue(data).Enqueue(data);

    private Stack<NotificationData> GetStack(NotificationData data)
    {
        if (Stack.ContainsKey(data.PriorityID))
        {
            return Stack[data.PriorityID];
        }

        var stack = new Stack<NotificationData>();
        Stack[data.PriorityID] = stack;

        return stack;
    }

    private Queue<NotificationData> GetQueue(NotificationData data)
    {
        if (Queue.ContainsKey(data.PriorityID))
        {
            return Queue[data.PriorityID];
        }

        var queue = new Queue<NotificationData>();
        Queue[data.PriorityID] = queue;

        return queue;
    }

    public bool TryGetNext(out (NotificationStateID StateID, NotificationData Data) result)
    {
        var isHasMaxInStack = TryGetMaxPriority(Stack, out var maxFromStack);
        var isHasMaxInQueue = TryGetMaxPriority(Queue, out var maxFromQueue);
        var isGetFromQueue = (isHasMaxInQueue && isHasMaxInStack && maxFromQueue > maxFromStack) || (isHasMaxInQueue && !isHasMaxInStack);

        if (isGetFromQueue)
        {
            result = (NotificationStateID.Queue, Get(Queue, maxFromQueue));
        }
        else
        {
            var isGetFromStack = (isHasMaxInQueue && isHasMaxInStack && maxFromQueue <= maxFromStack) || (!isHasMaxInQueue && isHasMaxInStack);
            if (isGetFromStack)
            {
                result = (NotificationStateID.Cache, Get(Stack, maxFromStack));
            }
            else
            {
                result = default;
            }
        }

        return isHasMaxInStack || isHasMaxInQueue;
    }

    bool TryGetMaxPriority<T>(Dictionary<PriorityID, T> datas, out PriorityID priority) where T : ICollection, IEnumerable<NotificationData>
    {
        if (datas.Values.Any(x => x.Count > 0))
        {
            priority = datas.Where(x => x.Value.Count > 0).Max(x => x.Key);
            return true;
        }

        priority = default;
        return false;
    }

    private NotificationData Get(Dictionary<PriorityID, Stack<NotificationData>> datas, PriorityID key) => datas[key].Pop();
    private NotificationData Get(Dictionary<PriorityID, Queue<NotificationData>> datas, PriorityID key) => datas[key].Dequeue();
}
