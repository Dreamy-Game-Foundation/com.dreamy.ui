using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dreamy.UI.Editor
{
    [CustomEditor(typeof(TweenEffectPlayer))]
    public sealed class TweenEffectPlayerEditor : UnityEditor.Editor
    {
        private SerializedProperty defaultSettingsProperty;
        private SerializedProperty entriesProperty;

        private void OnEnable()
        {
            defaultSettingsProperty = serializedObject.FindProperty("defaultSettings");
            entriesProperty = serializedObject.FindProperty("entries");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(defaultSettingsProperty);
            EditorGUILayout.Space(6f);
            DrawEntries();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEntries()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Entries", EditorStyles.boldLabel);
                if (GUILayout.Button("Add", GUILayout.Width(64f)))
                {
                    ShowAddMenu();
                }
            }

            for (int index = 0; index < entriesProperty.arraySize; index++)
            {
                SerializedProperty entryProperty =
                    entriesProperty.GetArrayElementAtIndex(index);

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        entryProperty.isExpanded = EditorGUILayout.Foldout(
                            entryProperty.isExpanded,
                            GetEntryLabel(entryProperty, index),
                            true);

                        if (GUILayout.Button("Up", GUILayout.Width(42f)) && index > 0)
                        {
                            entriesProperty.MoveArrayElement(index, index - 1);
                        }

                        if (GUILayout.Button("Down", GUILayout.Width(52f)) &&
                            index < entriesProperty.arraySize - 1)
                        {
                            entriesProperty.MoveArrayElement(index, index + 1);
                        }

                        if (GUILayout.Button("Remove", GUILayout.Width(72f)))
                        {
                            entriesProperty.DeleteArrayElementAtIndex(index);
                            return;
                        }
                    }

                    if (entryProperty.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(entryProperty, true);
                        EditorGUI.indentLevel--;
                    }
                }
            }
        }

        private void ShowAddMenu()
        {
            GenericMenu menu = new GenericMenu();
            Type[] entryTypes = TypeCache.GetTypesDerivedFrom<TweenEffectEntry>()
                .Where(type =>
                    !type.IsAbstract &&
                    !type.IsGenericType &&
                    type.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(type => type.Name)
                .ToArray();

            foreach (Type entryType in entryTypes)
            {
                menu.AddItem(
                    new GUIContent(GetTypeMenuName(entryType)),
                    false,
                    () => AddEntry(entryType));
            }

            menu.ShowAsContext();
        }

        private void AddEntry(Type entryType)
        {
            serializedObject.Update();
            int index = entriesProperty.arraySize;
            entriesProperty.InsertArrayElementAtIndex(index);
            SerializedProperty entryProperty =
                entriesProperty.GetArrayElementAtIndex(index);
            entryProperty.managedReferenceValue =
                Activator.CreateInstance(entryType);
            entryProperty.isExpanded = true;
            serializedObject.ApplyModifiedProperties();
        }

        private static string GetEntryLabel(
            SerializedProperty entryProperty,
            int index)
        {
            object entry = entryProperty.managedReferenceValue;
            return entry != null
                ? ObjectNames.NicifyVariableName(entry.GetType().Name)
                : $"Missing Entry {index}";
        }

        private static string GetTypeMenuName(Type type)
        {
            string label = type.Name.Replace("TweenEffectEntry", string.Empty);
            return ObjectNames.NicifyVariableName(label);
        }
    }
}
