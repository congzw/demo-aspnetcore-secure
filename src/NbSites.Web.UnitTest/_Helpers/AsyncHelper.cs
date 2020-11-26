using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable CheckNamespace
namespace Common
{
    //this helper will prevent deadlocks and could be used within a try/catch block to get the exception raised by the AsyncMethod itself.
    public static class AsyncHelper
    {
        #region workround for the bad use case: blocking, deadlock, exception handling...

        //refs => https://www.ryadel.com/en/asyncutil-c-helper-class-async-method-sync-result-wait/

        //var t = AsyncMethod().GetAwaiter().GetResult();
        //var t = AsyncMethod().Result;
        //var t = AsyncMethod().Wait;

        //The problem of the above approaches lies in the fact that they all configure a synchronous wait on the execution thread.
        //This basically mean that the main thread cannot be doing any other work while it is synchronously waiting for the task to complete:
        //this is not only inefficient, but it will also pose a serious risk of causing a deadlock the main thread.

        #endregion


        private static readonly TaskFactory TaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        /// <summary>
        /// Executes an async Task method which has a void return value synchronously
        /// USAGE: AsyncHelper.RunSync(() => AsyncMethod());
        /// </summary>
        /// <param name="task">Task method to execute</param>
        public static void RunSync(Func<Task> task)
            => TaskFactory
                .StartNew(task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();

        /// <summary>
        /// Executes an async Task
        /// method which has a T return type synchronously
        /// USAGE: T result = AsyncHelper.RunSync(() => AsyncMethod());
        /// </summary>
        /// <typeparam name="TResult">Return Type</typeparam>
        /// <param name="task" T="method to execute">Task</param>
        /// <returns></returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> task)
            => TaskFactory
                .StartNew(task)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
    }
}
