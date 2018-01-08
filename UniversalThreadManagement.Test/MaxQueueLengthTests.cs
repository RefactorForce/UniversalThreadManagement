using System;
using System.Threading;
using UniversalThreadManagement;
using STP = UniversalThreadManagement.SmartThreadPool;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace STPTests
{
    [TestClass]
    [TestCategory("TestMaxQueueLength")]
    public class MaxQueueLengthTests
    {
        [TestMethod]
        public void QueueWorkItem_WhenMaxIsNull_Queues()
        {
            var info = new STPStartInfo
            {
                MaxQueueLength = null,
            };
            var pool = new STP(info);
            pool.Start();
            var workItem = pool.QueueWorkItem<object>(ReturnNull);

            // If rejected, an exception would have been thrown instead.
            Assert.IsTrue(workItem.GetResult() == null);
        }

        [TestMethod]
        public void QueueWorkItem_WhenMaxIsSet_ThrowsExceptionWhenHit()
        {
            Assert.ThrowsException<QueueRejectedException>(() =>
            {

                var info = new STPStartInfo
                {
                    MaxQueueLength = 1,
                    MinWorkerThreads = 1,
                    MaxWorkerThreads = 1,
                };
                var pool = new STP(info);
                pool.Start();

                try
                {
                    pool.QueueWorkItem(SleepForOneSecond); // Taken by waiter immediately. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // No waiters available, pool at max threads. Queued.
                }
                catch (QueueRejectedException e)
                {
                    throw new Exception("Caught QueueRejectedException too early: ", e);
                }

                // No waiters available, queue is at max (1). Throws.
                pool.QueueWorkItem(SleepForOneSecond);
            });
        }

        [TestMethod]
        public void QueueWorkItem_WhenBiggerMaxIsSet_ThrowsExceptionWhenHit()
        {
            new Thread(() => Assert.ThrowsException<QueueRejectedException>(() =>
            {
                var info = new STPStartInfo
                {
                    MaxQueueLength = 5,
                    MinWorkerThreads = 5,
                    MaxWorkerThreads = 10,
                };
                var pool = new STP(info);
                pool.Start();

                try
                {
                    // Pool starts with 5 available waiters.

                    pool.QueueWorkItem(SleepForOneSecond); // Taken by waiter immediately. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // Taken by waiter immediately. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // Taken by waiter immediately. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // Taken by waiter immediately. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // Taken by waiter immediately. Not queued.

                    pool.QueueWorkItem(SleepForOneSecond); // New thread created, takes work item. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // New thread created, takes work item. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // New thread created, takes work item. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // New thread created, takes work item. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // New thread created, takes work item. Not queued.

                    pool.QueueWorkItem(SleepForOneSecond); // No waiters available. Queued.
                    pool.QueueWorkItem(SleepForOneSecond); // No waiters available. Queued.
                    pool.QueueWorkItem(SleepForOneSecond); // No waiters available. Queued.
                    pool.QueueWorkItem(SleepForOneSecond); // No waiters available. Queued.
                    pool.QueueWorkItem(SleepForOneSecond); // No waiters available. Queued.
                }
                catch (QueueRejectedException e)
                {
                    throw new Exception("Caught QueueRejectedException too early: ", e);
                }

                // All threads are busy, and queue is at its max. Throws.
                pool.QueueWorkItem(SleepForOneSecond);
            })).Start();
        }

        [TestMethod]
        public void QueueWorkItem_WhenQueueMaxLengthZero_RejectsInsteadOfQueueing()
        {
            new Thread(() => Assert.ThrowsException<QueueRejectedException>(() =>
            {
                var info = new STPStartInfo
                {
                    MaxQueueLength = 0,
                    MinWorkerThreads = 2,
                    MaxWorkerThreads = 2,
                };
                var pool = new STP(info);
                pool.Start();

                try
                {
                    pool.QueueWorkItem(SleepForOneSecond); // Taken by waiter immediately. Not queued.
                    pool.QueueWorkItem(SleepForOneSecond); // Taken by waiter immediately. Not queued.
                }
                catch (QueueRejectedException e)
                {
                    throw new Exception("Caught QueueRejectedException too early: ", e);
                }

                pool.QueueWorkItem(SleepForOneSecond);
            })).Start();
        }

        private object ReturnNull()
        {
            return null;
        }

        private void SleepForOneSecond()
        {
            Thread.Sleep(1000);
        }

        [TestMethod]
        public void StpStartInfo_WithNegativeMaxQueueLength_FailsValidation()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {

                var info = new STPStartInfo
                {
                    MaxQueueLength = -1,
                };
                new STP(info);
            });
        }

        [TestMethod]
        public void StpStartInfo_WithZeroMaxQueueLength_IsAllowed()
        {
            var info = new STPStartInfo
            {
                MaxQueueLength = 0,
            };
            var pool = new STP(info);
            pool.Start();
            Assert.IsTrue(0 == pool.STPStartInfo.MaxQueueLength);
        }

        [TestMethod]
        public void SetMaxQueueLength_FromNonZeroValueToZero_DisablesQueueing()
        {
            Assert.ThrowsException<QueueRejectedException>(() =>
            {

                var info = new STPStartInfo
                {
                    MinWorkerThreads = 1,
                    MaxWorkerThreads = 1,
                    MaxQueueLength = 1,
                };

                var pool = new STP(info);
                pool.Start();

                try
                {
                    pool.QueueWorkItem(SleepForOneSecond); // Picked up by waiter.
                    pool.QueueWorkItem(SleepForOneSecond); // Queued.
                }
                catch (QueueRejectedException e)
                {
                    throw new Exception("Caught QueueRejectedException too early: ", e);
                }

                try
                {
                    pool.QueueWorkItem(SleepForOneSecond);
                }
                catch (QueueRejectedException)
                {
                    // Expected
                    Assert.IsTrue(true);
                }

                pool.MaxQueueLength = 0;
                Thread.Sleep(2100); // Let the work items complete.

                try
                {
                    pool.QueueWorkItem(SleepForOneSecond); // Picked up by waiter.
                }
                catch (QueueRejectedException e)
                {
                    throw new Exception("Caught QueueRejectedException too early: ", e);
                }

                pool.QueueWorkItem(SleepForOneSecond); // Rejected (max queue length is zero).
            });
        }

        [TestMethod]
        public void SetMaxQueueLength_FromNullToZero_DisablesQueueing()
        {
            Assert.ThrowsException<QueueRejectedException>(() =>
            {

                var info = new STPStartInfo
                {
                    MinWorkerThreads = 1,
                    MaxWorkerThreads = 1,
                };

                var pool = new STP(info);
                pool.Start();

                try
                {
                    pool.QueueWorkItem(SleepForOneSecond); // Picked up by waiter.
                    pool.QueueWorkItem(SleepForOneSecond); // Queued.
                }
                catch (QueueRejectedException e)
                {
                    throw new Exception("Caught QueueRejectedException too early: ", e);
                }

                pool.MaxQueueLength = 0;
                Thread.Sleep(2100); // Let the work items complete.

                try
                {
                    pool.QueueWorkItem(SleepForOneSecond); // Picked up by waiter.
                }
                catch (QueueRejectedException e)
                {
                    throw new Exception("Caught QueueRejectedException too early: ", e);
                }

                pool.QueueWorkItem(SleepForOneSecond); // Rejected (max queue length is zero).
            });
        }

        [TestMethod]
        public void SetMaxQueueLength_IncreasedFromZero_AllowsLargerQueue()
        {
            var info = new STPStartInfo
            {
                MinWorkerThreads = 1,
                MaxWorkerThreads = 1,
                MaxQueueLength = 0,
            };

            var pool = new STP(info);
            pool.Start();

            try
            {
                pool.QueueWorkItem(SleepForOneSecond); // Picked up by waiter.
            }
            catch (QueueRejectedException e)
            {
                throw new Exception("Caught QueueRejectedException too early: ", e);
            }

            try
            {
                pool.QueueWorkItem(SleepForOneSecond);
            }
            catch (QueueRejectedException)
            {
                // Expected
                Assert.IsTrue(true);
            }

            pool.MaxQueueLength = 1;

            // Don't wait for worker item to complete, the queue should have immediately increased its allowance.

            var workItem = pool.QueueWorkItem<object>(ReturnNull);

            // If rejected, an exception would have been thrown instead.
            Assert.IsTrue(workItem.GetResult() == null);
        }
    }
}
