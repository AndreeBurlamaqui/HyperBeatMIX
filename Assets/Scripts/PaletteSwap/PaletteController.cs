using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using UnityEngine;


[CreateAssetMenu(fileName ="Color Palette", menuName ="TinyCacto/ColorPalette", order = 1)]
public class PaletteController : ScriptableObject
{

    public string[] languagesList;

    public List<PaletteLocalization> paletteLocal;

    public List<PaletteColor> paletteColor;


}

[Serializable]
public class PaletteLocalization
{
    public int languageID;

    public string paletteName;

    [TextArea(1,6)] public string paletteStoreDescription;
}

[Serializable]
public class PaletteColor
{
    public Color hexColorCode;

    public Material[] materialRespective;
}


#if UNITY_EDITOR
[CustomEditor(typeof(PaletteController))]
public class PaletteEditor : Editor
{

    private SerializedProperty LanguagesList;
    private SerializedProperty PaletteLocalizationItems;

    private ReorderableList languageReList;
    private ReorderableList paletteLocalizationReList;


    private SerializedProperty ColorList;

    private ReorderableList colorReList;

    private PaletteController paletteController;

    private GUIContent[] availableLanguageOptions;

    private void OnEnable()
    {
        paletteController = (PaletteController)target;

        LanguagesList = serializedObject.FindProperty(nameof(PaletteController.languagesList));
        PaletteLocalizationItems = serializedObject.FindProperty(nameof(PaletteController.paletteLocal));


        languageReList = new ReorderableList(serializedObject, LanguagesList)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = false, // for now disable reorder feature since we later go by index!

            // As the header we simply want to see the usual display name of the CharactersList
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, LanguagesList.displayName),

            // How shall elements be displayed
            drawElementCallback = (rect, index, focused, active) =>
            {
                // get the current element's SerializedProperty
                var element = LanguagesList.GetArrayElementAtIndex(index);

                // Get all characters as string[]
                var availableIDs = paletteController.languagesList;

                // store the original GUI.color
                var color = GUI.color;
                // Tint the field in red for invalid values
                // either because it is empty or a duplicate
                if (string.IsNullOrWhiteSpace(element.stringValue) || availableIDs.Count(item => string.Equals(item, element.stringValue)) > 1)
                {
                    GUI.color = Color.red;
                }
                // Draw the property which automatically will select the correct drawer -> a single line text field
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(element)), element);

                // reset to the default color
                GUI.color = color;

                // If the value is invalid draw a HelpBox to explain why it is invalid
                if (string.IsNullOrWhiteSpace(element.stringValue))
                {
                    rect.y += EditorGUI.GetPropertyHeight(element);
                    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "ID may not be empty!", MessageType.Error);
                }
                else if (availableIDs.Count(item => string.Equals(item, element.stringValue)) > 1)
                {
                    rect.y += EditorGUI.GetPropertyHeight(element);
                    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Duplicate! ID has to be unique!", MessageType.Error);
                }
            },

            // Get the correct display height of elements in the list
            // according to their values
            // in this case e.g. dependent whether a HelpBox is displayed or not
            elementHeightCallback = index =>
            {
                var element = LanguagesList.GetArrayElementAtIndex(index);
                var availableIDs = paletteController.languagesList;

                var height = EditorGUI.GetPropertyHeight(element);

                if (string.IsNullOrWhiteSpace(element.stringValue) || availableIDs.Count(item => string.Equals(item, element.stringValue)) > 1)
                {
                    height += EditorGUIUtility.singleLineHeight;
                }

                return height;
            },

            // Overwrite what shall be done when an element is added via the +
            // Reset all values to the defaults for new added elements
            // By default Unity would clone the values from the last or selected element otherwise
            onAddCallback = list =>
            {
                // This adds the new element but copies all values of the select or last element in the list
                list.serializedProperty.arraySize++;

                var newElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                newElement.stringValue = "";
            }

        };

        // Setup and configure the dialogItemsList we will use to display the content of the DialogueItems 
        // in a nicer way
        paletteLocalizationReList = new ReorderableList(serializedObject, PaletteLocalizationItems)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = true, // for the dialogue items we can allow re-ordering

            // As the header we simply want to see the usual display name of the DialogueItems
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, PaletteLocalizationItems.displayName),

            // How shall elements be displayed
            drawElementCallback = (rect, index, focused, active) =>
            {
                // get the current element's SerializedProperty
                var element = PaletteLocalizationItems.GetArrayElementAtIndex(index);

                // Get the nested property fields of the DialogueElement class
                var language = element.FindPropertyRelative(nameof(PaletteLocalization.languageID));
                var name = element.FindPropertyRelative(nameof(PaletteLocalization.paletteName));
                var desc = element.FindPropertyRelative(nameof(PaletteLocalization.paletteStoreDescription));


                var popUpHeight = EditorGUI.GetPropertyHeight(language);

                // store the original GUI.color
                var color = GUI.color;

                // if the value is invalid tint the next field red
                if (language.intValue < 0) GUI.color = Color.red;

                // Draw the Popup so you can select from the existing character names
                language.intValue = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, popUpHeight), new GUIContent(language.displayName), language.intValue, availableLanguageOptions);


                // reset the GUI.color
                GUI.color = color;
                rect.y += popUpHeight;

                // Draw the text field
                // since we use a PropertyField it will automatically recognize that this field is tagged [TextArea]
                // and will choose the correct drawer accordingly
                var textNameHeight = EditorGUI.GetPropertyHeight(name);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + 5f, rect.width, textNameHeight), name);

                

                var textDescHeight = EditorGUI.GetPropertyHeight(desc);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + 25f, rect.width, textDescHeight), desc);
            },

            // Get the correct display height of elements in the list
            // according to their values
            // in this case e.g. we add an additional line as a little spacing between elements
            elementHeightCallback = index =>
            {
                var element = PaletteLocalizationItems.GetArrayElementAtIndex(index);

                var language = element.FindPropertyRelative(nameof(PaletteLocalization.languageID));
                var name = element.FindPropertyRelative(nameof(PaletteLocalization.paletteName));
                var desc = element.FindPropertyRelative(nameof(PaletteLocalization.paletteStoreDescription));

                return EditorGUI.GetPropertyHeight(language) + EditorGUI.GetPropertyHeight(name) + EditorGUI.GetPropertyHeight(desc) + EditorGUIUtility.singleLineHeight;
            },

            // Overwrite what shall be done when an element is added via the +
            // Reset all values to the defaults for new added elements
            // By default Unity would clone the values from the last or selected element otherwise
            onAddCallback = list =>
            {
                // This adds the new element but copies all values of the select or last element in the list
                list.serializedProperty.arraySize++;

                var newElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                var language = newElement.FindPropertyRelative(nameof(PaletteLocalization.languageID));
                var name = newElement.FindPropertyRelative(nameof(PaletteLocalization.paletteName));
                var desc = newElement.FindPropertyRelative(nameof(PaletteLocalization.paletteStoreDescription));


                language.intValue = -1;
                name.stringValue = "";
                desc.stringValue = "";

            }
        };

        // Get the existing character names ONCE as GuiContent[]
        // Later only update this if the charcterList was changed

        if (availableLanguageOptions != null)
        {
            availableLanguageOptions = paletteController.languagesList.Select(item => new GUIContent(item)).ToArray();
        }


        ///////////
        // CORES //
        ///////////
        

        ColorList = serializedObject.FindProperty(nameof(PaletteController.paletteColor));

        colorReList = new ReorderableList(serializedObject, ColorList)
        {
            displayAdd = true,
            displayRemove = true,
            draggable = false, // for now disable reorder feature since we later go by index!

            // As the header we simply want to see the usual display name of the CharactersList
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, ColorList.displayName),

            // How shall elements be displayed
            drawElementCallback = (rect, index, focused, active) =>
            {
                // get the current element's SerializedProperty
                var element = ColorList.GetArrayElementAtIndex(index);
                element.isExpanded = true;

                // Get all characters as string[]
                var color = element.FindPropertyRelative(nameof(PaletteColor.hexColorCode));
                var material = element.FindPropertyRelative(nameof(PaletteColor.materialRespective));

                var popUpHeight = EditorGUI.GetPropertyHeight(color);

                // store the original GUI.color
                var colorGUI = GUI.color;

                // Draw the property which automatically will select the correct drawer -> a single line text field
                //EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(element)), element);

                // reset to the default color
                GUI.color = colorGUI;
                rect.y += popUpHeight;

                var colorHeight = EditorGUI.GetPropertyHeight(color);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, colorHeight), color);

                var materialHeight = EditorGUI.GetPropertyHeight(material);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y + popUpHeight, rect.width, materialHeight), material, true);

            },

            // Get the correct display height of elements in the list
            // according to their values
            // in this case e.g. dependent whether a HelpBox is displayed or not
            elementHeightCallback = index =>
            {
                var element = ColorList.GetArrayElementAtIndex(index);

                var color = element.FindPropertyRelative(nameof(PaletteColor.hexColorCode));
                var material = element.FindPropertyRelative(nameof(PaletteColor.materialRespective));

                return EditorGUI.GetPropertyHeight(color) + EditorGUI.GetPropertyHeight(material) + EditorGUIUtility.singleLineHeight;

            },

            // Overwrite what shall be done when an element is added via the +
            // Reset all values to the defaults for new added elements
            // By default Unity would clone the values from the last or selected element otherwise
            onAddCallback = list =>
            {
                // This adds the new element but copies all values of the select or last element in the list
                list.serializedProperty.arraySize++;

                var newElement = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                var color = newElement.FindPropertyRelative(nameof(PaletteColor.hexColorCode));
                var material = newElement.FindPropertyRelative(nameof(PaletteColor.materialRespective));

                color.colorValue = Color.red;
                material.objectReferenceValue = null;
            }

        };


            availableLanguageOptions = paletteController.languagesList.Select(item => new GUIContent(item)).ToArray();



    }

    public override void OnInspectorGUI()
    {
        DrawScriptField();

        // load real target values into SerializedProperties
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        languageReList.DoLayoutList();
        if (EditorGUI.EndChangeCheck())
        {
            // Write back changed values into the real target
            serializedObject.ApplyModifiedProperties();

            // Update the existing character names as GuiContent[]
            availableLanguageOptions = paletteController.languagesList.Select(item => new GUIContent(item)).ToArray();
        }

        paletteLocalizationReList.DoLayoutList();
        colorReList.DoLayoutList();


        // Write back changed values into the real target
        serializedObject.ApplyModifiedProperties();

    }
    private void DrawScriptField()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((PaletteController)target), typeof(PaletteController), false);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
    }
}



#endif
