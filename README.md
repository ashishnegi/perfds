# Perf data structures in C#

```
D:\gitrepos\perfds>set PATH=%PATH%;D:\tools\dotnet-sdk-2.2.108-win-x64\
code .

pushd test
dotnet test -c Release --filter ConcurrentEnqueAndDequeue
popd

pushd benchmark
dotnet run -c Release -- -f *Dequeue
popd
```

## PartitionedList
List which tries to optimally store the values in partitions.
This helps in
1. removing the need for large continuous memory.
2. does not allocate in LOH (large object heap).

## SnapshotSingleWriterDictionary
This in memory concurrent dictionary provides snapshot views for readers.
Constraints:
1. Only single writer can write at a time.
2. There is no background pruning of old values. To prune old values, you need to call API which will block snapshot view readers during pruning.
   Point reads and single writer will continue to work during pruning.

## ConcurrentAsyncQueue
You can dequeue items from queue without blocking.

## Good points
1. Used property based random input data testing to find bugs.
2. Used benchmarks to optimize the data structures. Still more work required. :)

## Todo
1. Improve enumeration in PartitionedList.
2. Add more functions in PartitionedList.
3. Benchmark SnapshotSingleWriterDictionary.
4. Add more functions in SnapshotSingleWriterDictionary.

```
BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


|                            Method |      N |       Mean |     Error |     StdDev |
|---------------------------------- |------- |-----------:|----------:|-----------:|
|           SystemListAddAndIterate |  10000 |  34.656 us | 0.2654 us |  0.2353 us |
|      PartitionedListAddAndIterate |  10000 |  81.394 us | 0.8049 us |  0.7135 us |
|                 SystemListAddOnly |  10000 |  15.316 us | 0.1874 us |  0.1753 us |
|            PartitionedListAddOnly |  10000 |  27.627 us | 0.4028 us |  0.3768 us |
|      SystemListAddOnlySizeUnknown |  10000 |  24.360 us | 0.3567 us |  0.3162 us |
| PartitionedListAddOnlySizeUnknown |  10000 |  27.718 us | 0.4295 us |  0.3586 us |
|                      ArrayAddOnly |  10000 |   6.419 us | 0.1241 us |  0.1477 us |
|           SystemListAddAndIterate | 100000 | 473.377 us | 9.5049 us | 11.3149 us |
|      PartitionedListAddAndIterate | 100000 | 858.465 us | 8.2485 us |  7.7157 us |
|                 SystemListAddOnly | 100000 | 279.695 us | 4.9558 us |  4.3932 us |
|            PartitionedListAddOnly | 100000 | 308.580 us | 5.5199 us |  4.8932 us |
|      SystemListAddOnlySizeUnknown | 100000 | 427.819 us | 5.9998 us |  5.6122 us |
| PartitionedListAddOnlySizeUnknown | 100000 | 306.309 us | 1.3961 us |  1.3059 us |
|                      ArrayAddOnly | 100000 | 191.030 us | 1.6156 us |  1.4322 us |

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


|                      Method |       N |         Mean |       Error |      StdDev |
|---------------------------- |-------- |-------------:|------------:|------------:|
|     SystemDictionaryAddOnly |   10000 |     289.1 us |     1.16 us |     1.08 us |
| ConcurrentDictionaryAddOnly |   10000 |   1,503.2 us |     2.77 us |     2.45 us |
|   SnapshotDictionaryAddOnly |   10000 |   7,310.1 us |   112.76 us |    94.16 us |
|     SystemDictionaryAddOnly | 1000000 |  32,803.1 us |   203.33 us |   169.79 us |
| ConcurrentDictionaryAddOnly | 1000000 | 233,504.5 us | 6,235.86 us | 5,833.03 us |
|   SnapshotDictionaryAddOnly | 1000000 | 866,842.0 us | 6,976.27 us | 6,525.60 us |


BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


|                      Method |       N |         Mean |      Error |     StdDev |
|---------------------------- |-------- |-------------:|-----------:|-----------:|
|     SystemDictionaryGetOnly |   10000 |     76.83 us |   0.404 us |   0.378 us |
| ConcurrentDictionaryGetOnly |   10000 |    108.16 us |   0.358 us |   0.335 us |
|   SnapshotDictionaryGetOnly |   10000 |    738.74 us |   5.214 us |   4.622 us |
|     SystemDictionaryGetOnly | 1000000 |  7,735.82 us |  51.994 us |  48.635 us |
| ConcurrentDictionaryGetOnly | 1000000 | 20,512.34 us | 308.474 us | 257.589 us |
|   SnapshotDictionaryGetOnly | 1000000 | 76,608.19 us | 146.169 us | 129.575 us |

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


|                                Method |     N |         Mean |      Error |     StdDev |
|-------------------------------------- |------ |-------------:|-----------:|-----------:|
|         ConcurrentQueueEnqueueDequeue |   100 |     2.158 us |  0.0509 us |  0.1501 us |
|    ConcurrentAsyncQueueEnqueueDequeue |   100 |     9.351 us |  0.1860 us |  0.5247 us |
| BlockingCollectionQueueEnqueueDequeue |   100 |    11.498 us |  0.2278 us |  0.5281 us |
|         ConcurrentQueueEnqueueDequeue | 10000 |   202.651 us |  4.0078 us |  8.4537 us |
|    ConcurrentAsyncQueueEnqueueDequeue | 10000 |   938.006 us | 18.6603 us | 53.5400 us |
| BlockingCollectionQueueEnqueueDequeue | 10000 | 1,128.594 us | 22.4870 us | 61.1776 us |


BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.201
  [Host]     : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT
  DefaultJob : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT


|               Method |    N |        Mean |     Error |   StdDev |
|--------------------- |----- |------------:|----------:|---------:|
|              Sum1000 | 1000 |    485.8 ns |  39.85 ns | 116.2 ns |
|           Atomic1000 | 1000 |  5,448.6 ns | 107.40 ns | 179.4 ns |
| MutexUncontended1000 | 1000 | 20,288.4 ns | 508.17 ns | 543.7 ns |

// * Warnings *
MultimodalDistribution
  Synchronization.Sum1000: Default -> It seems that the distribution is multimodal (mValue = 4.91)

> dotnet run  -c Release -- -m --filter *Dequeue

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


|                                Method |     N |         Mean |      Error |     StdDev |    Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------------------------- |------ |-------------:|-----------:|-----------:|---------:|------:|------:|----------:|
|         ConcurrentQueueEnqueueDequeue |   100 |     2.155 us |  0.0431 us |  0.0999 us |   0.1755 |     - |     - |     768 B |
|    ConcurrentAsyncQueueEnqueueDequeue |   100 |     9.326 us |  0.1834 us |  0.3988 us |   1.7242 |     - |     - |    7512 B |
| BlockingCollectionQueueEnqueueDequeue |   100 |    11.574 us |  0.2297 us |  0.6736 us |   0.2289 |     - |     - |    1056 B |
|         ConcurrentQueueEnqueueDequeue | 10000 |   203.541 us |  4.0292 us | 10.1823 us |        - |     - |     - |     768 B |
|    ConcurrentAsyncQueueEnqueueDequeue | 10000 |   903.440 us | 18.0253 us | 30.1163 us | 166.0156 |     - |     - |  720312 B |
| BlockingCollectionQueueEnqueueDequeue | 10000 | 1,063.805 us | 21.0520 us | 34.5890 us |        - |     - |     - |    1056 B |


```