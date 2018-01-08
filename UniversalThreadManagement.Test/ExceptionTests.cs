using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using STP = UniversalThreadManagement.SmartThreadPool;

namespace UniversalThreadManagement.Test
{
    [TestClass]
    public class ExceptionTests
    {
        private class DivArgs
        {
            public int x;
            public int y;
        }

        [TestMethod]
        public void ExceptionThrowing()
        {
            STP _smartThreadPool = new STP();

            DivArgs divArgs = new DivArgs
            {
                x = 10,
                y = 0
            };

            IWorkItemResult wir =
                _smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoDiv), divArgs);

            try
            {
                wir.GetResult();
            }
            catch (WorkItemResultException wire)
            {
                Assert.IsTrue(wire.InnerException is System.DivideByZeroException);
                return;
            }
            catch (System.Exception e)
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

            STP _smartThreadPool = new STP();

            DivArgs divArgs = new DivArgs
            {
                x = 10,
                y = 0
            };

            IWorkItemResult wir =
                _smartThreadPool.QueueWorkItem(new WorkItemCallback(this.DoDiv), divArgs);

            System.Exception e = null;
            try
            {
                wir.GetResult(out e);
            }
            catch (System.Exception ex)
            {
                ex.GetHashCode();
                success = false;
            }

            Assert.IsTrue(success);
            Assert.IsTrue(e is System.DivideByZeroException);
        }

        private object DoDiv(object state)
        {
            DivArgs divArgs = (DivArgs)state;
            return (divArgs.x / divArgs.y);
        }

        [TestMethod]
        public void ExceptionType()
        {
            STP smartThreadPool = new STP();

            var workItemResult = smartThreadPool.QueueWorkItem(new Func<int>(ExceptionMethod));

            smartThreadPool.WaitForIdle();

            Assert.IsInstanceOfType(workItemResult.Exception, typeof(NotImplementedException));

            smartThreadPool.Shutdown();
        }

        public int ExceptionMethod() => throw new NotImplementedException();
    }
}
