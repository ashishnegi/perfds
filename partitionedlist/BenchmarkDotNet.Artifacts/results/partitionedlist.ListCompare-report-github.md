``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18363
Intel Xeon W-2133 CPU 3.60GHz, 1 CPU, 12 logical and 6 physical cores
  [Host]     : .NET Framework 4.8 (4.8.4121.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8 (4.8.4121.0), X86 LegacyJIT


```
|          Method |       N |         Mean |      Error |       StdDev |
|---------------- |-------- |-------------:|-----------:|-------------:|
|      **SystemList** |   **10000** |     **57.06 us** |   **1.123 us** |     **2.535 us** |
| PartitionedList |   10000 |    246.58 us |   5.967 us |    17.122 us |
|      **SystemList** | **1000000** |  **7,007.04 us** | **139.330 us** |   **287.741 us** |
| PartitionedList | 1000000 | 24,814.10 us | 484.919 us | 1,054.175 us |
