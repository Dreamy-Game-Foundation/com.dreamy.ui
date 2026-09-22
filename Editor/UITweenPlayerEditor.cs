using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dreamy.UI.Editor
{
    [CustomEditor(typeof(UITweenPlayer), true)]
    public sealed class UITweenPlayerEditor : UnityEditor.Editor
    {
        private SerializedProperty collectionMode;
        private SerializedProperty manualTargets;

        private void OnEnable()
        {
            collectionMode = serializedObject.FindProperty("collectionMode");
            manualTargets = serializedObject.FindProperty("manualTargets");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(collectionMode);
            EditorGUILayout.HelpBox(
                "Default presets are loaded automatically from Resources/Dreamy/UI/TweenPresetLibrary.",
                MessageType.Info);
            if (TweenPresetLibrary.Load() == null &&
                GUILayout.Button("Create Tween Preset Library"))
            {
                CreatePresetLibrary();
            }
            EditorGUILayout.Space(6f);

            TweenCollectionMode mode = (TweenCollectionMode)collectionMode.enumValueIndex;
            if (mode == TweenCollectionMode.Manual)
            {
                EditorGUILayout.PropertyField(manualTargets, true);
                DrawAddEffectControls();
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Auto collects UITweenBase components below this transform and stops at nested UITweenPlayers.",
                    MessageType.Info);
            }

            serializedObject.ApplyModifiedProperties();

            UITweenPlayer player = (UITweenPlayer)target;
            if (GUILayout.Button("Rebuild Tween Cache"))
            {
                Undo.RecordObject(player, "Rebuild Tween Cache");
                player.RebuildCache();
                EditorUtility.SetDirty(player);
            }
        }

        private void DrawAddEffectControls()
        {
            for (int groupIndex = 0; groupIndex < manualTargets.arraySize; groupIndex++)
            {
                SerializedProperty group = manualTargets.GetArrayElementAtIndex(groupIndex);
                SerializedProperty tweens = group.FindPropertyRelative("tweens");
                if (GUILayout.Button($"Add Effect to Target {groupIndex + 1}"))
                {
                    ShowTweenMenu(tweens);
                }
            }
        }

        private void ShowTweenMenu(SerializedProperty tweens)
        {
            GenericMenu menu = new GenericMenu();
            Type[] types = TypeCache.GetTypesDerivedFrom<UITweenDefinition>()
                .Where(type => !type.IsAbstract && type.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(type => type.Name)
                .ToArray();

            foreach (Type type in types)
            {
                menu.AddItem(new GUIContent(ObjectNames.NicifyVariableName(type.Name)), false, () =>
                {
                    serializedObject.Update();
                    int index = tweens.arraySize;
                    tweens.InsertArrayElementAtIndex(index);
                    tweens.GetArrayElementAtIndex(index).managedReferenceValue =
                        Activator.CreateInstance(type);
                    serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }

        private static void CreatePresetLibrary()
        {
            const string resourcesFolder = "Assets/Resources";
            const string dreamyFolder = "Assets/Resources/Dreamy";
            const string uiFolder = "Assets/Resources/Dreamy/UI";
            if (!AssetDatabase.IsValidFolder(resourcesFolder))
                AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder(dreamyFolder))
                AssetDatabase.CreateFolder(resourcesFolder, "Dreamy");
            if (!AssetDatabase.IsValidFolder(uiFolder))
                AssetDatabase.CreateFolder(dreamyFolder, "UI");

            TweenPresetLibrary library = ScriptableObject.CreateInstance<TweenPresetLibrary>();
            AssetDatabase.CreateAsset(library, uiFolder + "/TweenPresetLibrary.asset");
            AssetDatabase.SaveAssets();
            Selection.activeObject = library;
        }
    }
}
