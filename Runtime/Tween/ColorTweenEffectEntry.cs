using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class ColorTweenEffectEntry : TweenEffectEntry
    {
        [SerializeField] private Graphic target;
        [SerializeField] private Color inactiveColor = Color.clear;

        private Color activeColor = Color.white;

        protected override void Setup(Component owner)
        {
            Graphic resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                LogMissingTarget(nameof(Graphic));
                return;
            }

            activeColor = resolvedTarget.color;
        }

        protected override Tween CreateShowTween(TweenTimingData timing)
        {
            Graphic resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                return null;
            }

            return TweenEffectFactory.Color(
                resolvedTarget,
                activeColor,
                timing.DurationIn,
                timing.EaseIn,
                timing.DelayIn);
        }

        protected override Tween CreateHideTween(TweenTimingData timing)
        {
            Graphic resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                return null;
            }

            return TweenEffectFactory.Color(
                resolvedTarget,
                inactiveColor,
                timing.DurationOut,
                timing.EaseOut,
                timing.DelayOut);
        }

        protected override void ApplyActive()
        {
            Graphic resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.color = activeColor;
            }
        }

        protected override void ApplyInactive()
        {
            Graphic resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.color = inactiveColor;
            }
        }
    }
}
