using System;
using System.Runtime.Remoting.Contexts;
using System.Threading;

namespace Concurrency
{
    [Synchronization]
    public class PrintingThread : ContextBoundObject
    {
        private readonly object threadLock = new object();

        public void ThreadJob()
        {
            //Monitor.Enter(threadLock);
            //try
            //lock (threadLock)
            //{
            Console.WriteLine(Thread.CurrentThread.Name);
            Console.WriteLine($"Here are the numbers for: {Thread.CurrentThread.Name}");
            for (var i = 0; i < 10; i++)
            {
                var random = new Random();
                Thread.Sleep(1000 * random.Next(2));
                Console.Write($"{i} ");
            }
            Console.WriteLine();
            //}
            //finally
            //{
            //    Monitor.Exit(threadLock);
            //}
        }
    }
}