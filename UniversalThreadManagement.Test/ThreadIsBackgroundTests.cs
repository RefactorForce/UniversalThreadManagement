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
    /// Summary description for TestThreadIsBackground.
    /// </summary>
    [TestClass]
    [TestCategory("TestThreadIsBackground")]
    public class ThreadIsBackgroundTests
    {
        [TestMethod]
        public void TestIsBackground() => CheckIsBackground(true);

        [TestMethod]
        public void TestNotIsBackground() => CheckIsBackground(false);

        private static void CheckIsBackground(bool isBackground)
        {
            STPStartInfo stpStartInfo = new STPStartInfo
            {
                AreThreadsBackground = isBackground
            };

            STP stp = new STP(stpStartInfo);

            IWorkItemResult<bool> wir = stp.QueueWorkItem(() => GetCurrentThreadIsBackground());

            bool resultIsBackground = wir.GetResult();

            stp.WaitForIdle();

            Assert.AreEqual(isBackground, resultIsBackground);
        }

        private static bool GetCurrentThreadIsBackground()
        {
            return Thread.CurrentThread.IsBackground;
        }
    }
}