``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


```
|                     Method |      N | NumThreads |      Mean |     Error |    StdDev |
|--------------------------- |------- |----------- |----------:|----------:|----------:|
| **ConcurrentDictionaryAddGet** | **100000** |          **1** |  **9.272 ms** | **0.4882 ms** | **1.4394 ms** |
| **ConcurrentDictionaryAddGet** | **100000** |          **2** | **12.570 ms** | **0.8658 ms** | **2.5529 ms** |
| **ConcurrentDictionaryAddGet** | **100000** |          **4** | **12.039 ms** | **0.5146 ms** | **1.5175 ms** |
| **ConcurrentDictionaryAddGet** | **100000** |          **8** | **17.288 ms** | **0.2746 ms** | **0.2569 ms** |
