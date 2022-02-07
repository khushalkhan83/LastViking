using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using System.IO;

namespace CustomeEditorTools.GameSaveTool
{

    public enum BackupType
    {
        Local,
        Global,
        GoogleDrive,
        InternalServer
    }

    [CreateAssetMenu(fileName = "GameSaveData", menuName = "EditorTools/GameSaveData", order = 0)]
    public class GameSaveData : ScriptableObject
    {

        #region Singltone
        private static GameSaveData instance;
        public static GameSaveData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<GameSaveData>("GameSaveData");
                }
                return instance;
            }
        }
        #endregion

        #region Layout
            
        public float maxToolBarButtonWidth = 1000;
        public float toolBarMaxWidth = 250;
        public float updateButtonMaxWidth = 50;

        #endregion

        [SerializeField] private string openedFolderPath;
        [SerializeField] private string googleDriveFolderPathRussian;
        [SerializeField] private string googleDriveFolderPathEnglish;
        [SerializeField] private string internalServerPath;

        [SerializeField] private BackupType backupType;
        public List<FolderData> Folders = new List<FolderData>();
        public string NewFolderName;

        public string OpenedFolderPath => openedFolderPath;
        public BackupType BackupType => backupType;

        public string InternalServerPath => internalServerPath;
        public string GoogleDriveFolderPath
        {
            get
            {
                if(Application.systemLanguage == SystemLanguage.English)
                    return googleDriveFolderPathEnglish;
                if(Application.systemLanguage == SystemLanguage.Russian)
                    return googleDriveFolderPathRussian;
                else
                {
                    Debug.LogError("Error here");
                    return null;
                }
            }
        }

        [Header("Documentation")]
        public string documentationURL;

        public void SetFolderPath(string value) => openedFolderPath = value;
        public void SetBackupType(BackupType type) => backupType = type;


        [Button]
        public void UpdateFolders()
        {
            Folders = GetAllSubFolders(string.IsNullOrEmpty(OpenedFolderPath) ? GameSaveTool.SavesFolder : OpenedFolderPath);
        }

        public void OpenFolder(string folderPath)
        {
            openedFolderPath = folderPath;
            UpdateFolders();
        }

        public void OpenFolder(FolderData folder)
        {
            OpenFolder(folder.FolderPath);
        }


        private List<FolderData> GetAllSubFolders(string path)
        {
            List<FolderData> answer = new List<FolderData>();
            var directories = Directory.GetDirectories(path);

            foreach (var dir in directories)
            {
                var subFolders = GetAllSubFolders(dir);
                var folder = new FolderData(dir, subFolders);
                answer.Add(folder);
            }

            return answer;
        }
    }
}
