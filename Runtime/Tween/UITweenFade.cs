using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public class UITweenFade : UITweenBase
    {
        [SerializeField] private CanvasGroup canvasGroup;

        protected override void Reset()
        {
            base.Reset();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override UniTask Setup()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
            }

            return UniTask.CompletedTask;
        }

        public override UniTask Show()
        {
            Tween tween = canvasGroup.DOFade(1f, durationIn)
                .SetEase(easeIn)
                .SetDelay(delayIn);

            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            canvasGroup.interactable = false;
            Tween tween = canvasGroup.DOFade(0f, durationOut)
                .SetEase(easeOut)
                .SetDelay(delayOut);

            return Play(tween, Inactive);
        }

        protected override void Active()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
        }

        protected override void Inactive()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
        }
    }
}
