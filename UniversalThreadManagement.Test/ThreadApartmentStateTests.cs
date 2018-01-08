using System.Threading;

using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// Summary description for TestThreadApartment.
    /// </summary>
    [TestClass]
    [TestCategory("TestThreadApartment")]
    public class ThreadApartmentStateTests
    {
        [TestMethod]
        public void TestSTA() => CheckApartmentState(ApartmentState.STA);

        [TestMethod]
        public void TestMTA() => CheckApartmentState(ApartmentState.MTA);

        private static void CheckApartmentState(ApartmentState requestApartmentState)
        {
            STPStartInfo stpStartInfo = new STPStartInfo
            {
                ApartmentState = requestApartmentState
            };

            STP stp = new STP(stpStartInfo);

            IWorkItemResult<ApartmentState> wir = stp.QueueWorkItem(() => GetCurrentThreadApartmentState());

            ApartmentState resultApartmentState = wir.GetResult();

            stp.WaitForIdle();

            Assert.AreEqual(requestApartmentState, resultApartmentState);
        }

        private static ApartmentState GetCurrentThreadApartmentState()
        {
            return Thread.CurrentThread.GetApartmentState();
        }
    }
}