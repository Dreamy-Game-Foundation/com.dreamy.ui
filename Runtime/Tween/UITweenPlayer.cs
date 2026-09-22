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

        private readonly List<ITween> cachedTweens = new List<ITween>();
        private bool cacheDirty = true;
        private bool initialized;

        public TweenCollectionMode CollectionMode => collectionMode;
        public IReadOnlyList<ITween> Tweens => cachedTweens;

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
            foreach (ITween tween in cachedTweens)
            {
                tween?.Kill();
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
                        tween.SetInheritedSettings(ResolvePreset(tween.EffectType));
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

                    group?.CollectTweens(cachedTweens, this);
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
                    foreach (ITween tween in cachedTweens)
                    {
                        if (tween != null && tween.IsAutoRun)
                            tasks.Add(show ? tween.Show(token) : tween.Hide(token));
                    }
                }
                else
                {
                    foreach (ITween tween in cachedTweens)
                    {
                        if (tween != null && tween.IsEnabled)
                            tasks.Add(show ? tween.Show(token) : tween.Hide(token));
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
            foreach (ITween tween in cachedTweens) tween?.Init();
        }

        private void PruneCache()
        {
            cachedTweens.RemoveAll(tween => !IsAlive(tween) ||
                (collectionMode == TweenCollectionMode.Auto && tween is UITweenBase component &&
                    !IsOwnedByThisPlayer(component)));
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

        private static bool IsAlive(ITween tween)
        {
            return tween != null &&
                (tween is not UnityEngine.Object unityObject || unityObject != null);
        }

        private TweenSettings ResolvePreset(TweenEffectType type)
        {
            return TweenPresetLibrary.Load()?.Get(type);
        }
    }
}
