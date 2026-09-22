using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dreamy.UI
{
    [DisallowMultipleComponent]
    public sealed class TweenEffectPlayer : MonoBehaviour, IPanelTransition
    {
        [SerializeField] private TweenSettings defaultSettings;
        [SerializeReference] private List<TweenEffectEntry> entries =
            new List<TweenEffectEntry>();

        public UniTask Init()
        {
            return UniTask.WhenAll(
                entries
                    .Where(entry => entry != null && entry.Enabled)
                    .Select(entry => entry.Init(this, defaultSettings)));
        }

        public UniTask ShowTween(CancellationToken token)
        {
            return PlayTweens(
                entries
                    .Where(entry => entry != null && entry.IsAutoRun)
                    .Select(entry => entry.Show()),
                token);
        }

        public UniTask HideTween(CancellationToken token)
        {
            return PlayTweens(
                entries
                    .Where(entry => entry != null && entry.IsAutoRun)
                    .Select(entry => entry.Hide()),
                token);
        }

        public void Kill()
        {
            foreach (TweenEffectEntry entry in entries)
            {
                entry?.Kill();
            }
        }

        private async UniTask PlayTweens(
            IEnumerable<UniTask> tweenTasks,
            CancellationToken token)
        {
            try
            {
                await UniTask.WhenAll(tweenTasks).AttachExternalCancellation(token);
            }
            catch (OperationCanceledException)
            {
                Kill();
                throw;
            }
        }
    }
}
