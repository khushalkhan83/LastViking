using Game.Providers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class LocalizationEditor : EditorWindow
{

    #region Data
#pragma warning disable 0649

    [SerializeField] private Object _dataFolder;
    [SerializeField] private LocalizationLanguageProvider _localizationLanguageProvider;
    [SerializeField] private Object _localizationLanguageIDs;
    [SerializeField] private Object _localizationKeysIDs;

#pragma warning restore 0649
    #endregion

    public string Url => @"https://docs.google.com/spreadsheets/d/1R5r_Rv9H4XIVpC9yz3LD_kLINBzlRy3xb1xthbMWeBc/export?exportFormat=csv";

    public Object DataFolder => _dataFolder;
    public Object LocalizationLanguageIDs => _localizationLanguageIDs;
    public Object LocalizationKeysIDs => _localizationKeysIDs;

    public LocalizationLanguageProvider LocalizationLanguageProvider => _localizationLanguageProvider;

    [MenuItem("Tools/Localization")]
    static public void ShowWindow()
    {
        GetWindow<LocalizationEditor>();
    }

    string text;
    public WWW www;
    Vector3 scroll;

    public event System.Action OnDownloadEnd;
    public event System.Action OnLocalizationParced;


    public enum State {Idle, ProcessingLocalizationFromURL, Error}

    public State state;

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        {
            if (www == null && GUILayout.Button("Get localization", GUILayout.Width(100)))
            {
                StartLocalization();
                EditorApplication.update += WWWUpdate;
            }
            GUILayout.Label(Url);
        }
        GUILayout.EndHorizontal();
        scroll = GUILayout.BeginScrollView(scroll);
        {
            GUILayout.Label(text);
        }
        GUILayout.EndScrollView();
    }

    public void StartLocalization()
    {
        www = new WWW(Url);
        state = State.ProcessingLocalizationFromURL;
    }

    public void WWWUpdate()
    {
        if (www != null && (www.isDone || !string.IsNullOrEmpty(www.error)))
        {
            EditorApplication.update -= WWWUpdate;

            if (www.isDone)
            {
                Parce(www.text);
            }
            else
            {
                text = www.error;
            }
            www = null;
            OnDownloadEnd?.Invoke();
        }
    }

    public void ButtonCallback()
    {
        www = new WWW(Url);
        EditorApplication.update += WWWUpdate;
    }

    private void Parce(string text)
    {
        var linesAll = text.Split('\n');
        var headerCount = linesAll.First().Split(',').Length;

        var dictionary = new Dictionary<string, List<string>>();
        int startIndex = 0;

        var colls = GetColls(www.text.Skip(startIndex), headerCount);
        var headers = colls[0];

        foreach (var header in headers)
        {
            dictionary[header] = new List<string>();
        }

        var dataColls = colls.Skip(1).Where(x => !string.IsNullOrEmpty(x[0]) && char.IsLetter(x[0][0])); //skip headers, empty key, invalid key

        foreach (var coll in dataColls)
        {
            var i = 0;
            foreach (var value in dictionary.Values)
            {
                value.Add(coll[i++]);
            }
        }

        var dataProviders = new LocalizationDataProvider[headerCount - 1]; //without Keys row

        if(IsLocalizationUpToDate(dictionary, headers))
        {
            state = State.Idle;
            return;
        }
        else
        {
            state = State.Error;
            Debug.LogError("Localization not up to date. Updating");
        }

        for (int i = 0; i < dataProviders.Length; i++)
        {
            var path = AssetDatabase.GetAssetPath(DataFolder);
            var data = CreateInstance<LocalizationDataProvider>();
            var header = headers[i + 1];  //without Keys row
            data.Init(dictionary[header].ToArray());

            dataProviders[i] = data;

            AssetDatabase.CreateAsset(data, Path.Combine(path, header + ".asset"));
        }

        LocalizationLanguageProvider.Init(dataProviders);

        File.WriteAllText(AssetDatabase.GetAssetPath(LocalizationLanguageIDs), GetProviderIDsCode(headers.Skip(1), "Game.Models", "LocalizationLanguageID"));
        File.WriteAllText(AssetDatabase.GetAssetPath(LocalizationKeysIDs), GetProviderIDsCode(dataColls.Select(x => x[0]), "Game.Models", "LocalizationKeyID"));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        state = State.Idle;

        OnLocalizationParced?.Invoke();
        Debug.Log("Localization Parced");
    }

    private bool IsLocalizationUpToDate(Dictionary<string, List<string>> dictionary, string[] headers)
    {
        int providerIndex = 0;
        foreach (LocalizationDataProvider localizationDataProvider in _localizationLanguageProvider.Data)
        {
            string header = headers[providerIndex + 1];  //without Keys row
            List<string> currentLocalization = dictionary[header];
            if (!localizationDataProvider.Data.Equals(currentLocalization))
            {
                return false;
            }
        }

        return true;
    }

    private string GetProviderIDsCode(IEnumerable<string> ids, string @namespace, string name)
    {
        var IDPattern =
@"        {0} = {1},
";
        var idResult = new StringBuilder();

        var i = 0;
        foreach (var item in ids)
        {
            idResult.AppendFormat(IDPattern, item, ++i);
        }

        var result =
$@"
//AUTOGENERATED
namespace {@namespace}
{{
    public enum {name}
    {{
        None = 0,
{idResult}
    }}
}}";

        return result;
    }

    private List<string[]> GetColls(IEnumerable<char> text, int length)
    {
        var result = new List<string[]>();

        var isString = false;
        var isQ = false;
        var cell = string.Empty;
        var qCount = 0;
        var indexCell = 0;
        var indexColl = 0;
        var skipComa = false;

        foreach (var ch in text)
        {
            if (ch == '"')
            {
                if (isString)
                {
                    if (isQ)
                    {
                        isQ = false;

                        if (qCount > 1)
                        {
                            cell += ch;
                        }

                        --qCount;
                    }
                    else
                    {
                        isQ = true;
                        ++qCount;
                    }

                    if (qCount == 2 && isQ)
                    {
                        isString = false;
                        qCount = 0;
                        isQ = false;

                        if (indexCell < length)
                        {
                            if (result.Count == 0)
                            {
                                result.Add(new string[length]);
                            }
                            result[indexColl][indexCell++] = cell;
                            cell = string.Empty;
                            skipComa = true;
                        }
                    }
                }
                else
                {
                    ++qCount;
                    isString = true;
                }
                continue;
            }
            else if (ch == ',')
            {
                if (isString)
                {
                    cell += ch;
                }
                else if (!skipComa)
                {
                    if (indexCell < length)
                    {
                        if (result.Count == 0)
                        {
                            result.Add(new string[length]);
                        }
                        result[indexColl][indexCell++] = cell;
                        cell = string.Empty;
                    }

                    if (indexCell == length)
                    {
                        result.Add(new string[length]);
                        indexCell = 0;
                        ++indexColl;
                    }
                    cell = string.Empty;
                }
                continue;
            }
            else if (ch == '\n')
            {
                if (isString)
                {
                    cell += ch;
                }
                else
                {
                    if (result.Count == 0)
                    {
                        result.Add(new string[length]);
                        indexCell = 0;
                    }

                    if (indexCell < length)
                    {
                        result[indexColl][indexCell] = cell;
                    }

                    result.Add(new string[length]);
                    indexColl++;
                    cell = string.Empty;
                    indexCell = 0;
                }
                continue;
            }
            else if (ch == '\r')
            {
                if (isString)
                {
                    cell += ch;
                }
                continue;
            }

            cell += ch;
            skipComa = false;
            continue;

        }

        if (!string.IsNullOrEmpty(cell))
        {
            result[indexColl][indexCell++] = cell;
        }

        return result;
    }
}
