using UnityEngine;

namespace Dreamy.UI
{
    [DisallowMultipleComponent]
    public sealed class TweenDelayByIndex : MonoBehaviour
    {
        [SerializeField, Min(0)] private int index;

        private bool hasDelayOverride;
        private float showDelay;
        private float hideDelay;

        public int Index => index;

        public void Apply(
            int value,
            float showInterval,
            float hideInterval,
            int hideIndex)
        {
            index = Mathf.Max(0, value);
            hasDelayOverride = true;
            showDelay = index * Mathf.Max(0f, showInterval);
            hideDelay = Mathf.Max(0, hideIndex) *
                              Mathf.Max(0f, hideInterval);
            foreach (UITweenBase tween in GetComponents<UITweenBase>())
            {
                if (tween != null)
                {
                    tween.SetDelayOverride(showDelay, hideDelay);
                }
            }
        }

        public void Clear()
        {
            hasDelayOverride = false;
            foreach (UITweenBase tween in GetComponents<UITweenBase>())
            {
                if (tween != null)
                {
                    tween.ClearDelayOverride();
                }
            }
        }

        internal TweenTimingData ApplyTo(TweenTimingData timing)
        {
            return hasDelayOverride ? timing.WithDelays(showDelay, hideDelay) : timing;
        }
    }
}
