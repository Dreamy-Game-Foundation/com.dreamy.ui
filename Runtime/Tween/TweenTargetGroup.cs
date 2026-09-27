using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class TweenTargetGroup
    {
        [SerializeField] private Transform target;
        [FormerlySerializedAs("preset")]
        [SerializeField, HideInInspector] private TweenSettings legacyPreset;
        [FormerlySerializedAs("effects")]
        [SerializeReference] private List<UITweenDefinition> tweens = new List<UITweenDefinition>();

        public Transform Target => target;
        public IReadOnlyList<UITweenDefinition> Tweens => tweens;

        internal bool ApplyMissingPresets(TweenPresetLibrary library)
        {
            bool changed = false;
            foreach (UITweenDefinition tween in tweens)
            {
                if (tween == null) continue;

                TweenSettings defaultPreset = legacyPreset != null
                    ? legacyPreset
                    : library != null ? library.Get(tween.Type) : null;
                changed |= tween.ApplyPresetIfMissing(defaultPreset);
            }

            return changed;
        }

        internal void CollectTweens(List<ITween> destination, Component owner)
        {
            if (target == null) return;

            foreach (UITweenDefinition tween in tweens)
            {
                if (tween == null) continue;

                TweenSettings inheritedPreset = legacyPreset != null
                    ? legacyPreset
                    : TweenPresetLibrary.Load()?.Get(tween.Type);
                tween.Bind(target, inheritedPreset, owner);
                destination.Add(tween);
            }
        }
    }
}
