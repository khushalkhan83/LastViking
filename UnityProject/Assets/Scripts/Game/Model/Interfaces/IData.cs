using Core.Storage;
using System;
using System.Collections.Generic;

namespace Game.Models
{
    public delegate void DataProcessing(IEnumerable<IUnique> uniques);

    public interface IData
    {
        IEnumerable<IUnique> Uniques { get; }
        void UUIDInitialize();
        event Action OnDataInitialize;
    }
}
