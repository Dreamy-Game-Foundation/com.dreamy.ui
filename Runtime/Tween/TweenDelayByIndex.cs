using Unity.Collections;
using UnityEngine;

namespace Dreamy.UI
{
    [DisallowMultipleComponent]
    public sealed class TweenDelayByIndex : MonoBehaviour
    
    {
        [SerializeField, ReadOnly] private int index;

        private UITweenBase[] tweens;

        public int Index => index;

        public void Apply(
            int value,
            float showInterval,
            float hideInterval,
            int hideIndex)
        {
            index = Mathf.Max(0, value);
            EnsureTweens();

            float showDelay = index * Mathf.Max(0f, showInterval);
            float hideDelay = Mathf.Max(0, hideIndex) *
                              Mathf.Max(0f, hideInterval);
            foreach (UITweenBase tween in tweens)
            {
                if (tween != null)
                {
                    tween.SetDelayOverride(showDelay, hideDelay);
                }
            }
        }

        public void Clear()
        {
            EnsureTweens();
            foreach (UITweenBase tween in tweens)
            {
                if (tween != null)
                {
                    tween.ClearDelayOverride();
                }
            }
        }

        private void EnsureTweens()
        {
            if (tweens == null || tweens.Length == 0 || System.Array.Exists(tweens, tween => tween == null))
            {
                tweens = GetComponents<UITweenBase>();
            }
        }
    }
}
