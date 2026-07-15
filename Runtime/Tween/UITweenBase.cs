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
        private Tween currentTween;
        private bool hasDelayOverride;
        private float delayInOverride;
        private float delayOutOverride;

        public bool IsAutoRun => runType == ETweenRun.Auto;
        public Ease EaseIn => overrideEase
            ? easeIn
            : settings ? settings.EaseIn : Ease.OutBack;
        public Ease EaseOut => overrideEase
            ? easeOut
            : settings ? settings.EaseOut : Ease.InBack;
        public float DurationIn => overrideDuration
            ? durationIn
            : settings ? settings.DurationIn : 0.25f;
        public float DurationOut => overrideDuration
            ? durationOut
            : settings ? settings.DurationOut : 0.2f;
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
            currentTween?.Kill();
            currentTween = null;
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

            currentTween?.Kill();
            currentTween = tween;

            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
            tween.OnComplete(() =>
            {
                if (this != null)
                {
                    onComplete?.Invoke();
                    currentTween = null;
                }
                completionSource.TrySetResult();
            });
            tween.OnKill(() =>
            {
                if (this != null)
                {
                    if (currentTween == tween)
                    {
                        currentTween = null;
                    }
                }
                completionSource.TrySetResult();
            });
            return completionSource.Task;
        }

        protected abstract void Active();

        protected abstract void Inactive();
    }
}
