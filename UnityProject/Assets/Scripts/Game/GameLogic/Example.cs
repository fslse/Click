using Cysharp.Threading.Tasks;
using Framework.UIModule;
using UnityEngine;

namespace Game.GameLogic
{
    public class Example : MonoBehaviour
    {
        private void Awake()
        {
            UIPanelManager.Instance.GetPanel("ExamplePanel", UIPanelLayer.Normal).Forget();
        }
    }
}