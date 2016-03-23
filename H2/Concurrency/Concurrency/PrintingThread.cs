using System;
using System.Threading;

namespace Concurrency
{
    public class PrintingThread
    {
        public void ThreadJob()
        {
            Console.WriteLine(Thread.CurrentThread.Name);
            Console.WriteLine($"Here are the numbers for: {Thread.CurrentThread.Name}");
            for (var i = 0; i < 10; i++)
            {
                var r = new Random();
                Thread.Sleep(1000 * r.Next(2));
                Console.Write($"{i} ");
            }
            Console.WriteLine();
        }
    }
}