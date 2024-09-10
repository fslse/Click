using UniRx;

namespace Scripts.Framework.Fire
{
    public abstract class StartupTask
    {
        protected readonly Workflow workflow;

        public readonly string taskName;
        public readonly int power;
        public float percent;

        protected StartupTask(Workflow workflow, string taskName, int power)
        {
            this.workflow = workflow;
            this.taskName = taskName;
            this.power = power;
        }

        public abstract void Start();

        public void Skip()
        {
            workflow.OnTaskFinished(this, true);
        }
    }

    public sealed class StartupProgressMessage
    {
        public float Value { get; set; }
    }

    public sealed class StartupModel
    {
        public ReactiveProperty<float> CurrentProgress { get; }

        public StartupModel()
        {
            CurrentProgress = new ReactiveProperty<float>(0.0f);
            MessageBroker.Default.Receive<StartupProgressMessage>().Subscribe(progress => { CurrentProgress.Value = progress.Value; });
        }
    }
}