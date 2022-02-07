namespace CustomeEditorTools.GameSaveTool
{
    public static class Extensions
    {
        public static string GetFolderName(this string folderPath)
        {
            var lastDeviderIndex = folderPath.LastIndexOf('\\');
            if(lastDeviderIndex == -1)
            {
                lastDeviderIndex = folderPath.LastIndexOf('/');
            }

            string folderName = folderPath.Substring(lastDeviderIndex + 1);
            return folderName;
        }
    }
}