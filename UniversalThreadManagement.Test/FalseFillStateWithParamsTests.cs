using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using UniversalThreadManagement;
using Guid = System.Guid;
using IntPtr = System.IntPtr;
using STP = UniversalThreadManagement.SmartThreadPool;

namespace STPTests
{
    [TestClass]
    public class FalseFillStateWithArgsTests
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
            _stp.WaitForIdle();
            _stp.Shutdown();
        }

        [TestMethod]
        public void STPActionT0()
        {
            IWorkItemResult wir = _stp.QueueWorkItem(Action0);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void STPActionT1()
        {
            IWorkItemResult wir = _stp.QueueWorkItem(Action1, 17);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void STPActionT2()
        {
            IWorkItemResult wir = _stp.QueueWorkItem(Action2, 'a', "bla bla");
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void STPActionT3()
        {
            char[] chars = new char[] { 'a', 'b' };
            object obj = new object();

            IWorkItemResult wir = _stp.QueueWorkItem(Action3, true, chars, obj);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void STPActionT4()
        {
            IntPtr p = new IntPtr(int.MaxValue);
            Guid guid = Guid.NewGuid();

            IPAddress ip = IPAddress.Parse("1.2.3.4");
            IWorkItemResult wir = _stp.QueueWorkItem(Action4, long.MinValue, p, ip, guid);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void STPFuncT0()
        {
            IWorkItemResult<int> wir = _stp.QueueWorkItem(new Func<int>(Func0));
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void STPFuncT1()
        {
            IWorkItemResult<bool> wir = _stp.QueueWorkItem(new Func<int, bool>(Func1), 17);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void STPFuncT2()
        {
            IWorkItemResult<string> wir = _stp.QueueWorkItem(new Func<char, string, string>(Func2), 'a', "bla bla");
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void STPFuncT3()
        {
            char[] chars = new char[] { 'a', 'b' };
            object obj = new object();

            IWorkItemResult<char> wir = _stp.QueueWorkItem(new Func<bool, char[], object, char>(Func3), true, chars, obj);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void STPFuncT4()
        {
            IntPtr p = new IntPtr(int.MaxValue);
            Guid guid = Guid.NewGuid();

            IPAddress ip = IPAddress.Parse("1.2.3.4");
            IWorkItemResult<IPAddress> wir = _stp.QueueWorkItem(new Func<long, IntPtr, IPAddress, Guid, IPAddress>(Func4), long.MinValue, p, ip, guid);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGActionT0()
        {
            IWorkItemResult wir = _wig.QueueWorkItem(Action0);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGActionT1()
        {
            IWorkItemResult wir = _wig.QueueWorkItem(Action1, 17);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGActionT2()
        {
            IWorkItemResult wir = _wig.QueueWorkItem(Action2, 'a', "bla bla");
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGActionT3()
        {
            char[] chars = new char[] { 'a', 'b' };
            object obj = new object();

            IWorkItemResult wir = _wig.QueueWorkItem(Action3, true, chars, obj);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGActionT4()
        {
            IntPtr p = new IntPtr(int.MaxValue);
            Guid guid = Guid.NewGuid();

            IPAddress ip = IPAddress.Parse("1.2.3.4");
            IWorkItemResult wir = _wig.QueueWorkItem(Action4, long.MinValue, p, ip, guid);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGFuncT0()
        {
            IWorkItemResult<int> wir = _wig.QueueWorkItem(new Func<int>(Func0));
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGFuncT1()
        {
            IWorkItemResult<bool> wir = _wig.QueueWorkItem(new Func<int, bool>(Func1), 17);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGFuncT2()
        {
            IWorkItemResult<string> wir = _wig.QueueWorkItem(new Func<char, string, string>(Func2), 'a', "bla bla");
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGFuncT3()
        {
            char[] chars = new char[] { 'a', 'b' };
            object obj = new object();

            IWorkItemResult<char> wir = _wig.QueueWorkItem(new Func<bool, char[], object, char>(Func3), true, chars, obj);
            Assert.IsNull(wir.State);
        }

        [TestMethod]
        public void WIGFuncT4()
        {
            IntPtr p = new IntPtr(int.MaxValue);
            Guid guid = Guid.NewGuid();

            IPAddress ip = IPAddress.Parse("1.2.3.4");
            IWorkItemResult<IPAddress> wir = _wig.QueueWorkItem(new Func<long, IntPtr, IPAddress, Guid, IPAddress>(Func4), long.MinValue, p, ip, guid);
            Assert.IsNull(wir.State);
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

        private int Func0() => 0;

        private bool Func1(int p1) => true;

        private string Func2(char p1, string p2) => "value";

        private char Func3(bool p1, char[] p2, object p3) => 'z';

        private IPAddress Func4(long p1, IntPtr p2, IPAddress p3, Guid p4) => IPAddress.None;
    }
}
