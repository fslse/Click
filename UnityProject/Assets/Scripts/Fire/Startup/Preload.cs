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
        }
    }
}