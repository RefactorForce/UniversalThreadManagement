using System;
using System.Threading;

using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// Summary description for GetResultExample.
    /// </summary>
    [TestClass]
    [TestCategory("TestGetResult")]
    public class GetResultTests
    {
        /// <summary>
        /// Example of how to queue a work item and then wait infinitely for the result.
        /// </summary>
        [TestMethod]
        public void BlockingCall()
        {
            STP smartThreadPool = new STP();

            bool success = false;

            IWorkItemResult wir =
                smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);

            if (!wir.IsCompleted)
            {
                int result = (int)wir.GetResult();
                success = (1 == result);
            }

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Example of how to queue a work item and then wait on a timeout for the result.
        /// </summary>
        [TestMethod]
        public void Timeout()
        {
            Assert.ThrowsException<WorkItemTimeoutException>(() =>
            {

                STP smartThreadPool = new STP();

                IWorkItemResult wir =
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);

                try
                {
                    wir.GetResult(500, true);
                }
                finally
                {
                    smartThreadPool.Shutdown();
                }
            });
        }

        /// <summary>
        /// Example of how to interrupt the waiting for a work item to complete.
        /// </summary>
        [TestMethod]
        public void WorkItemWaitCanceling()
        {
            Assert.ThrowsException<WorkItemTimeoutException>(() =>
            {
                STP smartThreadPool = new STP();

                ManualResetEvent cancelWaitHandle = new ManualResetEvent(false);

                // Queue a work item that will occupy the thread in the pool
                IWorkItemResult wir1 =
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);

                // Queue another work item that will wait for the first to complete
                IWorkItemResult wir2 =
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.SignalCancel), cancelWaitHandle);

                try
                {
                    wir1.GetResult(System.Threading.Timeout.Infinite, true, cancelWaitHandle);
                }
                finally
                {
                    smartThreadPool.Shutdown();
                }
            });
        }

        private object DoSomeWork(object state)
        {
            Thread.Sleep(1000);
            return 1;
        }

        private object SignalCancel(object state)
        {
            ManualResetEvent cancelWaitHandle = state as ManualResetEvent;
            Thread.Sleep(250);
            cancelWaitHandle.Set();
            return null;
        }

    }
}
