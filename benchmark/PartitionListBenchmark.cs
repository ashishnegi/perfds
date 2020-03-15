using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace PerfDS
{
    public class ListCompare
    {
        [Params(10000, 1000000)]
        public int N;

        [Benchmark]
        public long SystemListAddAndIterate()
        {
            var l = new List<int>(N);

            for (int i = 0; i < N; ++i)
            {
                l.Add(i);
            }

            long sum = 0;
            foreach (var i in l)
            {
                sum += i;
            }

            if (sum != ((long)N * (N - 1)) / 2)
            {
                throw new Exception("Sum is not equal");
            }

            return sum;
        }

        [Benchmark]
        public long PartitionedListAddAndIterate()
        {
            var l = new PartitionedList<int>(N, 18000);

            for (int i = 0; i < N; ++i)
            {
                l.Append(i);
            }

            long sum = 0;
            foreach (var i in l)
            {
                sum += i;
            }

            if (sum != ((long)N * (N - 1)) / 2)
            {
                throw new Exception("Sum is not equal");
            }

            return sum;
        }

        [Benchmark]
        public int SystemListAddOnly()
        {
            var l = new List<int>(N);

            for (int i = 0; i < N; ++i)
            {
                l.Add(i);
            }

            return l.Count;
        }

        [Benchmark]
        public int PartitionedListAddOnly()
        {
            var l = new PartitionedList<int>(N, 18000);

            for (int i = 0; i < N; ++i)
            {
                l.Append(i);
            }

            return l.Count();
        }

        [Benchmark]
        public int SystemListAddOnlySizeUnknown()
        {
            var l = new List<int>();

            for (int i = 0; i < N; ++i)
            {
                l.Add(i);
            }

            return l.Count;
        }

        [Benchmark]
        public int PartitionedListAddOnlySizeUnknown()
        {
            var l = new PartitionedList<int>(0, 18000);

            for (int i = 0; i < N; ++i)
            {
                l.Append(i);
            }

            return l.Count();
        }

        [Benchmark]
        public int ArrayAddOnly()
        {
            var l = new int[N];
            for (int i = 0; i < N; ++i)
            {
                l[i] = i;
            }

            return N;
        }
    }

}
