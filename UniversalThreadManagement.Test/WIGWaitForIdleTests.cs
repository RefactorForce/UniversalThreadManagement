using System;
using System.Threading;

using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkItemsGroupTests
{
    /// <summary>
    /// Summary description for TestWaitForIdle.
    /// </summary>
    [TestClass]
    [TestCategory("WorkItemsGroup")]
    public class TestWaitForIdle
    {
        /// <summary>
        /// Example of waiting for idle
        /// </summary>
        [TestMethod]
        public void WaitForIdle()
        {
            STP smartThreadPool = new STP(10 * 1000, 25, 0);
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            ManualResetEvent isRunning = new ManualResetEvent(false);

            for (int i = 0; i < 4; ++i)
            {
                workItemsGroup.QueueWorkItem(delegate { isRunning.WaitOne(); });
            }

            bool success = !workItemsGroup.WaitForIdle(1000);

            isRunning.Set();

            success = success && workItemsGroup.WaitForIdle(1000);

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void WaitForIdleOnSTPThread()
        {
            STP smartThreadPool = new STP(10 * 1000, 25, 0);
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            IWorkItemResult wir = workItemsGroup.QueueWorkItem(
                new WorkItemCallback(this.DoWaitForIdle),
                workItemsGroup);

            Exception e;
            wir.GetResult(out e);

            smartThreadPool.Shutdown();

            Assert.IsNotNull(e);
        }

        [TestMethod]
        public void WaitForIdleOnSTPThreadForAnotherWorkItemsGroup()
        {
            STP smartThreadPool = new STP(10 * 1000, 25, 0);
            IWorkItemsGroup workItemsGroup1 = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);
            IWorkItemsGroup workItemsGroup2 = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            workItemsGroup1.QueueWorkItem(
                new WorkItemCallback(this.DoSomeWork),
                1000);

            workItemsGroup1.QueueWorkItem(
                new WorkItemCallback(this.DoSomeWork),
                1000);

            IWorkItemResult wir = workItemsGroup2.QueueWorkItem(
                new WorkItemCallback(this.DoWaitForIdle),
                workItemsGroup1);

            Exception e;
            wir.GetResult(out e);

            smartThreadPool.Shutdown();

            Assert.IsNull(e);
        }


        private int _x = 0;
        private object DoSomeWork(object state)
        {
            int sleepTime = (int)state;
            int newX = Interlocked.Increment(ref _x);
            Console.WriteLine("{0}: Enter, newX = {1}", DateTime.Now.ToLongTimeString(), newX);
            Console.WriteLine("{0}: Sleeping for {1} ms", DateTime.Now.ToLongTimeString(), sleepTime);
            Thread.Sleep(sleepTime);
            newX = Interlocked.Increment(ref _x);
            Console.WriteLine("{0}: Leave, newX = {1}", DateTime.Now.ToLongTimeString(), newX);
            return 1;
        }

        private object DoWaitForIdle(object state)
        {
            IWorkItemsGroup workItemsGroup = state as IWorkItemsGroup;
            workItemsGroup.WaitForIdle();
            return null;
        }


        [TestMethod]
        public void WaitForIdleWithCancel()
        {
            STP smartThreadPool = new STP(10 * 1000, 1, 1);
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(2);

            _x = 0;

            IWorkItemResult wir1 = workItemsGroup.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), 1000);
            IWorkItemResult wir2 = workItemsGroup.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), 1000);
            IWorkItemResult wir3 = workItemsGroup.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), 1000);

            while (0 == _x)
            {
                Thread.Sleep(10);
            }

            Console.WriteLine("{0}: Cancelling WIG", DateTime.Now.ToLongTimeString());
            workItemsGroup.Cancel();

            // At this point:
            // The first work item is running
            // The second work item is cancelled, but waits in the STP queue
            // The third work item is cancelled.

            Assert.AreEqual(1, _x);

            Assert.IsTrue(wir2.IsCanceled);

            Assert.IsTrue(wir3.IsCanceled);

            // Make sure the workItemsGroup is still idle
            Assert.IsFalse(workItemsGroup.IsIdle);

            Console.WriteLine("{0}: Waiting for 1st result", DateTime.Now.ToLongTimeString());
            wir1.GetResult();

            Assert.AreEqual(2, _x);

            bool isIdle = workItemsGroup.WaitForIdle(100);

            Assert.IsTrue(isIdle);

            smartThreadPool.Shutdown();
        }

        [TestMethod]
        public void WaitForIdleEvent()
        {
            STP smartThreadPool = new STP();
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(1);
            ManualResetEvent wigIsIdle = new ManualResetEvent(false);

            workItemsGroup.OnIdle += wig => wigIsIdle.Set();

            workItemsGroup.QueueWorkItem(() => { });

            bool eventFired = wigIsIdle.WaitOne(100, true);

            smartThreadPool.Shutdown();

            Assert.IsTrue(eventFired);
        }
    }
}
