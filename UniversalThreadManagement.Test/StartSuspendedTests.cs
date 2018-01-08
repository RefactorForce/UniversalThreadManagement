using System.Threading;

#if NETCOREAPP2_0


#else
using NUnit.Framework;
using TestBase=System.Object;
#endif

using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// Summary description for TestStartSuspended.
    /// </summary>
    [TestClass]
    [TestCategory("TestStartSuspended")]
    public class StartSuspendedTests
    {
        [TestMethod]
        public void StartSuspended()
        {
            STPStartInfo stpStartInfo = new STPStartInfo
            {
                StartSuspended = true
            };

            STP stp = new STP(stpStartInfo);

            stp.QueueWorkItem(new WorkItemCallback(this.DoWork));

            Assert.IsFalse(stp.WaitForIdle(200));

            stp.Start();

            Assert.IsTrue(stp.WaitForIdle(200));
        }

        [TestMethod]
        public void WIGStartSuspended()
        {
            STP stp = new STP();

            WIGStartInfo wigStartInfo = new WIGStartInfo
            {
                StartSuspended = true
            };

            IWorkItemsGroup wig = stp.CreateWorkItemsGroup(10, wigStartInfo);

            wig.QueueWorkItem(new WorkItemCallback(this.DoWork));

            Assert.IsFalse(wig.WaitForIdle(200));

            wig.Start();

            Assert.IsTrue(wig.WaitForIdle(200));
        }

        [TestMethod]
        public void STPAndWIGStartSuspended()
        {
            STPStartInfo stpStartInfo = new STPStartInfo
            {
                StartSuspended = true
            };

            STP stp = new STP(stpStartInfo);

            WIGStartInfo wigStartInfo = new WIGStartInfo
            {
                StartSuspended = true
            };

            IWorkItemsGroup wig = stp.CreateWorkItemsGroup(10, wigStartInfo);

            wig.QueueWorkItem(new WorkItemCallback(this.DoWork));

            Assert.IsFalse(wig.WaitForIdle(200));

            wig.Start();

            Assert.IsFalse(wig.WaitForIdle(200));

            stp.Start();

            Assert.IsTrue(wig.WaitForIdle(5000), "WIG is not idle");
            Assert.IsTrue(stp.WaitForIdle(5000), "STP is not idle");
        }

        [TestMethod]
        public void TwoWIGsStartSuspended()
        {
            STP stp = new STP();

            WIGStartInfo wigStartInfo = new WIGStartInfo
            {
                StartSuspended = true
            };

            IWorkItemsGroup wig1 = stp.CreateWorkItemsGroup(10, wigStartInfo);
            IWorkItemsGroup wig2 = stp.CreateWorkItemsGroup(10, wigStartInfo);

            wig1.QueueWorkItem(new WorkItemCallback(this.DoWork));
            wig2.QueueWorkItem(new WorkItemCallback(this.DoWork));

            Assert.IsFalse(wig1.WaitForIdle(200));
            Assert.IsFalse(wig2.WaitForIdle(200));

            wig1.Start();

            Assert.IsTrue(wig1.WaitForIdle(200));
            Assert.IsFalse(wig2.WaitForIdle(200));

            wig2.Start();

            Assert.IsTrue(wig1.WaitForIdle(0));
            Assert.IsTrue(wig2.WaitForIdle(200));
        }


        private object DoWork(object state)
        {
            Thread.Sleep(100);
            return null;
        }

    }
}
