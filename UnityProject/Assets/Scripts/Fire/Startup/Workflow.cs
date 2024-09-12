using System.Collections.Generic;
using System.Linq;
using Scripts.Fire.Log;

namespace Scripts.Fire.Startup
{
    public class Workflow
    {
        private readonly List<StartupTask> tasks = new();
        private float totalPercent;

        public long localVersion = -1;
        public long remoteVersion = -1;

        public void AddTask(StartupTask task)
        {
            tasks.Add(task);
        }

        public void StartFlow()
        {
            int totalPower = tasks.Sum(task => task.power);
            foreach (var task in tasks)
            {
                task.percent = task.power * 100f / totalPower;
            }

            DoNextTask();
        }

        private void DoNextTask()
        {
            if (tasks.Count > 0)
            {
                tasks[0].Start();
            }
        }

        public void OnTaskStarted(StartupTask task)
        {
            GameLog.LogDebug($">>> Progress: {totalPercent}%");
            GameLog.LogDebug($">>> Start: {task.taskName}");
            OnProgress(task, 0);
        }

        public void OnTaskFinished(StartupTask task, bool skip = false)
        {
            GameLog.LogDebug(skip ? $">>> Skip: {task.taskName}" : $">>> Finish: {task.taskName}");
            OnProgress(task, 100);
            totalPercent += task.percent;
            tasks.Remove(task);
            DoNextTask();
        }

        public void OnTaskFailed(StartupTask task)
        {
            GameLog.LogError($">>> Fail: {task.taskName}");
        }

        /// <summary>
        /// This method is called anytime a task experiences measurable progress.
        /// </summary>
        /// <param name="task">任务</param>
        /// <param name="p">该任务完成情况 [0,100]</param>
        public void OnProgress(StartupTask task, int p)
        {
            UniRx.MessageBroker.Default.Publish(new StartupProgressMessage
            {
                Value = totalPercent + task.percent * p / 100
            });
        }
    }
}