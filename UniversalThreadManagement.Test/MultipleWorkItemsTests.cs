using System.Threading;

using System;

using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// Summary description for MultipleWorkItemsExample.
    /// </summary>
    [TestClass]
    [TestCategory("TestMultipleWorkItems")]
    public class MultipleWorkItemsTests
    {
        /// <summary>
        /// Example of how to queue several work items and then wait infinitely for 
        /// all of them to complete.
        /// </summary>
        [TestMethod]
        public void WaitAll()
        {
            STP smartThreadPool = new STP();

            bool success = true;

            IWorkItemResult[] wirs = new IWorkItemResult[5];

            for (int i = 0; i < wirs.Length; ++i)
            {
                wirs[i] =
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);
            }

            STP.WaitAll(wirs);

            for (int i = 0; i < wirs.Length; ++i)
            {
                if (!wirs[i].IsCompleted)
                {
                    success = false;
                    break;
                }
                else
                {
                    int result = (int)wirs[i].GetResult();
                    if (1 != result)
                    {
                        success = false;
                        break;
                    }
                }
            }

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Example of how to queue several work items and then wait infinitely for 
        /// one of them to complete.
        /// 
        /// You can use this technique if you have several work items that return the same 
        /// infomration, but use different method to aquire it. Just execute all of them at
        /// once and wait for the first work item to complete.
        /// 
        /// For example: You need an information about a person and you can query several 
        /// information sites (FBI, CIA, etc.). Query all of them at once and use the first
        /// answer to arrive.
        /// </summary>
        [TestMethod]
        public void WaitAny()
        {
            STP smartThreadPool = new STP();

            bool success = false;

            IWorkItemResult[] wirs = new IWorkItemResult[5];

            for (int i = 0; i < wirs.Length; ++i)
            {
                wirs[i] =
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);
            }

            int index = STP.WaitAny(wirs);

            if (wirs[index].IsCompleted)
            {
                int result = (int)wirs[index].GetResult();
                if (1 == result)
                {
                    success = true;
                }
            }

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Example of how to queue several work items and then wait on a timeout for all
        /// of them to complete.
        /// </summary>
        [TestMethod]
        public void WaitAllWithTimeoutSuccess()
        {
            STP smartThreadPool = new STP();

            IWorkItemResult[] wirs = new IWorkItemResult[5];

            for (int i = 0; i < wirs.Length; ++i)
            {
                wirs[i] =
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);
            }

            bool timeout = !STP.WaitAll(wirs, 1500, true);
            bool success = !timeout;

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Example of how to queue several work items and then wait on a timeout for all
        /// of them to complete.
        /// </summary>
        [TestMethod]
        public void WaitAllWithTimeoutFailure()
        {
            STP smartThreadPool = new STP();

            IWorkItemResult[] wirs = new IWorkItemResult[5];

            for (int i = 0; i < wirs.Length; ++i)
            {
                wirs[i] =
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);
            }

            bool timeout = !STP.WaitAll(wirs, 10, true);
            bool success = timeout;

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Example of how to queue several work items and then wait on a timeout for any
        /// of them to complete.
        /// </summary>
        [TestMethod]
        public void WaitAnyWithTimeoutSuccess()
        {
            STP smartThreadPool = new STP();

            bool success = true;

            IWorkItemResult[] wirs = new IWorkItemResult[5];

            for (int i = 0; i < wirs.Length; ++i)
            {
                wirs[i] =
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);
            }

            int index = STP.WaitAny(wirs, 1500, true);

            success = (index != WaitHandle.WaitTimeout);

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        /// <summary>
        /// Example of how to queue several work items and then wait on a timeout for any
        /// of them to complete.
        /// </summary>
        [TestMethod]
        public void WaitAnyWithTimeoutFailure()
        {
            STP smartThreadPool = new STP();

            bool success;

            IWorkItemResult[] wirs = new IWorkItemResult[5];

            for (int i = 0; i < wirs.Length; ++i)
            {
                wirs[i] =
                    smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoSomeWork), null);
            }

            int index = STP.WaitAny(wirs, 10, true);

            success = (index == WaitHandle.WaitTimeout);

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void WaitAllWithEmptyArray()
        {

            IWorkItemResult[] wirs = new IWorkItemResult[0];

            bool success = STP.WaitAll(wirs); ;

            Assert.IsTrue(success);
        }

        private object DoSomeWork(object state)
        {
            Thread.Sleep(1000);
            return 1;
        }

        [TestMethod]
        public void WaitAllT()
        {
            STP smartThreadPool = new STP();

            bool success = true;

            IWorkItemResult<int>[] wirs = new IWorkItemResult<int>[5];

            for (int i = 0; i < wirs.Length; ++i)
            {
                wirs[i] = smartThreadPool.QueueWorkItem(new Func<int, int, int>(System.Math.Min), i, i + 1);
            }

            STP.WaitAll(wirs);

            for (int i = 0; i < wirs.Length; ++i)
            {
                if (!wirs[i].IsCompleted)
                {
                    success = false;
                    break;
                }

                int result = wirs[i].GetResult();
                if (i != result)
                {
                    success = false;
                    break;
                }
            }

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void WaitAnyT()
        {
            STP smartThreadPool = new STP();

            bool success = false;

            IWorkItemResult<int>[] wirs = new IWorkItemResult<int>[5];

            for (int i = 0; i < wirs.Length; ++i)
            {
                wirs[i] = smartThreadPool.QueueWorkItem(new Func<int, int, int>(System.Math.Max), i, i - 1);
            }

            int index = STP.WaitAny(wirs);

            if (wirs[index].IsCompleted)
            {
                int result = wirs[index].GetResult();
                if (index == result)
                {
                    success = true;
                }
            }

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

    }
}
