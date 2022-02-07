using System.Collections.Generic;

namespace CustomeEditorTools.GameSaveTool
{
    [System.Serializable]
    public class FolderData
    {
        public FolderData(string folderPath, List<FolderData> subFolders)
        {
            FolderPath = folderPath;
            SubFolders = subFolders;
        }

        public string FolderPath;
        public bool HasSubFolders => SubFolders.Count > 0;
        public List<FolderData> SubFolders = new List<FolderData>();

        public string FolderName {get => FolderPath.GetFolderName();}
    }
}

