``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


```
|                     Method |      N | NumThreads |      Mean |     Error |    StdDev |    Median |
|--------------------------- |------- |----------- |----------:|----------:|----------:|----------:|
| **ConcurrentDictionaryAddGet** | **100000** |          **1** |  **5.034 ms** | **0.1024 ms** | **0.2987 ms** |  **5.004 ms** |
| **ConcurrentDictionaryAddGet** | **100000** |          **2** |  **7.594 ms** | **0.1809 ms** | **0.5334 ms** |  **7.544 ms** |
| **ConcurrentDictionaryAddGet** | **100000** |          **4** | **10.224 ms** | **0.2039 ms** | **0.4120 ms** | **10.389 ms** |
| **ConcurrentDictionaryAddGet** | **100000** |          **8** | **15.112 ms** | **0.2492 ms** | **0.2331 ms** | **15.096 ms** |
