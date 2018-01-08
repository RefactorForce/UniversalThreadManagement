using System;
using System.Threading;

using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    /// <summary>
    /// </summary>
    [TestClass]
    [TestCategory("TestThreadsCreate")]
    public class ThreadsCreateTests
    {
        private bool _initSuccess;
        private bool _workItemSuccess;
        private bool _termSuccess;

        private void ClearResults()
        {
            _initSuccess = false;
            _workItemSuccess = false;
            _termSuccess = false;
        }

        [TestMethod]
        public void TestThreadsEvents()
        {
            ClearResults();

            STP stp = new STP();

            stp.OnThreadInitialization += OnInitialization;
            stp.OnThreadTermination += OnTermination;

            stp.QueueWorkItem(new WorkItemCallback(DoSomeWork), null);

            stp.WaitForIdle();
            stp.Shutdown();

            Thread.Sleep(500); // Wait for the STP to shutdown.
            Assert.IsTrue(_initSuccess);
            Assert.IsTrue(_workItemSuccess);
            Assert.IsTrue(_termSuccess);
        }

        public void OnInitialization()
        {
            ThreadContextState.Current.Counter = 1234;
            _initSuccess = true;
        }

        private object DoSomeWork(object state)
        {
            int counter = ThreadContextState.Current.Counter;
            _workItemSuccess = (1234 == counter);

            ThreadContextState.Current.Counter = 1111;
            return 1;
        }

        public void OnTermination()
        {
            int counter = ThreadContextState.Current.Counter;
            _termSuccess = (1111 == counter);
        }


        // Can't run this test, StackOverflowException crashes the application and can't be catched and ignored
        //[TestMethod]
        public void NotTestThreadsMaxStackSize()
        {
            STPStartInfo stpStartInfo = new STPStartInfo()
            {
                MaxStackSize = 64 * 1024,
            };

            STP stp = new STP(stpStartInfo);
            stp.Start();

            IWorkItemResult<bool> wir = stp.QueueWorkItem(() => AllocateBufferOnStack(10 * 1024));

            bool result = wir.GetResult();
            Assert.IsTrue(result);

            wir = stp.QueueWorkItem(() => AllocateBufferOnStack(1000 * 1024));

            result = wir.GetResult();
            Assert.IsFalse(result);
        }

        private static unsafe bool AllocateBufferOnStack(int size)
        {
            try
            {
                byte* p = stackalloc byte[size];
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    internal class ThreadContextState
    {
        // Each thread will have its own ThreadContextState object
        [ThreadStatic]
        private static ThreadContextState _threadContextState;

        public int Counter { get; set; }

        // Static member so it can be used anywhere in code of the work item method
        public static ThreadContextState Current
        {
            get
            {
                // If the _threadContextState is null then it was not yet initialized
                // for this thread.
                if (null == _threadContextState)
                {
                    // Create a ThreadContextState object
                    _threadContextState = new ThreadContextState();
                }
                return _threadContextState;
            }
        }
    }
}
