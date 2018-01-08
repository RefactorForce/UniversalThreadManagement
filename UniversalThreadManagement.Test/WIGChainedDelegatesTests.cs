using System;

using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkItemsGroupTests
{
    /// <summary>
    /// Summary description for TestChainedDelegates.
    /// </summary>
    [TestClass]
    [TestCategory("Test WorkItemsGroup ChainedDelegates")]
    public class ChainedDelegatesTests
    {
        [TestMethod]
        public void GoodCallback()
        {
            STP smartThreadPool = new STP();
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            workItemsGroup.QueueWorkItem(new WorkItemCallback(DoWork));

            workItemsGroup.WaitForIdle();

            smartThreadPool.Shutdown();
        }

        [TestMethod]
        public void ChainedDelegatesCallback()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {

                STP smartThreadPool = new STP();
                IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

                WorkItemCallback workItemCallback = new WorkItemCallback(DoWork);
                workItemCallback += new WorkItemCallback(DoWork);

                workItemsGroup.QueueWorkItem(workItemCallback);

                workItemsGroup.WaitForIdle();

                smartThreadPool.Shutdown();
            });
        }

        [TestMethod]
        public void GoodPostExecute()
        {
            STP smartThreadPool = new STP();
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            workItemsGroup.QueueWorkItem(
                new WorkItemCallback(DoWork),
                null,
                new PostExecuteWorkItemCallback(DoPostExecute));

            workItemsGroup.WaitForIdle();

            smartThreadPool.Shutdown();
        }

        [TestMethod]
        public void ChainedDelegatesPostExecute()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {

                STP smartThreadPool = new STP();
                IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

                PostExecuteWorkItemCallback postExecuteWorkItemCallback =
                    new PostExecuteWorkItemCallback(DoPostExecute);
                postExecuteWorkItemCallback +=
                    new PostExecuteWorkItemCallback(DoPostExecute);

                workItemsGroup.QueueWorkItem(
                    new WorkItemCallback(DoWork),
                    null,
                    postExecuteWorkItemCallback);

                workItemsGroup.WaitForIdle();

                smartThreadPool.Shutdown();
            });
        }


        private object DoWork(object state)
        {
            return null;
        }

        private void DoPostExecute(IWorkItemResult wir)
        {
        }


    }
}
