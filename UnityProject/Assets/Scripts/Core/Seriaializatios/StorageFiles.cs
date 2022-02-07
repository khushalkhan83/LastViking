using System.IO;
using UnityEngine;

namespace Core.Storage
{
    public abstract class StorageFiles : StorageBase
    {
        public abstract string RootPath { get; }

        public override void Save<T>(T obj)
        {
            try
            {
                string filePath = Path.Combine(RootPath, obj.UUID);
                if (!File.Exists(filePath)) 
                {
                    File.Create(filePath).Close();
                   
                }
                File.WriteAllText(filePath, JsonUtility.ToJson(obj));
            }
            catch (System.Exception ex)
            {
                string extraMessage = obj != null ? obj.UUID : "no id";
                Debug.LogException(new System.Exception($"Can`t save {obj} {extraMessage}, {ex.ToString()}"));
                throw ex;
            }
        }

        public override void Load<T>(T obj)
        {
            try
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(Path.Combine(RootPath, obj.UUID)), obj);
            }
            catch (System.Exception ex)
            {
                string extraMessage = obj != null ? obj.UUID : "no id";
                Debug.LogException(new System.Exception($"Can`t load {obj} {extraMessage}, {ex.ToString()}"));
                throw ex;
            }
        }

        public override bool IsHasSave<T>(T obj)
        {
            try
            {
                return File.Exists(Path.Combine(RootPath, obj.UUID));
            }
            catch (System.Exception ex)
            {
                string extraMessage = obj != null ? obj.UUID : "no id";
                Debug.LogException(new System.Exception($"Can`t Check IsHasSave {obj} {extraMessage}, {ex.ToString()}"));
                throw ex;
            }
        }

        public override void Clear<T>(T obj)
        {
            try
            {
                File.Delete(Path.Combine(RootPath, obj.UUID));
            }
            catch (System.Exception ex)
            {
                string extraMessage = obj != null ? obj.UUID : "no id";
                Debug.LogException(new System.Exception($"Can`t Clear  {obj} {extraMessage}, {ex.ToString()}"));
                throw ex;
            }
        }

        public override void ClearByUUID(string id)
        {
            try
            {
                File.Delete(Path.Combine(RootPath, id));
            }
            catch (System.Exception ex)
            {
                string extraMessage = !string.IsNullOrEmpty(id) ? id : "no id";
                Debug.LogException(new System.Exception($"Can`t Clear  {id} {extraMessage}, {ex.ToString()}"));
                throw ex;
            }
        }

        public override void ClearAll()
        {
            foreach (var path in Directory.GetFiles(RootPath))
            {
                try
                {
                    File.Delete(path);
                }
                catch (System.Exception ex)
                {
                    
                    Debug.LogException(new System.Exception($"Can`t clear all: {ex.ToString()}"));
                }
            }
        }
    }
}
