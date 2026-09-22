using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dreamy.UI
{
    public sealed class UITweenColor : UITweenBase
    {
        [SerializeField] private Graphic graphic;
        [SerializeField] private Color inactiveColor = Color.clear;

        private Color activeColor;
        internal override TweenEffectType EffectType => TweenEffectType.Color;

        protected override void Reset()
        {
            graphic = GetComponent<Graphic>();
        }

        protected override void Setup()
        {
            Graphic target = ResolveTarget(graphic);
            if (target == null)
            {
                throw new MissingComponentException(
                    $"{nameof(UITweenColor)} requires a Graphic component.");
            }

            activeColor = target.color;
        }

        protected override UniTask CreateShowTween()
        {
            Graphic target = ResolveTarget(graphic);
            if (target == null)
            {
                return UniTask.CompletedTask;
            }
            Tween tween = target.DOColor(activeColor, DurationIn).SetEase(EaseIn).SetDelay(DelayIn);
            return Play(tween, Active);
        }

        protected override UniTask CreateHideTween()
        {
            Graphic target = ResolveTarget(graphic);
            if (target == null)
            {
                return UniTask.CompletedTask;
            }
            Tween tween = target.DOColor(inactiveColor, DurationOut).SetEase(EaseOut).SetDelay(DelayOut);
            return Play(tween, Inactive);
        }

        protected override void Active()
        {
            Graphic target = ResolveTarget(graphic);
            if (target != null)
            {
                target.color = activeColor;
            }
        }

        protected override void Inactive()
        {
            Graphic target = ResolveTarget(graphic);
            if (target != null)
            {
                target.color = inactiveColor;
            }
        }
    }
}
