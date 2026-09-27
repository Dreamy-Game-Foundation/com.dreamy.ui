using System.Collections.Generic;
using UnityEngine;

namespace Dreamy.UI
{
    [DisallowMultipleComponent]
    public sealed class UIShineWaveController : MonoBehaviour
    {
        [SerializeField] private bool findWavesInChildren = true;
        [SerializeField] private List<UIShineWave> shineWaves = new List<UIShineWave>();
        [SerializeField] private bool playOnEnable = true;
        [SerializeField] private bool startImmediately = true;
        [SerializeField] private Vector2 intervalRange = new Vector2(0.5f, 1.5f);
        [SerializeField] private Vector2 durationRange = new Vector2(0.35f, 0.8f);
        [SerializeField] private Vector2 rotationRange = new Vector2(25f, 65f);

        private readonly List<UIShineWave> availableWaves = new List<UIShineWave>();
        private float nextWaveTime;
        private bool isPlaying;

        private void Reset()
        {
            DiscoverWaves();
        }

        private void OnEnable()
        {
            if (findWavesInChildren)
            {
                DiscoverWaves();
            }

            foreach (UIShineWave shineWave in shineWaves)
            {
                if (shineWave == null) continue;
                shineWave.Stop();
                shineWave.SetPhase(1f);
            }

            isPlaying = playOnEnable;
            nextWaveTime = startImmediately ? 0f : RandomRange(intervalRange);
        }

        private void OnDisable()
        {
            Stop();
        }

        private void OnValidate()
        {
            intervalRange = NormalizeRange(intervalRange, 0f);
            durationRange = NormalizeRange(durationRange, 0.01f);
            rotationRange = NormalizeRange(rotationRange, -180f);
        }

        private void Update()
        {
            if (!isPlaying || shineWaves.Count == 0) return;

            nextWaveTime -= Time.unscaledDeltaTime;
            if (nextWaveTime > 0f) return;

            UIShineWave wave = GetRandomAvailableWave();
            if (wave == null)
            {
                nextWaveTime = 0.05f;
                return;
            }

            wave.PlayWave(RandomRange(durationRange), RandomRange(rotationRange));
            nextWaveTime = RandomRange(intervalRange);
        }

        public void Play()
        {
            isPlaying = true;
            nextWaveTime = 0f;
        }

        public void Stop()
        {
            isPlaying = false;
            foreach (UIShineWave shineWave in shineWaves)
            {
                shineWave?.Stop();
            }
        }

        [ContextMenu("Find Shine Waves")]
        public void DiscoverWaves()
        {
            shineWaves.Clear();
            shineWaves.AddRange(GetComponentsInChildren<UIShineWave>(true));
        }

        private UIShineWave GetRandomAvailableWave()
        {
            availableWaves.Clear();
            foreach (UIShineWave shineWave in shineWaves)
            {
                if (shineWave != null && !shineWave.IsPlaying)
                {
                    availableWaves.Add(shineWave);
                }
            }

            return availableWaves.Count == 0
                ? null
                : availableWaves[Random.Range(0, availableWaves.Count)];
        }

        private static float RandomRange(Vector2 range)
        {
            return Random.Range(range.x, range.y);
        }

        private static Vector2 NormalizeRange(Vector2 range, float minimum)
        {
            float min = Mathf.Max(minimum, range.x);
            return new Vector2(min, Mathf.Max(min, range.y));
        }
    }
}
