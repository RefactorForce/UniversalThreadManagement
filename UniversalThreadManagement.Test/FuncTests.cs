using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// Summary description for TestCancel.
    /// </summary>
    [TestClass]
    [TestCategory("TestFuncT")]
    public class FuncTests
    {
        private STP _stp;

        [TestInitialize]
        public void Init() => _stp = new STP();

        [TestCleanup]
        public void Fini() => _stp.Shutdown();

        [TestMethod]
        public void FuncT0()
        {
            IWorkItemResult<int> wir = _stp.QueueWorkItem(new Func<int>(MaxInt));

            int result = wir.GetResult();

            Assert.AreEqual(result, int.MaxValue);
        }

        [TestMethod]
        public void FuncT1()
        {
            IWorkItemResult<bool> wir = _stp.QueueWorkItem(new Func<bool, bool>(Not), true);

            bool result = wir.Result;

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void FuncT2()
        {
            IWorkItemResult<string> wir = _stp.QueueWorkItem(new Func<string, string, string>(string.Concat), "ABC", "xyz");

            string result = wir.Result;

            Assert.AreEqual(result, "ABCxyz");
        }

        [TestMethod]
        public void FuncT3()
        {
            IWorkItemResult<string> wir = _stp.QueueWorkItem(new Func<string, int, int, string>(Substring), "ABCDEF", 1, 2);

            string result = wir.Result;

            Assert.AreEqual(result, "BC");
        }

        [TestMethod]
        public void FuncT4()
        {
            int[] numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            IWorkItemResult<int[]> wir = _stp.QueueWorkItem(new Func<int[], int, int, int, int[]>(Subarray), numbers, 1, 2, 3);

            int[] result = wir.Result;

            CollectionAssert.AreEqual(result, new int[] { 2, 3, 2, 3, 2, 3, });
        }

        [TestMethod]
        public void FuncT()
        {
            STP stp = new STP();
            IWorkItemResult<int> wir =
                stp.QueueWorkItem(new Func<int, int>(Function), 1);

            int y = wir.GetResult();

            Assert.AreEqual(y, 2);

            try
            {
                wir.GetResult();
            }
            finally
            {
                stp.Shutdown();
            }
        }

        private int Function(int x) => x + 1;

        private int MaxInt() => int.MaxValue;

        private bool Not(bool flag) => !flag;

        private string Substring(string s, int startIndex, int length) => s.Substring(startIndex, length);

        private int[] Subarray(int[] numbers, int startIndex, int length, int repeat)
        {
            int[] result = new int[length * repeat];
            for (int i = 0; i < repeat; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    result[i * length + j] = numbers[startIndex + j];
                }
            }

            return result;
        }
    }
}
