# Perf data structures in C#

```
D:\gitrepos\perfds>set PATH=%PATH%;..\..\..\..\tools\dotnet-sdk-2.2.108-win-x64\
code .
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

## Good points
1. Used property based random input data testing to find bugs.
2. Used benchmarks to optimize the data structures.

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


|                      Method |       N |         Mean |       Error |      StdDev |
|---------------------------- |-------- |-------------:|------------:|------------:|
|     SystemDictionaryAddOnly |   10000 |     289.1 us |     1.16 us |     1.08 us |
| ConcurrentDictionaryAddOnly |   10000 |   1,503.2 us |     2.77 us |     2.45 us |
|   SnapshotDictionaryAddOnly |   10000 |   7,310.1 us |   112.76 us |    94.16 us |
|     SystemDictionaryAddOnly | 1000000 |  32,803.1 us |   203.33 us |   169.79 us |
| ConcurrentDictionaryAddOnly | 1000000 | 233,504.5 us | 6,235.86 us | 5,833.03 us |
|   SnapshotDictionaryAddOnly | 1000000 | 866,842.0 us | 6,976.27 us | 6,525.60 us |
```