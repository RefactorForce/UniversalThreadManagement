using System.Threading;

using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// Summary description for TestThreadPriority.
    /// </summary>
    [TestClass]
    [TestCategory("TestThreadPriority")]
    public class ThreadPriorityTests
    {
        [TestMethod]
        public void TestDefaultPriority() => CheckSinglePriority(STP.DefaultThreadPriority);

        [TestMethod]
        public void TestLowestPriority() => CheckSinglePriority(ThreadPriority.Lowest);

        [TestMethod]
        public void TestBelowNormalPriority() => CheckSinglePriority(ThreadPriority.BelowNormal);

        [TestMethod]
        public void TestNormalPriority() => CheckSinglePriority(ThreadPriority.BelowNormal);

        [TestMethod]
        public void TestAboveNormalPriority() => CheckSinglePriority(ThreadPriority.AboveNormal);

        [TestMethod]
        public void TestHighestPriority() => CheckSinglePriority(ThreadPriority.Highest);

        private void CheckSinglePriority(ThreadPriority threadPriority)
        {
            STPStartInfo stpStartInfo = new STPStartInfo
            {
                ThreadPriority = threadPriority
            };

            STP stp = new STP(stpStartInfo);

            IWorkItemResult wir = stp.QueueWorkItem(new WorkItemCallback(GetThreadPriority));
            ThreadPriority currentThreadPriority = (ThreadPriority)wir.GetResult();

            Assert.AreEqual(threadPriority, currentThreadPriority);
        }

        private object GetThreadPriority(object state)
        {
            return Thread.CurrentThread.Priority;
        }
    }
}