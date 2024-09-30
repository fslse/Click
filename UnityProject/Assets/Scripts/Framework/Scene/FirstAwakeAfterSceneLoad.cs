using Framework.UI;
using Scripts.Fire.Log;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Framework.Scene
{
    [DefaultExecutionOrder(-99999)]
    public class FirstAwakeAfterSceneLoad : MonoBehaviour
    {
        /// <summary>
        /// 场景加载后第一个 Awake
        /// </summary>
        private void Awake()
        {
            GameLog.LogWarning("This is the first Awake after scene is loaded");

            // Add UICamera to camera stack
            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(UIPanelManager.Instance.UICamera);
        }
    }
}