using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamy.UI
{
    public sealed class UITweenColor : UITweenBase
    {
        private const string SettingsPath = "Tween/ColorTweenSettings";

        [SerializeField] private Graphic graphic;
        [SerializeField] private Color inactiveColor = Color.clear;

        private Color activeColor;

        protected override string DefaultSettingsPath => SettingsPath;

        protected override void Reset()
        {
            base.Reset();
            graphic = GetComponent<Graphic>();
        }

        protected override UniTask Setup()
        {
            if (!graphic)
            {
                graphic = GetComponent<Graphic>();
            }

            if (!graphic)
            {
                throw new MissingComponentException(
                    $"{nameof(UITweenColor)} requires a Graphic component.");
            }

            activeColor = graphic.color;
            return UniTask.CompletedTask;
        }

        public override UniTask Show()
        {
            Tween tween = graphic.DOColor(activeColor, DurationIn)
                .SetEase(EaseIn)
                .SetDelay(DelayIn);
            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            Tween tween = graphic.DOColor(inactiveColor, DurationOut)
                .SetEase(EaseOut)
                .SetDelay(DelayOut);
            return Play(tween, Inactive);
        }

        protected override void Active()
        {
            graphic.color = activeColor;
        }

        protected override void Inactive()
        {
            graphic.color = inactiveColor;
        }
    }
}
