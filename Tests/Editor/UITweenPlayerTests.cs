/*
using NUnit.Framework;
using System.Threading;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

namespace Dreamy.UI.Tests.Editor
{
    public sealed class UITweenPlayerTests
    {
        [Test]
        public void AutoCollection_ExcludesEffectsOwnedByNestedPlayer()
        {
            GameObject root = new GameObject("Root");
            UITweenPlayer parent = root.AddComponent<UITweenPlayer>();
            root.AddComponent<UITweenScale>();

            GameObject nested = new GameObject("Nested");
            nested.transform.SetParent(root.transform);
            nested.AddComponent<UITweenPlayer>();
            nested.AddComponent<UITweenScale>();

            parent.RebuildCache();

            Assert.That(parent.Tweens.Count, Is.EqualTo(1));
            Object.DestroyImmediate(root);
        }

        [Test]
        public void AutoCollection_PrunesDestroyedEffects()
        {
            GameObject root = new GameObject("Root");
            UITweenPlayer player = root.AddComponent<UITweenPlayer>();
            UITweenScale effect = root.AddComponent<UITweenScale>();
            player.RebuildCache();

            Object.DestroyImmediate(effect);
            player.Kill();

            Assert.That(player.Tweens.Count, Is.Zero);
            Object.DestroyImmediate(root);
        }

        [Test]
        public void ManualCollection_PlaysInlineDefinition()
        {
            GameObject root = new GameObject("Root");
            GameObject target = new GameObject("Target", typeof(RectTransform));
            target.transform.SetParent(root.transform);
            UITweenPlayer player = root.AddComponent<UITweenPlayer>();

            try
            {
                SerializedObject serializedPlayer = new SerializedObject(player);
                serializedPlayer.FindProperty("collectionMode").enumValueIndex =
                    (int)TweenCollectionMode.Manual;
                SerializedProperty targets = serializedPlayer.FindProperty("manualTargets");
                targets.arraySize = 1;
                SerializedProperty group = targets.GetArrayElementAtIndex(0);
                group.FindPropertyRelative("target").objectReferenceValue = target.transform;
                SerializedProperty tweens = group.FindPropertyRelative("tweens");
                tweens.arraySize = 1;
                tweens.GetArrayElementAtIndex(0).managedReferenceValue = new ScaleTweenEffect();
                serializedPlayer.ApplyModifiedPropertiesWithoutUndo();

                SerializedObject serializedTween = new SerializedObject(player);
                SerializedProperty tween = serializedTween.FindProperty("manualTargets")
                    .GetArrayElementAtIndex(0)
                    .FindPropertyRelative("tweens")
                    .GetArrayElementAtIndex(0);
                tween.FindPropertyRelative("timing")
                    .FindPropertyRelative("overrideDurationIn").boolValue = true;
                tween.FindPropertyRelative("timing")
                    .FindPropertyRelative("durationIn").floatValue = 0f;
                serializedTween.ApplyModifiedPropertiesWithoutUndo();

                player.Init();
                Assert.That(player.Tweens[0], Is.AssignableTo<ITween>());
                var show = player.ShowTween(CancellationToken.None);
                DOTween.CompleteAll();
                show.GetAwaiter().GetResult();

                Assert.That(target.transform.localScale, Is.EqualTo(Vector3.one));
            }
            finally
            {
                DOTween.KillAll();
                Object.DestroyImmediate(root);
            }
        }
    }
}
*/
