using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamy.UI
{
    public enum TweenEffectType
    {
        Scale,
        Fade,
        Move,
        Rotate,
        Size,
        Color
    }

    [Serializable]
    public sealed class TweenTimingOverride
    {
        [SerializeField] private bool overrideEaseIn;
        [SerializeField] private Ease easeIn = Ease.OutBack;
        [SerializeField] private bool overrideEaseOut;
        [SerializeField] private Ease easeOut = Ease.InBack;
        [SerializeField] private bool overrideDurationIn;
        [SerializeField, Min(0f)] private float durationIn = 0.25f;
        [SerializeField] private bool overrideDurationOut;
        [SerializeField, Min(0f)] private float durationOut = 0.2f;
        [SerializeField] private bool overrideDelayIn;
        [SerializeField, Min(0f)] private float delayIn;
        [SerializeField] private bool overrideDelayOut;
        [SerializeField, Min(0f)] private float delayOut;

        internal TweenTimingData Resolve(TweenSettings preset)
        {
            return TweenSettingsResolver.Resolve(
                preset,
                overrideEaseIn, easeIn,
                overrideEaseOut, easeOut,
                overrideDurationIn, durationIn,
                overrideDurationOut, durationOut,
                overrideDelayIn ? delayIn : preset ? preset.DelayIn : 0f,
                overrideDelayOut ? delayOut : preset ? preset.DelayOut : 0f);
        }
    }

    [Serializable]
    public abstract class TweenEffect
    {
        [SerializeField] private bool enabled = true;
        [SerializeField] private ETweenRun runType = ETweenRun.Auto;
        [SerializeField] private TweenSettings preset;
        [SerializeField] private TweenTimingOverride timing = new TweenTimingOverride();

        [NonSerialized] private TweenPlayback playback;
        [NonSerialized] private Transform initializedTarget;

        public bool IsAutoRun => enabled && runType == ETweenRun.Auto;
        public abstract TweenEffectType Type { get; }

        internal void Init(Transform target)
        {
            if (target == null || initializedTarget == target)
            {
                return;
            }

            initializedTarget = target;
            CaptureShownState(target);
            ApplyHidden(target);
        }

        internal UniTask Show(Transform target, TweenSettings inheritedPreset, Component owner)
        {
            return Play(target, inheritedPreset, owner, true);
        }

        internal UniTask Hide(Transform target, TweenSettings inheritedPreset, Component owner)
        {
            return Play(target, inheritedPreset, owner, false);
        }

        internal void Kill()
        {
            playback?.Kill();
        }

        protected abstract void CaptureShownState(Transform target);
        protected abstract Tween CreateTween(Transform target, bool show, TweenTimingData timing);
        protected abstract void ApplyShown(Transform target);
        protected abstract void ApplyHidden(Transform target);

        private UniTask Play(
            Transform target,
            TweenSettings inheritedPreset,
            Component owner,
            bool show)
        {
            if (!enabled || target == null || owner == null)
            {
                return UniTask.CompletedTask;
            }

            Init(target);
            TweenTimingData resolved = timing.Resolve(preset ? preset : inheritedPreset);
            try
            {
                Tween tween = CreateTween(target, show, resolved);
                return Playback.Play(
                    tween,
                    () =>
                    {
                        if (target == null)
                        {
                            return;
                        }

                        if (show) ApplyShown(target);
                        else ApplyHidden(target);
                    },
                    owner);
            }
            catch (MissingReferenceException)
            {
                return UniTask.CompletedTask;
            }
        }

        private TweenPlayback Playback => playback ??= new TweenPlayback();
    }

    [Serializable]
    public sealed class ScaleTweenEffect : TweenEffect
    {
        [SerializeField] private Vector3 shownScale = Vector3.one;
        [SerializeField] private Vector3 hiddenScale = Vector3.zero;
        public override TweenEffectType Type => TweenEffectType.Scale;
        protected override void CaptureShownState(Transform target) => shownScale = target.localScale;
        protected override Tween CreateTween(Transform target, bool show, TweenTimingData timing) =>
            target.DOScale(show ? shownScale : hiddenScale, show ? timing.DurationIn : timing.DurationOut)
                .SetEase(show ? timing.EaseIn : timing.EaseOut)
                .SetDelay(show ? timing.DelayIn : timing.DelayOut);
        protected override void ApplyShown(Transform target) => target.localScale = shownScale;
        protected override void ApplyHidden(Transform target) => target.localScale = hiddenScale;
    }

    [Serializable]
    public sealed class FadeTweenEffect : TweenEffect
    {
        [SerializeField, Range(0f, 1f)] private float shownAlpha = 1f;
        [SerializeField, Range(0f, 1f)] private float hiddenAlpha;
        [SerializeField] private bool controlInteractable = true;
        public override TweenEffectType Type => TweenEffectType.Fade;
        protected override void CaptureShownState(Transform target)
        {
            CanvasGroup group = target.GetComponent<CanvasGroup>();
            if (group != null) shownAlpha = group.alpha;
        }
        protected override Tween CreateTween(Transform target, bool show, TweenTimingData timing)
        {
            CanvasGroup group = target.GetComponent<CanvasGroup>();
            if (group == null) return null;
            if (!show && controlInteractable) group.interactable = false;
            return group.DOFade(show ? shownAlpha : hiddenAlpha, show ? timing.DurationIn : timing.DurationOut)
                .SetEase(show ? timing.EaseIn : timing.EaseOut)
                .SetDelay(show ? timing.DelayIn : timing.DelayOut);
        }
        protected override void ApplyShown(Transform target)
        {
            CanvasGroup group = target.GetComponent<CanvasGroup>();
            if (group == null) return;
            group.alpha = shownAlpha;
            if (controlInteractable)
            {
                group.interactable = true;
                group.blocksRaycasts = true;
            }
        }
        protected override void ApplyHidden(Transform target)
        {
            CanvasGroup group = target.GetComponent<CanvasGroup>();
            if (group == null) return;
            group.alpha = hiddenAlpha;
            if (controlInteractable)
            {
                group.interactable = false;
                group.blocksRaycasts = false;
            }
        }
    }

    [Serializable]
    public sealed class MoveTweenEffect : TweenEffect
    {
        [SerializeField] private Vector2 hiddenOffset;
        [NonSerialized] private Vector2 shownPosition;
        public override TweenEffectType Type => TweenEffectType.Move;
        protected override void CaptureShownState(Transform target)
        {
            RectTransform rect = target as RectTransform;
            if (rect != null) shownPosition = rect.anchoredPosition;
        }
        protected override Tween CreateTween(Transform target, bool show, TweenTimingData timing)
        {
            RectTransform rect = target as RectTransform;
            return rect == null ? null : rect.DOAnchorPos(show ? shownPosition : shownPosition + hiddenOffset,
                    show ? timing.DurationIn : timing.DurationOut)
                .SetEase(show ? timing.EaseIn : timing.EaseOut).SetDelay(show ? timing.DelayIn : timing.DelayOut);
        }
        protected override void ApplyShown(Transform target) { if (target is RectTransform rect) rect.anchoredPosition = shownPosition; }
        protected override void ApplyHidden(Transform target) { if (target is RectTransform rect) rect.anchoredPosition = shownPosition + hiddenOffset; }
    }

    [Serializable]
    public sealed class RotateTweenEffect : TweenEffect
    {
        [SerializeField] private Vector3 hiddenRotation = new Vector3(0f, 0f, -12f);
        [NonSerialized] private Vector3 shownRotation;
        public override TweenEffectType Type => TweenEffectType.Rotate;
        protected override void CaptureShownState(Transform target) => shownRotation = target.localEulerAngles;
        protected override Tween CreateTween(Transform target, bool show, TweenTimingData timing) =>
            target.DOLocalRotate(show ? shownRotation : hiddenRotation, show ? timing.DurationIn : timing.DurationOut)
                .SetEase(show ? timing.EaseIn : timing.EaseOut).SetDelay(show ? timing.DelayIn : timing.DelayOut);
        protected override void ApplyShown(Transform target) => target.localEulerAngles = shownRotation;
        protected override void ApplyHidden(Transform target) => target.localEulerAngles = hiddenRotation;
    }

    [Serializable]
    public sealed class SizeTweenEffect : TweenEffect
    {
        [SerializeField] private Vector2 hiddenSize;
        [NonSerialized] private Vector2 shownSize;
        public override TweenEffectType Type => TweenEffectType.Size;
        protected override void CaptureShownState(Transform target)
        {
            if (target is RectTransform rect) shownSize = rect.sizeDelta;
        }
        protected override Tween CreateTween(Transform target, bool show, TweenTimingData timing)
        {
            RectTransform rect = target as RectTransform;
            return rect == null ? null : rect.DOSizeDelta(show ? shownSize : hiddenSize,
                    show ? timing.DurationIn : timing.DurationOut)
                .SetEase(show ? timing.EaseIn : timing.EaseOut).SetDelay(show ? timing.DelayIn : timing.DelayOut);
        }
        protected override void ApplyShown(Transform target) { if (target is RectTransform rect) rect.sizeDelta = shownSize; }
        protected override void ApplyHidden(Transform target) { if (target is RectTransform rect) rect.sizeDelta = hiddenSize; }
    }

    [Serializable]
    public sealed class ColorTweenEffect : TweenEffect
    {
        [SerializeField] private Color hiddenColor = Color.clear;
        [NonSerialized] private Color shownColor = Color.white;
        public override TweenEffectType Type => TweenEffectType.Color;
        protected override void CaptureShownState(Transform target)
        {
            Graphic graphic = target.GetComponent<Graphic>();
            if (graphic != null) shownColor = graphic.color;
        }
        protected override Tween CreateTween(Transform target, bool show, TweenTimingData timing)
        {
            Graphic graphic = target.GetComponent<Graphic>();
            return graphic == null ? null : graphic.DOColor(show ? shownColor : hiddenColor,
                    show ? timing.DurationIn : timing.DurationOut)
                .SetEase(show ? timing.EaseIn : timing.EaseOut).SetDelay(show ? timing.DelayIn : timing.DelayOut);
        }
        protected override void ApplyShown(Transform target) { Graphic graphic = target.GetComponent<Graphic>(); if (graphic != null) graphic.color = shownColor; }
        protected override void ApplyHidden(Transform target) { Graphic graphic = target.GetComponent<Graphic>(); if (graphic != null) graphic.color = hiddenColor; }
    }
}
