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
