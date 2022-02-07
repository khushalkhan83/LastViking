using System;
using EnemiesAttack;
using Game.Controllers.Controllers.States;
using Game.Controllers.Controllers.States.Modificators;
using Game.Models;
using Game.Objectives.Stacks;
using Game.QuestSystem.Data;
using static Game.Models.QuestsLifecycleModel;

[Serializable] public class ColorColorPresetDictionary : SerializableDictionary<Game.Colors.ColorPreset,UnityEngine.Color> {}
[Serializable] public class IntEnvironmentTransitionDictionary : SerializableDictionary<int, EnvironmentTransition> {}
[Serializable] public class UIntWorldObjectSavableDataNewDictinary : SerializableDictionary<uint,WorldObjectSavableDataNew> {}
[Serializable] public class QuestEventQuestEventDataDictionary : SerializableDictionary<QuestEvent, QuestEventData> {}
[Serializable] public class IntQuestDataDictionary : SerializableDictionary<int, QuestData> {}
[Serializable] public class IntObjectiveStackDictionary : SerializableDictionary<int, ObjectivesStack> {}
[Serializable] public class ControllerCollectionProcessingTypeDictionary : SerializableDictionary<ControllerIDsCollectionNew, ProcessingType> {}
[Serializable] public class SideQuestEventQuestEventDataDictionary : SerializableDictionary<SideQuestEvent, SideQuestEventData> {}
[Serializable] public class IntEquipmentCategoryDictionary : SerializableDictionary<int, EquipmentCategory> {}
[Serializable] public class HouseBuildingInfoDictionary : SerializableDictionary<HouseBuildingType, HouseBuildingInfo> {}
[Serializable] public class IntEnemiesAttackConfigDictionary : SerializableDictionary<int, EnemiesAttackConfig> {}