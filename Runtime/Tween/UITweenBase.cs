using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public abstract class UITweenBase : MonoBehaviour, ITween
    {
        private const string DefaultSettingsPath = "Tween/TweenBaseSettings";

        [SerializeField] protected ETweenRun runType = ETweenRun.Auto;
        [SerializeField] protected TweenSettings settings;

        private bool isInitialized;

        public bool IsAutoRun => runType == ETweenRun.Auto;
        protected Ease EaseIn => settings ? settings.EaseIn : Ease.OutBack;
        protected Ease EaseOut => settings ? settings.EaseOut : Ease.InBack;
        protected float DurationIn => settings ? settings.DurationIn : 0.25f;
        protected float DurationOut => settings ? settings.DurationOut : 0.2f;
        protected float DelayIn => settings ? settings.DelayIn : 0f;
        protected float DelayOut => settings ? settings.DelayOut : 0f;

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
            await Setup();
            Inactive();
        }

        public abstract UniTask Show();

        public abstract UniTask Hide();

        protected virtual UniTask Setup()
        {
            if (!settings)
            {
                settings = Resources.Load<TweenSettings>(DefaultSettingsPath);
            }

            return UniTask.CompletedTask;
        }

        protected static UniTask Play(Tween tween, System.Action onComplete)
        {
            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
            tween.OnComplete(() =>
            {
                onComplete?.Invoke();
                completionSource.TrySetResult();
            });
            tween.OnKill(() =>
            {
                completionSource.TrySetResult();
            });
            return completionSource.Task;
        }

        protected abstract void Active();

        protected abstract void Inactive();
    }
}
