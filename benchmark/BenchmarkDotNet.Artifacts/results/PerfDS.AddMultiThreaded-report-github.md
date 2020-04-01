``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


```
|    Method |      N | NumThreads |     Mean |    Error |   StdDev |   Median |
|---------- |------- |----------- |---------:|---------:|---------:|---------:|
| **EmptyTask** | **100000** |          **1** | **270.5 us** | **13.98 us** | **41.21 us** | **275.3 us** |
| **EmptyTask** | **100000** |          **2** | **335.6 us** | **12.13 us** | **35.75 us** | **326.2 us** |
| **EmptyTask** | **100000** |          **4** | **526.4 us** |  **4.38 us** |  **3.89 us** | **526.0 us** |
| **EmptyTask** | **100000** |          **8** | **982.9 us** | **16.24 us** | **15.19 us** | **982.6 us** |
