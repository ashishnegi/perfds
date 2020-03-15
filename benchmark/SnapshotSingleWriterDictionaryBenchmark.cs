using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace PerfDS
{
    public class DictionaryCompareAdd
    {
        [Params(10000, 100000)]
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

    public class DictionaryCompareGet
    {
        [Params(10000, 100000)]
        public int N;

        Dictionary<int, int> dict = new Dictionary<int, int>();
        ConcurrentDictionary<int, int> cdict = new ConcurrentDictionary<int, int>();
        SnapshotSingleWriterDictionary<int, int> sdict = new SnapshotSingleWriterDictionary<int, int>();

        [GlobalSetup]
        public void Setup()
        {
            for (int i = 0; i < N; ++i)
            {
                dict.Add(i, i);
                cdict.GetOrAdd(i, i);
                sdict.Add(i, i);
            }
        }

        [Benchmark]
        public long SystemDictionaryGetOnly()
        {
            long sum = 0;
            for (int i = 0; i < N; ++i)
            {
                int v = 0;
                dict.TryGetValue(i, out v);
                sum += v;
            }

            return sum;
        }

        [Benchmark]
        public long ConcurrentDictionaryGetOnly()
        {
            long sum = 0;
            for (int i = 0; i < N; ++i)
            {
                int v = 0;
                cdict.TryGetValue(i, out v);
                sum += v;
            }

            return sum;
        }

        [Benchmark]
        public long SnapshotDictionaryGetOnly()
        {
            long sum = 0;
            for (int i = 0; i < N; ++i)
            {
                int v = 0;
                sdict.TryGetValue(i, out v);
                sum += v;
            }

            return sum;
        }
    }
}
