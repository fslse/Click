using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using Scripts.Fire.Singleton;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UIModule
{
    public class UIPanelManager : Singleton<UIPanelManager>
    {
        private GameObject uiRoot;
        private Camera uiCamera;
        private Canvas uiCanvas;

        // 层级节点
        private Transform mainGame;
        private Transform miniGame;
        private Transform normal;
        private Transform message;
        private Transform system;
        private Transform recycle;
        private readonly Transform[] layers;

        private readonly ReactiveCollection<string> panelNames = new(); // 待显示的Panel队列 仅用于Normal层 
        private Dictionary<string, object> panelParams = new(); // 对应的参数

        // Panel字典 针对所有层级
        private readonly Dictionary<string, UIPanelBase> panelDict = new();

        // 构造函数
        private UIPanelManager()
        {
            uiRoot = GameObject.Find("UIRoot");
            uiCamera = uiRoot.transform.Find("UICamera").GetComponent<Camera>();
            uiCanvas = uiRoot.transform.Find("UICanvas").GetComponent<Canvas>();

            // /
            mainGame = uiCanvas.transform.Find("MainGame");
            miniGame = uiCanvas.transform.Find("MiniGame");
            normal = uiCanvas.transform.Find("Normal");
            message = uiCanvas.transform.Find("Message");
            system = uiCanvas.transform.Find("System");
            recycle = uiCanvas.transform.Find("Recycle");

            // 构建层级数组
            layers = new[] { mainGame, miniGame, normal, message, system, recycle };
            if (layers.Any(layer => !layer))
            {
                GameLog.LogError("UIPanelManager", "层级节点丢失");
            }
            else GameLog.LogDebug("UIPanelManager", "层级节点收集完成");

            panelNames.ObserveRemove().Subscribe(async removedPanel =>
            {
                if (panelNames.Count <= 0)
                {
                    return;
                }
            });
        }

        private const string UIPanelPrefabsPath = "Assets/AssetPackages/Game/UIPanel";

        public UIPanelBase MainPanel { get; private set; }

        public async UniTask<UIPanelBase> ShowPanel(string panelName, UIPanelLayer layer, object udata = null, Action<UIPanelBase> action = null)
        {
            if (layer == UIPanelLayer.Normal)
            {
                panelNames.Add(panelName);
                panelParams[panelName] = udata;
                if (panelNames.Count > 1)
                    return null;
            }

            UIPanelBase uiPanelBase = await GetPanel(panelName, layer, action);
            if (!uiPanelBase.gameObject.activeSelf)
                uiPanelBase.Init(udata);
            return uiPanelBase;
        }


        public async UniTask<UIPanelBase> GetPanel(string panelName, UIPanelLayer layer, Action<UIPanelBase> action = null)
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

        public void RecyclePanel(UIPanelBase uiPanelBase)
        {
        }
    }
}