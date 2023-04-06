using System;
using System.Threading;
using System.Threading.Tasks;

namespace _4People.Extensions
{
    public static class SemaphoreSlimExtensions
    {
        public static void WaitHandle(this SemaphoreSlim slimLocker, Action action)
        {
            slimLocker.Wait();
            try
            {
                action();
            } finally
            {
                slimLocker.Release();
            }
        }

        public static async Task WaitHandleAsync(this SemaphoreSlim slimLocker, Func<Task> action)
        {
            await slimLocker.WaitAsync();
            try
            {
                action();
            }
            finally
            {
                slimLocker.Release();
            }
        }
    }
}