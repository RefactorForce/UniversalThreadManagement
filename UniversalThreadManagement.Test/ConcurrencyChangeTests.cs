using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using STP = UniversalThreadManagement.SmartThreadPool;

namespace UniversalThreadManagement.Test
{
    [TestClass]
    public class ConcurrencyChangeTests
    {
        [TestMethod]
        public void TestMaxThreadsChange()
        {
            STP smartThreadPool = new STP(1 * 1000, 1, 0);

            for (int i = 0; i < 100; ++i)
            {
                smartThreadPool.QueueWorkItem(
                    new WorkItemCallback(this.DoSomeWork),
                    null);
            }

            bool success = WaitForMaxThreadsValue(smartThreadPool, 1, 1 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.MaxThreads = 5;
            success = WaitForMaxThreadsValue(smartThreadPool, 5, 2 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.MaxThreads = 25;
            success = WaitForMaxThreadsValue(smartThreadPool, 25, 4 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.MaxThreads = 10;
            success = WaitForMaxThreadsValue(smartThreadPool, 10, 10 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.Shutdown();
        }

        [TestMethod]
        public void TestMinThreadsChange()
        {
            STP smartThreadPool = new STP(1 * 1000, 25, 0);



            bool success = WaitForMinThreadsValue(smartThreadPool, 0, 1 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.MinThreads = 5;
            success = WaitForMinThreadsValue(smartThreadPool, 5, 2 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.MinThreads = 25;
            success = WaitForMinThreadsValue(smartThreadPool, 25, 4 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.MinThreads = 10;
            success = WaitForMinThreadsValue(smartThreadPool, 10, 10 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.Shutdown();
        }

        [TestMethod]
        public void TestConcurrencyChange()
        {
            STP smartThreadPool = new STP(10 * 1000, 1, 0);

            bool success = false;

            for (int i = 0; i < 100; ++i)
            {
                smartThreadPool.QueueWorkItem(
                    new WorkItemCallback(this.DoSomeWork),
                    null);
            }

            smartThreadPool.Concurrency = 1;
            success = WaitForMaxThreadsValue(smartThreadPool, 1, 1 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.Concurrency = 5;
            success = WaitForMaxThreadsValue(smartThreadPool, 5, 2 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.Concurrency = 25;
            success = WaitForMaxThreadsValue(smartThreadPool, 25, 4 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.Concurrency = 10;
            success = WaitForMaxThreadsValue(smartThreadPool, 10, 10 * 1000);
            Assert.IsTrue(success);

            smartThreadPool.Shutdown();
        }


        private bool WaitForMaxThreadsValue(STP smartThreadPool, int maxThreadsCount, int timeout)
        {
            DateTime end = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout);

            bool success = false;
            while (DateTime.Now <= end && !success)
            {
                success = (smartThreadPool.InUseThreads == maxThreadsCount);
                Thread.Sleep(10);
            }

            return success;
        }

        private bool WaitForMinThreadsValue(STP smartThreadPool, int minThreadsCount, int timeout)
        {
            DateTime end = DateTime.Now + new TimeSpan(0, 0, 0, 0, timeout);

            bool success = false;
            while (DateTime.Now <= end && !success)
            {
                success = (smartThreadPool.ActiveThreads == minThreadsCount);
                Thread.Sleep(10);
            }

            return success;
        }


        private int x = 0;
        private object DoSomeWork(object state)
        {
            Debug.WriteLine(Interlocked.Increment(ref x));
            Thread.Sleep(1000);
            return 1;
        }
    }
}
