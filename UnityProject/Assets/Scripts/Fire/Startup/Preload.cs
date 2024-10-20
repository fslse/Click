using Cysharp.Threading.Tasks;
using Scripts.Fire.Manager;
using UnityEngine;

namespace Scripts.Fire.Startup
{
    public class Preload : StartupTask
    {
        public Preload(Workflow workflow, string taskName, int power) : base(workflow, taskName, power)
        {
        }

        public override void Start()
        {
            base.Start();
            Execute().Forget();
        }

        private async UniTaskVoid Execute()
        {
            if (Application.isEditor)
            {
                Skip();
                return;
            }

            await AssetManager.Instance.Initialize();
            workflow.OnTaskFinished(this);
        }
    }
}