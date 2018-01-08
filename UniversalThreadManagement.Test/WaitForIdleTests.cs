using System;
using System.Threading;
using System.Diagnostics;

using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// Summary description for TestWaitForIdle.
    /// </summary>
    [TestClass]
    [TestCategory("TestWaitForIdle")]
    public class WaitForIdleTests
    {
        /// <summary>
        /// Example of waiting for idle
        /// </summary>
        [TestMethod]
        public void WaitForIdle()
        {
            STP smartThreadPool = new STP(10 * 1000, 25, 0);

            ManualResetEvent isRunning = new ManualResetEvent(false);

            for (int i = 0; i < 4; ++i)
            {
                smartThreadPool.QueueWorkItem(delegate { isRunning.WaitOne(); });
            }

            bool success = !smartThreadPool.WaitForIdle(1000);

            isRunning.Set();

            success = success && smartThreadPool.WaitForIdle(1000);

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void WaitForIdleOnWrongThread()
        {
            STP smartThreadPool = new STP(10 * 1000, 25, 0);

            IWorkItemResult wir = smartThreadPool.QueueWorkItem(
                new WorkItemCallback(this.DoWaitForIdle),
                smartThreadPool);

            wir.GetResult(out Exception e);

            smartThreadPool.Shutdown();

            Assert.IsTrue(e is NotSupportedException);
        }


        private int x = 0;
        private object DoSomeWork(object state)
        {
            Debug.WriteLine(Interlocked.Increment(ref x));
            Thread.Sleep(1000);
            return 1;
        }

        private object DoWaitForIdle(object state)
        {
            STP smartThreadPool = state as STP;
            smartThreadPool.WaitForIdle();
            return null;
        }
    }
}
