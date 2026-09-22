using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public class UITweenMove : UITweenBase
    {
        private const string SettingsPath = "Tween/MoveTweenSettings";

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Vector2 offset;

        private Vector2 activePosition;
        private Vector2 inactivePosition;

        protected override string DefaultSettingsPath => SettingsPath;

        protected override void Reset()
        {
            base.Reset();
            rectTransform = transform as RectTransform;
        }

        protected override void Setup()
        {
            if (rectTransform == null)
            {
                rectTransform = transform as RectTransform;
            }

            activePosition = rectTransform.anchoredPosition;
            inactivePosition = activePosition + offset;
        }

        public override UniTask Show()
        {
            Tween tween = TweenEffectFactory.Move(
                rectTransform,
                activePosition,
                DurationIn,
                EaseIn,
                DelayIn);

            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            Tween tween = TweenEffectFactory.Move(
                rectTransform,
                inactivePosition,
                DurationOut,
                EaseOut,
                DelayOut);

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
