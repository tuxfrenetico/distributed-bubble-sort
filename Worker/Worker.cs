using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using DistributedBubbleSort.Models;
using static System.Console;

namespace Worker
{
  class Worker
  {
    static void Main(string[] args)
    {
      int port = GetAvailablePort();

      var listener = new TcpListener(IPAddress.Loopback, port);
      listener.Start();

      var thread = new Thread(new ThreadStart(() =>
      {
        while (true)
        {
          TcpClient client = listener.AcceptTcpClient();
          BinaryFormatter bf = new BinaryFormatter();
          SerializedArray array = (SerializedArray)bf.Deserialize(client.GetStream());
          WriteLine("");
          WriteLine("Recebido!");
          array.Items.PrintItems();
          array.Items.Sort();
          WriteLine("Ordenado!");
          array.Items.PrintItems();
          bf.Serialize(client.GetStream(), array);
        }
      }));
      thread.Start();
      WriteLine($"Started in port {port}");
      Console.ReadKey();
      thread.Interrupt();
      WriteLine($"Stopped!");

      string text = File.ReadAllText(Constants.PATH_FILE_WORKERS);
      string[] jobs = text.Split(",")
                        .Where(p => !string.IsNullOrWhiteSpace(p) && p != $"{port}")
                        .ToArray();
      File.WriteAllText(Constants.PATH_FILE_WORKERS, $"{string.Join(",", jobs)}");
      WriteLine("Porta liberada");
    }

    private static void ReleasePort(int port)
    {
      string text = File.ReadAllText(Constants.PATH_FILE_WORKERS);
      string[] jobs = text.Split(",")
                        .Where(p => !string.IsNullOrWhiteSpace(p) && p != $"{port}")
                        .ToArray();
      File.WriteAllText(Constants.PATH_FILE_WORKERS, $"{string.Join(",", jobs)}");
      WriteLine("Porta liberada");
    }

    private static int GetAvailablePort()
    {
      int port = 3001;
      string text = File.ReadAllText(Constants.PATH_FILE_WORKERS);
      string[] jobs = text.Split(",").Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();
      if (jobs.Length == 0)
      {
        File.WriteAllText(Constants.PATH_FILE_WORKERS, $"{port}");
      }
      else
      {
        port = Convert.ToInt32(jobs[jobs.Length - 1]) + 1;
        File.WriteAllText(Constants.PATH_FILE_WORKERS, $"{string.Join(",", jobs)},{port}");
      }
      return port;
    }
  }
}