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
        private PanelState state = PanelState.Hidden;

        public abstract bool CanBack { get; }
        public virtual UILayer Layer => UILayer.Screen;
        public virtual bool CanCache => false;
        public virtual bool ShowOnStart => false;

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
            if (state == PanelState.Showing || state == PanelState.Shown)
            {
                return;
            }

            state = PanelState.Showing;
            ResetToken();
            OnPreShow?.Invoke();
            gameObject.SetActive(true);
            PanelManager.Instance.MarkShown(this);
            await ShowTween();
            state = PanelState.Shown;
            OnPostShow?.Invoke();
        }

        public async UniTask Hide()
        {
            if (state == PanelState.Hiding || state == PanelState.Hidden)
            {
                return;
            }

            state = PanelState.Hiding;
            ResetToken();
            OnPreHide?.Invoke();
            await HideTween();
            PanelManager.Instance.MarkHidden(this);
            if (CanCache)
            {
                gameObject.SetActive(false);
                state = PanelState.Hidden;
            }
            else
            {
                Destroy(gameObject);
            }

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
                canvasGroup.blocksRaycasts = interactable;
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

        private enum PanelState
        {
            Hidden,
            Showing,
            Shown,
            Hiding
        }
    }
}
