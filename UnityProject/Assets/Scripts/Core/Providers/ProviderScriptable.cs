using System;
using Game.Controllers;
using UnityEngine;

namespace Core.Providers
{
    public abstract class ProviderScriptable<Key, Value> : ScriptableObject
    {
        public abstract Value this[Key key] {get;}
    }
}
