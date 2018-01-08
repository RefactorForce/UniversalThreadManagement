using System;

using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    [TestClass]
    [TestCategory("TestChainedDelegates")]
    public class ChainedDelegateTests
    {
        [TestMethod]
        public void GoodCallback()
        {
            STP stp = new STP();

            stp.QueueWorkItem(new WorkItemCallback(DoWork));

            stp.WaitForIdle();

            stp.Shutdown();
        }

        [TestMethod]
        public void ChainedDelegatesCallback()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {

                STP stp = new STP();

                WorkItemCallback workItemCallback = new WorkItemCallback(DoWork);
                workItemCallback += new WorkItemCallback(DoWork);

                stp.QueueWorkItem(workItemCallback);

                stp.WaitForIdle();

                stp.Shutdown();
            });
        }

        [TestMethod]
        public void GoodPostExecute()
        {
            STP stp = new STP();

            stp.QueueWorkItem(
                new WorkItemCallback(DoWork),
                null,
                new PostExecuteWorkItemCallback(DoPostExecute));

            stp.WaitForIdle();

            stp.Shutdown();
        }

        [TestMethod]
        public void ChainedDelegatesPostExecute()
        {
            Assert.ThrowsException<NotSupportedException>(() =>
            {

                STP stp = new STP();

                PostExecuteWorkItemCallback postExecuteWorkItemCallback = DoPostExecute;
                postExecuteWorkItemCallback += DoPostExecute;

                stp.QueueWorkItem(
                    new WorkItemCallback(DoWork),
                    null,
                    postExecuteWorkItemCallback);

                stp.WaitForIdle();

                stp.Shutdown();
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
