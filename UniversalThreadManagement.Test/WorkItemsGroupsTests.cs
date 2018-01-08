using System;
using System.Threading;
using UniversalThreadManagement.Test;
#if NETCOREAPP2_0


#else
using NUnit.Framework;
using TestBase=System.Object;
#endif

using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkItemsGroupTests
{
    /// <summary>
    /// Summary description for TestWorkItemsGroups.
    /// </summary>
    [TestClass]
    [TestCategory("TestWorkItemsGroups")]
    public class WorkItemsGroupsTests
    {
        [TestMethod]
        public void BlockingCall()
        {
            STP smartThreadPool = new STP();
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            bool success = false;

            IWorkItemResult wir =
                workItemsGroup.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);

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
            STP smartThreadPool = new STP();
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            bool success = false;

            IWorkItemResult wir =
                workItemsGroup.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);

            try
            {
                wir.GetResult(500, true);
            }
            catch (WorkItemTimeoutException)
            {
                success = true;
            }

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Example of how to interrupt the waiting for a work item to complete.
        /// </summary>
        [TestMethod]
        public void WorkItemWaitCanceling()
        {
            STP smartThreadPool = new STP();
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            ManualResetEvent cancelWaitHandle = new ManualResetEvent(false);

            bool success = false;

            // Queue a work item that will occupy the thread in the pool
            IWorkItemResult wir1 =
                workItemsGroup.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);

            // Queue another work item that will wait for the first to complete
            IWorkItemResult wir2 =
                workItemsGroup.QueueWorkItem(new WorkItemCallback(this.SignalCancel), cancelWaitHandle);

            try
            {
                wir1.GetResult(System.Threading.Timeout.Infinite, true, cancelWaitHandle);
            }
            catch (WorkItemTimeoutException)
            {
                success = true;
            }

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
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

        [TestMethod]
        public void Concurrency()
        {
        }

        [TestMethod]
        public void WaitForIdle()
        {
        }

        [TestMethod]
        public void OnIdleEvent()
        {
        }

        [TestMethod]
        public void MultipleGroups()
        {
        }

    }
}
