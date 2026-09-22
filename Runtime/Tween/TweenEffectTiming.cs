using System;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    [Serializable]
    public sealed class TweenEffectTiming
    {
        [SerializeField] private TweenSettings preset;
        [SerializeField] private bool overrideEaseIn;
        [SerializeField] private Ease easeIn = Ease.OutBack;
        [SerializeField] private bool overrideEaseOut;
        [SerializeField] private Ease easeOut = Ease.InBack;
        [SerializeField] private bool overrideDurationIn;
        [SerializeField, Min(0f)] private float durationIn = 0.25f;
        [SerializeField] private bool overrideDurationOut;
        [SerializeField, Min(0f)] private float durationOut = 0.2f;

        internal TweenTimingData Resolve(
            TweenSettings defaultSettings,
            float delayIn,
            float delayOut)
        {
            TweenSettings effectiveSettings = preset ? preset : defaultSettings;
            return TweenSettingsResolver.Resolve(
                effectiveSettings,
                overrideEaseIn,
                easeIn,
                overrideEaseOut,
                easeOut,
                overrideDurationIn,
                durationIn,
                overrideDurationOut,
                durationOut,
                delayIn,
                delayOut);
        }
    }
}
