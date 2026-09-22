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
        [SerializeField] private TweenSettings preset;
        [FormerlySerializedAs("effects")]
        [SerializeReference] private List<UITweenDefinition> tweens = new List<UITweenDefinition>();

        public Transform Target => target;
        public TweenSettings Preset => preset;
        public IReadOnlyList<UITweenDefinition> Tweens => tweens;

        internal void CollectTweens(List<ITween> destination, Component owner)
        {
            if (target == null) return;

            foreach (UITweenDefinition tween in tweens)
            {
                if (tween == null) continue;

                TweenSettings inheritedPreset = preset != null
                    ? preset
                    : TweenPresetLibrary.Load()?.Get(tween.Type);
                tween.Bind(target, inheritedPreset, owner);
                destination.Add(tween);
            }
        }
    }
}
