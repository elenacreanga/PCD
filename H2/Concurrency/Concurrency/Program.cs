using System;
using System.Threading;

namespace Concurrency
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var pt = new PrintingThread();

            // 10 threads that are all pointing to the same method on the same object
            var threads = new Thread[10];
            for (var i = 0; i < 10; i++)
            {
                threads[i] = new Thread(new ThreadStart(pt.ThreadJob));
                threads[i].Name = $"Worker thread [{i}]";
            }

            foreach (var thread in threads) thread.Start();

            Console.ReadLine();
        }
    }
}