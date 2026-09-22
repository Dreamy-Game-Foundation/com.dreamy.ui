using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public abstract class UITweenBase : MonoBehaviour, ITween
    {
        [SerializeField] protected ETweenRun runType = ETweenRun.Auto;
        [SerializeField] protected TweenSettings settings;
        [SerializeField] private bool overrideEase;
        [SerializeField] private Ease easeIn = Ease.OutBack;
        [SerializeField] private Ease easeOut = Ease.InBack;
        [SerializeField] private bool overrideDuration;
        [SerializeField, Min(0f)] private float durationIn = 0.25f;
        [SerializeField, Min(0f)] private float durationOut = 0.2f;
        [SerializeField, Min(0f)] private float delayIn;
        [SerializeField, Min(0f)] private float delayOut;

        private bool isInitialized;
        private TweenPlayback playback;
        private bool hasDelayOverride;
        private float delayInOverride;
        private float delayOutOverride;

        public bool IsAutoRun => runType == ETweenRun.Auto;
        public Ease EaseIn => ResolveTiming().EaseIn;
        public Ease EaseOut => ResolveTiming().EaseOut;
        public float DurationIn => ResolveTiming().DurationIn;
        public float DurationOut => ResolveTiming().DurationOut;
        public float DelayIn => hasDelayOverride
            ? delayInOverride
            : delayIn;
        public float DelayOut => hasDelayOverride
            ? delayOutOverride
            : delayOut;
        protected abstract string DefaultSettingsPath { get; }

        protected virtual void Reset()
        {
            settings = Resources.Load<TweenSettings>(DefaultSettingsPath);
        }

        public UniTask Init()
        {
            if (isInitialized)
            {
                return UniTask.CompletedTask;
            }

            isInitialized = true;
            LoadDefaultSettings();
            Setup();
            Inactive();
            return UniTask.CompletedTask;
        }

        public abstract UniTask Show();

        public abstract UniTask Hide();

        public void Kill()
        {
            Playback.Kill();
        }

        public void SetDelayOverride(float showDelay, float hideDelay)
        {
            hasDelayOverride = true;
            delayInOverride = Mathf.Max(0f, showDelay);
            delayOutOverride = Mathf.Max(0f, hideDelay);
        }

        public void ClearDelayOverride()
        {
            hasDelayOverride = false;
        }

        protected virtual void Setup()
        {
        }

        private void LoadDefaultSettings()
        {
            if (!settings)
            {
                settings = Resources.Load<TweenSettings>(DefaultSettingsPath);
            }
        }

        protected UniTask Play(Tween tween, System.Action onComplete)
        {
            if (tween == null)
            {
                return UniTask.CompletedTask;
            }

            return Playback.Play(tween, onComplete, this);
        }

        protected abstract void Active();

        protected abstract void Inactive();

        private TweenTimingData ResolveTiming()
        {
            return TweenSettingsResolver.Resolve(
                settings,
                overrideEase,
                easeIn,
                overrideEase,
                easeOut,
                overrideDuration,
                durationIn,
                overrideDuration,
                durationOut,
                DelayIn,
                DelayOut);
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
