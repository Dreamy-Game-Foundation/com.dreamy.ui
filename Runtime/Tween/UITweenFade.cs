using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UITweenFade : UITweenBase
    {
        private const string SettingsPath = "Tween/FadeTweenSettings";

        [SerializeField] private CanvasGroup canvasGroup;

        protected override string DefaultSettingsPath => SettingsPath;

        protected override void Reset()
        {
            base.Reset();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override UniTask Setup()
        {
            if (!canvasGroup)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            return UniTask.CompletedTask;
        }

        public override UniTask Show()
        {
            if (this == null || canvasGroup == null)
            {
                return UniTask.CompletedTask;
            }

            Tween tween = canvasGroup.DOFade(1f, DurationIn)
                .SetEase(EaseIn)
                .SetDelay(DelayIn);

            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            if (this == null || canvasGroup == null)
            {
                return UniTask.CompletedTask;
            }

            canvasGroup.interactable = false;
            Tween tween = canvasGroup.DOFade(0f, DurationOut)
                .SetEase(EaseOut)
                .SetDelay(DelayOut);

            return Play(tween, Inactive);
        }

        protected override void Active()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
            }
        }

        protected override void Inactive()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
            }
        }
    }
}
