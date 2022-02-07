using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Game.Objectives;
using Game.Models;
using System.Text;
using Game.Objectives.Stacks;

namespace CustomeEditorTools
{

    public class GetNotUsedObjectives : EditorWindow
    {
        private ConditionID conditionID;
        private bool ignoreNotUsedObjectives;
        private List<ObjectiveID> notUsedObjectiveIds = new List<ObjectiveID>();
        private List<ConditionID> notUsedConditionIDs = new List<ConditionID>();
        private string output1;
        private string output2;
        private string output3;
        

        private static ObjectivesProvider _objectivesProvider;
        private static PlayerObjectivesModel _playerObjectivesModel;
        private Vector2 scrollPos1;
        private Vector2 scrollPos2;
        private Vector2 scrollPos3;

        private static PlayerObjectivesModel PlayerObjectivesModel
        {
            get
            {
                if (_playerObjectivesModel == null)
                {
                    _playerObjectivesModel = GameObject.FindObjectOfType(typeof(PlayerObjectivesModel)) as PlayerObjectivesModel;
                }
                return _playerObjectivesModel;
            }
        }
        private static ObjectivesProvider ObjectivesProvider
        {
            get
            {
                if (_objectivesProvider == null)
                {
                    _objectivesProvider = ModelsSystem.Instance._objectivesProvider;
                }
                return _objectivesProvider;
            }
        }

        private void OnEnable() 
        {
            _objectivesProvider = GameObject.FindObjectOfType(typeof(ObjectivesProvider)) as ObjectivesProvider;
            _playerObjectivesModel = GameObject.FindObjectOfType(typeof(PlayerObjectivesModel)) as PlayerObjectivesModel;
            _objectiveIDProvider = new ObjectiveIDProvider(ObjectivesProvider);    
        }

        private static ObjectiveIDProvider _objectiveIDProvider;
        private static ObjectiveIDProvider ObjectiveIDProvider
        {
            get
            {
                if(_objectiveIDProvider == null)
                {
                    _objectiveIDProvider = new ObjectiveIDProvider(ObjectivesProvider);
                }
                return _objectiveIDProvider;
            }
        }

        [MenuItem("EditorTools/Windows/Objectives/GetNotUsedObjectives")]
        private static void ShowWindow()
        {
            var window = GetWindow<GetNotUsedObjectives>();
            window.titleContent = new GUIContent("GetNotUsedObjectives");
            window.Show();
        }

        private void OnGUI()
        {

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginVertical();
                {
                    conditionID = (ConditionID)EditorGUILayout.EnumPopup("ConditionID:", conditionID);
                    if (GUILayout.Button("Analize All Objectives In Project")) AnalizeAllObjectivesInProject();
                    GUILayout.Space(10);

                    scrollPos1 =
                        EditorGUILayout.BeginScrollView(scrollPos1);
                    EditorGUILayout.TextArea(output1);
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            {
                EditorGUILayout.BeginVertical();
                {
                    ignoreNotUsedObjectives = EditorGUILayout.Toggle("ignoreNotUsedObjectives", ignoreNotUsedObjectives);
                    if (GUILayout.Button("Analize All Objectives Stacks In Project")) AnalizeAllObjectivesStacksInProject();
                    GUILayout.Space(10);

                    scrollPos2 =
                        EditorGUILayout.BeginScrollView(scrollPos2);
                    EditorGUILayout.TextArea(output2);
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            {
                EditorGUILayout.BeginVertical();
                {
                    if(GUILayout.Button("Analize objectives thet repeating in stacks")) AnalizeObjectivesRepeatingInStacks();

                    scrollPos3 =
                        EditorGUILayout.BeginScrollView(scrollPos3);
                    EditorGUILayout.TextArea(output3);
                    EditorGUILayout.EndScrollView();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AnalizeAllObjectivesInProject() => Analize(GetAllObjectiveDatasInProject());

        private void Analize(List<ObjectiveData> selectedObjectives)
        {
            ObjectiveIDProvider.Reset();

            List<ConditionID> conditionIDsInSelectedObjectives = new List<ConditionID>();
            selectedObjectives.ForEach(x =>
            {
                var conditions = x.Conditions.ToList();
                foreach (var c in conditions)
                {
                    if (!conditionIDsInSelectedObjectives.Contains(c.ConditionID))
                    {
                        conditionIDsInSelectedObjectives.Add(c.ConditionID);
                    }
                }
            });

            notUsedConditionIDs = new List<ConditionID>();
            var allConditionIds = Helpers.EnumsHelper.GetValues<ConditionID>().ToList();
            allConditionIds.ForEach(x =>
            {
                if (notUsedConditionIDs.Contains(x)) return;
                if (conditionIDsInSelectedObjectives.Contains(x)) return;

                notUsedConditionIDs.Add(x);
            });
            List<ConditionID> usedConditionIds = new List<ConditionID>();
            allConditionIds.ForEach(x =>
            {
                if (notUsedConditionIDs.Contains(x)) return;

                usedConditionIds.Add(x);
            });


            List<ObjectiveID> objectivesInTiers = new List<ObjectiveID>();

            foreach (var tier in PlayerObjectivesModel.Tiers)
            {
                var tierObjectives = tier.Configurations.ToList().SelectMany(x => x.ObjectiveIDs).ToList();
                objectivesInTiers.AddRange(tierObjectives);
            }

            var distinctObjectivesInTiers = new List<ObjectiveID>();
            var repeatableObjectivesInTiers = new List<ObjectiveID>();

            foreach (var objective in objectivesInTiers)
            {
                if (distinctObjectivesInTiers.Contains(objective))
                {
                    if (!repeatableObjectivesInTiers.Contains(objective))
                        repeatableObjectivesInTiers.Add(objective);

                    continue;
                }

                distinctObjectivesInTiers.Add(objective);
            }

            var temp = GetTiersThatIncludeObjectives(repeatableObjectivesInTiers);
            var repetableTiers = temp.Item1;
            var repetableTiersIndexes = temp.Item2;

            notUsedObjectiveIds = new List<ObjectiveID>();
            var allObjectiveIds = Helpers.EnumsHelper.GetValues<ObjectiveID>().ToList();
            foreach (var id in allObjectiveIds)
            {
                if (distinctObjectivesInTiers.Contains(id)) continue;

                notUsedObjectiveIds.Add(id);
            }

            var selectedWithSelectedObjective = new List<ObjectiveData>(); 
            selectedObjectives.ForEach(x => 
            {
                var conditions =  x.Conditions.Select(y => y.ConditionID);
                if(conditions.Contains(conditionID))
                {
                    if(!selectedWithSelectedObjective.Contains(x))
                        selectedWithSelectedObjective.Add(x);
                }
            });


            StringBuilder sb = new StringBuilder();

            sb.AppendLine("selected objectives: " + selectedObjectives.Count);
            sb.AppendLine("objectives in tiers: " + objectivesInTiers.Count);
            sb.AppendLine("distinctObjectivesInTiers: " + distinctObjectivesInTiers.Count);
            sb.AppendLine("repeatableObjectivesInTiers: " + repeatableObjectivesInTiers.Count);
            sb.AppendLine("");
            repeatableObjectivesInTiers.ForEach(x => sb.AppendLine(x.ToString()));
            sb.AppendLine("");
            sb.AppendLine("repetableTiers: " + repetableTiers.Count);
            sb.AppendLine("");
            repetableTiersIndexes.ForEach(x => sb.AppendLine(x.ToString()));
            sb.AppendLine("");
            sb.AppendLine("notUsedObjectiveIds: " + notUsedObjectiveIds.Count);
            sb.AppendLine("");
            notUsedObjectiveIds.ForEach(x => sb.AppendLine(x.ToString()));
            sb.AppendLine("");
            sb.AppendLine("notUsedConditionIDs: " + notUsedConditionIDs.Count);
            sb.AppendLine("");
            notUsedConditionIDs.ForEach(x => sb.AppendLine(x.ToString()));
            sb.AppendLine("");
            sb.AppendLine("usedConditionIds: " + usedConditionIds.Count);
            sb.AppendLine("");
            usedConditionIds.ForEach(x => sb.AppendLine(x.ToString()));
            sb.AppendLine("");
            sb.AppendLine("selectedWithSelectedObjective: " + selectedWithSelectedObjective.Count);
            sb.AppendLine("");
            selectedWithSelectedObjective.ForEach(x => sb.AppendLine(x.ToString()));
            sb.AppendLine("");
            output1 = sb.ToString();
        }

        private static (List<PlayerObjectivesModel.TierData>, List<int>) GetTiersThatIncludeObjectives(List<ObjectiveID> objectiveIds)
        {
            var answer = new List<PlayerObjectivesModel.TierData>();
            var answerInt = new List<int>();

            foreach (var tier in PlayerObjectivesModel.Tiers)
            {
                var tierObjectives = tier.Configurations.ToList().SelectMany(x => x.ObjectiveIDs).ToList();

                foreach (var tO in tierObjectives)
                {
                    if (objectiveIds.Contains(tO))
                    {
                        if (!answer.Contains(tier))
                        {
                            answer.Add(tier);
                            answerInt.Add(PlayerObjectivesModel.Tiers.ToList().IndexOf(tier));
                        }
                    }
                }
            }

            return (answer, answerInt);
        }

        private List<ObjectiveData> GetAllObjectiveDatasInProject()
        {
            List<ObjectiveData> answer = new List<ObjectiveData>();

            foreach (string guid in AssetDatabase.FindAssets("t:objectiveData", null))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var objectiveData = AssetDatabase.LoadAssetAtPath(path,typeof(ObjectiveData)) as ObjectiveData;
                answer.Add(objectiveData);
            }
            return answer;
        }
        
        private List<ObjectivesStack> GetAllObjectiveStacksInProject()
        {
            List<ObjectivesStack> answer = new List<ObjectivesStack>();

            foreach (string guid in AssetDatabase.FindAssets("t:objectivesstack", null))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var objectiveData = AssetDatabase.LoadAssetAtPath(path,typeof(ObjectivesStack)) as ObjectivesStack;
                answer.Add(objectiveData);
            }
            return answer;
        }

        private void AnalizeAllObjectivesStacksInProject() => AnalizeObjectiveStacks(GetAllObjectiveStacksInProject());
        private void AnalizeObjectivesRepeatingInStacks() => AnalizeObjectivesRepeatingInStacks(GetAllObjectiveStacksInProject());
        private void AnalizeObjectiveStacks(List<ObjectivesStack> stacks)
        {
            ObjectiveIDProvider.Reset();

            StringBuilder sb = new StringBuilder();

            List<ObjectiveData> allObjectivesInProject = GetAllObjectiveDatasInProject();

            List<ObjectivesStack.OjbectiveStackData> stacksData = stacks.SelectMany(x => x.Datas).ToList();
            List<ObjectiveData> allObjectivesInStacks = stacksData.Select(y => y.Objective).ToList();

            var objectivesNotInStacks = new List<ObjectiveData>();
            foreach (var item in allObjectivesInProject)
            {
                if(allObjectivesInStacks.Contains(item)) continue;

                if(ignoreNotUsedObjectives)
                {
                    ObjectiveID id = ObjectiveIDProvider.GetID(item);
                    bool exclude = notUsedObjectiveIds.Contains(id);
                    if(exclude)
                        continue;
                }

                objectivesNotInStacks.Add(item);
            }
            var ignoredCount = (ignoreNotUsedObjectives ? notUsedObjectiveIds.Count - 1 : 0); // -1 because of none id

            sb.AppendLine("ignore = " + ignoreNotUsedObjectives + " , ignored: " + ignoredCount);
            sb.AppendLine("Objectives not in stacks:" + objectivesNotInStacks.Count);
            sb.AppendLine();
            objectivesNotInStacks.ForEach(x => sb.AppendLine(x.name));
            sb.AppendLine();
            sb.AppendLine("All objectives in stacks" + allObjectivesInStacks.Count);
            allObjectivesInStacks.ForEach(x => sb.AppendLine(x.name));

            output2 = sb.ToString();
        }
        private void AnalizeObjectivesRepeatingInStacks(List<ObjectivesStack> stacks)
        {
            Dictionary<ObjectiveData,List<ObjectivesStack>> stacksPerObjective = new Dictionary<ObjectiveData, List<ObjectivesStack>>();
            output3 = string.Empty;
            StringBuilder sb = new StringBuilder();


            foreach (var stack in stacks)
            {
                foreach (var data in stack.Datas)
                {
                    var objective = data.Objective;
                    bool stcksExist = stacksPerObjective.TryGetValue(objective, out var stcks);

                    if(stcksExist)
                    {
                        stcks.Add(stack);
                    }
                    else
                    {
                        stacksPerObjective.Add(objective,new List<ObjectivesStack>(){stack});
                    }
                }
            }
            
            var dublicates = stacksPerObjective.Where(x => x.Value != null && x.Value.Count > 1).ToList();

            foreach (var dublicate in dublicates)
            {
                sb.AppendLine(dublicate.Key.ToString());

                foreach (var stack in dublicate.Value)
                {
                    sb.AppendLine(stack.name);
                }

                sb.AppendLine();
            }

            output3 = sb.ToString();
        }
    }

}