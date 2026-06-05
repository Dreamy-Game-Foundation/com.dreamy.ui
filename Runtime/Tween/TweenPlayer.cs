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
            return UniTask.WhenAll(uiTweens.Where(tween => tween.IsAutoRun).Select(tween => tween.Show()))
                .AttachExternalCancellation(token);
        }

        public UniTask HideTween(CancellationToken token)
        {
            return UniTask.WhenAll(uiTweens.Where(tween => tween.IsAutoRun).Select(tween => tween.Hide()))
                .AttachExternalCancellation(token);
        }
    }
}
