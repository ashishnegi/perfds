``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4121.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8 (4.8.4121.0), X86 LegacyJIT


```
|                            Method |       N |         Mean |       Error |      StdDev |
|---------------------------------- |-------- |-------------:|------------:|------------:|
|           **SystemListAddAndIterate** |   **10000** |    **59.367 us** |   **1.3915 us** |   **4.0591 us** |
|      PartitionedListAddAndIterate |   10000 |    85.280 us |   1.6567 us |   4.3645 us |
|                 SystemListAddOnly |   10000 |    27.568 us |   0.5508 us |   1.1001 us |
|            PartitionedListAddOnly |   10000 |    30.077 us |   0.6992 us |   0.6198 us |
|      SystemListAddOnlySizeUnknown |   10000 |    38.428 us |   0.7620 us |   1.8976 us |
| PartitionedListAddOnlySizeUnknown |   10000 |    31.331 us |   0.6245 us |   1.6232 us |
|                      ArrayAddOnly |   10000 |     6.613 us |   0.1476 us |   0.1516 us |
|           **SystemListAddAndIterate** | **1000000** | **7,013.675 us** | **139.3258 us** | **212.7652 us** |
|      PartitionedListAddAndIterate | 1000000 | 8,983.573 us | 175.9889 us | 359.4987 us |
|                 SystemListAddOnly | 1000000 | 4,147.194 us |  82.6280 us | 168.7871 us |
|            PartitionedListAddOnly | 1000000 | 3,652.852 us |  71.0809 us |  69.8109 us |
|      SystemListAddOnlySizeUnknown | 1000000 | 8,746.580 us | 173.5519 us | 447.9928 us |
| PartitionedListAddOnlySizeUnknown | 1000000 | 3,642.608 us |  69.2174 us |  87.5378 us |
|                      ArrayAddOnly | 1000000 | 2,013.333 us |  39.6450 us |  84.4867 us |
