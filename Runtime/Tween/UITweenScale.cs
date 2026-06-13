using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public class UITweenScale : UITweenBase
    {
        private const string SettingsPath = "Tween/ScaleTweenSettings";

        [SerializeField] private float inactiveScale;
        [SerializeField] private float activeScale = 1f;

        protected override string DefaultSettingsPath => SettingsPath;

        public override UniTask Show()
        {
            Tween tween = transform.DOScale(activeScale, DurationIn)
                .SetEase(EaseIn)
                .SetDelay(DelayIn);

            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            Tween tween = transform.DOScale(inactiveScale, DurationOut)
                .SetEase(EaseOut)
                .SetDelay(DelayOut);

            return Play(tween, Inactive);
        }

        protected override void Active()
        {
            transform.localScale = Vector3.one * activeScale;
        }

        protected override void Inactive()
        {
            transform.localScale = Vector3.one * inactiveScale;
        }
    }
}
