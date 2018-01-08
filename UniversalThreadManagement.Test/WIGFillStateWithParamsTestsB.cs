using System;
using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UniversalThreadManagement.Test;
#if NETCOREAPP2_0


#else
using NUnit.Framework;
using TestBase=System.Object;
#endif
using System.Net;

namespace STPTests
{
    /// <summary>
    /// Summary description for TestFillStateWithParams.
    /// </summary>
    [TestClass]
    [TestCategory("TestFillStateWithParams")]
    public class TestFillStateWithParams
    {
        private STP _stp;

        [TestInitialize]
        public void Init()
        {
            STPStartInfo stpStartInfo = new STPStartInfo
            {
                FillStateWithArgs = true//FIXME:FillStateWithParams
            };
            _stp = new STP(stpStartInfo);
        }

        [TestCleanup]
        public void Fini()
        {
            _stp.WaitForIdle();
            _stp.Shutdown();
        }

        [TestMethod]
        public void ActionT0()
        {
            IWorkItemResult wir = _stp.QueueWorkItem(Action0);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void ActionT1()
        {
            IWorkItemResult wir = _stp.QueueWorkItem(Action1, 17);
            object[] args = wir.State as object[];

            Assert.IsNotNull(args);
            Assert.AreEqual(args.Length, 1);
            Assert.AreEqual(args[0], 17);
        }

        [TestMethod]
        public void ActionT2()
        {
            IWorkItemResult wir = _stp.QueueWorkItem(Action2, 'a', "bla bla");
            object[] args = wir.State as object[];

            Assert.IsNotNull(args);
            Assert.AreEqual(args.Length, 2);
            Assert.AreEqual(args[0], 'a');
            Assert.AreEqual(args[1], "bla bla");
        }

        [TestMethod]
        public void ActionT3()
        {
            char[] chars = new char[] { 'a', 'b' };
            object obj = new object();

            IWorkItemResult wir = _stp.QueueWorkItem(Action3, true, chars, obj);
            object[] args = wir.State as object[];

            Assert.IsNotNull(args);
            Assert.AreEqual(args.Length, 3);
            Assert.AreEqual(args[0], true);
            Assert.AreEqual(args[1], chars);
            Assert.AreEqual(args[2], obj);
        }

        [TestMethod]
        public void ActionT4()
        {
            IntPtr p = new IntPtr(int.MaxValue);
            Guid guid = Guid.NewGuid();

            IPAddress ip = IPAddress.Parse("1.2.3.4");
            IWorkItemResult wir = _stp.QueueWorkItem(Action4, long.MinValue, p, ip, guid);
            object[] args = wir.State as object[];

            Assert.IsNotNull(args);
            Assert.AreEqual(args.Length, 4);
            Assert.AreEqual(args[0], long.MinValue);
            Assert.AreEqual(args[1], p);
            Assert.AreEqual(args[2], ip);
            Assert.AreEqual(args[3], guid);
        }

        [TestMethod]
        public void FuncT0()
        {
            IWorkItemResult<int> wir = _stp.QueueWorkItem(new Func<int>(Func0));
            Assert.AreEqual(wir.State, null);
        }

        [TestMethod]
        public void FuncT1()
        {
            IWorkItemResult<bool> wir = _stp.QueueWorkItem(new Func<int, bool>(Func1), 17);
            object[] args = wir.State as object[];

            Assert.IsNotNull(args);
            Assert.AreEqual(args.Length, 1);
            Assert.AreEqual(args[0], 17);
        }

        [TestMethod]
        public void FuncT2()
        {
            IWorkItemResult<string> wir = _stp.QueueWorkItem(new Func<char, string, string>(Func2), 'a', "bla bla");
            object[] args = wir.State as object[];

            Assert.IsNotNull(args);
            Assert.AreEqual(args.Length, 2);
            Assert.AreEqual(args[0], 'a');
            Assert.AreEqual(args[1], "bla bla");
        }

        [TestMethod]
        public void FuncT3()
        {
            char[] chars = new char[] { 'a', 'b' };
            object obj = new object();

            IWorkItemResult<char> wir = _stp.QueueWorkItem(new Func<bool, char[], object, char>(Func3), true, chars, obj);
            object[] args = wir.State as object[];

            Assert.IsNotNull(args);
            Assert.AreEqual(args.Length, 3);
            Assert.AreEqual(args[0], true);
            Assert.AreEqual(args[1], chars);
            Assert.AreEqual(args[2], obj);
        }

        [TestMethod]
        public void FuncT4()
        {
            IntPtr p = new IntPtr(int.MaxValue);
            Guid guid = Guid.NewGuid();

            IPAddress ip = IPAddress.Parse("1.2.3.4");
            IWorkItemResult<IPAddress> wir = _stp.QueueWorkItem(new Func<long, IntPtr, IPAddress, Guid, IPAddress>(Func4), long.MinValue, p, ip, guid);

            object[] args = wir.State as object[];

            Assert.IsNotNull(args);
            Assert.AreEqual(args.Length, 4);
            Assert.AreEqual(args[0], long.MinValue);
            Assert.AreEqual(args[1], p);
            Assert.AreEqual(args[2], ip);
            Assert.AreEqual(args[3], guid);
        }


        private void Action0()
        {
        }

        private void Action1(int p1)
        {
        }

        private void Action2(char p1, string p2)
        {
        }

        private void Action3(bool p1, char[] p2, object p3)
        {
        }

        private void Action4(long p1, IntPtr p2, IPAddress p3, Guid p4)
        {
        }

        private int Func0()
        {
            return 0;
        }

        private bool Func1(int p1)
        {
            return true;
        }

        private string Func2(char p1, string p2)
        {
            return "value";
        }

        private char Func3(bool p1, char[] p2, object p3)
        {
            return 'z';
        }

        private IPAddress Func4(long p1, IntPtr p2, IPAddress p3, Guid p4)
        {
            return IPAddress.None;
        }
    }
}
