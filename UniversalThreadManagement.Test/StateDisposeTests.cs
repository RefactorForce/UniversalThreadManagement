using System;
using System.Threading;

using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UniversalThreadManagement.Test
{
    public class CallerState
    {
        public int Value { get; private set; }

        protected void IncValue() => ++Value;
    }

    public class NonDisposableCallerState : CallerState
    {
        public NonDisposableCallerState() => IncValue();
    }

    public class DisposableCallerState : CallerState, IDisposable
    {
        public DisposableCallerState() => IncValue();

        #region IDisposable Members

        public void Dispose() => IncValue();

        #endregion
    }


    /// <summary>
    /// Summary description for StateDisposeExample.
    /// </summary>
    [TestClass]
    [TestCategory("TestStateDispose")]
    public class TestStateDispose
    {
        /// <summary>
        /// Example of non disposable caller state
        /// </summary>
        [TestMethod]
        public void DisposeCallerState()
        {
            STPStartInfo stpStartInfo = new STPStartInfo
            {
                DisposeOfStateObjects = true
            };

            STP smartThreadPool = new STP(stpStartInfo);

            CallerState nonDisposableCallerState = new NonDisposableCallerState();
            CallerState disposableCallerState = new DisposableCallerState();

            IWorkItemResult wir1 =
                smartThreadPool.QueueWorkItem(
                new WorkItemCallback(this.DoSomeWork),
                nonDisposableCallerState);

            IWorkItemResult wir2 =
                smartThreadPool.QueueWorkItem(
                new WorkItemCallback(this.DoSomeWork),
                disposableCallerState);

            wir1.GetResult();
            Assert.AreEqual(1, nonDisposableCallerState.Value);

            wir2.GetResult();

            // Wait a little bit for the working thread to call dispose on the 
            // work item's state.
            smartThreadPool.WaitForIdle();

            Assert.AreEqual(2, disposableCallerState.Value);

            smartThreadPool.Shutdown();
        }

        /// <summary>
        /// Example of non disposable caller state
        /// </summary>
        [TestMethod]
        public void DontDisposeCallerState()
        {
            STPStartInfo stpStartInfo = new STPStartInfo
            {
                DisposeOfStateObjects = false
            };

            STP smartThreadPool = new STP(stpStartInfo);

            CallerState nonDisposableCallerState = new NonDisposableCallerState();
            CallerState disposableCallerState = new DisposableCallerState();

            IWorkItemResult wir1 =
                smartThreadPool.QueueWorkItem(
                new WorkItemCallback(this.DoSomeWork),
                nonDisposableCallerState);

            IWorkItemResult wir2 =
                smartThreadPool.QueueWorkItem(
                new WorkItemCallback(this.DoSomeWork),
                disposableCallerState);

            wir1.GetResult();
            bool success = (1 == nonDisposableCallerState.Value);

            wir2.GetResult();

            success = success && (1 == disposableCallerState.Value);

            smartThreadPool.Shutdown();

            Assert.IsTrue(success);
        }

        private object DoSomeWork(object state)
        {
            Thread.Sleep(1000);
            return 1;
        }
    }
}
