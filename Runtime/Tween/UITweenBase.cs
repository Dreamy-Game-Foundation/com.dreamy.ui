using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public abstract class UITweenBase : MonoBehaviour, ITween
    {
        [SerializeField] protected ETweenRun runType = ETweenRun.Auto;
        [SerializeField] protected Ease easeIn = Ease.OutBack;
        [SerializeField] protected Ease easeOut = Ease.InBack;
        [SerializeField] protected float durationIn = 0.25f;
        [SerializeField] protected float durationOut = 0.2f;
        [SerializeField] protected float delayIn;
        [SerializeField] protected float delayOut;

        private bool isInitialized;

        public bool IsAutoRun => runType == ETweenRun.Auto;

        protected virtual void Reset()
        {
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
