namespace Helpers
{
    public static class ApplicationHelper
    {
        public static bool IsEditorApplication()
        {
            #if UNITY_EDITOR
            return true;
            #else
            return false;
            #endif
        }
    }
}
