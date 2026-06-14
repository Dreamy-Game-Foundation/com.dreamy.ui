using UnityEngine;

namespace Dreamy.UI
{
    [DisallowMultipleComponent]
    public sealed class TweenDelayControl : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float showInterval = 0.05f;
        [SerializeField, Min(0f)] private float hideInterval = 0.03f;
        [SerializeField] private bool reverseHideOrder = true;
        [SerializeField] private bool includeInactive = true;

        private void Awake()
        {
            ApplyDelays();
        }

        private void OnValidate()
        {
            showInterval = Mathf.Max(0f, showInterval);
            hideInterval = Mathf.Max(0f, hideInterval);
        }

        [ContextMenu("Apply Tween Delays")]
        public void ApplyDelays()
        {
            TweenDelayByIndex[] entries =
                GetComponentsInChildren<TweenDelayByIndex>(includeInactive);

            for (int index = 0; index < entries.Length; index++)
            {
                int hideIndex = reverseHideOrder
                    ? entries.Length - index - 1
                    : index;
                entries[index].Apply(
                    index,
                    showInterval,
                    hideInterval,
                    hideIndex);
            }
        }

        [ContextMenu("Clear Tween Delays")]
        public void ClearDelays()
        {
            TweenDelayByIndex[] entries =
                GetComponentsInChildren<TweenDelayByIndex>(includeInactive);
            foreach (TweenDelayByIndex entry in entries)
            {
                entry.Clear();
            }
        }
    }
}
