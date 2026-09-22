using System;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class SizeTweenEffectEntry : TweenEffectEntry
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private Vector2 inactiveSize;

        private Vector2 activeSize;

        protected override void Setup(Component owner)
        {
            RectTransform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                LogMissingTarget(nameof(RectTransform));
                return;
            }

            activeSize = resolvedTarget.sizeDelta;
        }

        protected override Tween CreateShowTween(TweenTimingData timing)
        {
            RectTransform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                return null;
            }

            return TweenEffectFactory.Size(
                resolvedTarget,
                activeSize,
                timing.DurationIn,
                timing.EaseIn,
                timing.DelayIn);
        }

        protected override Tween CreateHideTween(TweenTimingData timing)
        {
            RectTransform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                return null;
            }

            return TweenEffectFactory.Size(
                resolvedTarget,
                inactiveSize,
                timing.DurationOut,
                timing.EaseOut,
                timing.DelayOut);
        }

        protected override void ApplyActive()
        {
            RectTransform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.sizeDelta = activeSize;
            }
        }

        protected override void ApplyInactive()
        {
            RectTransform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.sizeDelta = inactiveSize;
            }
        }
    }
}
