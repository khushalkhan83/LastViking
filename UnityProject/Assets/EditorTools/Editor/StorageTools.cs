using System.IO;
using UnityEditor;
using UnityEngine;

public static class StorageTools
{
    public static string EditorSavesPath {get {return  Application.dataPath.Replace("/Assets", "/EditorSaves/");}}
    public static string LocalSavesPath {get{ return  EditorSavesPath + "_LocalSaves";}}
    public static string LocalBackupsPath {get{ return  EditorSavesPath + "_LocalBackups";}}
    public static string GlobalBackupsPath {get{ return  EditorSavesPath + "_GlobalBackups";}}



    [MenuItem("EditorTools/Saves/Clear Save files #%g")] // shift ctrl-g on Windows, shift cmd-g on macOS
    private static void ClearStorage()
    {
        if (!EditorUtility.DisplayDialog("Message", "Cleare save files ?", "Ok", "No way!!")) return;

        foreach (var path in Directory.GetFiles(LocalSavesPath))
        {
            File.Delete(path);
        }

        Debug.Log("Cleared storage");

    }
    [MenuItem("EditorTools/Saves/Open Saves Folder #%y")] // shift ctrl-g on Windows, shift cmd-y on macOS
    private static void OpenSaveFolder()
    {
        var path = LocalSavesPath;
        
        #if UNITY_EDITOR_WIN
        path = path.Replace(@"/", @"\");   // explorer doesn't like front slashes
        #endif

        EditorUtility.RevealInFinder(path);
    }

    // if folders dont exist script will create them
    [InitializeOnLoadMethod]
    private static void CreateLocalFoldersIfNoneExist()
    {
        System.IO.Directory.CreateDirectory(LocalSavesPath);
        System.IO.Directory.CreateDirectory(LocalBackupsPath);
        System.IO.Directory.CreateDirectory(GlobalBackupsPath);
    }
}
