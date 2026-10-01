using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Dreamy.UI.Tests.Editor
{
    public sealed class UITweenPlayerTests
    {
        private GameObject root;
        private UITweenPlayer player;
        private TweenSettings preset;
        private readonly List<Object> assets = new List<Object>();

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Tween test root");
            player = root.AddComponent<UITweenPlayer>();
            preset = ScriptableObject.CreateInstance<TweenSettings>();
            assets.Add(preset);
            preset.DelayIn = 0.4f;
            preset.DelayOut = 0.3f;
            preset.DurationIn = preset.DurationOut = 0.1f;
            preset.EaseIn = preset.EaseOut = Ease.Linear;
        }

        [TearDown]
        public void TearDown()
        {
            player.Kill();
            Object.DestroyImmediate(root);
            foreach (Object asset in assets) Object.DestroyImmediate(asset);
            assets.Clear();
        }

        [TestCase(TweenCollectionMode.Auto)]
        [TestCase(TweenCollectionMode.Manual)]
        public void Playback_RespectsPresetShowAndHideDelay(TweenCollectionMode mode)
        {
            Transform target = AddTween(mode);
            player.Init();
            UniTask show = player.ShowTween(CancellationToken.None);
            MakeManual(target);
            DOTween.ManualUpdate(0.39f, 0.39f);
            Assert.That(target.localScale, Is.EqualTo(Vector3.zero));
            DOTween.ManualUpdate(0.12f, 0.12f);
            show.GetAwaiter().GetResult();
            Assert.That(target.localScale, Is.EqualTo(Vector3.one));

            UniTask hide = player.HideTween(CancellationToken.None);
            MakeManual(target);
            DOTween.ManualUpdate(0.29f, 0.29f);
            Assert.That(target.localScale, Is.EqualTo(Vector3.one));
            DOTween.ManualUpdate(0.12f, 0.12f);
            hide.GetAwaiter().GetResult();
            Assert.That(target.localScale, Is.EqualTo(Vector3.zero));
        }

        [TestCase(TweenCollectionMode.Auto)]
        [TestCase(TweenCollectionMode.Manual)]
        public void IndexDelay_ReplacesConfiguredDelay_AndClearRestoresPreset(TweenCollectionMode mode)
        {
            Transform target = AddTween(mode);
            TweenDelayByIndex index = target.gameObject.AddComponent<TweenDelayByIndex>();
            player.RebuildCache();
            index.Apply(2, 0.05f, 0.03f, 1);
            AssertTiming(player.Tweens[0], 0.1f, 0.03f);
            index.Clear();
            AssertTiming(player.Tweens[0], 0.4f, 0.3f);
            index.Apply(-1, -1f, -1f, -1);
            AssertTiming(player.Tweens[0], 0f, 0f);
        }

        [TestCase(TweenCollectionMode.Auto)]
        [TestCase(TweenCollectionMode.Manual)]
        public void ExplicitZeroDelay_OverridesPreset(TweenCollectionMode mode)
        {
            AddTween(mode);
            player.RebuildCache();
            object settingsOwner = mode == TweenCollectionMode.Auto
                ? (object)player.Tweens[0]
                : Get(player.Tweens[0], "overrideSettings");
            Set(settingsOwner, "overrideDelayIn", true);
            Set(settingsOwner, "overrideDelayOut", true);
            AssertTiming(player.Tweens[0], 0f, 0f);
        }

        [TestCase(TweenCollectionMode.Auto)]
        [TestCase(TweenCollectionMode.Manual)]
        public void Stagger_AddsOnce_ReversesHide_AndClearsWhenDisabled(TweenCollectionMode mode)
        {
            AddTween(mode);
            AddTween(mode);
            object stagger = Get(player, "stagger");
            Set(stagger, "enabled", true);
            Set(stagger, "showInterval", 0.05f);
            Set(stagger, "hideInterval", 0.03f);
            player.Init();
            PlayAndKill();
            AssertTiming(player.Tweens[0], 0.4f, 0.33f);
            AssertTiming(player.Tweens[1], 0.45f, 0.3f);
            PlayAndKill();
            AssertTiming(player.Tweens[1], 0.45f, 0.3f);
            Set(stagger, "enabled", false);
            PlayAndKill();
            AssertTiming(player.Tweens[0], 0.4f, 0.3f);
            AssertTiming(player.Tweens[1], 0.4f, 0.3f);
        }

        [TestCase(TweenCollectionMode.Auto)]
        [TestCase(TweenCollectionMode.Manual)]
        public void DisabledEffect_DoesNotConsumeStaggerSlot(TweenCollectionMode mode)
        {
            AddTween(mode);
            AddTween(mode);
            player.RebuildCache();
            if (mode == TweenCollectionMode.Auto) ((UITweenBase)player.Tweens[0]).enabled = false;
            else Set(player.Tweens[0], "enabled", false);
            Set(Get(player, "stagger"), "enabled", true);
            player.Init();
            PlayAndKill();
            AssertTiming(player.Tweens[1], 0.4f, 0.3f);
        }

        [Test]
        public void LegacyComponentDelay_IsPreserved_AndNegativeValuesAreClamped()
        {
            AddTween(TweenCollectionMode.Auto);
            player.RebuildCache();
            object tween = player.Tweens[0];
            Set(tween, "delayIn", 0.15f);
            Set(tween, "delayOut", -1f);
            AssertTiming(tween, 0.15f, 0f);
        }

        [Test]
        public void AutoCollection_ExcludesNestedPlayer_AndPrunesDestroyedEffects()
        {
            Transform target = AddTween(TweenCollectionMode.Auto);
            GameObject nested = new GameObject("Nested player");
            nested.transform.SetParent(root.transform);
            nested.AddComponent<UITweenPlayer>();
            nested.AddComponent<UITweenScale>();
            player.RebuildCache();
            Assert.That(player.Tweens.Count, Is.EqualTo(1));
            Object.DestroyImmediate(target.GetComponent<UITweenScale>());
            player.Kill();
            Assert.That(player.Tweens.Count, Is.Zero);
        }

        [UnityTest]
        public IEnumerator RebuildCache_KillsPlaybackOfRemovedManualTarget()
        {
            AddTween(TweenCollectionMode.Manual);
            player.Init();
            UniTask task = player.ShowTween(CancellationToken.None);
            ((List<TweenTargetGroup>)Get(player, "manualTargets")).Clear();
            player.RebuildCache();
            Assert.That(task.Status, Is.EqualTo(UniTaskStatus.Succeeded));
            yield return task.ToCoroutine();
            Assert.That(player.Tweens.Count, Is.Zero);
        }

        [TestCase(typeof(UITweenScale), TweenEffectType.Scale)]
        [TestCase(typeof(UITweenFade), TweenEffectType.Fade)]
        [TestCase(typeof(UITweenMove), TweenEffectType.Move)]
        [TestCase(typeof(UITweenRotate), TweenEffectType.Rotate)]
        [TestCase(typeof(UITweenSize), TweenEffectType.Size)]
        public void Reset_AssignsResourcePreset_ForEveryComponent(Type type, TweenEffectType effectType)
        {
            TweenPresetLibrary previous = (TweenPresetLibrary)GetStatic(typeof(TweenPresetLibrary), "cached");
            TweenPresetLibrary library = ScriptableObject.CreateInstance<TweenPresetLibrary>();
            assets.Add(library);
            TweenDefaultPreset mapping = new TweenDefaultPreset();
            Set(mapping, "type", effectType);
            Set(mapping, "preset", preset);
            Set(library, "presets", new List<TweenDefaultPreset> { mapping });
            SetStatic(typeof(TweenPresetLibrary), "cached", library);
            try
            {
                GameObject target = new GameObject("Reset target", typeof(RectTransform));
                target.transform.SetParent(root.transform);
                Component tween = target.AddComponent(type);
                type.GetMethod("Reset", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(tween, null);
                Assert.That(Get(tween, "settings"), Is.SameAs(preset));
                Assert.That(TweenPresetLibrary.Resolve(effectType), Is.SameAs(preset));
            }
            finally { SetStatic(typeof(TweenPresetLibrary), "cached", previous); }
        }

        private Transform AddTween(TweenCollectionMode mode)
        {
            Set(player, "collectionMode", mode);
            GameObject target = new GameObject("Tween target", typeof(RectTransform));
            target.transform.SetParent(root.transform);
            if (mode == TweenCollectionMode.Auto)
            {
                UITweenScale component = target.AddComponent<UITweenScale>();
                Set(component, "settings", preset);
            }
            else
            {
                ScaleTweenEffect effect = new ScaleTweenEffect();
                Set(effect, "settings", preset);
                TweenTargetGroup group = new TweenTargetGroup();
                Set(group, "target", target.transform);
                Set(group, "tweens", new List<UITweenDefinition> { effect });
                ((List<TweenTargetGroup>)Get(player, "manualTargets")).Add(group);
            }
            return target.transform;
        }

        private void PlayAndKill()
        {
            UniTask task = player.ShowTween(CancellationToken.None);
            player.Kill();
            task.Forget();
        }

        private static void MakeManual(Transform target)
        {
            List<Tween> tweens = DOTween.TweensByTarget(target);
            Assert.That(tweens, Is.Not.Null);
            foreach (Tween tween in tweens) tween.SetUpdate(UpdateType.Manual);
        }

        private static void AssertTiming(object tween, float show, float hide)
        {
            Type type = tween is UITweenBase ? typeof(UITweenBase) : typeof(UITweenDefinition);
            TweenTimingData timing = (TweenTimingData)type.GetMethod("ResolveTiming", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(tween, null);
            Assert.That(timing.DelayIn, Is.EqualTo(show).Within(0.00001f));
            Assert.That(timing.DelayOut, Is.EqualTo(hide).Within(0.00001f));
        }

        private static FieldInfo Field(Type type, string name)
        {
            while (type != null)
            {
                FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
                if (field != null) return field;
                type = type.BaseType;
            }
            throw new MissingFieldException(name);
        }
        private static object Get(object target, string name) => Field(target.GetType(), name).GetValue(target);
        private static void Set(object target, string name, object value) => Field(target.GetType(), name).SetValue(target, value);
        private static object GetStatic(Type type, string name) => Field(type, name).GetValue(null);
        private static void SetStatic(Type type, string name, object value) => Field(type, name).SetValue(null, value);
    }
}
