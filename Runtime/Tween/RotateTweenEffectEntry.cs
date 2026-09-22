using System;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class RotateTweenEffectEntry : TweenEffectEntry
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 inactiveRotation = new Vector3(0f, 0f, -12f);

        private Vector3 activeRotation;

        protected override void Setup(Component owner)
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                LogMissingTarget(nameof(Transform));
                return;
            }

            activeRotation = resolvedTarget.localEulerAngles;
        }

        protected override Tween CreateShowTween(TweenTimingData timing)
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                return null;
            }

            return TweenEffectFactory.Rotate(
                resolvedTarget,
                activeRotation,
                timing.DurationIn,
                timing.EaseIn,
                timing.DelayIn);
        }

        protected override Tween CreateHideTween(TweenTimingData timing)
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget == null)
            {
                return null;
            }

            return TweenEffectFactory.Rotate(
                resolvedTarget,
                inactiveRotation,
                timing.DurationOut,
                timing.EaseOut,
                timing.DelayOut);
        }

        protected override void ApplyActive()
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.localEulerAngles = activeRotation;
            }
        }

        protected override void ApplyInactive()
        {
            Transform resolvedTarget = ResolveTarget(target);
            if (resolvedTarget != null)
            {
                resolvedTarget.localEulerAngles = inactiveRotation;
            }
        }
    }
}
