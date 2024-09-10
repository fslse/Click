using System.Collections.Generic;
using System.Linq;
using Scripts.Framework.Log;

namespace Scripts.Framework.Fire
{
    public class Workflow
    {
        private List<StartupTask> tasks = new();
        private float totalPercent;

        private string localVersion;
        private string remoteVersion;

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
                GameLog.LogDebug($"启动流程 {tasks[0].taskName}");
                tasks[0].Start();
            }
        }

        public void OnTaskStarted(StartupTask task)
        {
            GameLog.LogDebug($">>>开始任务: {task.taskName}");
        }

        public void OnTaskFinished(StartupTask task, bool skip)
        {
            GameLog.LogDebug(skip ? $">>>跳过任务: {task.taskName}" : $">>>完成任务: {task.taskName}");
            OnProgress(task, 100);
            totalPercent += task.percent;
            tasks.Remove(task);
        }

        public void OnTaskFailed(StartupTask task)
        {
            GameLog.LogError($">>>任务失败: {task.taskName}");
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