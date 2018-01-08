using System.Threading;

using System;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// Summary description for TestParallelMethods.
    /// </summary>
    [TestClass]
    [TestCategory("TestParallelMethods")]
    public class ParallelMethodsTests
    {
        [TestMethod]
        public void TestJoin()
        {
            STP stp = new STP();

            SafeCounter sc = new SafeCounter();

            stp.Join(
                sc.Increment,
                sc.Increment,
                sc.Increment);

            Assert.AreEqual(3, sc.Counter);

            for (int j = 0; j < 10; j++)
            {
                sc.Reset();

                Action[] actions = new Action[1000];
                for (int i = 0; i < actions.Length; i++)
                {
                    actions[i] = sc.Increment;
                }

                stp.Join(actions);

                Assert.AreEqual(actions.Length, sc.Counter);
            }

            stp.Shutdown();
        }

        private class SafeCounter
        {
            private int _counter;

            public void Increment()
            {
                Interlocked.Increment(ref _counter);
            }

            public int Counter
            {
                get { return _counter; }
            }

            public void Reset()
            {
                _counter = 0;
            }
        }

        [TestMethod]
        public void TestChoice()
        {
            STP stp = new STP();

            int index = stp.Choice(
                () => Thread.Sleep(1000),
                () => Thread.Sleep(1500),
                () => Thread.Sleep(500));

            Assert.AreEqual(2, index);

            index = stp.Choice(
                () => Thread.Sleep(300),
                () => Thread.Sleep(100),
                () => Thread.Sleep(200));

            Assert.AreEqual(1, index);

            stp.Shutdown();
        }

        [TestMethod]
        public void TestPipe()
        {
            SafeCounter sc = new SafeCounter();
            STP stp = new STP();

            stp.Pipe(
                sc,
                sc1 => { if (sc.Counter == 0) { sc1.Increment(); } },
                sc1 => { if (sc.Counter == 1) { sc1.Increment(); } },
                sc1 => { if (sc.Counter == 2) { sc1.Increment(); } }
                );

            Assert.AreEqual(3, sc.Counter);

            stp.Shutdown();
        }
    }
}
