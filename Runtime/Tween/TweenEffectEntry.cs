using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [Serializable]
    public abstract class TweenEffectEntry
    {
        [SerializeField] private bool enabled = true;
        [SerializeField] private ETweenRun runType = ETweenRun.Auto;
        [SerializeField, Min(0f)] private float delayIn;
        [SerializeField, Min(0f)] private float delayOut;
        [SerializeField] private TweenEffectTiming timing = new TweenEffectTiming();

        [NonSerialized] private Component owner;
        [NonSerialized] private TweenSettings defaultSettings;
        [NonSerialized] private TweenPlayback playback;
        private bool isInitialized;

        public bool Enabled => enabled;
        public bool IsAutoRun => enabled && runType == ETweenRun.Auto;

        internal UniTask Init(Component owner, TweenSettings defaultSettings)
        {
            if (isInitialized)
            {
                return UniTask.CompletedTask;
            }

            this.owner = owner;
            this.defaultSettings = defaultSettings;
            isInitialized = true;
            Setup(owner);
            ApplyInactive();
            return UniTask.CompletedTask;
        }

        internal UniTask Show()
        {
            if (!enabled)
            {
                return UniTask.CompletedTask;
            }

            TweenTimingData resolvedTiming = ResolveTiming();
            return Playback.Play(CreateShowTween(resolvedTiming), ApplyActive, owner);
        }

        internal UniTask Hide()
        {
            if (!enabled)
            {
                return UniTask.CompletedTask;
            }

            TweenTimingData resolvedTiming = ResolveTiming();
            return Playback.Play(CreateHideTween(resolvedTiming), ApplyInactive, owner);
        }

        internal void Kill()
        {
            Playback.Kill();
        }

        protected virtual void Setup(Component owner)
        {
        }

        protected abstract Tween CreateShowTween(TweenTimingData timing);

        protected abstract Tween CreateHideTween(TweenTimingData timing);

        protected abstract void ApplyActive();

        protected abstract void ApplyInactive();

        protected T ResolveTarget<T>(T serializedTarget)
            where T : Component
        {
            if (serializedTarget != null)
            {
                return serializedTarget;
            }

            if (owner == null)
            {
                return null;
            }

            return owner.GetComponent<T>();
        }

        protected void LogMissingTarget(string targetName)
        {
            if (owner != null)
            {
                Debug.LogWarning(
                    $"{GetType().Name} skipped because {targetName} is missing.",
                    owner);
            }
        }

        private TweenTimingData ResolveTiming()
        {
            if (timing == null)
            {
                timing = new TweenEffectTiming();
            }

            return timing.Resolve(defaultSettings, delayIn, delayOut);
        }

        private TweenPlayback Playback
        {
            get
            {
                if (playback == null)
                {
                    playback = new TweenPlayback();
                }

                return playback;
            }
        }
    }
}
