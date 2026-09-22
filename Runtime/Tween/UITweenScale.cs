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
            Tween tween = TweenEffectFactory.Scale(
                transform,
                Vector3.one * activeScale,
                DurationIn,
                EaseIn,
                DelayIn);

            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            Tween tween = TweenEffectFactory.Scale(
                transform,
                Vector3.one * inactiveScale,
                DurationOut,
                EaseOut,
                DelayOut);

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
