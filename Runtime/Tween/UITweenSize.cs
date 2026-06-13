using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public sealed class UITweenSize : UITweenBase
    {
        private const string SettingsPath = "Tween/SizeTweenSettings";

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Vector2 inactiveSize;

        private Vector2 activeSize;

        protected override string DefaultSettingsPath => SettingsPath;

        protected override void Reset()
        {
            base.Reset();
            rectTransform = transform as RectTransform;
        }

        protected override UniTask Setup()
        {
            if (!rectTransform)
            {
                rectTransform = transform as RectTransform;
            }

            if (!rectTransform)
            {
                throw new MissingComponentException(
                    $"{nameof(UITweenSize)} requires a RectTransform.");
            }

            activeSize = rectTransform.sizeDelta;
            return UniTask.CompletedTask;
        }

        public override UniTask Show()
        {
            Tween tween = rectTransform.DOSizeDelta(activeSize, DurationIn)
                .SetEase(EaseIn)
                .SetDelay(DelayIn);
            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            Tween tween = rectTransform.DOSizeDelta(inactiveSize, DurationOut)
                .SetEase(EaseOut)
                .SetDelay(DelayOut);
            return Play(tween, Inactive);
        }

        protected override void Active()
        {
            rectTransform.sizeDelta = activeSize;
        }

        protected override void Inactive()
        {
            rectTransform.sizeDelta = inactiveSize;
        }
    }
}
