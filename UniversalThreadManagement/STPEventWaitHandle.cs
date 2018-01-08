using System;
using System.Threading;

namespace UniversalThreadManagement.Internal
{
    internal static class STPEventWaitHandle
    {
        public const int WaitTimeout = Timeout.Infinite;

        internal static bool WaitAll(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext) => WaitHandle.WaitAll(waitHandles, millisecondsTimeout, exitContext);

        internal static int WaitAny(WaitHandle[] waitHandles) => WaitHandle.WaitAny(waitHandles);

        internal static int WaitAny(WaitHandle[] waitHandles, int millisecondsTimeout, bool exitContext) => WaitHandle.WaitAny(waitHandles, millisecondsTimeout, exitContext);

        internal static bool WaitOne(WaitHandle waitHandle, int millisecondsTimeout, bool exitContext) => waitHandle.WaitOne(millisecondsTimeout, exitContext);
    }
}