using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using Scripts.Fire.Singleton;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Framework.Modules.UI
{
    [UsedImplicitly]
    public class UIPanelManager : Singleton<UIPanelManager>
    {
        /// <summary>
        /// UIPanel预制体路径
        /// </summary>
        private const string UIPanelPrefabsPath = "Assets/AssetPackages/UIPanel";

        // 层级节点
        private readonly Transform[] layers;

        // Panel队列 仅用于Normal层 
        private readonly ReactiveCollection<string> panelNames = new();
        private readonly Dictionary<string, object> panelParams = new(); // 对应的参数

        // Panel字典 针对所有层级
        private readonly Dictionary<string, UIPanelBase> panelDict = new();

        public GameObject UIRoot { get; }
        public Camera UICamera { get; }
        public Canvas UICanvas { get; }
        public Image UIMask { get; }

        public UIPanelBase MainPanel { get; private set; }

        // 构造函数
        private UIPanelManager()
        {
            UIRoot = GameObject.Find("UIRoot");
            UICamera = UIRoot.transform.Find("UICamera").GetComponent<Camera>();
            UICanvas = UIRoot.transform.Find("UICanvas").GetComponent<Canvas>();
            UIMask = UICanvas.transform.Find("--- UIMask ---").GetComponent<Image>();

            // 获取层级节点
            var mainGame = UICanvas.transform.Find("MainGame");
            var miniGame = UICanvas.transform.Find("MiniGame");
            var normal = UICanvas.transform.Find("Normal");
            var message = UICanvas.transform.Find("Message");
            var system = UICanvas.transform.Find("System");
            var recycle = UICanvas.transform.Find("Recycle");

            // 构建层级数组
            layers = new[] { mainGame, miniGame, normal, message, system, recycle };
            if (layers.Any(layer => !layer))
            {
                GameLog.LogError("UIPanelManager", "层级节点丢失");
            }
            else GameLog.LogDebug("UIPanelManager", "层级节点收集完成");

            panelNames.ObserveRemove().Subscribe(removedPanelName => OnNext(removedPanelName).Forget());
            return;

            async UniTaskVoid OnNext(CollectionRemoveEvent<string> _)
            {
                if (panelNames.Count <= 0)
                {
                    return;
                }

                string panelName = panelNames[0];
                if (string.IsNullOrEmpty(panelName))
                    Debug.LogError("panelName is null or empty.");
                else
                {
                    object udata = panelParams[panelName];
                    await GetPanel(panelName, UIPanelLayer.Normal, uiPanelBase =>
                    {
                        if (uiPanelBase.gameObject.activeSelf == false)
                            uiPanelBase.OnEnter(udata).Forget();
                        else
                            Debug.LogError($"{panelName} is already active.");
                    });
                }
            }
        }

        /// <summary>
        /// 针对MainGame层，适用于有多个主玩法界面
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="udata"></param>
        /// <returns></returns>
        public async UniTask<UIPanelBase> Jump2MainPanel(string panelName, object udata = null)
        {
            if (MainPanel)
                RecyclePanel(MainPanel);
            MainPanel = await GetPanel(panelName, UIPanelLayer.MainGame);
            MainPanel.OnEnter(udata).Forget();
            return MainPanel;
        }

        /// <summary>
        /// 通用方法，但对于Normal层的Panel做特殊处理，保证只有一个Normal层的Panel显示
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="layer"></param>
        /// <param name="udata"></param>
        /// <returns>注意，对于Normal层的Panel，可能会返回null</returns>
        public async UniTask<UIPanelBase> ShowPanel(string panelName, UIPanelLayer layer, object udata = null)
        {
            if (layer == UIPanelLayer.Normal)
            {
                panelNames.Add(panelName);
                panelParams[panelName] = udata;
                if (panelNames.Count > 1)
                    return null;
            }

            UIPanelBase uiPanelBase = await GetPanel(panelName, layer);
            if (!uiPanelBase.gameObject.activeSelf)
                uiPanelBase.OnEnter(udata).Forget();
            return uiPanelBase;
        }

        /// <summary>
        /// 针对Normal层的Panel，立即显示，处于队列首位，这种情况下可能有多个Normal层的Panel同时显示
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="udata"></param>
        /// <returns></returns>
        public async UniTask<UIPanelBase> ShowNormalPanel(string panelName, object udata = null)
        {
            panelNames.Insert(0, panelName);
            panelParams[panelName] = udata;
            UIPanelBase uiPanelBase = await GetPanel(panelName, UIPanelLayer.Normal);
            uiPanelBase.OnEnter(udata).Forget();
            return uiPanelBase;
        }

        private async UniTask<UIPanelBase> GetPanel(string panelName, UIPanelLayer layer, Action<UIPanelBase> action = null)
        {
            // 字典中不存在
            if (!panelDict.TryGetValue(panelName, out var uiPanelBase))
            {
                GameObject prefab = await AssetManager.Instance.LoadAssetAsync<GameObject>($"{UIPanelPrefabsPath}/{panelName}/{panelName}.prefab");
                if (!prefab)
                    GameLog.LogError("UIPanelManager", $"{panelName}.prefab not found.\nPath: {UIPanelPrefabsPath}/{panelName}/{panelName}.prefab.");

                var panel = Object.Instantiate(prefab, layers[(int)layer]);
                panel.name = panelName;
                panel.SetActive(false);
                GameLog.LogDebug("UIPanelManager", $"{panelName} instantiated.");
                uiPanelBase = panel.GetComponent<UIPanelBase>();
                uiPanelBase.PanelLayer = layer;
                panelDict.Add(panelName, uiPanelBase);
            }
            else
                uiPanelBase.gameObject.transform.SetParent(layers[(int)layer], false);

            action?.Invoke(uiPanelBase);
            return uiPanelBase;
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="uiPanelBase"></param>
        public void RecyclePanel(UIPanelBase uiPanelBase)
        {
            uiPanelBase.gameObject.SetActive(false);
            uiPanelBase.gameObject.transform.SetParent(layers[(int)UIPanelLayer.Recycle], false);
            if (panelNames.Contains(uiPanelBase.PanelName))
                panelNames.Remove(uiPanelBase.PanelName); // 删除第一个匹配项
        }
    }
}