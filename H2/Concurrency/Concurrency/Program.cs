using System;
using System.Threading;

namespace Concurrency
{
    public class Program
    {
        private static readonly Mutex Mutex = new Mutex(false, "bogotobogo.com");

        public static void Main(string[] args)
        {
            PrintThread();

            UseMutex();
        }

        private static void PrintThread()
        {
            var printingThread = new PrintingThread();

            // 10 threads that are all pointing to the same method on the same object
            var threads = new Thread[10];
            for (var i = 0; i < 10; i++)
            {
                threads[i] = new Thread(new ThreadStart(printingThread.ThreadJob));
                threads[i].Name = $"Worker thread [{i}]";
            }

            foreach (var thread in threads) thread.Start();

            Console.ReadLine();
        }

        private static void UseMutex()
        {
            if (!Mutex.WaitOne(TimeSpan.FromSeconds(5), false))
            {
                Console.WriteLine("Busy running another app!");
                return;
            }

            try
            {
                Console.WriteLine("Now running. Hit any key to exit.");
                Console.ReadLine();
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }
    }
}