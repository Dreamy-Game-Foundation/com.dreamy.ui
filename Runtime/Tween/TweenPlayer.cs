using System;

namespace Dreamy.UI
{
    /// <summary>
    /// Keeps existing prefabs serialized with the old script GUID valid.
    /// New prefabs must use <see cref="UITweenPlayer"/> directly.
    /// </summary>
    [Obsolete("Use UITweenPlayer. This migration component exists for existing prefabs only.")]
    public sealed class TweenPlayer : UITweenPlayer
    {
    }
}
