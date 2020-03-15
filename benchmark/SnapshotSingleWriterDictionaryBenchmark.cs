using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace PerfDS
{
    public class DictionaryCompare
    {
        [Params(10000, 1000000)]
        public int N;

        [Benchmark]
        public long SystemDictionaryAddOnly()
        {
            var dict = new Dictionary<int, int>();

            for (int i = 0; i < N; ++i)
            {
                dict.Add(i, i);
            }

            return dict.Count;
        }

        [Benchmark]
        public long ConcurrentDictionaryAddOnly()
        {
            var dict = new ConcurrentDictionary<int, int>();

            for (int i = 0; i < N; ++i)
            {
                dict.TryAdd(i, i);
            }

            return dict.Count;
        }

        [Benchmark]
        public int SnapshotDictionaryAddOnly()
        {
            var dict = new SnapshotSingleWriterDictionary<int, int>();

            for (int i = 0; i < N; ++i)
            {
                dict.Add(i, i);
            }

            int value = -1;
            dict.TryGetValue(0, out value);
            return value;
        }
    }
}
