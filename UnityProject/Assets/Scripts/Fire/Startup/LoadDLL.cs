namespace Scripts.Fire.Startup
{
    public class LoadDLL : StartupTask
    {
        public LoadDLL(Workflow workflow, string taskName, int power) : base(workflow, taskName, power)
        {
        }

        public override void Start()
        {
            base.Start();
        }
    }
}