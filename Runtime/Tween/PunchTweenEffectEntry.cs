using System;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class PunchTweenEffectEntry : TweenEffectEntry
    {
        [SerializeField] private Transform target;
        [SerializeField] private TweenPunchMode mode = TweenPunchMode.Scale;
        [SerializeField] private Vector3 punch = new Vector3(0.15f, 0.15f, 0f);
        [SerializeField, Min(0)] private int vibrato = 8;
        [SerializeField, Range(0f, 1f)] private float elasticity = 0.8f;
        [SerializeField] private bool playOnHide;

        protected override Tween CreateShowTween(TweenTimingData timing)
        {
            return CreatePunchTween(timing.DurationIn, timing.DelayIn, timing.EaseIn);
        }

        protected override Tween CreateHideTween(TweenTimingData timing)
        {
            return playOnHide
                ? CreatePunchTween(timing.DurationOut, timing.DelayOut, timing.EaseOut)
                : null;
        }

        protected override void ApplyActive()
        {
        }

        protected override void ApplyInactive()
        {
        }

        private Tween CreatePunchTween(float duration, float delay, Ease ease)
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                LogMissingTarget(nameof(Transform));
                return null;
            }

            return TweenEffectFactory.Punch(
                resolvedTarget,
                mode,
                punch,
                duration,
                vibrato,
                elasticity,
                ease,
                delay);
        }
    }
}
