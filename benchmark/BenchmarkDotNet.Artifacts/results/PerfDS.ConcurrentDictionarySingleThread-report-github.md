``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=2.2.108
  [Host]     : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT
  DefaultJob : .NET Core 2.2.6 (CoreCLR 4.6.27817.03, CoreFX 4.6.27818.02), X64 RyuJIT


```
|                     Method |      N |     Mean |     Error |    StdDev |
|--------------------------- |------- |---------:|----------:|----------:|
| ConcurrentDictionaryAddGet | 100000 | 4.453 ms | 0.0755 ms | 0.0707 ms |
