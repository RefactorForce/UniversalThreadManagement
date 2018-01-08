using System;

using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WorkItemsGroupTests
{
    /// <summary>
    /// Summary description for TestExceptions.
    /// </summary>
    [TestClass]
    [TestCategory("Test WorkItemsGroup Exceptions")]
    public class ExceptionsTests
    {
        private class DivArgs
        {
            public int x;
            public int y;
        }

        [TestMethod]
        public void ExceptionThrowing()
        {
            STP smartThreadPool = new STP();
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            DivArgs divArgs = new DivArgs
            {
                x = 10,
                y = 0
            };

            IWorkItemResult wir =
                workItemsGroup.QueueWorkItem(new WorkItemCallback(this.DoDiv), divArgs);

            try
            {
                wir.GetResult();
            }
            catch (WorkItemResultException wire)
            {
                Assert.IsTrue(wire.InnerException is DivideByZeroException);
                return;
            }
            catch (Exception e)
            {
                e.GetHashCode();
                Assert.Fail();
            }
            Assert.Fail();
        }

        [TestMethod]
        public void ExceptionReturning()
        {
            bool success = true;

            STP smartThreadPool = new STP();
            IWorkItemsGroup workItemsGroup = smartThreadPool.CreateWorkItemsGroup(int.MaxValue);

            DivArgs divArgs = new DivArgs
            {
                x = 10,
                y = 0
            };

            IWorkItemResult wir =
                workItemsGroup.QueueWorkItem(new WorkItemCallback(this.DoDiv), divArgs);

            Exception e = null;
            try
            {
                wir.GetResult(out e);
            }
            catch (Exception ex)
            {
                ex.GetHashCode();
                success = false;
            }

            Assert.IsTrue(success);
            Assert.IsTrue(e is DivideByZeroException);
        }

        private object DoDiv(object state)
        {
            DivArgs divArgs = (DivArgs)state;
            return (divArgs.x / divArgs.y);
        }

    }
}
