using System;
using System.Threading;
using EasyNetQ;
using Messages;

namespace JobWorker
{
    internal class Program
    {
        private static IBus _bus;

        private static void Main(string[] args)
        {
            const string rabbitMqConnectionString = "host=localhost;prefetchcount=1;virtualHost=/;username=guest;password=guest";
            _bus = RabbitHutch.CreateBus(rabbitMqConnectionString);

            _bus.Subscribe<StartLongRunningJob>("LongRunningWorkerStart",
                _ => { PerformLongRunningTask(); });

            Console.WriteLine("Worker Waiting for Start Event. Now Run the Test.");
            new AutoResetEvent(false).WaitOne();
        }

        private static void PerformLongRunningTask()
        {
            Console.WriteLine("Starting Long Running Job");
            Thread.Sleep(TimeSpan.FromSeconds(10));
            Console.WriteLine("Finished Long Running Job");
            _bus.Publish(new LongRunningJobCompleted { CompletedTime = DateTime.UtcNow });
        }
    }
}
