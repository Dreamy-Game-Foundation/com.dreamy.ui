using NUnit.Framework;
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
    }
}
