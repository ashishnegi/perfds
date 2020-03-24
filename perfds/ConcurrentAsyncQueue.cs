using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PerfDS
{
    public class ConcurrentAsyncQueue<T>
    {
        private readonly SemaphoreSlim waitSem;
        private readonly ConcurrentQueue<T> queue;

        public ConcurrentAsyncQueue()
        {
            waitSem = new SemaphoreSlim(0);
            queue = new ConcurrentQueue<T>();
        }

        public void Enqueue(T item)
        {
            queue.Enqueue(item);
            waitSem.Release();
        }

        public void EnqueueRange(IEnumerable<T> source)
        {
            var n = 0;
            foreach (var item in source)
            {
                queue.Enqueue(item);
                n++;
            }
            waitSem.Release(n);
        }

        public async Task<T> DequeueAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await waitSem.WaitAsync(cancellationToken);

            T item;
            if (queue.TryDequeue(out item))
            {
                return item;
            }

            Environment.FailFast("Expected item after WaitAsync finishes.");
            return default(T);
        }
    }

}
