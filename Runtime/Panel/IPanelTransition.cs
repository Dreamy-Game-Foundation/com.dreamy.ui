using System.Threading;
using Cysharp.Threading.Tasks;

namespace Dreamy.UI
{
    public interface IPanelTransition
    {
        UniTask Init();

        UniTask ShowTween(CancellationToken token);

        UniTask HideTween(CancellationToken token);

        void Kill();
    }
}
