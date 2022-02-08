using UnityEditor;
using Sirenix.OdinInspector.Editor;

namespace BuildAutomation
{
    public class BuildPipelineWindow : OdinEditorWindow
    {
        [MenuItem("EditorTools/Build/Build Pipeline")]
        public static void OpenWindow()
        {
            GetWindow<BuildPipelineWindow>().Show();
        }

        protected override object GetTarget()
        {
            return BuildPipeline.Instance;
        }

        protected override void OnGUI()
        {
            try
            {
                base.OnGUI(); 
            }
            catch (System.Exception)
            {
            }
        }
    }
}