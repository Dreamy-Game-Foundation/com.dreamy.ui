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
        [SerializeField] private Transform panelRoot;

        private readonly List<UIPanel> stackPanels = new List<UIPanel>();

        public UIPanel LastPanel => stackPanels.Count > 0 ? stackPanels[stackPanels.Count - 1] : null;

        public Type LastPanelType => LastPanel != null ? LastPanel.GetType() : null;

        protected override bool IsPersistent => false;

        protected override void Awake()
        {
            base.Awake();

            if (panelRoot == null)
            {
                panelRoot = transform;
            }
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
                panel.Show().Forget();
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
            GameObject instance = Instantiate(prefab, panelRoot);
            TPanel panel = instance.GetComponent<TPanel>();

            if (panel == null)
            {
                Destroy(instance);
                throw new MissingComponentException($"Addressable UI prefab '{address}' does not contain {typeof(TPanel).Name}.");
            }

            await panel.Init();
            await panel.PostInit();
            return panel;
        }

        public async UniTask<TPanel> Show<TPanel>(string address) where TPanel : UIPanel
        {
            TPanel panel = await Create<TPanel>(address);
            await panel.Show();
            return panel;
        }

        public async UniTask Close<TPanel>() where TPanel : UIPanel
        {
            UIPanel panel = Get<TPanel>();
            if (panel == null)
            {
                return;
            }

            await panel.Hide();
        }

        public UIPanel Get<TPanel>() where TPanel : UIPanel
        {
            for (int i = stackPanels.Count - 1; i >= 0; i--)
            {
                UIPanel panel = stackPanels[i];
                if (panel != null && panel.GetType() == typeof(TPanel))
                {
                    return panel;
                }
            }

            Debug.LogWarning($"[PANEL] Not found panel {typeof(TPanel).Name}.");
            return null;
        }

        public async UniTask<TPanel> Transition<TPanel>(string address) where TPanel : UIPanel
        {
            UIPanel lastPanel = LastPanel;
            if (lastPanel != null)
            {
                lastPanel.SetInteractable(false);
            }

            TPanel newPanel = await Create<TPanel>(address);
            if (lastPanel != null)
            {
                newPanel.OnPreShow += () => lastPanel.HideTween().Forget();
                newPanel.OnPreHide += () => lastPanel.ShowTween().Forget();
                newPanel.OnPostHide += () => lastPanel.SetInteractable(true);
            }

            await newPanel.Show();
            return newPanel;
        }

        public void Register(UIPanel panel)
        {
            if (panel != null && !stackPanels.Contains(panel))
            {
                stackPanels.Add(panel);
            }
        }

        public void Unregister(UIPanel panel)
        {
            stackPanels.Remove(panel);
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
    }
}
