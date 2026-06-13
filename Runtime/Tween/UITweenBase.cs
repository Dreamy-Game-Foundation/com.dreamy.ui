using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public abstract class UITweenBase : MonoBehaviour, ITween
    {
        [SerializeField] protected ETweenRun runType = ETweenRun.Auto;
        [SerializeField] protected TweenSettings settings;
        [SerializeField, Min(0f)] private float delayIn;
        [SerializeField, Min(0f)] private float delayOut;

        private bool isInitialized;
        private Tween currentTween;

        public bool IsAutoRun => runType == ETweenRun.Auto;
        protected Ease EaseIn => settings ? settings.EaseIn : Ease.OutBack;
        protected Ease EaseOut => settings ? settings.EaseOut : Ease.InBack;
        protected float DurationIn => settings ? settings.DurationIn : 0.25f;
        protected float DurationOut => settings ? settings.DurationOut : 0.2f;
        protected float DelayIn => delayIn;
        protected float DelayOut => delayOut;
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
