using System;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class ScaleTweenEffectEntry : TweenEffectEntry
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 inactiveScale = Vector3.zero;
        [SerializeField] private Vector3 activeScale = Vector3.one;

        protected override Tween CreateShowTween(TweenTimingData timing)
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                LogMissingTarget(nameof(Transform));
                return null;
            }

            return TweenEffectFactory.Scale(
                resolvedTarget,
                activeScale,
                timing.DurationIn,
                timing.EaseIn,
                timing.DelayIn);
        }

        protected override Tween CreateHideTween(TweenTimingData timing)
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                LogMissingTarget(nameof(Transform));
                return null;
            }

            return TweenEffectFactory.Scale(
                resolvedTarget,
                inactiveScale,
                timing.DurationOut,
                timing.EaseOut,
                timing.DelayOut);
        }

        protected override void ApplyActive()
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.localScale = activeScale;
            }
        }

        protected override void ApplyInactive()
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.localScale = inactiveScale;
            }
        }
    }
}
