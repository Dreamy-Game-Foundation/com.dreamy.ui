using System;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class FadeTweenEffectEntry : TweenEffectEntry
    {
        [SerializeField] private CanvasGroup target;
        [SerializeField, Range(0f, 1f)] private float inactiveAlpha;
        [SerializeField, Range(0f, 1f)] private float activeAlpha = 1f;
        [SerializeField] private bool controlInteractable = true;

        protected override Tween CreateShowTween(TweenTimingData timing)
        {
            CanvasGroup resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                LogMissingTarget(nameof(CanvasGroup));
                return null;
            }

            return TweenEffectFactory.Fade(
                resolvedTarget,
                activeAlpha,
                timing.DurationIn,
                timing.EaseIn,
                timing.DelayIn);
        }

        protected override Tween CreateHideTween(TweenTimingData timing)
        {
            CanvasGroup resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                LogMissingTarget(nameof(CanvasGroup));
                return null;
            }

            if (controlInteractable)
            {
                resolvedTarget.interactable = false;
            }

            return TweenEffectFactory.Fade(
                resolvedTarget,
                inactiveAlpha,
                timing.DurationOut,
                timing.EaseOut,
                timing.DelayOut);
        }

        protected override void ApplyActive()
        {
            CanvasGroup resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                return;
            }

            resolvedTarget.alpha = activeAlpha;
            if (controlInteractable)
            {
                resolvedTarget.interactable = true;
            }
        }

        protected override void ApplyInactive()
        {
            CanvasGroup resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                return;
            }

            resolvedTarget.alpha = inactiveAlpha;
            if (controlInteractable)
            {
                resolvedTarget.interactable = false;
            }
        }
    }
}
