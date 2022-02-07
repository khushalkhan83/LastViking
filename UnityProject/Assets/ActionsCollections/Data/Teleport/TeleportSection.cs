using System.Collections.Generic;
using ActionsCollections;
using UnityEngine;

namespace DebugActions
{
    public partial class DebugSections {public static TeleportSection Teleport {get;} = new TeleportSection();}
    public class TeleportSection : SectionBase
    {
        public override string SectionName => "Teleports";
        public override List<ActionBase> Actions
        {
            get => new List<ActionBase>()
                {
                    new TeleportActionGeneric("InitPlace", new Vector3(556.8083f,26.6613f,183.3604f)),
                    new TeleportActionGeneric("InitPlace 00", new Vector3(539.2904f,28.59534f,179.058f), new Vector3(0, -30.929f, 0)),
                    new TeleportActionGeneric("Waterfall After enter", new Vector3(417.692f,35.15f,184.794f)),
                    new TeleportActionGeneric("Waterfall before Enter", new Vector3(415.91f,35.15f,180.19f)),
                    new TeleportActionGeneric("Waterfall column", new Vector3(454.95f,55.45f,250.67f)),
                    new TeleportActionGeneric("Waterfall before column", new Vector3(429.1953f,41.31012f,249.7039f)),
                    new TeleportActionGeneric("Treasure hunt", new Vector3(493.36f,29.58f,141.05f)),
                };
        }
    }

}