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
        [SerializeField] protected UITweenPlayer tweenPlayer;
        [SerializeField] private MonoBehaviour transitionBehaviour;

        private CancellationTokenSource tokenSource;
        private PanelState state = PanelState.Hidden;
        private int operationVersion;
        private IPanelTransition panelTransition;

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
            tweenPlayer = GetComponent<UITweenPlayer>();
            transitionBehaviour = GetComponent<IPanelTransition>() as MonoBehaviour;
        }

        public virtual UniTask Init()
        {
            PanelManager.Instance.Register(this);
            return UniTask.CompletedTask;
        }

        public virtual async UniTask PostInit()
        {
            gameObject.SetActive(false);

            IPanelTransition transition = ResolvePanelTransition();
            if (transition != null)
            {
                await transition.Init();
            }
        }

        public async UniTask Show()
        {
            if (state == PanelState.Showing || state == PanelState.Shown)
            {
                return;
            }

            state = PanelState.Showing;
            int version = ResetToken();
            OnPreShow?.Invoke();
            gameObject.SetActive(true);
            PanelManager.Instance.MarkShown(this);
            try
            {
                await ShowTween();
                if (version != operationVersion)
                {
                    return;
                }

                state = PanelState.Shown;
                OnPostShow?.Invoke();
            }
            catch (OperationCanceledException)
            {
                if (version == operationVersion)
                {
                    state = PanelState.Hidden;
                    if (PanelManager.HasInstance)
                    {
                        PanelManager.Instance.MarkHidden(this);
                    }
                }
                // A newer panel operation owns the final state otherwise.
            }
            catch
            {
                if (version == operationVersion)
                {
                    state = PanelState.Hidden;
                    PanelManager.Instance.MarkHidden(this);
                    gameObject.SetActive(false);
                }

                throw;
            }
        }

        public async UniTask Hide()
        {
            if (state == PanelState.Hiding || state == PanelState.Hidden)
            {
                return;
            }

            state = PanelState.Hiding;
            int version = ResetToken();
            OnPreHide?.Invoke();
            try
            {
                UniTask restorePreviousTask = PanelManager.HasInstance
                    ? PanelManager.Instance.RestorePreviousPanelVisual(this)
                    : UniTask.CompletedTask;
                await UniTask.WhenAll(HideTween(), restorePreviousTask);
                if (version != operationVersion)
                {
                    return;
                }

                PanelManager.Instance.MarkHidden(this);
                PanelManager.Instance.CompletePanelHide(this);
                state = PanelState.Hidden;

                OnPostHide?.Invoke();

                if (CanCache)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            catch (OperationCanceledException)
            {
                if (PanelManager.HasInstance)
                {
                    if (version == operationVersion)
                    {
                        PanelManager.Instance.MarkHidden(this);
                        PanelManager.Instance.CompletePanelHide(this);
                        state = PanelState.Hidden;
                    }
                    else
                    {
                        PanelManager.Instance.CancelPanelHide(this);
                    }
                }
            }
            catch
            {
                if (version == operationVersion)
                {
                    state = PanelState.Shown;
                    SetInteractable(true);
                }

                if (PanelManager.HasInstance)
                {
                    PanelManager.Instance.CancelPanelHide(this);
                }

                throw;
            }
        }

        public UniTask ShowTween()
        {
            IPanelTransition transition = ResolvePanelTransition();
            return transition != null
                ? transition.ShowTween(tokenSource?.Token ?? CancellationToken.None)
                : UniTask.CompletedTask;
        }

        public UniTask HideTween()
        {
            IPanelTransition transition = ResolvePanelTransition();
            return transition != null
                ? transition.HideTween(tokenSource?.Token ?? CancellationToken.None)
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
            ResolvePanelTransition()?.Kill();

            if (PanelManager.HasInstance)
            {
                PanelManager.Instance.Unregister(this);
            }
        }

        protected virtual void OnDisable()
        {
            if (state == PanelState.Showing || state == PanelState.Hiding)
            {
                tokenSource?.Cancel();
            }
        }

        private int ResetToken()
        {
            tokenSource?.Cancel();
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            operationVersion++;
            return operationVersion;
        }

        private IPanelTransition ResolvePanelTransition()
        {
            if (panelTransition != null)
            {
                return panelTransition;
            }

            if (transitionBehaviour is IPanelTransition assignedTransition)
            {
                panelTransition = assignedTransition;
                return panelTransition;
            }

            if (tweenPlayer == null)
            {
                tweenPlayer = GetComponent<UITweenPlayer>();
            }

            panelTransition = tweenPlayer;
            if (panelTransition != null)
            {
                return panelTransition;
            }

            panelTransition = GetComponent<IPanelTransition>();
            return panelTransition;
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
