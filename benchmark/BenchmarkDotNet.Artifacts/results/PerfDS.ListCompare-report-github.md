``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


```
|                            Method |      N |       Mean |     Error |     StdDev |
|---------------------------------- |------- |-----------:|----------:|-----------:|
|           **SystemListAddAndIterate** |  **10000** |  **34.656 us** | **0.2654 us** |  **0.2353 us** |
|      PartitionedListAddAndIterate |  10000 |  81.394 us | 0.8049 us |  0.7135 us |
|                 SystemListAddOnly |  10000 |  15.316 us | 0.1874 us |  0.1753 us |
|            PartitionedListAddOnly |  10000 |  27.627 us | 0.4028 us |  0.3768 us |
|      SystemListAddOnlySizeUnknown |  10000 |  24.360 us | 0.3567 us |  0.3162 us |
| PartitionedListAddOnlySizeUnknown |  10000 |  27.718 us | 0.4295 us |  0.3586 us |
|                      ArrayAddOnly |  10000 |   6.419 us | 0.1241 us |  0.1477 us |
|           **SystemListAddAndIterate** | **100000** | **473.377 us** | **9.5049 us** | **11.3149 us** |
|      PartitionedListAddAndIterate | 100000 | 858.465 us | 8.2485 us |  7.7157 us |
|                 SystemListAddOnly | 100000 | 279.695 us | 4.9558 us |  4.3932 us |
|            PartitionedListAddOnly | 100000 | 308.580 us | 5.5199 us |  4.8932 us |
|      SystemListAddOnlySizeUnknown | 100000 | 427.819 us | 5.9998 us |  5.6122 us |
| PartitionedListAddOnlySizeUnknown | 100000 | 306.309 us | 1.3961 us |  1.3059 us |
|                      ArrayAddOnly | 100000 | 191.030 us | 1.6156 us |  1.4322 us |
