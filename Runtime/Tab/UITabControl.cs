using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dreamy.UI
{
    public class UITabControl : MonoBehaviour
    {
        [SerializeField] private List<TabGroup> tabs = new List<TabGroup>();
        [SerializeField] private bool autoInit;
        [SerializeField] private bool autoOpen = true;
        [SerializeField] private int autoOpenTab;

        private void Awake()
        {
            if (autoInit)
            {
                Init().Forget();
            }
        }

        private void OnEnable()
        {
            if (autoOpen)
            {
                OpenTab(autoOpenTab);
            }
        }

        public async UniTask Init()
        {
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].Register(this, i);
                await tabs[i].Init();
            }
        }

        public void OpenTab(int index)
        {
            if (tabs.Count == 0)
            {
                return;
            }

            if (index < 0 || index >= tabs.Count)
            {
                Debug.LogWarning($"Invalid tab index: {index}");
                index = 0;
            }

            for (int i = 0; i < tabs.Count; i++)
            {
                if (i == index)
                {
                    tabs[i].Show();
                }
                else
                {
                    tabs[i].Hide();
                }
            }
        }
    }
}
