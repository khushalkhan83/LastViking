#if UNITY_EDITOR
using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class ItemManagementWindow : EditorWindow
{
    public enum Tab { ItemEditor, PropertyEditor }

    private const float DESCRIPTION_HEIGHT = 54f;
    private const float PROPERTY_HEIGHT = 40f;
    private const float RAW_LIST_HEIGHT = 38f;
    private const float LINE_PROPERTY_HEIGTH = 20f;

    private Tab m_SelectedTab;
    private SerializedObject m_ItemDatabase;
    private ReorderableList m_CategoryList;
    private ReorderableList m_PropertyList;

    private Vector2 m_CategoriesScrollPos;
    private Vector2 m_TypesScrollPos;
    private Vector2 m_PropsScrollPos;
    private Vector2 m_ItemsScrollPos;
    private Vector2 m_ItemInspectorScrollPos;

    private ReorderableList m_ItemList;
    private ReorderableList m_CurItemDescriptions;
    private ReorderableList m_CurItemProperties;
    private ReorderableList m_CurItemRequiredItems;
    private ReorderableList m_CurItemRequiredUnlockablesItems;

    private string[] m_ItemNamesFull;
    private string[] m_ItemNames;


    [MenuItem("Tools/Ultimate Survival/Item Management")]
    public static void Init()
    {
        EditorWindow.GetWindow<ItemManagementWindow>(false, "Item Management");
    }

    public void OnGUI()
    {
        if (m_ItemDatabase == null)
        {
            EditorGUILayout.HelpBox("No ItemDatabase was found in the Resources folder!", MessageType.Error);

            if (GUILayout.Button("Refresh"))
                InitializeWindow();

            if (m_ItemDatabase == null)
                return;
        }

        GUIStyle richTextStyle = new GUIStyle() { richText = true, alignment = TextAnchor.UpperRight };

        // Display the database path.
        EditorGUILayout.LabelField(string.Format("Database path: '{0}'", AssetDatabase.GetAssetPath(m_ItemDatabase.targetObject)));

        // Display the shortcuts
        EditorGUI.LabelField(new Rect(position.width - 262f, 0f, 256f, 16f), "<b>Shift + D</b> to duplicate", richTextStyle);
        EditorGUI.LabelField(new Rect(position.width - 262f, 16f, 256f, 16f), "<b>Delete</b> to delete", richTextStyle);

        Vector2 buttonSize = new Vector2(192f, 32f);
        float topPadding = 32f;

        // Draw the "Item Editor" button.
        Rect itemEditorButtonRect = new Rect(position.width * 0.25f - buttonSize.x / 2f, topPadding, buttonSize.x, buttonSize.y);

        if (m_SelectedTab == Tab.ItemEditor)
            GUI.backgroundColor = Color.grey;
        else
            GUI.backgroundColor = Color.white;

        if (GUI.Button(itemEditorButtonRect, "Item Editor"))
            m_SelectedTab = Tab.ItemEditor;

        // Draw the "Property Editor" button.
        Rect propertyEditorButtonRect = new Rect(position.width * 0.75f - buttonSize.x / 2f, topPadding, buttonSize.x, buttonSize.y);

        if (m_SelectedTab == Tab.PropertyEditor)
            GUI.backgroundColor = Color.grey;
        else
            GUI.backgroundColor = Color.white;

        if (GUI.Button(propertyEditorButtonRect, "Property Editor"))
            m_SelectedTab = Tab.PropertyEditor;

        // Reset the bg color.
        GUI.backgroundColor = Color.white;

        // Horizontal line.
        GUI.Box(new Rect(0f, topPadding + buttonSize.y * 1.25f, position.width, 1f), "");

        // Draw the item / recipe editors.
        m_ItemDatabase.Update();

        float innerWindowPadding = 8f;
        Rect innerWindowRect = new Rect(innerWindowPadding, topPadding + buttonSize.y * 1.25f + innerWindowPadding, position.width - innerWindowPadding * 2f, position.height - (topPadding + buttonSize.y * 1.25f + innerWindowPadding * 4.5f));

        // Inner window box.
        GUI.backgroundColor = Color.grey;
        GUI.Box(innerWindowRect, "");
        GUI.backgroundColor = Color.white;

        if (m_SelectedTab == Tab.ItemEditor)
            DrawItemEditor(innerWindowRect);
        else if (m_SelectedTab == Tab.PropertyEditor)
            DrawPropertyEditor(innerWindowRect);

        m_ItemDatabase.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        InitializeWindow();

        Undo.undoRedoPerformed += Repaint;
    }

    private void InitializeWindow()
    {
        itemEditorState = ItemEditorState.categoryView;
        
        var database = Resources.LoadAll<ItemDatabase>(string.Empty)[0];

        if (database)
        {
            m_ItemDatabase = new SerializedObject(database);

            m_CategoryList = new ReorderableList(m_ItemDatabase, m_ItemDatabase.FindProperty("m_Categories"), true, true, true, true);
            m_CategoryList.drawElementCallback += DrawCategory;
            m_CategoryList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, string.Empty);
            m_CategoryList.onSelectCallback += On_SelectedCategory;
            m_CategoryList.onRemoveCallback = l => m_CategoryList.serializedProperty.DeleteArrayElementAtIndex(m_CategoryList.index);

            m_PropertyList = new ReorderableList(m_ItemDatabase, m_ItemDatabase.FindProperty("m_ItemProperties"), true, true, true, true);
            m_PropertyList.drawElementCallback += DrawItemPropertyDefinition;
            m_PropertyList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, string.Empty);
        }
    }

    private void On_SelectedCategory(ReorderableList list)
    {
        m_ItemList = new ReorderableList(m_ItemDatabase, m_CategoryList.serializedProperty.GetArrayElementAtIndex(m_CategoryList.index).FindPropertyRelative("m_Items"), true, true, true, true);
        m_ItemList.drawElementCallback += DrawItem;
        m_ItemList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, string.Empty);
        m_ItemList.onSelectCallback += On_SelectedItem;
        m_ItemList.onRemoveCallback = l => m_ItemList.serializedProperty.DeleteArrayElementAtIndex(m_ItemList.index);
        m_ItemList.onChangedCallback += On_SelectedItem;
        m_ItemList.onAddCallback = l => AddNewItem(l);
    }

    private void AddNewItem(ReorderableList l)
    {
        l.serializedProperty.InsertArrayElementAtIndex(l.index);
        l.index++;

        var id = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("m_Id");
        var category = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("m_Category");
        var newId = m_ItemDatabase.FindProperty("_itemIdCurrent");

        newId.intValue++;
        id.SetObscuredInt(newId.intValue);// newId.intValue;
        category.SetObscuredString(m_ItemDatabase.FindProperty("m_Categories").GetArrayElementAtIndex(m_CategoryList.index).FindPropertyRelative("m_Name").stringValue);
    }

    private void On_SelectedItem(ReorderableList list)
    {
        if (m_ItemList == null || m_ItemList.count == 0 || m_ItemList.index == -1 || m_ItemList.index >= m_ItemList.count)
            return;

        itemEditorState = ItemEditorState.itemView;

        m_ItemNames = ItemManagementUtility.GetItemNames(m_CategoryList.serializedProperty);
        m_ItemNamesFull = ItemManagementUtility.GetItemNamesFull(m_CategoryList.serializedProperty);

        m_CurItemDescriptions = new ReorderableList(m_ItemDatabase, m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index).FindPropertyRelative("m_Descriptions"), true, true, true, true);
        m_CurItemDescriptions.drawHeaderCallback = rect => EditorGUI.LabelField(rect, string.Empty);
        m_CurItemDescriptions.drawElementCallback += DrawItemDescription;
        m_CurItemDescriptions.elementHeight = DESCRIPTION_HEIGHT;

        m_CurItemProperties = new ReorderableList(m_ItemDatabase, m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index).FindPropertyRelative("m_PropertyValues"), true, true, true, true);
        m_CurItemProperties.drawHeaderCallback = rect => EditorGUI.LabelField(rect, string.Empty);
        m_CurItemProperties.drawElementCallback += DrawItemPropertyValue;
        m_CurItemProperties.elementHeight = PROPERTY_HEIGHT;

        m_CurItemRequiredItems = new ReorderableList(m_ItemDatabase, m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index).FindPropertyRelative("m_Recipe").FindPropertyRelative("m_RequiredItems"), true, true, true, true);
        m_CurItemRequiredItems.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Required Items");
        m_CurItemRequiredItems.drawElementCallback += DrawRequiredItem;
        m_CurItemRequiredItems.elementHeight = LINE_PROPERTY_HEIGTH;

        m_CurItemRequiredUnlockablesItems = new ReorderableList(m_ItemDatabase, m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index).FindPropertyRelative("m_UnlockablesItems"), true, true, true, true);
        m_CurItemRequiredUnlockablesItems.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Required Items");
        m_CurItemRequiredUnlockablesItems.drawElementCallback = DrawRequiredUnlockableItem;
        m_CurItemRequiredUnlockablesItems.elementHeight = LINE_PROPERTY_HEIGTH;
    }

    private void DrawItemDescription(Rect rect, int index, bool isActive, bool isFocused)
    {
        var list = m_CurItemDescriptions;

        if (list.serializedProperty.arraySize == index)
            return;

        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2f;
        rect.height -= 2f;
        element.stringValue = EditorGUI.TextArea(rect, element.stringValue);

        ItemManagementUtility.DoListElementBehaviours(list, index, isFocused, this);
    }

    private void DrawItemPropertyValue(Rect rect, int index, bool isActive, bool isFocused)
    {
        var list = m_CurItemProperties;

        if (list.serializedProperty.arraySize == index)
            return;

        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        rect.y += 2f;
        rect.height -= 2f;
        ItemManagementUtility.DrawItemProperty(rect, element, m_PropertyList);

        ItemManagementUtility.DoListElementBehaviours(list, index, isFocused, this);
    }

    private void DrawRequiredItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        var list = m_CurItemRequiredItems;

        if (list.serializedProperty.arraySize == index)
            return;

        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        var name = element.FindPropertyRelative("m_Name");
        var amount = element.FindPropertyRelative("m_Amount");

        var fullRect = new Rect(rect);

        rect.y += 2f;
        rect.height -= 2f;

        // Name field.
        rect.width = fullRect.width - 64;
        rect.height = 16f;

        int selectedIndex = ItemManagementUtility.GetItemIndex(m_CategoryList.serializedProperty, name.GetObscuredStringValue());
        int oldIndex = selectedIndex;
        selectedIndex = EditorGUI.Popup(rect, selectedIndex, m_ItemNamesFull);
        if(selectedIndex != oldIndex)
        {
            name.SetObscuredString(m_ItemNames[Mathf.Clamp(selectedIndex, 0, 9999999)]);
        }

        // Amount.
        rect.x = rect.xMax + 4f;
        rect.width = 16f;
        GUI.Label(rect, "x");

        rect.x = rect.xMax;
        rect.width = fullRect.xMax - rect.x;
        EditorGUI.PropertyField(rect, amount, GUIContent.none);

        ItemManagementUtility.DoListElementBehaviours(list, index, isFocused, this);
    }

    private void DrawRequiredUnlockableItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        var list = m_CurItemRequiredUnlockablesItems;

        if (list.serializedProperty.arraySize == index)
            return;

        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        var name = element.FindPropertyRelative("m_Name");
        var amount = element.FindPropertyRelative("m_Amount");

        var fullRect = new Rect(rect);

        rect.y += 2f;
        rect.height -= 2f;

        // Name field.
        rect.width = fullRect.width - 64;
        rect.height = 16f;

        int selectedIndex = ItemManagementUtility.GetItemIndex(m_CategoryList.serializedProperty, name.GetObscuredStringValue());
        int oldIndex = selectedIndex;
        selectedIndex = EditorGUI.Popup(rect, selectedIndex, m_ItemNamesFull);
        if(selectedIndex != oldIndex)
        {
            name.SetObscuredString(m_ItemNames[Mathf.Clamp(selectedIndex, 0, 9999999)]);
        }

        // Amount.
        rect.x = rect.xMax + 4f;
        rect.width = 16f;
        GUI.Label(rect, "x");

        rect.x = rect.xMax;
        rect.width = fullRect.xMax - rect.x;
        EditorGUI.PropertyField(rect, amount, GUIContent.none);

        ItemManagementUtility.DoListElementBehaviours(list, index, isFocused, this);
    }

    private enum ItemEditorState {categoryView, itemView}
    private ItemEditorState itemEditorState;

    private void DrawItemEditor(Rect totalRect)
    {
        // Inner window cross (partitioning in 4 smaller boxes)
        GUI.Box(new Rect(totalRect.x, totalRect.y + totalRect.height * 0.5f, totalRect.width * 0.35f, 1f), "");
        GUI.Box(new Rect(totalRect.x + totalRect.width * 0.35f, totalRect.y, 1f, totalRect.height), "");

        Vector2 labelSize = new Vector2(192f, 20f);

        if(itemEditorState == ItemEditorState.categoryView)
        {
            // Draw the item list.
            string itemListName = string.Format("Item List ({0})", (m_CategoryList.count == 0 || m_CategoryList.index == -1) ? "None" : m_CategoryList.serializedProperty.GetArrayElementAtIndex(m_CategoryList.index).FindPropertyRelative("m_Name").stringValue);

            GUI.Box(new Rect(totalRect.x + totalRect.width * 0.35f / 2 - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), itemListName);
            Rect itemListRect = new Rect(totalRect.x, totalRect.y + labelSize.y, totalRect.width * 0.35f - 2f, totalRect.height * 0.5f - labelSize.y - 1f);

            if (m_CategoryList.count != 0 && m_CategoryList.index != -1 && m_CategoryList.index < m_CategoryList.count)
                DrawList(m_ItemList, itemListRect, ref m_ItemsScrollPos);
            else
            {
                itemListRect.x += 8f;
                GUI.Label(itemListRect, "Select a category...", new GUIStyle() { fontStyle = FontStyle.BoldAndItalic });
            }

            // Draw the categories.
            GUI.Box(new Rect(totalRect.x + totalRect.width * 0.35f / 2 - labelSize.x * 0.5f, totalRect.y + totalRect.height * 0.5f + 2f, labelSize.x, labelSize.y), "Category List");

            
            DrawList(m_CategoryList, new Rect(totalRect.x, totalRect.y + totalRect.height * 0.5f + labelSize.y + 2f, totalRect.width * 0.35f - 2f, totalRect.height * 0.5f - labelSize.y - 3f), ref m_CategoriesScrollPos);

        }
        else if (itemEditorState == ItemEditorState.itemView)
        {
            // Inspector label.
            GUI.Box(new Rect(totalRect.x + totalRect.width * 0.7f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), "Item Inspector");

            // Draw the inspector.
            bool itemIsSelected = m_CategoryList.count != 0 && m_ItemList != null && m_ItemList.count != 0 && m_ItemList.index != -1 && m_ItemList.index < m_ItemList.count && m_CurItemDescriptions != null;
            Rect inspectorRect = new Rect(totalRect.x + totalRect.width * 0.35f + 4f, totalRect.y + labelSize.y, totalRect.width * 0.65f - 5f, totalRect.height - labelSize.y - 1f);
            
            
            if (itemIsSelected)
                DrawItemInspector(inspectorRect);
            else
            {
                inspectorRect.x += 4f;
                inspectorRect.y += 4f;
                GUI.Label(inspectorRect, "Select an item to inspect...", new GUIStyle() { fontStyle = FontStyle.BoldAndItalic });
            }
        }

    }

    private void DrawList(ReorderableList list, Rect totalRect, ref Vector2 scrollPosition)
    {
        float scrollbarWidth = 16f;

        Rect onlySeenRect = new Rect(totalRect.x, totalRect.y, totalRect.width, totalRect.height);
        Rect allContentRect = new Rect(totalRect.x, totalRect.y, totalRect.width - scrollbarWidth, (list.count + 4) * list.elementHeight);

        scrollPosition = GUI.BeginScrollView(onlySeenRect, scrollPosition, allContentRect, false, true);

        // Draw the clear button.
        Vector2 buttonSize = new Vector2(56f, 16f);

        if (list.count > 0 && GUI.Button(new Rect(allContentRect.x + 2f, allContentRect.yMax - 60f, buttonSize.x, buttonSize.y), "Clear"))
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure you want the list to be cleared? (All elements will be deleted)", "Yes", "Cancel"))
                list.serializedProperty.ClearArray();

        list.DoList(allContentRect);
        GUI.EndScrollView();
    }

    private void DrawListElement(ReorderableList list, Rect rect, int index, bool isActive, bool isFocused)
    {
        if (list.serializedProperty.arraySize == index)
            return;

        var element = list.serializedProperty.GetArrayElementAtIndex(index);

        rect.y += 2;
        var r = rect;
        var m_Name = element.FindPropertyRelative("m_Name");
        var m_Type = element.FindPropertyRelative("m_Type");

        const float SPACE = 16;
        const float WIDTH_TYPE = 156;

        r.xMax -= WIDTH_TYPE - SPACE;
        EditorGUI.PropertyField(r, m_Name, GUIContent.none);

        r.xMin = r.xMax + SPACE;
        r.xMax = rect.xMax;
        EditorGUI.PropertyField(r, m_Type, GUIContent.none);

        ItemManagementUtility.DoListElementBehaviours(list, index, isFocused, this);
    }

    private void DrawCategory(Rect rect, int index, bool isActive, bool isFocused) => ItemManagementUtility.DrawListElementByName(m_CategoryList, index, rect, "m_Name", isFocused, this);

    private void DrawItem(Rect rect, int index, bool isActive, bool isFocused)
    {
        if (m_ItemList.serializedProperty.arraySize > index)
        {
            SerializedProperty item = m_ItemList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty displayProp = item.FindPropertyRelative("m_Name");

            ItemManagementUtility.DrawListElementByName(m_ItemList, index, rect, "m_Name", isFocused, this, AddNewItemData);
        }
    }

    private void DrawItemPropertyDefinition(Rect rect, int index, bool isActive, bool isFocused)
    {
        DrawListElement(m_PropertyList, rect, index, isActive, isFocused);
    }
    
    private bool draw_BaseValues = true;
    private bool draw_Properties = true;
    private bool draw_Recepy = true;

    private void DrawItemInspector(Rect totalRect)
    {
        var item = m_ItemList.serializedProperty.GetArrayElementAtIndex(m_ItemList.index);

        //SerializedProperty category = item.FindPropertyRelative("m_Category");
        var id = item.FindPropertyRelative("m_Id");
        var name = item.FindPropertyRelative("m_Name");
        var displayNameKeyID = item.FindPropertyRelative("_displayNameKeyID");
        var DescriptionKeyID = item.FindPropertyRelative("_descriptionKeyID");
        var icon = item.FindPropertyRelative("m_Icon");
        var worldObjectID = item.FindPropertyRelative("_worldObjectID");
        var stackSize = item.FindPropertyRelative("m_StackSize");
        var isBuildable = item.FindPropertyRelative("m_IsBuildable");
        var isCraftable = item.FindPropertyRelative("m_IsCraftable");
        var craftDuration = item.FindPropertyRelative("m_Recipe").FindPropertyRelative("m_Duration");
        var craftCount = item.FindPropertyRelative("m_Recipe").FindPropertyRelative("_craftCount");
        var craftCategoryID = item.FindPropertyRelative("m_Recipe").FindPropertyRelative("_categoryID");
        var isUnlockable = item.FindPropertyRelative("m_isUnlockable");
        var itemRarity = item.FindPropertyRelative("_itemRarity");

        float scrollbarWidth = 16f;
        float spacing = 4f;
        float stdWidth = 154f;

        Rect onlySeenRect = new Rect(totalRect.x, totalRect.y, totalRect.width, totalRect.height);

        float totalContentHeight = 0f;

        totalContentHeight =
            11 * 18 + 5 * spacing + LINE_PROPERTY_HEIGTH +
            (RAW_LIST_HEIGHT + m_CurItemProperties.elementHeight) +
            Mathf.Clamp(m_CurItemProperties.count - 1, 0, Mathf.Infinity) * m_CurItemProperties.elementHeight +
            (isCraftable.GetObscuredBoolValue() ? RAW_LIST_HEIGHT : 0f) +
            (isCraftable.GetObscuredBoolValue() ? RAW_LIST_HEIGHT : 0f) +
            (isCraftable.GetObscuredBoolValue() ? (RAW_LIST_HEIGHT + m_CurItemRequiredItems.elementHeight) : 0) +
            (isCraftable.GetObscuredBoolValue() ? (Mathf.Clamp(m_CurItemRequiredItems.count - 1, 0, Mathf.Infinity) * m_CurItemRequiredItems.elementHeight) : 0) +
            (isUnlockable.GetObscuredBoolValue() ? RAW_LIST_HEIGHT : 0) +
            (isUnlockable.GetObscuredBoolValue() ? (RAW_LIST_HEIGHT + m_CurItemRequiredUnlockablesItems.elementHeight) : 0) +
            (isUnlockable.GetObscuredBoolValue() ? (Mathf.Clamp(m_CurItemRequiredUnlockablesItems.count - 1, 0, Mathf.Infinity) * m_CurItemRequiredUnlockablesItems.elementHeight) : 0);

        Rect allContentRect = new Rect(totalRect.x, totalRect.y, totalRect.width - scrollbarWidth, totalContentHeight);

        // SCROLL VIEW BEGIN.
        m_ItemInspectorScrollPos = GUI.BeginScrollView(onlySeenRect, m_ItemInspectorScrollPos, allContentRect, false, true);

        if (totalContentHeight > 0f)
            GUI.Box(allContentRect, "");

        Rect rect = new Rect(allContentRect.x + spacing, allContentRect.y + spacing, stdWidth, 16f);

        GUIStyle richTextStyle = new GUIStyle() { richText = true, fontStyle = FontStyle.Bold };

        /*// Category.
            GUI.Label(rect, string.Format("<b>Category:</b> {0}", category.stringValue), richTextStyle);*/

        // Back button
        rect.width = 200f;
        if(GUI.Button(rect,"Back to categories"))
        {
            itemEditorState = ItemEditorState.categoryView;
        }

        // draw settings

        rect.y = rect.yMax + spacing;
        rect.width = 500f;
        GUI.Label(rect,"Use draw settings to optimize editor");
        rect.y = rect.yMax + spacing;
        draw_BaseValues = GUI.Toggle(rect,draw_BaseValues,"draw_BaseValues");
        rect.y = rect.yMax + spacing;
        draw_Properties = GUI.Toggle(rect,draw_Properties,"draw_Properties");
        rect.y = rect.yMax + spacing;
        draw_Recepy = GUI.Toggle(rect,draw_Recepy,"draw_Recepy");
        rect.y = rect.yMax + spacing;


        if(draw_BaseValues)
        {
            // Id.
            rect.width = 48f;
            rect.y = rect.yMax + spacing;
            GUI.Label(rect, string.Format("<b>ID:</b> {0}", id.GetObscuredIntValue()), richTextStyle);

            // Name.
            rect.y = rect.yMax + spacing;
            rect.width = 48f;
            GUI.Label(rect, "<b>Name:</b> ", richTextStyle);

            rect.x = rect.xMax + 54f;
            rect.width = stdWidth;
            EditorGUI.PropertyField(rect, name, GUIContent.none);

            rect.x = allContentRect.x + spacing;
            rect.y = rect.yMax + spacing;
            rect.width = 48f;
            GUI.Label(rect, "<b>Display Name Key ID</b> ", richTextStyle);

            rect.x = rect.xMax + 54f;
            rect.width = stdWidth;
            EditorGUI.PropertyField(rect, displayNameKeyID, GUIContent.none);

            rect.x = allContentRect.x + spacing;
            rect.y = rect.yMax + spacing;
            rect.width = 48f;
            GUI.Label(rect, "<b>Description Key ID</b> ", richTextStyle);

            rect.x = rect.xMax + 54f;
            rect.width = stdWidth;
            EditorGUI.PropertyField(rect, DescriptionKeyID, GUIContent.none);

            // Icon.
            rect.x = allContentRect.x + spacing;
            rect.y = rect.yMax + spacing;
            rect.width = 48f;
            GUI.Label(rect, "<b>Icon:</b> ", richTextStyle);

            rect.x = rect.xMax + 54f;
            rect.width = stdWidth;
            EditorGUI.PropertyField(rect, icon, GUIContent.none);

            // World object.
            rect.x = allContentRect.x + spacing;
            rect.y = rect.yMax + spacing;
            rect.width = 48f;
            GUI.Label(rect, "<b>World Object:</b> ", richTextStyle);

            rect.x = rect.xMax + 54f;
            rect.width = stdWidth;
            EditorGUI.PropertyField(rect, worldObjectID, GUIContent.none);

            // Stack size.
            rect.x = allContentRect.x + spacing;
            rect.y = rect.yMax + spacing;
            rect.width = 48f;
            GUI.Label(rect, "<b>Stack Size:</b> ", richTextStyle);

            rect.x = rect.xMax + 54f;
            rect.width = stdWidth * 2f;
            EditorGUI.PropertyField(rect, stackSize, GUIContent.none);


            //Item Rarity
            rect.x = allContentRect.x + spacing;
            rect.y = rect.yMax + spacing;
            rect.width = 48f;
            GUI.Label(rect, "<b>Item Rarity:</b> ", richTextStyle);

            rect.x = rect.xMax + 54f;
            rect.width = stdWidth;
            EditorGUI.PropertyField(rect, itemRarity, GUIContent.none);
        }

        if(draw_Properties)
        {
            // Properties label.
            rect.x = allContentRect.x + spacing;
            rect.y = rect.yMax;

            rect.width = 48f;
            GUI.Label(rect, "<b>Properties:</b> ", richTextStyle);

            // Property list.
            rect.x = rect.xMax + 54f;
            rect.y = rect.yMax + spacing;
            rect.width = allContentRect.xMax - rect.x - spacing;
            m_CurItemProperties.DoList(rect);
        }

        // Is buildable label.
        rect.x = allContentRect.x + spacing;
        rect.y = rect.yMax + 26f + Mathf.Max(m_CurItemProperties.count, 1) * PROPERTY_HEIGHT;

        rect.width = 48f;
        GUI.Label(rect, "<b>Is Placeable?</b>", richTextStyle);

        // Is craftable toggle.
        rect.x = rect.xMax + 54f;
        EditorGUI.PropertyField(rect, isBuildable, GUIContent.none);

        // Is craftable label.
        rect.x = allContentRect.x + spacing;
        rect.y = rect.yMax;

        rect.width = 48f;
        GUI.Label(rect, "<b>Is Craftable?</b>", richTextStyle);

        // Is craftable toggle.
        rect.x = rect.xMax + 54f;
        EditorGUI.PropertyField(rect, isCraftable, GUIContent.none);

        if(draw_Recepy)
        {
            if (isCraftable.GetObscuredBoolValue())
            {
                // Recipe label.
                rect.x = allContentRect.x + spacing;
                rect.y = rect.yMax + spacing;
                rect.width = 48f;
                GUI.Label(rect, "<b>Recipe:</b> ", richTextStyle);

                // Duration label.
                rect.x = rect.xMax + 54f;
                rect.width = 128f;
                GUI.Label(rect, "Duration (seconds):");

                // Duration slider.
                rect.x = rect.xMax;
                rect.width = 128f;
                EditorGUI.PropertyField(rect, craftDuration, GUIContent.none);

                rect.y += 24f;
                rect.x = allContentRect.x + spacing + 102f;
                rect.width = 128f;
                GUI.Label(rect, "Category");

                rect.x = rect.xMax;
                EditorGUI.PropertyField(rect, craftCategoryID, GUIContent.none);

                // Craft Count label.
                rect.y += 24f;
                rect.x = allContentRect.x + spacing + 102f;
                rect.width = 128f;
                GUI.Label(rect, "Craft Count:");

                // Craft Count value.
                rect.x = rect.xMax;
                rect.width = 128f;
                EditorGUI.PropertyField(rect, craftCount, GUIContent.none);

                // Required items list.
                rect.y += 24f;
                rect.x = allContentRect.x + spacing + 102f;
                rect.width = allContentRect.xMax - rect.x - spacing;

                
                m_CurItemRequiredItems.DoList(rect);

                if (m_CurItemRequiredItems.count == 0)
                {
                    m_CurItemRequiredItems.serializedProperty.arraySize = 1;
                    Repaint();
                }

                // Is craftable unlockable label.
                rect.x = allContentRect.x + spacing;
                rect.y = rect.yMax;

                if (isCraftable.GetObscuredBoolValue())
                {
                    rect.y += 26 + Mathf.Max(m_CurItemRequiredItems.count, 1) * LINE_PROPERTY_HEIGTH;
                }

                rect.width = 48f;
                GUI.Label(rect, "<b>Is Unlockable?</b>", richTextStyle);

                // Is craftable toggle.
                rect.x = rect.xMax + 54f;
                EditorGUI.PropertyField(rect, isUnlockable, GUIContent.none);

                if (isUnlockable.GetObscuredBoolValue())
                {
                    // UnlockableLabel label.
                    rect.x = allContentRect.x + spacing;
                    rect.y = rect.yMax + spacing;
                    rect.width = 48f;
                    GUI.Label(rect, "<b>Items for unlock:</b> ", richTextStyle);

                    // Required items list.
                    rect.y += 24f;
                    rect.x = allContentRect.x + spacing + 102f;
                    rect.width = allContentRect.xMax - rect.x - spacing;

                    
                    m_CurItemRequiredUnlockablesItems.DoList(rect);

                    if (m_CurItemRequiredUnlockablesItems.count == 0)
                    {
                        m_CurItemRequiredUnlockablesItems.serializedProperty.arraySize = 1;
                        Repaint();
                    }

                }
            }
        }


        GUI.EndScrollView();
    }

    private void DrawPropertyEditor(Rect totalRect)
    {
        Vector2 labelSize = new Vector2(128f, 24f);

        // Properties label.
        GUI.Box(new Rect(totalRect.x + totalRect.width * 0.5f - labelSize.x * 0.5f, totalRect.y, labelSize.x, labelSize.y), "Property List");

        // Draw the properties.
        totalRect.y += 24f;
        totalRect.height -= 25f;
        DrawList(m_PropertyList, totalRect, ref m_PropsScrollPos);
    }

    private void AddNewItemData(ReorderableList list, EditorWindow window)
    {
        AddNewItem(list);

        Event.current.Use();
        if (window)
            window.Repaint();
    }
}

public static class ItemManagementUtility
{

    static private void AddDefaultElement(ReorderableList list, EditorWindow window)
    {
        list.serializedProperty.InsertArrayElementAtIndex(list.index);
        list.index++;
        list.onSelectCallback?.Invoke(list);

        Event.current.Use();
        if (window)
            window.Repaint();
    }

    static private void RemoveDefaultElement(ReorderableList list, EditorWindow window, int index)
    {
        int newIndex = 0;
        if (list.count == 1)
            newIndex = -1;
        else if (index == list.count - 1)
            newIndex = index - 1;
        else if (index > 0)
            newIndex = index - 1;

        list.serializedProperty.DeleteArrayElementAtIndex(index);

        if (newIndex != -1)
        {
            list.index = newIndex;
            list.onSelectCallback?.Invoke(list);
        }

        Event.current.Use();
        if (window)
            window.Repaint();
    }

    public static void DoListElementBehaviours(
        ReorderableList list,
        int index,
        bool isFocused,
        EditorWindow window = default,
        Action<ReorderableList, EditorWindow> actionAddItem = default,
        Action<ReorderableList, EditorWindow, int> actionRemoveItem = default)
    {
        var current = Event.current;

        if (current.type == EventType.KeyDown)
        {
            if (list.index == index && isFocused)
            {
                if (current.keyCode == KeyCode.Delete)
                {
                    if (actionRemoveItem != null)
                        actionRemoveItem?.Invoke(list, window, index);
                    else
                        RemoveDefaultElement(list, window, index);
                }
                else if (current.shift && current.keyCode == KeyCode.D)
                {
                    if (actionAddItem != null)
                        actionAddItem?.Invoke(list, window);
                    else
                        AddDefaultElement(list, window);
                }
            }
        }
    }

    public static string[] GetItemNamesFull(SerializedProperty categoryList)
    {
        var names = new List<string>();

        for (int i = 0; i < categoryList.arraySize; i++)
        {
            var category = categoryList.GetArrayElementAtIndex(i);
            var itemList = category.FindPropertyRelative("m_Items");
            for (int j = 0; j < itemList.arraySize; j++)
                names.Add(category.FindPropertyRelative("m_Name").stringValue + "/" + itemList.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue);
        }

        return names.ToArray();
    }

    public static string[] GetItemNames(SerializedProperty categoryList)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < categoryList.arraySize; i++)
        {
            var category = categoryList.GetArrayElementAtIndex(i);
            var itemList = category.FindPropertyRelative("m_Items");
            for (int j = 0; j < itemList.arraySize; j++)
                names.Add(itemList.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue);
        }

        return names.ToArray();
    }

    public static int GetItemIndex(SerializedProperty categoryList, string itemName)
    {
        int index = 0;
        for (int i = 0; i < categoryList.arraySize; i++)
        {
            var category = categoryList.GetArrayElementAtIndex(i);
            var itemList = category.FindPropertyRelative("m_Items");
            for (int j = 0; j < itemList.arraySize; j++)
            {
                var name = itemList.GetArrayElementAtIndex(j).FindPropertyRelative("m_Name").stringValue;
                if (name == itemName)
                    return index;

                index++;
            }
        }

        return -1;
    }

    public static void DrawListElementByName(ReorderableList list, int index, Rect rect, string nameProperty, bool isFocused, EditorWindow window,
        Action<ReorderableList, EditorWindow> actionAddItem = default,
        Action<ReorderableList, EditorWindow, int> actionRemoveItem = default)
    {
        if (list.serializedProperty.arraySize == index)
            return;

        rect.y += 2;
        var element = list.serializedProperty.GetArrayElementAtIndex(index);
        var name = element.FindPropertyRelative(nameProperty);

        var position = new Rect(rect.x + 16, rect.y, rect.width - 32, 16f);
        name.stringValue = EditorGUI.TextField(position, name.stringValue);
        //EditorGUI.PropertyField(position rect, name, GUIContent.none);

        DoListElementBehaviours(list, index, isFocused, window, actionAddItem, actionRemoveItem);
    }

    public static void DrawItemProperty(Rect rect, SerializedProperty itemProperty, ReorderableList propertyList)
    {
        var name = itemProperty.FindPropertyRelative("m_Name");
        var type = itemProperty.FindPropertyRelative("m_Type");

        float initialX = rect.x;

        var fullRect = new Rect(rect);

        // Source label.
        rect.width = 64f;
        rect.height = 16f;
        GUI.Label(rect, "Property: ");

        // Source popup.
        var allProperties = GetStringNames(propertyList.serializedProperty, "m_Name");
        if (allProperties.Length == 0)
            return;

        rect.x = rect.xMax;
        rect.width = fullRect.xMax - rect.x;

        int selectedIndex = GetStringIndex(name.GetObscuredStringValue(), allProperties);
        int oldIndex = selectedIndex;
        selectedIndex = EditorGUI.Popup(rect, selectedIndex, allProperties);
        if(selectedIndex != oldIndex)
        {
            name.SetObscuredString(allProperties[selectedIndex]);
        }

        type.SetObscuredInt(propertyList.serializedProperty.GetArrayElementAtIndex(selectedIndex).FindPropertyRelative("m_Type").GetObscuredIntValue());

        ItemProperty.Type typeToEnum = (ItemProperty.Type)type.GetObscuredIntValue();

        // Value label.
        rect.x = initialX;
        rect.width = 64;
        rect.y = rect.yMax + 4f;

        GUI.Label(rect, "Value: ");

        // Editing the value based on the type.
        rect.x = rect.xMax;
        rect.width = fullRect.xMax - rect.x;

        if (typeToEnum == ItemProperty.Type.Bool)
            DrawBoolProperty(rect, itemProperty.FindPropertyRelative("m_Bool"));
        if (typeToEnum == ItemProperty.Type.Int)
            DrawIntProperty(rect, itemProperty.FindPropertyRelative("m_Int"));
        else if (typeToEnum == ItemProperty.Type.IntRange)
            DrawIntRangeProperty(rect, itemProperty.FindPropertyRelative("m_IntRange"));
        else if (typeToEnum == ItemProperty.Type.RandomInt)
            DrawRandomIntProperty(rect, itemProperty.FindPropertyRelative("m_RandomInt"));
        else if (typeToEnum == ItemProperty.Type.Float)
            DrawFloatProperty(rect, itemProperty.FindPropertyRelative("m_Float"));
        else if (typeToEnum == ItemProperty.Type.RandomFloat)
            DrawRandomFloatProperty(rect, itemProperty.FindPropertyRelative("m_RandomFloat"));
        else if (typeToEnum == ItemProperty.Type.FloatRange)
            DrawFloatRangeProperty(rect, itemProperty.FindPropertyRelative("m_FloatRange"));
        else if (typeToEnum == ItemProperty.Type.String)
            DrawStringProperty(rect, itemProperty.FindPropertyRelative("m_String"));
        else if (typeToEnum == ItemProperty.Type.Sound)
            DrawSoundProperty(rect, itemProperty.FindPropertyRelative("m_Sound"));
        else if (typeToEnum == ItemProperty.Type.Prefab)
            DrawPrefabProperty(rect, itemProperty.FindPropertyRelative("m_Prefab"));
        else if (typeToEnum == ItemProperty.Type.AudioID)
            DrawPrefabProperty(rect, itemProperty.FindPropertyRelative("_audioID"));
        else if (typeToEnum == ItemProperty.Type.ItemCategoryID)
            DrawPrefabProperty(rect, itemProperty.FindPropertyRelative("_itemCategoryID"));
        else if (typeToEnum == ItemProperty.Type.AbilityID)
            DrawPrefabProperty(rect, itemProperty.FindPropertyRelative("_abilityID"));
        else if (typeToEnum == ItemProperty.Type.PlayerWeaponID)
            DrawPrefabProperty(rect, itemProperty.FindPropertyRelative("_playerWeaponID"));
        else if (typeToEnum == ItemProperty.Type.PrefabID)
            DrawPrefabProperty(rect, itemProperty.FindPropertyRelative("_prefabID"));
        else if (typeToEnum == ItemProperty.Type.ConstructionCategoryID)
            DrawPrefabProperty(rect, itemProperty.FindPropertyRelative("_constructionCategoryID"));
        else if (typeToEnum == ItemProperty.Type.EquipmentCategory)
            DrawPrefabProperty(rect, itemProperty.FindPropertyRelative("_equipmentCategory"));
        else if (typeToEnum == ItemProperty.Type.EquimpentSet)
            DrawPrefabProperty(rect, itemProperty.FindPropertyRelative("_equipmentSet"));

    }

    public static string[] GetStringNames(SerializedProperty property, string subProperty = "")
    {
        List<string> strings = new List<string>();
        for (int i = 0; i < property.arraySize; i++)
        {
            if (subProperty == "")
                strings.Add(property.GetArrayElementAtIndex(i).GetObscuredStringValue());
            else
                strings.Add(property.GetArrayElementAtIndex(i).FindPropertyRelative(subProperty).GetObscuredStringValue());
        }

        return strings.ToArray();
    }

    public static int GetStringIndex(string str, string[] strings)
    {
        for (int i = 0; i < strings.Length; i++)
            if (strings[i] == str)
                return i;

        return 0;
    }

    private static void DrawBoolProperty(Rect position, SerializedProperty property)
    {
        EditorGUI.PropertyField(position, property, GUIContent.none);
    }

    private static void DrawIntProperty(Rect position, SerializedProperty property)
    {
        var current = property.FindPropertyRelative("m_Current");
        var defaultVal = property.FindPropertyRelative("m_Default");

        EditorGUI.PropertyField(position, current, GUIContent.none);
        defaultVal.CopyFromObscuredIntValue(current);
    }

    private static void DrawIntRangeProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty current = property.FindPropertyRelative("m_Current");
        SerializedProperty min = property.FindPropertyRelative("m_Min");
        SerializedProperty max = property.FindPropertyRelative("m_Max");

        float fieldWidth = 36f;

        // Current label
        position.width = 54f;
        EditorGUI.PrefixLabel(position, new GUIContent("Current:"));

        // Current field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, current, GUIContent.none);

        // Min label
        position.x = position.xMax;
        position.width = 32f;
        EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

        // Min field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, min, GUIContent.none);

        // Max label
        position.x = position.xMax;
        position.width = 32f;
        EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

        // Max field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, max, GUIContent.none);
    }

    private static void DrawRandomIntProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty min = property.FindPropertyRelative("m_Min");
        SerializedProperty max = property.FindPropertyRelative("m_Max");

        float fieldWidth = 36f;

        // Min label
        position.width = 32f;
        EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

        // Min field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, min, GUIContent.none);

        // Max label
        position.x = position.xMax;
        position.width = 32f;
        EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

        // Max field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, max, GUIContent.none);
    }

    private static void DrawFloatProperty(Rect position, SerializedProperty property)
    {
        var current = property.FindPropertyRelative("m_Current");
        var defaultVal = property.FindPropertyRelative("m_Default");

        EditorGUI.PropertyField(position, current, GUIContent.none);
        defaultVal.CopyFromObscuredFloatValue(current);
    }

    private static void DrawFloatRangeProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty current = property.FindPropertyRelative("m_Current");
        SerializedProperty min = property.FindPropertyRelative("m_Min");
        SerializedProperty max = property.FindPropertyRelative("m_Max");

        float fieldWidth = 36f;

        // Current label
        position.width = 54f;
        EditorGUI.PrefixLabel(position, new GUIContent("Current:"));

        // Current field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, current, GUIContent.none);

        // Min label
        position.x = position.xMax;
        position.width = 32f;
        EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

        // Min field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, min, GUIContent.none);

        // Max label
        position.x = position.xMax;
        position.width = 32f;
        EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

        // Max field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, max, GUIContent.none);
    }

    private static void DrawRandomFloatProperty(Rect position, SerializedProperty property)
    {
        SerializedProperty min = property.FindPropertyRelative("m_Min");
        SerializedProperty max = property.FindPropertyRelative("m_Max");

        float fieldWidth = 36f;

        // Min label
        position.width = 32f;
        EditorGUI.PrefixLabel(position, new GUIContent("Min:"));

        // Min field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, min, GUIContent.none);

        // Max label
        position.x = position.xMax;
        position.width = 32f;
        EditorGUI.PrefixLabel(position, new GUIContent("Max:"));

        // Max field
        position.x = position.xMax;
        position.width = fieldWidth;
        EditorGUI.PropertyField(position, max, GUIContent.none);
    }

    private static void DrawStringProperty(Rect position, SerializedProperty property)
    {
        EditorGUI.PropertyField(position, property, GUIContent.none);
    }

    private static void DrawSoundProperty(Rect position, SerializedProperty property)
    {
        EditorGUI.PropertyField(position, property, GUIContent.none);
    }

    private static void DrawPrefabProperty(Rect position, SerializedProperty property)
    {
        EditorGUI.PropertyField(position, property, GUIContent.none);
    }
}

static public class ObsuredTypesExtensions
{

    public static void CopyFromObscuredIntValue(this SerializedProperty to, SerializedProperty from)
    {
        to.FindPropertyRelative("hiddenValue").intValue = from.FindPropertyRelative("hiddenValue").intValue;
        to.FindPropertyRelative("currentCryptoKey").intValue = from.FindPropertyRelative("currentCryptoKey").intValue;
        to.FindPropertyRelative("fakeValue").intValue = from.FindPropertyRelative("fakeValue").intValue;
        to.FindPropertyRelative("inited").boolValue = from.FindPropertyRelative("inited").boolValue;
        to.FindPropertyRelative("fakeValueActive").boolValue = from.FindPropertyRelative("fakeValueActive").boolValue;
    }

    public static void CopyFromObscuredFloatValue(this SerializedProperty to, SerializedProperty from)
    {
        to.FindPropertyRelative("hiddenValueOldByte4").FindPropertyRelative("b1").intValue = from.FindPropertyRelative("hiddenValueOldByte4").FindPropertyRelative("b1").intValue;
        to.FindPropertyRelative("hiddenValueOldByte4").FindPropertyRelative("b2").intValue = from.FindPropertyRelative("hiddenValueOldByte4").FindPropertyRelative("b2").intValue;
        to.FindPropertyRelative("hiddenValueOldByte4").FindPropertyRelative("b3").intValue = from.FindPropertyRelative("hiddenValueOldByte4").FindPropertyRelative("b3").intValue;
        to.FindPropertyRelative("hiddenValueOldByte4").FindPropertyRelative("b4").intValue = from.FindPropertyRelative("hiddenValueOldByte4").FindPropertyRelative("b4").intValue;

        to.FindPropertyRelative("hiddenValue").intValue = from.FindPropertyRelative("hiddenValue").intValue;
        to.FindPropertyRelative("currentCryptoKey").intValue = from.FindPropertyRelative("currentCryptoKey").intValue;
        to.FindPropertyRelative("fakeValue").floatValue = from.FindPropertyRelative("fakeValue").floatValue;
        to.FindPropertyRelative("inited").boolValue = from.FindPropertyRelative("inited").boolValue;
        to.FindPropertyRelative("fakeValueActive").boolValue = from.FindPropertyRelative("fakeValueActive").boolValue;
    }

    public static void SetObscuredInt(this SerializedProperty prop, int value)
    {
        // return;
        var hiddenValue = prop.FindPropertyRelative("hiddenValue");
        var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
        var inited = prop.FindPropertyRelative("inited");
        var fakeValue = prop.FindPropertyRelative("fakeValue");
        var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

        var currentCryptoKey = cryptoKey.intValue;

        if (!inited.boolValue)
        {
            if (currentCryptoKey == 0)
            {
                currentCryptoKey = cryptoKey.intValue = ObscuredInt.cryptoKeyEditor;
            }
            hiddenValue.intValue = ObscuredInt.Encrypt(0, currentCryptoKey);
            inited.boolValue = true;
        }

        hiddenValue.intValue = ObscuredInt.Encrypt(value, currentCryptoKey);
        fakeValue.intValue = value;
        fakeValueActive.boolValue = true;
    }

    public static void SetObscuredString(this SerializedProperty prop, string value)
    {
        // return;
        Func<string, byte[]> GetBytes = (str) =>
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        };

        Action<string, SerializedProperty, string> EncryptAndSetBytes = (val, p, key) =>
        {
            var encrypted = ObscuredString.EncryptDecrypt(val, key);
            var encryptedBytes = GetBytes(encrypted);

            p.ClearArray();
            p.arraySize = encryptedBytes.Length;

            for (var i = 0; i < encryptedBytes.Length; i++)
            {
                p.GetArrayElementAtIndex(i).intValue = encryptedBytes[i];
            }
        };

        Func<byte[], string> GetString = (bytes) =>
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        };

        var hiddenValue = prop.FindPropertyRelative("hiddenValue");
        var cryptoKey = prop.FindPropertyRelative("currentCryptoKey");
        var inited = prop.FindPropertyRelative("inited");
        var fakeValue = prop.FindPropertyRelative("fakeValue");
        var fakeValueActive = prop.FindPropertyRelative("fakeValueActive");

        var currentCryptoKey = cryptoKey.stringValue;

        if (!inited.boolValue)
        {
            if (string.IsNullOrEmpty(currentCryptoKey))
            {
                currentCryptoKey = cryptoKey.stringValue = ObscuredString.cryptoKeyEditor;
            }
            inited.boolValue = true;
            EncryptAndSetBytes(value, hiddenValue, currentCryptoKey);
            fakeValue.stringValue = value;
        }
        else
        {
            var size = hiddenValue.FindPropertyRelative("Array.size");
            var showMixed = size.hasMultipleDifferentValues;

            if (!showMixed)
            {
                for (var i = 0; i < hiddenValue.arraySize; i++)
                {
                    showMixed |= hiddenValue.GetArrayElementAtIndex(i).hasMultipleDifferentValues;
                    if (showMixed) break;
                }
            }
        }

        EncryptAndSetBytes(value, hiddenValue, currentCryptoKey);
        fakeValue.stringValue = value;
        fakeValueActive.boolValue = true;
    }

    public static int GetObscuredIntValue(this SerializedProperty prop)
    {
        return prop.FindPropertyRelative("fakeValue").intValue;
    }

    public static string GetObscuredStringValue(this SerializedProperty prop)
    {
        return prop.FindPropertyRelative("fakeValue").stringValue;
    }

    public static bool GetObscuredBoolValue(this SerializedProperty prop)
    {
        return prop.FindPropertyRelative("fakeValue").boolValue;
    }

    public static float GetObscuredFloatValue(this SerializedProperty prop)
    {
        return prop.FindPropertyRelative("fakeValue").floatValue;
    }

}

#endif