using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// Tests for QueueWorkItem.
    /// </summary>
    [TestClass]
    [TestCategory("TestQueueWorkItem")]
    public class QueueWorkItemTests
    {
        private STP _stp;

        [TestInitialize]
        public void Initialize() => _stp = new STP();

        [TestCleanup]
        public void Cleanup() => _stp.Shutdown();

        [TestMethod]
        public void TestQueueWorkItemCall() => QueueWorkItemHelper.TestQueueWorkItemCall(_stp);

        [TestMethod]
        public void TestQueueWorkItemCallPrio() => QueueWorkItemHelper.TestQueueWorkItemCallPrio(_stp);

        [TestMethod]
        public void TestQueueWorkItemCallStat() => QueueWorkItemHelper.TestQueueWorkItemCallStat(_stp);

        [TestMethod]
        public void TestQueueWorkItemCallStatPrio() => QueueWorkItemHelper.TestQueueWorkItemCallStatPrio(_stp);

        [TestMethod]
        public void TestQueueWorkItemCallStatPost() => QueueWorkItemHelper.TestQueueWorkItemCallStatPost(_stp);

        [TestMethod]
        public void TestQueueWorkItemCallStatPostPrio() => QueueWorkItemHelper.TestQueueWorkItemCallStatPostPrio(_stp);

        [TestMethod]
        public void TestQueueWorkItemCallStatPostPflg() => QueueWorkItemHelper.TestQueueWorkItemCallStatPostPflg(_stp);

        [TestMethod]
        public void TestQueueWorkItemCallStatPostPflgPrio() => QueueWorkItemHelper.TestQueueWorkItemCallStatPostPflgPrio(_stp);

        [TestMethod]
        public void TestQueueWorkItemInfoCall() => QueueWorkItemHelper.TestQueueWorkItemInfoCall(_stp);

        [TestMethod]
        public void TestQueueWorkItemInfoCallStat() => QueueWorkItemHelper.TestQueueWorkItemInfoCallStat(_stp);

        [TestMethod]
        public void TestQueueWorkItemActionT0()
        {
            int result = 0;

            _stp.QueueWorkItem((Action)(() => result = int.MaxValue)).GetResult();

            Assert.AreEqual(result, int.MaxValue);
        }

        [TestMethod]
        public void TestQueueWorkItemActionT1()
        {
            bool result = true;

            _stp.QueueWorkItem((Action<bool>)(flag => result = !flag), true).GetResult();

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void TestQueueWorkItemActionT2()
        {
            string result = null;

            _stp.QueueWorkItem((Action<string, string>)((s1, s2) => result = s1 + s2), "ABC", "xyz").GetResult();

            Assert.AreEqual(result, "ABCxyz");
        }

        [TestMethod]
        public void TestQueueWorkItemActionT3()
        {
            string result = null;

            _stp.QueueWorkItem((Action<string, int, int>)((string s, int startIndex, int length) => result = s.Substring(startIndex, length)), "ABCDEF", 1, 2).GetResult();

            Assert.AreEqual(result, "BC");
        }

        [TestMethod]
        public void TestQueueWorkItemActionT4()
        {
            int[] result = new int[] { };

            _stp.QueueWorkItem((int[] numbers, int startIndex, int length, int repeat) =>
            {
                int[] _result = new int[length * repeat];
                for (int i = 0; i < repeat; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        _result[i * length + j] = numbers[startIndex + j];
                    }
                }

                result = _result;
            }, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, 1, 2, 3).GetResult();

            CollectionAssert.AreEqual(result as ICollection, new int[] { 2, 3, 2, 3, 2, 3, });
        }
    }
}
