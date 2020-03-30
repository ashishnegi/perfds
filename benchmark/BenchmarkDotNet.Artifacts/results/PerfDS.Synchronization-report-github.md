``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.201
  [Host]     : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT
  DefaultJob : .NET Core 3.1.3 (CoreCLR 4.700.20.11803, CoreFX 4.700.20.12001), X64 RyuJIT


```
|               Method |    N |        Mean |     Error |   StdDev |
|--------------------- |----- |------------:|----------:|---------:|
|              Sum1000 | 1000 |    485.8 ns |  39.85 ns | 116.2 ns |
|           Atomic1000 | 1000 |  5,448.6 ns | 107.40 ns | 179.4 ns |
| MutexUncontended1000 | 1000 | 20,288.4 ns | 508.17 ns | 543.7 ns |
