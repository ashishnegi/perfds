using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace PerfDS
{
    public class ConcurrentAsyncQueueBenchmark
    {
        [Params(100, 10000)]
        public int N;

        [Benchmark]
        public long ConcurrentQueueEnqueueDequeue()
        {
            int res = 0;
            long sum = 0;

            ConcurrentQueue<int> concQueue = new ConcurrentQueue<int>();

            for (int i = 0; i < N; ++i)
            {
                concQueue.Enqueue(i);
                concQueue.TryDequeue(out res);

                sum += res;
            }

            return sum;
        }

        [Benchmark]
        async public Task<long> ConcurrentAsyncQueueEnqueueDequeue()
        {
            using (var asyncQueue = new ConcurrentAsyncQueue<int>())
            {
                long sum = 0;
                for (int i = 0; i < N; ++i)
                {
                    asyncQueue.Enqueue(i);
                    sum += await asyncQueue.DequeueAsync();
                }
                return sum;
            }
        }


        [Benchmark]
        public long BlockingCollectionQueueEnqueueDequeue()
        {
            using (var asyncQueue = new BlockingCollection<int>())
            {
                long sum = 0;
                for (int i = 0; i < N; ++i)
                {
                    asyncQueue.Add(i);
                    sum += asyncQueue.Take();
                }
                return sum;
            }
        }
    }
}