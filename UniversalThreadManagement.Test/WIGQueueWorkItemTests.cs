using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversalThreadManagement.Test;

namespace WorkItemsGroupTests
{
    /// <summary>
    /// Tests for QueueWorkItem.
    /// </summary>
    [TestClass]
    [TestCategory("TestQueueWorkItem")]
    public class QueueWorkItemTests
    {
        private STP _stp;
        private IWorkItemsGroup _wig;

        [TestInitialize]
        public void Init()
        {
            _stp = new STP();
            _wig = _stp.CreateWorkItemsGroup(STP.DefaultMaxWorkerThreads);
        }

        [TestCleanup]
        public void Fini()
        {
            _stp.Shutdown();
        }

        //IWorkItemResult QueueWorkItem(WorkItemCallback callback);
        [TestMethod]
        public void TestQueueWorkItemCall()
        {
            QueueWorkItemHelper.TestQueueWorkItemCall(_wig);
        }

        //IWorkItemResult QueueWorkItem(WorkItemCallback callback, WorkItemPriority workItemPriority);
        [TestMethod]
        public void TestQueueWorkItemCallPrio()
        {
            QueueWorkItemHelper.TestQueueWorkItemCallPrio(_wig);
        }

        //IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state);
        [TestMethod]
        public void TestQueueWorkItemCallStat()
        {
            QueueWorkItemHelper.TestQueueWorkItemCallStat(_wig);
        }

        //IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, WorkItemPriority workItemPriority);
        [TestMethod]
        public void TestQueueWorkItemCallStatPrio()
        {
            QueueWorkItemHelper.TestQueueWorkItemCallStatPrio(_wig);
        }

        //IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback);
        [TestMethod]
        public void TestQueueWorkItemCallStatPost()
        {
            QueueWorkItemHelper.TestQueueWorkItemCallStatPost(_wig);
        }

        //IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, WorkItemPriority workItemPriority);
        [TestMethod]
        public void TestQueueWorkItemCallStatPostPrio()
        {
            QueueWorkItemHelper.TestQueueWorkItemCallStatPostPrio(_wig);
        }

        //IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute);
        [TestMethod]
        public void TestQueueWorkItemCallStatPostPflg()
        {
            QueueWorkItemHelper.TestQueueWorkItemCallStatPostPflg(_wig);
        }

        //IWorkItemResult QueueWorkItem(WorkItemCallback callback, object state, PostExecuteWorkItemCallback postExecuteWorkItemCallback, CallToPostExecute callToPostExecute, WorkItemPriority workItemPriority);
        [TestMethod]
        public void TestQueueWorkItemCallStatPostPflgPrio()
        {
            QueueWorkItemHelper.TestQueueWorkItemCallStatPostPflgPrio(_wig);
        }

        //IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback);
        [TestMethod]
        public void TestQueueWorkItemInfoCall()
        {
            QueueWorkItemHelper.TestQueueWorkItemInfoCall(_wig);
        }

        //IWorkItemResult QueueWorkItem(WorkItemInfo workItemInfo, WorkItemCallback callback, object state);
        [TestMethod]
        public void TestQueueWorkItemInfoCallStat()
        {
            QueueWorkItemHelper.TestQueueWorkItemInfoCallStat(_wig);
        }
    }
}
