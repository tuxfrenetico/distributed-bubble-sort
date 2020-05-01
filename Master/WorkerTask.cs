using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using DistributedBubbleSort.Models;

namespace DistributedBubbleSort.Master
{
  public class WorkerTask
  {
    private Thread thread;
    private TcpClient client;
    public Action<int, int, string, int[]> Callback;
    public readonly int TaskId;
    public WorkerTask(int taskId, int workerId, int[] items)
    {
      TaskId = taskId;
      var arr = new SerializedArray() { Items = items };
      client = new TcpClient();
      client.Connect(IPAddress.Loopback, workerId);
      var bf = new BinaryFormatter();
      thread = new Thread(new ThreadStart(() =>
      {
        var startTime = DateTime.Now;
        bf.Serialize(client.GetStream(), arr);
        var result = (SerializedArray)bf.Deserialize(client.GetStream());
        client.Close();
        client.Dispose();
        var endedTime = DateTime.Now;
        TimeSpan ts = endedTime.Subtract(startTime);
        string elapsedTime = $"{ts.Seconds}.{ts.Milliseconds}";
        Callback?.Invoke(workerId, taskId, elapsedTime, result.Items);
      }));
    }

    public void Start()
    {
      thread.Start();
    }
  }
}