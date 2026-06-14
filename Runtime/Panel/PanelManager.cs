using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Dreamy.Assets;
using Dreamy.Core;
using UnityEngine;

namespace Dreamy.UI
{
    public sealed class PanelManager : MonoSingleton<PanelManager>
    {
        private readonly Dictionary<UILayer, Transform> layerRoots =
            new Dictionary<UILayer, Transform>();
        private readonly List<UIPanel> panels = new List<UIPanel>();
        private readonly List<UIPanel> stackPanels = new List<UIPanel>();

        public UIPanel LastPanel => stackPanels.Count > 0 ? stackPanels[stackPanels.Count - 1] : null;

        public Type LastPanelType => LastPanel != null ? LastPanel.GetType() : null;

        protected override bool IsPersistent => false;

        protected override void Awake()
        {
            base.Awake();

            RefreshLayerRoots(true);
        }

        private void OnValidate()
        {
            RefreshLayerRoots(false);
        }

        private async void Start()
        {
            List<UIPanel> childPanels = GetComponentsInChildren<UIPanel>(true).ToList();
            foreach (UIPanel panel in childPanels)
            {
                await panel.Init();
            }

            foreach (UIPanel panel in childPanels)
            {
                await panel.PostInit();
            }

            foreach (UIPanel panel in childPanels)
            {
                if (panel.ShowOnStart)
                {
                    panel.Show().Forget();
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TryCloseCurrentPanel();
            }
        }

        public async UniTask<TPanel> Create<TPanel>(string address) where TPanel : UIPanel
        {
            GameObject prefab = await AssetLoader.LoadAsync<GameObject>(address);
            GameObject instance = Instantiate(prefab, transform);
            TPanel panel = instance.GetComponent<TPanel>();

            if (panel == null)
            {
                Destroy(instance);
                throw new MissingComponentException($"Addressable UI prefab '{address}' does not contain {typeof(TPanel).Name}.");
            }

            instance.transform.SetParent(GetLayerRoot(panel.Layer), false);

            await panel.Init();
            await panel.PostInit();
            return panel;
        }

        public async UniTask<TPanel> Show<TPanel>(string address) where TPanel : UIPanel
        {
            if (!TryGet(out TPanel panel))
            {
                panel = await Create<TPanel>(address);
            }

            await panel.Show();
            return panel;
        }

        public async UniTask Close<TPanel>() where TPanel : UIPanel
        {
            if (!TryGet(out TPanel panel))
            {
                return;
            }

            await panel.Hide();
        }

        public TPanel Get<TPanel>() where TPanel : UIPanel
        {
            if (TryGet(out TPanel panel))
            {
                return panel;
            }

            Debug.LogWarning($"[PANEL] Not found panel {typeof(TPanel).Name}.");
            return null;
        }

        public bool TryGet<TPanel>(out TPanel result) where TPanel : UIPanel
        {
            for (int i = panels.Count - 1; i >= 0; i--)
            {
                UIPanel panel = panels[i];
                if (panel != null && panel.GetType() == typeof(TPanel))
                {
                    result = (TPanel)panel;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public async UniTask<TPanel> Transition<TPanel>(string address) where TPanel : UIPanel
        {
            UIPanel lastPanel = LastPanel;
            if (lastPanel != null)
            {
                lastPanel.SetInteractable(false);
            }

            if (!TryGet(out TPanel newPanel))
            {
                newPanel = await Create<TPanel>(address);
            }

            if (lastPanel != null)
            {
                UIPanel previousPanel = lastPanel;
                Action preShow = null;
                Action preHide = null;
                Action postHide = null;
                preShow = () =>
                {
                    newPanel.OnPreShow -= preShow;
                    previousPanel.HideTween().Forget();
                };
                preHide = () =>
                {
                    newPanel.OnPreHide -= preHide;
                    previousPanel.ShowTween().Forget();
                };
                postHide = () =>
                {
                    newPanel.OnPostHide -= postHide;
                    previousPanel.SetInteractable(true);
                };
                newPanel.OnPreShow += preShow;
                newPanel.OnPreHide += preHide;
                newPanel.OnPostHide += postHide;
            }

            await newPanel.Show();
            return newPanel;
        }

        public void Register(UIPanel panel)
        {
            if (panel != null && !panels.Contains(panel))
            {
                panel.transform.SetParent(GetLayerRoot(panel.Layer), false);
                panels.Add(panel);
            }
        }

        public void Unregister(UIPanel panel)
        {
            panels.Remove(panel);
            stackPanels.Remove(panel);
        }

        public void MarkShown(UIPanel panel)
        {
            stackPanels.Remove(panel);
            stackPanels.Add(panel);
        }

        public void MarkHidden(UIPanel panel)
        {
            stackPanels.Remove(panel);
        }

        public Transform GetLayerRoot(UILayer layer)
        {
            return layerRoots.TryGetValue(layer, out Transform root)
                ? root
                : transform;
        }

        private void TryCloseCurrentPanel()
        {
            UIPanel panel = LastPanel;
            if (panel == null || !panel.CanBack)
            {
                return;
            }

            panel.Hide().Forget();
        }

        [ContextMenu("Create Missing Layer Roots")]
        private void CreateMissingLayerRoots()
        {
            RefreshLayerRoots(true);
        }

        private void RefreshLayerRoots(bool createMissing)
        {
            layerRoots.Clear();
            UILayerRoot[] roots = GetComponentsInChildren<UILayerRoot>(true);
            foreach (UILayerRoot root in roots)
            {
                if (!layerRoots.TryAdd(root.Layer, root.transform))
                {
                    Debug.LogWarning(
                        $"[PANEL] Multiple roots found for {root.Layer}. " +
                        "The first root will be used.",
                        root);
                }
            }

            if (!createMissing)
            {
                return;
            }

            foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
            {
                if (layerRoots.ContainsKey(layer))
                {
                    continue;
                }

                GameObject rootObject = new GameObject(layer.ToString());
                rootObject.transform.SetParent(transform, false);
                UILayerRoot layerRoot = rootObject.AddComponent<UILayerRoot>();
                layerRoot.SetLayer(layer);
                layerRoots.Add(layer, rootObject.transform);
            }
        }
    }
}
