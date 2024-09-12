using UniRx;

namespace Scripts.Fire.Startup
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

        public virtual void Start()
        {
            workflow.OnTaskStarted(this);
        }

        protected virtual void Skip()
        {
            workflow.OnTaskFinished(this, true);
        }
    }

    public sealed class StartupProgressMessage
    {
        public float Value { get; set; }
    }
}