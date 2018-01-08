using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversalThreadManagement.Test;
#if NETCOREAPP2_0


using System;
#else
using NUnit.Framework;
using TestBase=System.Object;
#endif

namespace WorkItemsGroupTests
{
    [TestClass]
    [TestCategory("TestWIGFuncT")]
    public class WIGFuncTTests
    {
        private STP _stp;
        private IWorkItemsGroup _wig;

        [TestInitialize]
        public void Init()
        {
            _stp = new STP();
            _wig = _stp.CreateWorkItemsGroup(10);
        }

        [TestCleanup]
        public void Fini()
        {
            _stp.Shutdown();
        }

        [TestMethod]
        public void FuncT0()
        {
            IWorkItemResult<int> wir = _wig.QueueWorkItem(new Func<int>(MaxInt));

            int result = wir.GetResult();

            Assert.AreEqual(result, int.MaxValue);
        }

        [TestMethod]
        public void FuncT1()
        {
            IWorkItemResult<bool> wir = _wig.QueueWorkItem(new Func<bool, bool>(Not), true);

            bool result = wir.Result;

            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void FuncT2()
        {
            IWorkItemResult<string> wir = _wig.QueueWorkItem(new Func<string, string, string>(string.Concat), "ABC", "xyz");

            string result = wir.Result;

            Assert.AreEqual(result, "ABCxyz");
        }

        [TestMethod]
        public void FuncT3()
        {
            IWorkItemResult<string> wir = _wig.QueueWorkItem(new Func<string, int, int, string>(Substring), "ABCDEF", 1, 2);

            string result = wir.Result;

            Assert.AreEqual(result, "BC");
        }

        [TestMethod]
        public void FuncT4()
        {
            int[] numbers = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            IWorkItemResult<int[]> wir = _wig.QueueWorkItem(new Func<int[], int, int, int, int[]>(Subarray), numbers, 1, 2, 3);

            int[] result = wir.Result;

            CollectionAssert.AreEqual(result, new int[] { 2, 3, 2, 3, 2, 3, });
        }

        private int MaxInt()
        {
            return int.MaxValue;
        }

        private bool Not(bool flag)
        {
            return !flag;
        }

        private string Substring(string s, int startIndex, int length)
        {
            return s.Substring(startIndex, length);
        }

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
