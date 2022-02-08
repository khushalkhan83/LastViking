using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEditor.AddressableAssets;
//using Google.Android.AppBundle.Editor;
using System.Reflection;
using System;
// #if !UNITY_STANDALONE_OSX
// using Khepri.PlayAssetDelivery.Editor;
// using Khepri.PlayAssetDelivery.Editor.Settings.GroupSchemas;
// #endif
//using Google.Android.AppBundle.Editor.Internal;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.IO;
using Sirenix.OdinInspector;
// using Khepri.AssetDelivery.ResourceProviders;
// using Game.Features.Store.Validation.Editor;
using Game.Models;
using BuildAutomation.CustomBuildSettings;
// using Game.Features.SpecialEventStore.Validation.Editor;

namespace BuildAutomation
{

    #region Config classes
         
    namespace CustomBuildSettings
    {
        public enum BuildType
        {
            Debug,
            Release
        }

        public struct BuildSettings
        {
            private const string k_apkExtension = ".apk";

            public readonly BuildTarget BuildTarget;
            public readonly BuildType BuildType;
            public readonly bool BuildAppBundle;
            public readonly bool UseAPKExpansionFiles;
            public readonly bool GooglePlayGamesServicesAreUsed;
            public readonly bool PlayAssetDelivery;

            public BuildSettings(BuildTarget buildTarget, BuildType buildType, bool buildAppBundle = false, bool useAPKExpansionFiles = false, bool googlePlayGamesServicesAreUsed = false, bool playAssetDelivery = false)
            {
                BuildTarget = buildTarget;
                BuildType = buildType;
                BuildAppBundle = buildAppBundle;
                UseAPKExpansionFiles = useAPKExpansionFiles;
                GooglePlayGamesServicesAreUsed = googlePlayGamesServicesAreUsed;
                PlayAssetDelivery = playAssetDelivery;
            }

            public string GetBuildPath()
            {
                string platformName = BuildTarget.ToString();

                string buildPath = $"Build/{platformName}/{platformName.ToLower()}_{PlayerSettings.bundleVersion}_{BuildType.ToString().ToLower()}";

                if (BuildTarget == BuildTarget.Android)
                    buildPath += k_apkExtension;

                return buildPath;
            }
        }
    }

    public class EditorGameSettingsSetup
    {
        public bool EnableAnalitics { get; private set; }
        public bool DebugControllersSettings { get; private set; }

        public EditorGameSettingsSetup()
        {
            DebugControllersSettings = EditorGameSettings.Instance.debugControllersSettings;
            EnableAnalitics = EditorGameSettings.Instance.enableAnalitics;
        }
    }

    #endregion

    [CreateAssetMenu(fileName = "SO_BuildPipeline_Editor", menuName = "BuildPipeline/BuildPipeline", order = 0)]
    public class BuildPipeline : ScriptableObject
    {
        static BuildPipeline()
        {
            SetupBuildOptions();
            OnBuildFailed += OnBuildFailedHandler;
        }

        #region Singltone
        private const string k_pipelinePath = @"Assets/ScriptableObjects/BuildPipeline/Editor/SO_BuildPipeline_Editor.asset";
        private static BuildPipeline _instance;
        public static BuildPipeline Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (BuildPipeline)AssetDatabase.LoadAssetAtPath(k_pipelinePath, typeof(BuildPipeline));
                }
                return _instance;
            }
        }

        #endregion

        #region Configs

        private const int _bytesInMegabyte = 1048576;
        readonly static string[] _scenesPaths = new string[6]
        {
            "Assets/Intro/Scenes/IntroLoadingScene.unity",
            "Assets/Intro/Scenes/Intro.unity",
            "Assets/Scenes/LoadingScene.unity",
            "Assets/Scenes/CoreScene.unity",
            "Assets/Scenes/VikingsIslandScene_lands.unity",
            "Assets/Scenes/PreloadScene.unity",
        };

        #region BuildSettingPresets

        private static readonly BuildSettings AndroidDebugSettings = new BuildSettings(BuildTarget.Android, BuildType.Debug);
        private static readonly BuildSettings AndroidReleaseSettings = new BuildSettings(BuildTarget.Android, BuildType.Release);
        private static readonly BuildSettings AndroidGoogleReleaseSettings = 
            new BuildSettings(BuildTarget.Android, BuildType.Release, buildAppBundle: true, useAPKExpansionFiles: true, googlePlayGamesServicesAreUsed: true, playAssetDelivery: true);
        private static readonly BuildSettings IOSDebugSettings = new BuildSettings(BuildTarget.iOS, BuildType.Debug);
        private static readonly BuildSettings IOSReleaseSettings = new BuildSettings(BuildTarget.iOS, BuildType.Release);

        #endregion

        #endregion
        private static RemoteConfigModel RemoteConfigModel => ModelsSystem.Instance._remoteConfigModel;

        public static Action OnBuildFailed { get; set; }


        #region Variables
        private static BuildPlayerOptions _buildPlayerOptions;
        private static EditorGameSettingsSetup _currentEditorSetup;
             
        #endregion

        private static void StartBuild(BuildSettings buildSettings)
        {
            var buildtarget = buildSettings.BuildTarget;

            ApplyPrebuildSetup(buildtarget);

            _buildPlayerOptions.locationPathName = buildSettings.GetBuildPath();

            if (buildtarget == BuildTarget.Android)
            {
                LoadApplicationSignKeys();
                EditorUserBuildSettings.buildAppBundle = buildSettings.BuildAppBundle;

                if (buildSettings.PlayAssetDelivery)
                {
                    // SetAssetBundleProvider(typeof(AssetPackBundleAsyncProvider));
                    // EnsureAssetPackInstallTimeDeliveryMode();

                    // CreateConfig();
                }
                else
                {
                    PlayerSettings.Android.useAPKExpansionFiles = buildSettings.UseAPKExpansionFiles;

                    SetAssetBundleProvider(typeof(AssetBundleProvider));
                }
            }
            else if (buildtarget == BuildTarget.iOS)
            {
                SetAssetBundleProvider(typeof(AssetBundleProvider));
            }

            EditorGameSettings.Instance.googlePlayGamesServicesAreUsed = buildSettings.GooglePlayGamesServicesAreUsed;

            ApplyBuildSettings(buildSettings.BuildType);

            if (buildSettings.BuildAppBundle)
                BuildAppBundle();
            else
                Build(_buildPlayerOptions);

            OpenBuildFolder(buildSettings.BuildTarget.ToString());
        }


        #region Menu buttons

        [TabGroup("Android")]
        [TabGroup("iOS")]
        [Title("Version", "Project Settings -> Player -> Other Settings -> Indentification", TitleAlignments.Centered)]
        [ReadOnly, ShowInInspector]
        public static string BundleVersion { get => PlayerSettings.bundleVersion; private set => PlayerSettings.bundleVersion = value; }

        [TabGroup("Android")]
        [ReadOnly, ShowInInspector]
        public static int BundleVersionCodeAndroid { get => PlayerSettings.Android.bundleVersionCode; private set => PlayerSettings.Android.bundleVersionCode = value; }

        [TabGroup("Android")]
        [TabGroup("iOS")]
        [Button(ButtonSizes.Large)]
        public void OpenProjectSettings() => SettingsService.OpenProjectSettings("Project/Player");

        [TabGroup("Android")]
        [TabGroup("iOS")]
        [Button("Build Notes", ButtonSizes.Large)]
        public void OpenBuildNotes() => Application.OpenURL("https://docs.google.com/document/d/1RabEnZc-Gh8W5pmST-O0jgm4UBSByz6vArzxM1shDvw/edit");

        [TabGroup("Android")]
        [Button("Android supported versions Google Table", ButtonSizes.Large)]
        public void OpenAndroidSupportedVersionGoogleTable() => Application.OpenURL("https://docs.google.com/spreadsheets/d/1aBpZo4qJ0NUERncwBCn-fxF0SDa9fEwqhFK4s4oqzeU/edit#gid=0");

        [TabGroup("iOS")]
        [Button("iOS supported versions Google Table", ButtonSizes.Large)]
        public void OpenIOSSupportedVersionGoogleTable() => Application.OpenURL("https://docs.google.com/spreadsheets/d/1giHfvF3Znyg3nxeRpHtFptHFeTcw39qvIIQPCz70rYo/edit#gid=0");

        [TabGroup("Android")]
        [Title("Android", null, TitleAlignments.Centered)]
        [Button("Android Debug Build (apk)", ButtonSizes.Large)]
        public static void AndroidDebugBuild()
        {
            if(TryGetVersion(BundleVersionCodeAndroid, out int version))
            {
                BundleVersionCodeAndroid = version;
                StartBuild(AndroidDebugSettings);
            }
        }

        [TabGroup("Android")]
        [Button("Android Release Build (apk)", ButtonSizes.Large)]
        public static void AndroidReleaseBuild()
        {
            if(TryGetVersion(BundleVersionCodeAndroid, out int version))
            {
                BundleVersionCodeAndroid = version;
                StartBuild(AndroidReleaseSettings);
            }
        }

        [TabGroup("Android")]
        [Title("Google", null, TitleAlignments.Centered)]
        [Button("Android Google Release Build (aab)", ButtonSizes.Large)]
        public static void AndroidGoogleReleaseBuild()
        {
            if(TryGetVersion(BundleVersionCodeAndroid, out int version))
            {
                BundleVersionCodeAndroid = version;
                StartBuild(AndroidGoogleReleaseSettings);
            }
        }

        [TabGroup("iOS")]
        [ReadOnly, ShowInInspector]
        public static string BundleVersionCodeIOS { get => PlayerSettings.iOS.buildNumber; private set => PlayerSettings.iOS.buildNumber = value; }

        [TabGroup("iOS")]
        [Title("iOS", null, TitleAlignments.Centered)]
        [Button("iOS Debug Build", ButtonSizes.Large)]
        public static void IOSDebugBuild()
        {
            if(TryGetVersion(Int32.Parse(BundleVersionCodeIOS), out int version))
            {
                var bundleVersionCode = version;
                BundleVersionCodeIOS = bundleVersionCode.ToString();
                StartBuild(IOSDebugSettings);
            }
        }

        [TabGroup("iOS")]
        [Button("iOS Release Build", ButtonSizes.Large)]
        public static void IOSReleaseBuild()
        {
            StartBuild(IOSReleaseSettings);
        }

        [Title("Editor tools you might need", null, TitleAlignments.Centered)]
        [TabGroup("Editor tools")]
        [Button("Localization Editor", ButtonSizes.Large)]
        private static void OpenLocalizationEditor() => EditorApplication.ExecuteMenuItem("Tools/Localization");

        [TabGroup("Editor tools")]
        [Button("Addressables Groups", ButtonSizes.Large)]
        private static void OpenAddressablesGroups() => EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");

        [TabGroup("Debug")]
        [Title("Addressables", null, TitleAlignments.Centered)]
        [Button("Set addressables settings for google build", ButtonSizes.Large)]
        private static void GoogleAddressablesSetup()
        {
            // SetAssetBundleProvider(typeof(AssetPackBundleAsyncProvider));
            // EnsureAssetPackInstallTimeDeliveryMode();
        }

        [TabGroup("Debug")]
        [Button("Set addressables settings for android", ButtonSizes.Large)]
        private static void AndroidAddressables()
        {
            SetAssetBundleProvider(typeof(AssetBundleProvider));
        }

        [TabGroup("Debug")]
        [Title("Publishing settings", null, TitleAlignments.Centered)]
        [Button("Load Application Sign Keys", ButtonSizes.Large)]
        private static void LoadSignKeys() => LoadApplicationSignKeys();

        [TabGroup("Debug")]
        [Button("Delete Sign Keys", ButtonSizes.Large)]
        private static void DeleteSignKeys()
        {
            var emptyString = String.Empty;

            PlayerSettings.Android.keystoreName = emptyString;
            PlayerSettings.Android.keystorePass = emptyString;

            PlayerSettings.Android.keyaliasName = emptyString;
            PlayerSettings.Android.keyaliasPass = emptyString;
        }

        [TabGroup("Debug")]
        [Title("Localization", null, TitleAlignments.Centered)]
        [Button("Update Localization", ButtonSizes.Large)]
        private static void UpdateLocalization()
        {
            var window = EditorWindow.GetWindow<LocalizationEditor>();
            window.StartLocalization();
            window.WWWUpdate();

            EditorApplication.update += window.WWWUpdate;
        }

        #region Version pop up
        private static bool TryGetVersion(int bundleVersionCode, out int output)
        {
            output = default;
            int answer = EditorUtility.DisplayDialogComplex ("Version", "Would you like to increment bundle version Code?", "Yes", "No", "Stop Build");
            switch (answer)
            {
                // yes
                case 0:
                    output = bundleVersionCode + 1;
                    return true;

                // no
                case 1:
                    output = bundleVersionCode;
                    return true;

                // stop build
                case 2:
                return false;
                default:
                    return false;
            }
        }
        #endregion

        #region  Store Validators

        [TabGroup("Debug")]
        [Title("Store", null, TitleAlignments.Centered)]
        [Button("Validate Store Products", ButtonSizes.Large)]
        private static void ValidateStoreProducts()
        {
            try
            {
                // ProductsValidation.ValidateStoreProducts();
                // SpecialEventProductsValidation.ValidateProducts();
            }
            catch (System.Exception)
            {
                throw new BuildFailedException("Store products didn't pass the validation process!");
            }
        }

        #endregion

        [TabGroup("Debug")]
        [Title("Explorer", null, TitleAlignments.Centered)]
        [Button("Open builds folder", ButtonSizes.Large)]
        private static void OpenBuildFolder(string path)
        {
            var currentPath = Directory.GetCurrentDirectory();
            var splittedPath = currentPath.Split(new char[] { '/' });

            var resultPath = string.Empty;
            foreach (var dir in splittedPath)
            {
                resultPath += dir + "/";
                if (dir == "Assets")
                {
                    Debug.Log(resultPath);
                    break;
                }
            }

            resultPath += "/Build/" + path;
            System.Diagnostics.Process.Start(resultPath);
        }

        #endregion

        private static void SetupBuildOptions()
        {
            _buildPlayerOptions = new BuildPlayerOptions();
            _buildPlayerOptions.scenes = _scenesPaths;
        }

        private static void ApplyPrebuildSetup(BuildTarget buildTarget)
        {
            UpdateLocalization();

            //ValidateStoreProducts();

            _buildPlayerOptions.target = buildTarget;
        }

        private static void ApplyBuildSettings(BuildType buildType)
        {
            _currentEditorSetup = new EditorGameSettingsSetup();

            if (buildType == BuildType.Debug)
            {
                EditorGameSettings.Instance.debugControllersSettings = true;
                EditorGameSettings.Instance.enableAnalitics = false;

                _buildPlayerOptions.options = BuildOptions.Development;
                //EditorGameSettings.Instance.developmentRemoteSettings = true;
            }
            else if (buildType == BuildType.Release)
            {
                EditorGameSettings.Instance.debugControllersSettings = false;
                EditorGameSettings.Instance.enableAnalitics = true;

                _buildPlayerOptions.options = BuildOptions.None;
                //EditorGameSettings.Instance.developmentRemoteSettings = false;
            }
        }

        #region Andriod prebuild setups

        #region Andriod Google prebuild setup

        //step 1
//         private static void EnsureAssetPackInstallTimeDeliveryMode()
//         {
// #if !UNITY_STANDALONE_OSX
//             var groups = AddressableAssetSettingsDefaultObject.Settings.groups;

//             foreach (var group in groups)
//             {
//                 var schema = group.GetSchema<AssetPackGroupSchema>();
//                 if (schema)
//                 {
//                     Type type = schema.GetType();
//                     FieldInfo fieldInfo = type.GetField("m_DeliveryMode", BindingFlags.NonPublic | BindingFlags.Instance);

//                     fieldInfo.SetValue(schema, AssetPackDeliveryMode.InstallTime);
//                     EditorUtility.SetDirty(schema);
//                 }
//             }
// #endif
//         }

        //step 2
        private static void CreateConfig()
        {
#if !UNITY_STANDALONE_OSX
            //AssetPackBuilderMenu.CreateConfig();
#endif
        }

        // step 3
        private static void LoadApplicationSignKeys()
        {
            PlayerSettings.Android.useCustomKeystore = true;

            PlayerSettings.Android.keystoreName = @"\\Server\personal_folders\_SURV\VikingKey\LastVikingKey.keystore";

            StreamReader streamReader = new StreamReader(@"\\Server\personal_folders\_SURV\VikingKey\Password.txt");
            var password = streamReader.ReadLine();

            PlayerSettings.Android.keystorePass = password;

            PlayerSettings.Android.keyaliasName = "lastpirate";
            PlayerSettings.Android.keyaliasPass = password;
        }

        // step 4
        private static void BuildAppBundle()
        {
            //AppBundlePublisher.Build();
        }

        #endregion

        private static void SetAssetBundleProvider(Type type)
        {
            // var groups = AddressableAssetSettingsDefaultObject.Settings.groups;

            // foreach (var group in groups)
            // {
            //     var schema = group.GetSchema<BundledAssetGroupSchema>();
            //     if (!schema)
            //     {
            //         group.AddSchema<BundledAssetGroupSchema>();
            //     }

            //     Type schemaType = schema.GetType();
            //     PropertyInfo propInfo = schemaType.GetProperty("AssetBundleProviderType");

            //     var serializedType = new SerializedType
            //     {
            //         Value = type,
            //         ValueChanged = true
            //     };

            //     var type2 = typeof(BundledAssetGroupSchema);
            //     var name = "m_AssetBundleProviderType";
            //     var attr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            //     var value = type2.GetField(name, attr);

            //     value.SetValue(schema, serializedType);
            //     EditorUtility.SetDirty(schema);
            // }
        }

        #endregion

        #region iOS prebuild setup

        #endregion

        private static void Build(BuildPlayerOptions BuildPlayerOptions)
        {
            try
            {
                var buildReport = UnityEditor.BuildPipeline.BuildPlayer(BuildPlayerOptions);
                ProcessBuildSummary(buildReport);
            }
            catch (BuildFailedException exception)
            {
                OnBuildFailed?.Invoke();
                Debug.Log("Build failed! Check details below.\n" + exception);
                ResetEditorGameSettings();
            }
            ResetEditorGameSettings();
        }

        private static void ProcessBuildSummary(BuildReport buildReport)
        {
            BuildReport report = buildReport;
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize / _bytesInMegabyte + " megabytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                throw new BuildFailedException("Build failed");
            }

            ResetEditorGameSettings();
        }

        private static void ResetEditorGameSettings()
        {
            if (_currentEditorSetup != null)
            {
                EditorGameSettings.Instance.debugControllersSettings = _currentEditorSetup.DebugControllersSettings;
                EditorGameSettings.Instance.enableAnalitics = _currentEditorSetup.EnableAnalitics;
            }
            else
            {
                Debug.LogError("No editor setup provided");
            }
        }

        private static void OnBuildFailedHandler()
        {
            EditorUserBuildSettings.buildAppBundle = false;
        }
    }
}