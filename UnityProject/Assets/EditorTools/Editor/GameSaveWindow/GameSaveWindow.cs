using UnityEngine;
using UnityEditor;

namespace CustomeEditorTools.GameSaveTool
{
    public class GameSaveWindow : EditorWindow
    {
        [MenuItem("EditorTools/Windows/GameSaveWindow")]
        private static void ShowWindow()
        {
            var window = GetWindow<GameSaveWindow>();
            window.titleContent = new GUIContent("GameSaveWindow");
            window.Show();
        }

        private GUIStyle style;
        private GUIStyle Style => style ?? (style = new GUIStyle(UnityEditor.EditorStyles.boldLabel));

        private void OnGUI()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.MaxWidth(GameSaveData.Instance.toolBarMaxWidth));
            ToolBar();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Update",GUILayout.MaxWidth(GameSaveData.Instance.updateButtonMaxWidth)))
            {
                GameSaveData.Instance.UpdateFolders();
            }

            bool error = !TryGetRelativePath(out string relativePath);

            if (error)
            {
                GUILayout.Label("Error. Cant draw window");
                if (GUILayout.Button("Reset Folder Path"))
                {
                    SetLocal();
                }
                return;
            }

            if (GUILayout.Button(relativePath, Style))
            {
                GameSaveData.Instance.OpenFolder(GameSaveTool.GetBackupsFolderPath());
            }
            GUILayout.EndHorizontal();


            

            GUILayout.Space(20);

            GUILayout.Label("Saves");

            foreach (var folder in GameSaveData.Instance.Folders)
            {
                if (GUILayout.Button(folder.HasSubFolders ? "Folder_" + folder.FolderName : folder.FolderName))
                {
                    ButtonClickedHandler(folder);
                }
            }

            GUILayout.Space(20);

            GameSaveData.Instance.NewFolderName = EditorGUILayout.TextField(GameSaveData.Instance.NewFolderName);

            if (GUILayout.Button("SaveToFolder"))
            {
                RestartTools.SaveGame();
                GameSaveTool.SaveToNewFolder(GameSaveData.Instance.NewFolderName);
            }
            if (GUILayout.Button("CreateFolder"))
            {
                GameSaveTool.CreateSubFolder(GameSaveData.Instance.OpenedFolderPath + "/" + GameSaveData.Instance.NewFolderName);
            }
        }

        private void OpenFolder(string path)
        {
#if UNITY_EDITOR_WIN
            path = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
#endif

            EditorUtility.RevealInFinder(path);
        }

        private bool TryGetRelativePath(out string answer)
        {
            bool error = false;
            try
            {
                var openedPath = GameSaveData.Instance.OpenedFolderPath;
                var fullPath = GameSaveTool.GetBackupsFolderPath();
                var mainFolderName = fullPath.GetFolderName();
                var index = openedPath.IndexOf(mainFolderName);

                answer = $"{GameSaveData.Instance.BackupType.ToString()}\\{openedPath.Substring(index)}";

                error = false;
            }
            catch (System.Exception)
            {
                answer = null;
                error = true;
            }

            return !error;
        }

        private void ButtonClickedHandler(FolderData folder)
        {
            GenericMenu menu = new GenericMenu();

            if (folder.HasSubFolders)
            {

                menu.AddItem(new GUIContent("OpenFolder"), false, OpenFolder);
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("DeleteFolder"), false, DeleteFolder);

                menu.ShowAsContext();

                void OpenFolder()
                {
                    GameSaveData.Instance.OpenFolder(folder);
                }

                void DeleteFolder()
                {
                    var details = folder.HasSubFolders ? $"Папка не пуста. Колечество сейвов {folder.SubFolders.Count}" : "";
                    bool deleteSave = EditorUtility.DisplayDialog("Внимание", $"Удалить папку?{ details}", "Ок", "Отмена");
                    if (!deleteSave) return;

                    GameSaveTool.DeleteFolder(folder);
                }
            }
            else
            {
                menu.AddItem(new GUIContent("LoadFrom"), false, LoadFrom);
                menu.AddItem(new GUIContent("SaveTo"), false, SaveTo);
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("ShowInExplorer"), false, ShowInExplorer);
                menu.AddSeparator(string.Empty);
                menu.AddItem(new GUIContent("DeleteSave"), false, DeleteSave);

                menu.ShowAsContext();

                void LoadFrom()
                {
                    GameSaveTool.LoadFromFolder(folder);
                    RestartTools.RestartCore();
                }
                void SaveTo()
                {
                    if (!EditorUtility.DisplayDialog("Внимание", "Вы уверены что хотите сохранить сейв в текищий слот?", "Да", "Охрана отмена")) return;
                    RestartTools.SaveGame();
                    GameSaveTool.SaveToFolder(folder);
                }

                void ShowInExplorer()
                {
                    OpenFolder(folder.FolderPath);
                }

                void DeleteSave()
                {
                    bool deleteSave = EditorUtility.DisplayDialog("Внимание", "Удалить сейв?", "Ок", "Отмена");
                    if (!deleteSave) return;

                    GameSaveTool.DeleteFolder(folder);
                }
            }
        }

        private void RestFolderPath()
        {
            GameSaveData.Instance.SetFolderPath(GameSaveTool.SavesFolder);
            GameSaveData.Instance.UpdateFolders();
        }

        // folder path and root name
        

        private void ToolBar()
        {
            if (GUILayout.Button("StorageType", EditorStyles.toolbarDropDown, GUILayout.MaxWidth(GameSaveData.Instance.maxToolBarButtonWidth))) SelectBackupsFolder();
            if (GUILayout.Button("OpenSelected", EditorStyles.toolbarButton, GUILayout.MaxWidth(GameSaveData.Instance.maxToolBarButtonWidth))) OpenSelected();
            if (GUILayout.Button("Settings", EditorStyles.toolbarButton, GUILayout.MaxWidth(GameSaveData.Instance.maxToolBarButtonWidth))) Settings();
            if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.MaxWidth(GameSaveData.Instance.maxToolBarButtonWidth))) Help();

            void SelectBackupsFolder()
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("LocalSaves"), false, SelectLocal);
                menu.AddItem(new GUIContent("GlobalSaves"), false, SelectGlobal);
                menu.AddItem(new GUIContent("GoogleDrive"), false, GoogleDrive);
                menu.AddItem(new GUIContent("InternalServer"), false, InternalServer);

                menu.ShowAsContext();


                void SelectLocal()
                {
                    SetLocal();
                }

                void SelectGlobal()
                {
                    GameSaveData.Instance.SetFolderPath(GameSaveTool.GlobalSavesFolder);
                    GameSaveData.Instance.SetBackupType(BackupType.Global);
                    GameSaveData.Instance.UpdateFolders();
                }

                void GoogleDrive()
                {
                    GameSaveData.Instance.SetFolderPath(GameSaveData.Instance.GoogleDriveFolderPath);
                    GameSaveData.Instance.SetBackupType(BackupType.GoogleDrive);
                    GameSaveData.Instance.UpdateFolders();
                }

                void InternalServer()
                {
                    GameSaveData.Instance.SetFolderPath(GameSaveData.Instance.InternalServerPath);
                    GameSaveData.Instance.SetBackupType(BackupType.InternalServer);
                    GameSaveData.Instance.UpdateFolders();
                }
            }

            void OpenSelected()
            {
                OpenFolder(GameSaveData.Instance.OpenedFolderPath);
            }

            void Settings()
            {
                Selection.activeObject = GameSaveData.Instance;
                EditorGUIUtility.PingObject(GameSaveData.Instance);
            }
            void Help()
            {
                Application.OpenURL(GameSaveData.Instance.documentationURL);
            }
        }

        private void SetLocal()
        {
            GameSaveData.Instance.SetFolderPath(GameSaveTool.SavesFolder);
            GameSaveData.Instance.SetBackupType(BackupType.Local);
            GameSaveData.Instance.UpdateFolders();
        }
    }
}
