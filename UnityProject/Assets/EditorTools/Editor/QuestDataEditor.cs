using System.Collections;
using System.Collections.Generic;
using Game.Objectives;
using Game.QuestSystem.Data;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace CustomeEditorTools
{
    public class QuestDataEditor : OdinMenuEditorWindow
    {
        private const string QuestsPath = "Assets/Data/Quests";

        [MenuItem("EditorTools/Windows/QuestDataEditor")]
        private static void OpenWindow()
        {
            GetWindow<QuestDataEditor>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.AddAllAssetsAtPath("Quests", QuestsPath, typeof(QuestData),true);
            tree.SortMenuItemsByName();

            return tree;
        }
    }
}

