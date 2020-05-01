using System;
using static System.Console;

namespace DistributedBubbleSort.Models
{
  public static class ArrayUtils
  {
    private const int LENGTH = 10000;
    
    public static void Sort(this int[] items)
    {
      int temp;
      int length = items.Length;
      for (int h = 0; h < length; h++)
      {
        for (int i = 0; i < length - 1; i++)
        {
          if (items[i] > items[i + 1])
          {
            temp = items[i];
            items[i] = items[i + 1];
            items[i + 1] = temp;
          }
        }
      }
    }

    public static int[] GenerateRandomList()
    {
      int[] items = new int[LENGTH];
      Random r = new Random();

      for (int i = 0; i < LENGTH; i++)
        items[i] = r.Next(0, 99999);
      return items;
    }

    public static void PrintItems(this int[] items)
    {
      int i;
      int interval = 20;
      for (i = 0; i < interval; i++)
      {
        Write($"{items[i]},");
      }

      Write($"{items[++i]}...{items[++i]}");

      for (i = items.Length - interval; i < items.Length - 1; i++)
      {
        Write($"{items[i]},");
      }
      WriteLine(items[items.Length - 1]);
    }
  }
}