using System;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class MoveTweenEffectEntry : TweenEffectEntry
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private Vector2 offset;

        private Vector2 activePosition;
        private Vector2 inactivePosition;

        protected override void Setup(Component owner)
        {
            RectTransform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                LogMissingTarget(nameof(RectTransform));
                return;
            }

            activePosition = resolvedTarget.anchoredPosition;
            inactivePosition = activePosition + offset;
        }

        protected override Tween CreateShowTween(TweenTimingData timing)
        {
            RectTransform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                return null;
            }

            return TweenEffectFactory.Move(
                resolvedTarget,
                activePosition,
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

            return TweenEffectFactory.Move(
                resolvedTarget,
                inactivePosition,
                timing.DurationOut,
                timing.EaseOut,
                timing.DelayOut);
        }

        protected override void ApplyActive()
        {
            RectTransform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.anchoredPosition = activePosition;
            }
        }

        protected override void ApplyInactive()
        {
            RectTransform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.anchoredPosition = inactivePosition;
            }
        }
    }
}
