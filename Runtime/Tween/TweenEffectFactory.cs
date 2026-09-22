using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamy.UI
{
    internal static class TweenEffectFactory
    {
        public static Tween Scale(
            Transform target,
            Vector3 scale,
            float duration,
            Ease ease,
            float delay)
        {
            return target.DOScale(scale, duration)
                .SetEase(ease)
                .SetDelay(delay);
        }

        public static Tween Fade(
            CanvasGroup target,
            float alpha,
            float duration,
            Ease ease,
            float delay)
        {
            return target.DOFade(alpha, duration)
                .SetEase(ease)
                .SetDelay(delay);
        }

        public static Tween Move(
            RectTransform target,
            Vector2 position,
            float duration,
            Ease ease,
            float delay)
        {
            return target.DOAnchorPos(position, duration)
                .SetEase(ease)
                .SetDelay(delay);
        }

        public static Tween Rotate(
            Transform target,
            Vector3 rotation,
            float duration,
            Ease ease,
            float delay)
        {
            return target.DOLocalRotate(rotation, duration)
                .SetEase(ease)
                .SetDelay(delay);
        }

        public static Tween Size(
            RectTransform target,
            Vector2 size,
            float duration,
            Ease ease,
            float delay)
        {
            return target.DOSizeDelta(size, duration)
                .SetEase(ease)
                .SetDelay(delay);
        }

        public static Tween Color(
            Graphic target,
            Color color,
            float duration,
            Ease ease,
            float delay)
        {
            return target.DOColor(color, duration)
                .SetEase(ease)
                .SetDelay(delay);
        }

        public static Tween Punch(
            Transform target,
            TweenPunchMode mode,
            Vector3 punch,
            float duration,
            int vibrato,
            float elasticity,
            Ease ease,
            float delay)
        {
            Tween tween = mode switch
            {
                TweenPunchMode.Position => target.DOPunchPosition(
                    punch,
                    duration,
                    vibrato,
                    elasticity),
                TweenPunchMode.Rotation => target.DOPunchRotation(
                    punch,
                    duration,
                    vibrato,
                    elasticity),
                _ => target.DOPunchScale(
                    punch,
                    duration,
                    vibrato,
                    elasticity)
            };

            return tween.SetEase(ease).SetDelay(delay);
        }
    }
}
