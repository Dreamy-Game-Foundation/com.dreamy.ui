using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public class UITweenMove : UITweenBase
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Vector2 offset;

        private Vector2 activePosition;
        private Vector2 inactivePosition;

        protected override void Reset()
        {
            base.Reset();
            rectTransform = transform as RectTransform;
        }

        protected override UniTask Setup()
        {
            if (rectTransform == null)
            {
                rectTransform = transform as RectTransform;
            }

            activePosition = rectTransform.anchoredPosition;
            inactivePosition = activePosition + offset;
            return UniTask.CompletedTask;
        }

        public override UniTask Show()
        {
            Tween tween = rectTransform.DOAnchorPos(activePosition, durationIn)
                .SetEase(easeIn)
                .SetDelay(delayIn);

            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            Tween tween = rectTransform.DOAnchorPos(inactivePosition, durationOut)
                .SetEase(easeOut)
                .SetDelay(delayOut);

            return Play(tween, Inactive);
        }

        protected override void Active()
        {
            rectTransform.anchoredPosition = activePosition;
        }

        protected override void Inactive()
        {
            rectTransform.anchoredPosition = inactivePosition;
        }
    }
}
