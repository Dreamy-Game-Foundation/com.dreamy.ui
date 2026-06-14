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
        protected Ease EaseIn => overrideEase
            ? easeIn
            : settings ? settings.EaseIn : Ease.OutBack;
        protected Ease EaseOut => overrideEase
            ? easeOut
            : settings ? settings.EaseOut : Ease.InBack;
        protected float DurationIn => overrideDuration
            ? durationIn
            : settings ? settings.DurationIn : 0.25f;
        protected float DurationOut => overrideDuration
            ? durationOut
            : settings ? settings.DurationOut : 0.2f;
        protected float DelayIn => hasDelayOverride
            ? delayInOverride
            : delayIn;
        protected float DelayOut => hasDelayOverride
            ? delayOutOverride
            : delayOut;
        protected abstract string DefaultSettingsPath { get; }

        protected virtual void Reset()
        {
            settings = Resources.Load<TweenSettings>(DefaultSettingsPath);
        }

        public async UniTask Init()
        {
            if (isInitialized)
            {
                return;
            }

            isInitialized = true;
            LoadDefaultSettings();
            await Setup();
            Inactive();
        }

        public abstract UniTask Show();

        public abstract UniTask Hide();

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

        protected virtual UniTask Setup()
        {
            return UniTask.CompletedTask;
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
            currentTween?.Kill();
            currentTween = tween;

            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
            tween.OnComplete(() =>
            {
                onComplete?.Invoke();
                currentTween = null;
                completionSource.TrySetResult();
            });
            tween.OnKill(() =>
            {
                if (currentTween == tween)
                {
                    currentTween = null;
                }

                completionSource.TrySetResult();
            });
            return completionSource.Task;
        }

        protected abstract void Active();

        protected abstract void Inactive();
    }
}
