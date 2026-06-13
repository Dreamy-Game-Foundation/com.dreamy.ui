using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [CreateAssetMenu(
        fileName = "TweenBaseSettings",
        menuName = "Dreamy/UI/Tween Settings")]
    public sealed class TweenSettings : ScriptableObject
    {
        public Ease EaseIn = Ease.OutBack;
        public Ease EaseOut = Ease.InBack;
        public float DurationIn = 0.25f;
        public float DurationOut = 0.2f;
        public float DelayIn;
        public float DelayOut;
    }
}
