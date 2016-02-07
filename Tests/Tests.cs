using System;
using System.Threading;
using EasyNetQ;
using Messages;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class LongRunningJobTests
    {
        private IBus _bus;

        [SetUp]
        public void SetUp()
        {
            const string rabbitMqConnectionString =
                "host=localhost;prefetchcount=1;virtualHost=/;username=guest;password=guest";
            _bus = RabbitHutch.CreateBus(rabbitMqConnectionString);
        }

        [Test]
        public void TestCompletionEvent()
        {
            using (var signal = new AutoResetEvent(false))
            {
                _bus.Subscribe<LongRunningJobCompleted>("LongRunningJobCompletedTest", _ => signal.Set());

                _bus.Publish(new StartLongRunningJob { StartDateTime = DateTime.UtcNow });

                if (!signal.WaitOne(TimeSpan.FromSeconds(30)))
                {
                    // Add additional assets here.
                    Assert.Fail("Test timed out");
                }

                Assert.Pass("Test Completed Succesfully");
            }
        }

        [TearDown]
        public void FixtureTearDown()
        {
            _bus.Dispose();
        }
    }
}