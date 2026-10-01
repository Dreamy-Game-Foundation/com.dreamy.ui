using UnityEngine;

namespace Dreamy.UI
{
    /// <summary>Assigns increasing show delays to child slots in hierarchy order.</summary>
    [DefaultExecutionOrder(-100)]
    [DisallowMultipleComponent]
    public sealed class TweenDelayControl : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float showInterval = 0.05f;
        [SerializeField, Min(0f)] private float startDelay;

        private void Awake()
        {
            ApplyStaggerDelays();
        }

        private void OnValidate()
        {
            showInterval = Mathf.Max(0f, showInterval);
            startDelay = Mathf.Max(0f, startDelay);
        }

        /// <summary>Recalculates delays, including inactive and runtime-spawned slots.</summary>
        [ContextMenu("Apply Tween Delays")]
        public void ApplyStaggerDelays()
        {
            TweenDelayByIndex[] entries = GetComponentsInChildren<TweenDelayByIndex>(true);
            float delay = Mathf.Max(0f, startDelay);
            for (int index = 0; index < entries.Length; index++)
            {
                entries[index].OverrideDelay(true, delay, 0f);
                delay += Mathf.Max(0f, showInterval);
            }
        }

        public void ApplyDelays() => ApplyStaggerDelays();

        [ContextMenu("Clear Tween Delays")]
        public void ClearDelays()
        {
            foreach (TweenDelayByIndex entry in GetComponentsInChildren<TweenDelayByIndex>(true))
                entry.Clear();
        }
    }
}
