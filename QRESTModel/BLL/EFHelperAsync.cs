using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace QRESTModel.BLL
{
    public static class EFHelperAsync
    {
        /// <summary>
        /// This method was added to support async calls to stored procedures
        /// https://stackoverflow.com/questions/34227749/asynchronous-methods-not-available-for-objectresultt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<List<T>> ToListAsync<T>(this IDbAsyncEnumerable<T> source, CancellationToken cancellationToken)
        {
            TaskCompletionSource<List<T>> tcs = new TaskCompletionSource<List<T>>();
            List<T> list = new List<T>();
            ForEachAsync<T>(source.GetAsyncEnumerator(), new Action<T>(list.Add), cancellationToken).ContinueWith((Action<Task>)(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException((IEnumerable<Exception>)t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(list);
            }), TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        public static Task<List<T>> ToListAsync<T>(this IDbAsyncEnumerable<T> source)
        {
            return ToListAsync<T>(source, CancellationToken.None);
        }

        private static async Task ForEachAsync<T>(IDbAsyncEnumerator<T> enumerator, Action<T> action, CancellationToken cancellationToken)
        {
            using (enumerator)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (await System.Data.Entity.Utilities.TaskExtensions.WithCurrentCulture<bool>(enumerator.MoveNextAsync(cancellationToken)))
                {
                    Task<bool> moveNextTask;
                    do
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        T current = enumerator.Current;
                        moveNextTask = enumerator.MoveNextAsync(cancellationToken);
                        action(current);
                    }
                    while (await System.Data.Entity.Utilities.TaskExtensions.WithCurrentCulture<bool>(moveNextTask));
                }
            }
        }
    }
}
