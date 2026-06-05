using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dreamy.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIPanel : MonoBehaviour, IPanel
    {
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected TweenPlayer tweenPlayer;

        private CancellationTokenSource tokenSource;

        public abstract bool CanBack { get; }

        public event Action OnPreShow;
        public event Action OnPostShow;
        public event Action OnPreHide;
        public event Action OnPostHide;

        protected virtual void Reset()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            tweenPlayer = GetComponent<TweenPlayer>();
        }

        public virtual UniTask Init()
        {
            PanelManager.Instance.Register(this);
            return UniTask.CompletedTask;
        }

        public virtual async UniTask PostInit()
        {
            gameObject.SetActive(false);

            if (tweenPlayer != null)
            {
                await tweenPlayer.Init();
            }
        }

        public async UniTask Show()
        {
            ResetToken();
            OnPreShow?.Invoke();
            gameObject.SetActive(true);
            await ShowTween();
            OnPostShow?.Invoke();
        }

        public async UniTask Hide()
        {
            ResetToken();
            OnPreHide?.Invoke();
            await HideTween();
            Destroy(gameObject);
            OnPostHide?.Invoke();
        }

        public UniTask ShowTween()
        {
            return tweenPlayer != null
                ? tweenPlayer.ShowTween(tokenSource.Token)
                : UniTask.CompletedTask;
        }

        public UniTask HideTween()
        {
            return tweenPlayer != null
                ? tweenPlayer.HideTween(tokenSource.Token)
                : UniTask.CompletedTask;
        }

        public void SetInteractable(bool interactable)
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = interactable;
            }
        }

        protected virtual void OnDestroy()
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();

            if (PanelManager.HasInstance)
            {
                PanelManager.Instance.Unregister(this);
            }
        }

        private void ResetToken()
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
        }
    }
}
