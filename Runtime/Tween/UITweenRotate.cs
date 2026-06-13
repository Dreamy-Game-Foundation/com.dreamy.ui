using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    public sealed class UITweenRotate : UITweenBase
    {
        private const string SettingsPath = "Tween/RotateTweenSettings";

        [SerializeField] private Vector3 inactiveRotation = new(0f, 0f, -12f);

        private Vector3 activeRotation;

        protected override string DefaultSettingsPath => SettingsPath;

        protected override UniTask Setup()
        {
            activeRotation = transform.localEulerAngles;
            return UniTask.CompletedTask;
        }

        public override UniTask Show()
        {
            Tween tween = transform.DOLocalRotate(activeRotation, DurationIn)
                .SetEase(EaseIn)
                .SetDelay(DelayIn);
            return Play(tween, Active);
        }

        public override UniTask Hide()
        {
            Tween tween = transform.DOLocalRotate(inactiveRotation, DurationOut)
                .SetEase(EaseOut)
                .SetDelay(DelayOut);
            return Play(tween, Inactive);
        }

        protected override void Active()
        {
            transform.localEulerAngles = activeRotation;
        }

        protected override void Inactive()
        {
            transform.localEulerAngles = inactiveRotation;
        }
    }
}
