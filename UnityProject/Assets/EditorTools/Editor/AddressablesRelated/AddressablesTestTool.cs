
using System;
using Game.Controllers;
using Game.Models;
using Game.Providers;
using UnityEditor;
using UnityEngine;

namespace CustomeEditorTools.AddressablesRelated
{
    public static class AddressablesTestTool
    {

        private static WeaponAssetsLoadingModel WeaponAssetsLoadingModel {get => ModelsSystem.Instance._weaponAssetsLoadingModel;}
        private static PlayerWeaponProvider PlayerWeaponProvider {get => ModelsSystem.Instance.playerWeaponProvider;}
        

        [MenuItem("EditorTools/Testing/LoadAllWeapons")]
        static void LoadAllWeapons()
        {
            if (!EditorInPlayMode())
            {
                EditorUtility.DisplayDialog("Error", "Enter play mode first", "Ok");
                return;
            }

            
            for (int i = 0; i < PlayerWeaponProvider.Data.Length; i++)
            {
                var data = PlayerWeaponProvider.Data[i];
                WeaponAssetsLoadingModel.WeaponPool.InstantiateAsync(data);
            }
        }

        private static bool EditorInPlayMode()
        {
            return EditorApplication.isPlaying;
        }
    }
}