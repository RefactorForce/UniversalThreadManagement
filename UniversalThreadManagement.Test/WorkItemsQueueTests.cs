using UniversalThreadManagement.Internal;
using UniversalThreadManagement.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement
{
    /// <summary>
    /// Summary description for TestWorkItemsQueue.
    /// </summary>
    [TestClass]
    [TestCategory("TestWorkItemsQueue")]
    public class WorkItemsQueueTests
    {
        [TestMethod]
        public void Init()
        {
        }

        [TestMethod]
        public void IdempotenceWaiterEntry()
        {
            WorkItemsQueue q = new WorkItemsQueue();

            Assert.AreEqual(0, q.WaitersCount);

            WorkItemsQueue.WaiterEntry we1 = new WorkItemsQueue.WaiterEntry();
            q.PushWaiter(we1);

            Assert.AreEqual(1, q.WaitersCount);

            q.PushWaiter(we1);

            Assert.AreEqual(1, q.WaitersCount);

            WorkItemsQueue.WaiterEntry we2 = new WorkItemsQueue.WaiterEntry();
            q.PushWaiter(we2);

            Assert.AreEqual(2, q.WaitersCount);

            q.PushWaiter(we2);

            Assert.AreEqual(2, q.WaitersCount);
        }
    }
}
