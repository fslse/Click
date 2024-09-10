namespace Scripts.Fire.Startup
{
    public class DownloadAssets : StartupTask
    {
        public DownloadAssets(Workflow workflow, string taskName, int power) : base(workflow, taskName, power)
        {
        }

        public override void Start()
        {
            base.Start();
        }
    }
}