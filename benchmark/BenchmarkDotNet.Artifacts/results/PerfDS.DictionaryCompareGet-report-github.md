``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


```
|                      Method |       N |         Mean |      Error |     StdDev |
|---------------------------- |-------- |-------------:|-----------:|-----------:|
|     **SystemDictionaryGetOnly** |   **10000** |     **76.83 us** |   **0.404 us** |   **0.378 us** |
| ConcurrentDictionaryGetOnly |   10000 |    108.16 us |   0.358 us |   0.335 us |
|   SnapshotDictionaryGetOnly |   10000 |    738.74 us |   5.214 us |   4.622 us |
|     **SystemDictionaryGetOnly** | **1000000** |  **7,735.82 us** |  **51.994 us** |  **48.635 us** |
| ConcurrentDictionaryGetOnly | 1000000 | 20,512.34 us | 308.474 us | 257.589 us |
|   SnapshotDictionaryGetOnly | 1000000 | 76,608.19 us | 146.169 us | 129.575 us |
