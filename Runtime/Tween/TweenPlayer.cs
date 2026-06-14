using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dreamy.UI
{
    public class TweenPlayer : MonoBehaviour
    {
        private List<ITween> uiTweens = new List<ITween>();

        public UniTask Init()
        {
            uiTweens = GetComponentsInChildren<ITween>(true).ToList();
            return UniTask.WhenAll(uiTweens.Select(tween => tween.Init()));
        }

        public UniTask ShowTween(CancellationToken token)
        {
            return PlayTweens(
                uiTweens.Where(tween => tween.IsAutoRun).Select(tween => tween.Show()),
                token);
        }

        public UniTask HideTween(CancellationToken token)
        {
            return PlayTweens(
                uiTweens.Where(tween => tween.IsAutoRun).Select(tween => tween.Hide()),
                token);
        }

        public void Kill()
        {
            foreach (ITween tween in uiTweens)
            {
                tween.Kill();
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
