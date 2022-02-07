using System;
using Core.Storage;
using UnityEditor;
using UnityEngine;

namespace Game.Storage
{
    public class StorageEditor : StorageFiles
    {
        private string _path;
        public override string RootPath {
            get
            {
                if (String.IsNullOrEmpty(_path))
                {
                    string dataPath = Application.dataPath;
                    _path = dataPath.Replace("/Assets", "/EditorSaves/_LocalSaves");
                }
                return _path;
            }
        }
    }
}
