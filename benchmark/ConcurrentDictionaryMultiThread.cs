using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace PerfDS
{
    public class ConcurrentDictionaryMultiThread
    {
        [Params(100000)]
        public int N;

        [Params(1, 2, 4, 8)]
        public int NumThreads;

        [GlobalSetup]
        public void Setup()
        {
            // Adding all the data beforehand
            // so that multiple invocations of benchmark fn
            // work on similar ConcurrentDictionary.
            // we could have used IterationSetup/Cleanup but that plays well only with > 100 msec benchmarks.
            for (int i = 0; i < N; ++i)
            {
                cdict.TryAdd(i, i);
            }
        }

        [Benchmark]
        public async Task<long> ConcurrentDictionaryAddGet()
        {
            var tasks = new List<Task<long>>(NumThreads);
            for (int i = 0; i < NumThreads; ++i)
            {
                tasks.Add(Task.Run(() => {
                    long internal_sum = 0;
                    int v = 0;

                    for (int j = 0; j < N; ++j)
                    {
                        cdict.TryAdd(j, j);
                        cdict.TryGetValue(j, out v);
                        internal_sum += v;
                    }

                    return internal_sum;
                }));
            }

            await Task.WhenAll(tasks);

            long sum = 0;
            foreach (var t in tasks)
            {
                sum += t.Result;
            }

            return sum;
        }

        ConcurrentDictionary<int, int> cdict = new ConcurrentDictionary<int, int>();
    }
}
