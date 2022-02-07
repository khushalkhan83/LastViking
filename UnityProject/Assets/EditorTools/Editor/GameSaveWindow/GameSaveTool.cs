using System;
using UnityEditor;
using UnityEngine;

namespace CustomeEditorTools.GameSaveTool
{
    public static class GameSaveTool
    {
        public static string SaveFolder => StorageTools.LocalSavesPath;
        public static string SavesFolder => StorageTools.LocalBackupsPath;
        public static string GlobalSavesFolder => StorageTools.GlobalBackupsPath;

        public static string GetBackupsFolderPath()
        {
            switch (GameSaveData.Instance.BackupType)
            {
                case BackupType.Local:
                    return GameSaveTool.SavesFolder;

                case BackupType.Global:
                    return GameSaveTool.GlobalSavesFolder;

                case BackupType.GoogleDrive:
                    return GameSaveData.Instance.GoogleDriveFolderPath;

                case BackupType.InternalServer:
                    return GameSaveData.Instance.InternalServerPath;

                default:
                    return null;
            }
        }

        public static void SaveToFolder(FolderData folder)
        {
            SaveToFolder(folder.FolderPath);
        }
        public static void SaveToFolder(string folderPath)
        {
            FileUtil.ReplaceDirectory(SaveFolder, folderPath);
        }
        public static void SaveToNewFolder(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                Debug.LogError("error");
                return;
            }

            var folderPath = GameSaveData.Instance.OpenedFolderPath + "\\" + folderName;

            SaveToFolder(folderPath);
            GameSaveData.Instance.UpdateFolders();
        }

        public static void CreateSubFolder(string path)
        {
            System.IO.Directory.CreateDirectory(path);
            GameSaveData.Instance.UpdateFolders();
        }


        public static void LoadFromFolder(FolderData folder)
        {
            FileUtil.ReplaceDirectory(folder.FolderPath, SaveFolder);
        }

        public static void DeleteFolder(FolderData folder)
        {
            FileUtil.DeleteFileOrDirectory(folder.FolderPath);
            GameSaveData.Instance.UpdateFolders();
        }
    }
}