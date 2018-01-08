using System.Threading;

using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    [TestClass]
    [TestCategory("TestWorkItemTimeout")]
    public class WorkItemTimeoutTests
    {
        /// <summary>
        /// 1. Create STP in suspended mode
        /// 2. Queue work item into the STP
        /// 3. Wait for the work item to expire
        /// 4. Work item's GetResult should throw WorkItemCancelException
        /// </summary>        
        [TestMethod]
        public void CancelInQueueWorkItem()
        {
            Assert.ThrowsException<WorkItemCancelException>(() =>
            {
                STPStartInfo stpStartInfo = new STPStartInfo();
                stpStartInfo.StartSuspended = true;

                bool hasRun = false;

                STP stp = new STP(stpStartInfo);
                IWorkItemResult wir = stp.QueueWorkItem(
                    new WorkItemInfo() { Timeout = 500 },
                    state =>
                    {
                        hasRun = true;
                        return null;
                    });

                Assert.IsFalse(wir.IsCanceled);

                Thread.Sleep(2000);

                Assert.IsTrue(wir.IsCanceled);

                stp.Start();
                stp.WaitForIdle();

                Assert.IsFalse(hasRun);

                try
                {
                    wir.GetResult();
                }
                finally
                {
                    stp.Shutdown();
                }
            });
        }

        /// <summary>
        /// 1. Create STP
        /// 2. Queue work item that takes some time
        /// 3. Wait for it to start
        /// 4. The work item timeout expires
        /// 5. Make sure, in the work item, that STP.IsWorkItemCanceled is true
        /// 5. Wait for the STP to get idle
        /// 6. Work item's GetResult should throw WorkItemCancelException
        /// </summary>        
        [TestMethod]
        public void TimeoutInProgressWorkItemWithSample()
        {
            bool timedout = false;
            ManualResetEvent waitToStart = new ManualResetEvent(false);
            ManualResetEvent waitToComplete = new ManualResetEvent(false);

            STP stp = new STP();
            IWorkItemResult wir = stp.QueueWorkItem(
                new WorkItemInfo() { Timeout = 500 },
                state =>
                {
                    waitToStart.Set();
                    Thread.Sleep(1000);
                    timedout = STP.IsWorkItemCanceled;
                    waitToComplete.Set();
                    return null;
                });

            waitToStart.WaitOne();

            waitToComplete.WaitOne();

            Assert.IsTrue(timedout);

            stp.Shutdown();
        }

        /// <summary>
        /// 1. Create STP
        /// 2. Queue work item into the STP
        /// 3. Wait for the STP to get idle
        /// 4. Work item's GetResult should return value
        /// 4. The work item expires
        /// 5. Work item's GetResult should return value
        /// </summary>        
        [TestMethod]
        public void TimeoutCompletedWorkItem()
        {
            STP stp = new STP();
            IWorkItemResult wir =
                stp.QueueWorkItem(
                new WorkItemInfo() { Timeout = 500 },
                state => 1);

            stp.WaitForIdle();

            Assert.AreEqual(wir.GetResult(), 1);

            Thread.Sleep(1000);

            Assert.AreEqual(wir.GetResult(), 1);

            stp.Shutdown();
        }

        /// <summary>
        /// 1. Create STP
        /// 2. Queue work item that takes some time
        /// 3. Wait for it to start
        /// 4. Cancel the work item (soft)
        /// 5. Call to AbortOnWorkItemCancel
        /// 5. Wait for the STP to get idle
        /// 6. Make sure nothing ran in the work item after the AbortOnWorkItemCancel
        /// </summary>        
        [TestMethod]
        public void TimeoutInProgressWorkItemSoftWithAbortOnWorkItemCancel()
        {
            bool abortFailed = false;
            ManualResetEvent waitToStart = new ManualResetEvent(false);
            ManualResetEvent waitToComplete = new ManualResetEvent(false);

            STP stp = new STP();
            IWorkItemResult wir = stp.QueueWorkItem(
                new WorkItemInfo() { Timeout = 500 },
                state =>
                {
                    waitToStart.Set();
                    Thread.Sleep(1000);
                    STP.AbortOnWorkItemCancel();
                    abortFailed = true;
                    return null;
                });

            waitToStart.WaitOne();

            stp.WaitForIdle();

            Assert.IsTrue(wir.IsCanceled);
            Assert.IsFalse(abortFailed);
            stp.Shutdown();
        }
    }
}
