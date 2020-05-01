using System;
using static System.Console;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DistributedBubbleSort.Models;

namespace DistributedBubbleSort.Master
{
  class Master
  {
    private static Dictionary<int, int[]> tasks;
    private static int pendingTasks = 20;
    private static DateTime startTasks = DateTime.Now;
    static void Main(string[] args)
    {
      int[] workers = GetWorkers();
      if (workers.Length == 0)
      {
        WriteLine("Não existem workers disponíveis.");
        return;
      }

      tasks = new Dictionary<int, int[]>();
      for (int i = 1; i <= pendingTasks; i++)
        tasks.Add(i, ArrayUtils.GenerateRandomList());

      foreach (int port in workers)
      {
        if (tasks.Any())
        {
          var task = tasks.First();
          tasks.Remove(task.Key);
          var workerTask = new WorkerTask(task.Key, port, task.Value);
          workerTask.Callback = Callback;
          workerTask.Start();
        }
      }
    }

    public static void Callback(int workerId, int taskId, string elapsedTime, int[] ordenedItems)
    {
      pendingTasks--;
      WriteLine($"worker {workerId} finished the task {taskId} in {elapsedTime}s");
      if (tasks.Any())
      {
        var task = tasks.First();
        tasks.Remove(task.Key);
        var workerTask = new WorkerTask(task.Key, workerId, task.Value);
        workerTask.Callback = Callback;
        workerTask.Start();
      }
      if (pendingTasks <= 0)
      {
        TimeSpan ts = DateTime.Now.Subtract(startTasks);
        string elapsedAllTime = $"{ts.Seconds}.{ts.Milliseconds}";
        WriteLine($"All tasks was finished in {elapsedAllTime}s");
      }
    }

    private static int[] GetWorkers()
    {
      string text = File.ReadAllText(Constants.PATH_FILE_WORKERS);
      var jobs = text.Split(",").Where(p => !string.IsNullOrWhiteSpace(p));
      return jobs.Select(p => Convert.ToInt32(p)).ToArray();
    }
  }
}