using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dreamy.UI
{
    /// <summary>
    /// Owns UI transitions. Auto collection stops at nested players; Manual mode
    /// stores serializable effects grouped by their target.
    /// </summary>
    [DisallowMultipleComponent]
    public class UITweenPlayer : MonoBehaviour, IPanelTransition
    {
        [SerializeField] private TweenCollectionMode collectionMode = TweenCollectionMode.Auto;
        [SerializeField] private List<TweenTargetGroup> manualTargets =
            new List<TweenTargetGroup>();

        private readonly List<UITweenBase> cachedTweens = new List<UITweenBase>();
        private bool cacheDirty = true;
        private bool initialized;

        public TweenCollectionMode CollectionMode => collectionMode;
        public IReadOnlyList<UITweenBase> Tweens => cachedTweens;

        protected virtual void Awake()
        {
            RebuildCache();
        }

        protected virtual void OnEnable()
        {
            cacheDirty = true;
        }

        protected virtual void OnDisable()
        {
            Kill();
        }

        protected virtual void OnDestroy()
        {
            Kill();
            cachedTweens.Clear();
        }

        protected virtual void OnTransformChildrenChanged()
        {
            cacheDirty = true;
        }

        public UniTask Init()
        {
            EnsureCache();
            initialized = true;
            InitializeCachedTweens();

            return UniTask.CompletedTask;
        }

        public UniTask ShowTween(CancellationToken token)
        {
            return Play(true, token);
        }

        public UniTask HideTween(CancellationToken token)
        {
            return Play(false, token);
        }

        public void Kill()
        {
            PruneCache();
            foreach (UITweenBase tween in cachedTweens)
            {
                tween?.Kill();
            }

            foreach (TweenTargetGroup group in manualTargets)
            {
                if (group == null) continue;
                foreach (TweenEffect effect in group.Effects) effect?.Kill();
            }
        }

        [ContextMenu("Rebuild Tween Cache")]
        public void RebuildCache()
        {
            cachedTweens.Clear();
            if (collectionMode == TweenCollectionMode.Auto)
            {
                HashSet<UITweenBase> unique = new HashSet<UITweenBase>();
                UITweenBase[] discovered = GetComponentsInChildren<UITweenBase>(true);
                foreach (UITweenBase tween in discovered)
                {
                    if (IsOwnedByThisPlayer(tween) && unique.Add(tween))
                    {
                        cachedTweens.Add(tween);
                    }
                }
            }
            else
            {
                foreach (TweenTargetGroup group in manualTargets)
                {
                    if (group == null || group.Target == null)
                    {
                        continue;
                    }

                    foreach (TweenEffect effect in group.Effects) effect?.Init(group.Target);
                }
            }

            cacheDirty = false;
        }

        private async UniTask Play(bool show, CancellationToken token)
        {
            EnsureCache();
            try
            {
                List<UniTask> tasks = new List<UniTask>();
                if (collectionMode == TweenCollectionMode.Auto)
                {
                    foreach (UITweenBase tween in cachedTweens)
                    {
                        if (tween != null && tween.IsAutoRun)
                            tasks.Add(show ? tween.Show(token) : tween.Hide(token));
                    }
                }
                else
                {
                    foreach (TweenTargetGroup group in manualTargets)
                    {
                        if (group == null || group.Target == null) continue;
                        foreach (TweenEffect effect in group.Effects)
                        {
                            if (effect != null && effect.IsAutoRun)
                                tasks.Add(show
                                    ? effect.Show(group.Target, ResolvePreset(effect, group), this)
                                    : effect.Hide(group.Target, ResolvePreset(effect, group), this));
                        }
                    }
                }

                await UniTask.WhenAll(tasks).AttachExternalCancellation(token);
            }
            catch (OperationCanceledException)
            {
                Kill();
                throw;
            }
            finally
            {
                PruneCache();
            }
        }

        private void EnsureCache()
        {
            if (cacheDirty || !initialized)
            {
                RebuildCache();
            }

            PruneCache();
            if (initialized)
            {
                InitializeCachedTweens();
            }
        }

        private void InitializeCachedTweens()
        {
            for (int index = 0; index < cachedTweens.Count; index++)
            {
                UITweenBase tween = cachedTweens[index];
                if (tween != null)
                {
                    tween.SetInheritedSettings(ResolvePreset(tween.EffectType));
                    tween.Init();
                }
            }
        }

        private void PruneCache()
        {
            cachedTweens.RemoveAll(tween => tween == null ||
                (collectionMode == TweenCollectionMode.Auto && !IsOwnedByThisPlayer(tween)));
        }

        private bool IsOwnedByThisPlayer(UITweenBase tween)
        {
            if (tween == null)
            {
                return false;
            }

            UITweenPlayer nearestPlayer = tween.GetComponentInParent<UITweenPlayer>(true);
            return nearestPlayer == this;
        }

        private TweenSettings ResolvePreset(TweenEffect effect, TweenTargetGroup group)
        {
            if (group.Preset != null) return group.Preset;
            return ResolvePreset(effect.Type);
        }

        private TweenSettings ResolvePreset(TweenEffectType type)
        {
            return TweenPresetLibrary.Load()?.Get(type);
        }
    }
}
