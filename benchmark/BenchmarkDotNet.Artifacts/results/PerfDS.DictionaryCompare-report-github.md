``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


```
|                    Method |       N |       Mean |     Error |    StdDev |
|-------------------------- |-------- |-----------:|----------:|----------:|
| **SnapshotDictionaryAddOnly** |   **10000** |   **7.141 ms** | **0.0794 ms** | **0.0704 ms** |
| **SnapshotDictionaryAddOnly** | **1000000** | **831.315 ms** | **2.6968 ms** | **2.2519 ms** |
