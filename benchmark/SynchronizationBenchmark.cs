using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BenchmarkDotNet.Attributes;

namespace PerfDS
{
    public class Synchronization
    {
        [Params(1000)]
        public int N;

        [Benchmark]
        public long Sum1000()
        {
            int sum = 0;
            for (int i = 0; i < N; ++i)
            {
                sum += 1;
            }

            return sum;
        }

        [Benchmark]
        public long Atomic1000()
        {
            int sum = 0;
            for (int i = 0; i < N; ++i)
            {
                Interlocked.Increment(ref sum);
            }

            return sum;
        }

        [Benchmark]
        public long MutexUncontended1000()
        {
            int sum = 0;
            var obj = new object();

            for (int i = 0; i < N; ++i)
            {
                lock(obj)
                {
                    sum += 1;
                }
            }

            return sum;
        }
    }
}