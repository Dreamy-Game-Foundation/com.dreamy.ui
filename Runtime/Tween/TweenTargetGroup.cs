using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class TweenTargetGroup
    {
        [SerializeField] private Transform target;
        [SerializeField] private TweenSettings preset;
        [SerializeReference] private List<TweenEffect> effects = new List<TweenEffect>();

        public Transform Target => target;
        public TweenSettings Preset => preset;
        public IReadOnlyList<TweenEffect> Effects => effects;
    }
}
