using System.Collections.Generic;
using System.Text;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;
using UnityEngine.Profiling;
using System.Linq;
using System.Collections;

namespace DebugActions
{
    public class PerfomanceActionDisplayAssetFileSizeToConsole<T> : ActionBase
    {
        private string _operationName;
        private int _messagesPerLog;
        private bool _noLimitForMessagesPerLog;

        public PerfomanceActionDisplayAssetFileSizeToConsole(string name, int messagesPerLog = 50)
        {
            _messagesPerLog = messagesPerLog;
            if(messagesPerLog < 0) _messagesPerLog = 1;

            _operationName = name;
        }
        public PerfomanceActionDisplayAssetFileSizeToConsole(string name, bool noLimitForMessagesPerLog)
        {
            _noLimitForMessagesPerLog = noLimitForMessagesPerLog;

            _operationName = name;
        }
        public override string OperationName => _operationName;

        List<KeyValuePair<string, long>> files = new List<KeyValuePair<string, long>>();

        public override void DoAction()
        {
            #if ENABLE_PROFILER
            var assets = Resources.FindObjectsOfTypeAll(typeof(T));

            StringBuilder sb = new StringBuilder();
            float totalSizeInMb = 0;
            

            foreach (var asset in assets)
            {
                var size = Profiler.GetRuntimeMemorySizeLong(asset);

                files.Add(new KeyValuePair<string, long>(asset.name + " using: " + GetAssetSize(size,true),size));
            }

            var myList = files.ToList();

            int index = 0;

            files.Sort(
                delegate (KeyValuePair<string, long> pair1,
                KeyValuePair<string, long> pair2)
                {
                    return pair2.Value.CompareTo(pair1.Value);
                }
            );

            Debug.Log(_operationName + " total size: " + totalSizeInMb + "Mb");


            var messagesPerLog = _noLimitForMessagesPerLog ?  files.Count: _messagesPerLog;
            for (int i = 0; i < files.Count; i++)
            {
                sb.AppendLine(files[i].Key);
                index++;
                if(index > messagesPerLog)
                {
                    Debug.Log(sb.ToString());
                    sb.Clear();
                    index = 0;
                    continue;
                }
            }

            Debug.Log(sb.ToString());


            string GetAssetSize(long bytes, bool registerSize)
            {
                float bytesConverted = (float)bytes;
                float fileSizeInMb = bytesConverted * Mathf.Pow(10,-6);

                if(registerSize)
                    totalSizeInMb += fileSizeInMb;

                return fileSizeInMb + "Mb";
            }

            #else
            Debug.LogError("Cant display profile info. Create build with Profiling enabled");
            #endif
        }
    }
}