using Cysharp.Threading.Tasks;

namespace Dreamy.UI
{
    public interface ITween
    {
        bool IsAutoRun { get; }

        UniTask Init();

        UniTask Show();

        UniTask Hide();
    }
}
