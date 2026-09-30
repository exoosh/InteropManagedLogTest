using System;
using System.Runtime.InteropServices;

internal static class NativeMethods
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void LogCallback(int step, int item, IntPtr message);

    [DllImport("interop", EntryPoint = "interop_add", CallingConvention = CallingConvention.Cdecl)]
    internal static extern int InteropAdd(int left, int right, LogCallback log);
}

internal static class Program
{
    private static int Main()
    {
        const int expected = 42;
        Exception? callbackException = null;

        void Log(int step, int item, IntPtr message)
        {
            try
            {
                var text = Marshal.PtrToStringUTF8(message) ?? "<null>";
                Console.WriteLine($"Native log: step {step}, item {item}, message: {text}");
            }
            catch (Exception exception)
            {
                callbackException ??= exception;
            }
        }

        NativeMethods.LogCallback callback = Log;
        var actual = NativeMethods.InteropAdd(19, 23, callback);
        GC.KeepAlive(callback);

        if (callbackException is not null)
        {
            Console.Error.WriteLine($"Native callback failed: {callbackException.Message}");
            return 1;
        }

        if (actual != expected)
        {
            Console.Error.WriteLine($"Native call returned {actual}; expected {expected}.");
            return 1;
        }

        Console.WriteLine($"Native interop succeeded: 19 + 23 = {actual}");
        return 0;
    }
}
