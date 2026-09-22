using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Dreamy.UI
{
    internal sealed class TweenPlayback
    {
        private Tween currentTween;

        public UniTask Play(
            Tween tween,
            Action onComplete,
            UnityEngine.Object owner)
        {
            if (tween == null)
            {
                return UniTask.CompletedTask;
            }

            Kill();
            currentTween = tween;

            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
            tween.OnComplete(() =>
            {
                try
                {
                    if (owner != null)
                    {
                        onComplete?.Invoke();
                    }
                }
                finally
                {
                    if (currentTween == tween)
                    {
                        currentTween = null;
                    }

                    completionSource.TrySetResult();
                }
            });
            tween.OnKill(() =>
            {
                if (owner != null && currentTween == tween)
                {
                    currentTween = null;
                }

                completionSource.TrySetResult();
            });
            return completionSource.Task;
        }

        public void Kill()
        {
            currentTween?.Kill();
            currentTween = null;
        }
    }
}
