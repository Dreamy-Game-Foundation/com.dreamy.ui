using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public class UITweenScale : UITweenBase
    {
        [SerializeField] private float inactiveScale;
        [SerializeField] private float activeScale = 1f;

        public override UniTask Show()
        {
            Tween tween = transform.DOScale(activeScale, durationIn)
                .SetEase(easeIn)
                .SetDelay(delayIn);

            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            Tween tween = transform.DOScale(inactiveScale, durationOut)
                .SetEase(easeOut)
                .SetDelay(delayOut);

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
