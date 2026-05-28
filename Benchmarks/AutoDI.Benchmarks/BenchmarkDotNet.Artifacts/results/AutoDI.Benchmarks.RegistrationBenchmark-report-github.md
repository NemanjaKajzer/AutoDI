```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26100.7623/24H2/2024Update/HudsonValley)
12th Gen Intel Core i7-12800H 1.80GHz, 1 CPU, 20 logical and 14 physical cores
.NET SDK 10.0.300
  [Host]     : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.8 (10.0.8, 10.0.826.23019), X64 RyuJIT x86-64-v3


```
| Method                  | Mean      | Error     | StdDev    | Ratio | RatioSD | Rank | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------ |----------:|----------:|----------:|------:|--------:|-----:|-------:|-------:|----------:|------------:|
| &#39;Manual registration&#39;   |  2.126 μs | 0.0174 μs | 0.0145 μs |  1.00 |    0.01 |    1 | 1.4038 | 0.1526 |  17.22 KB |        1.00 |
| &#39;AutoDI compile-time&#39;   |  2.138 μs | 0.0402 μs | 0.0577 μs |  1.01 |    0.03 |    1 | 1.4038 | 0.1526 |  17.22 KB |        1.00 |
| &#39;Scrutor assembly scan&#39; | 45.557 μs | 0.8706 μs | 1.0364 μs | 21.43 |    0.50 |    2 | 4.8828 | 0.2441 |  59.92 KB |        3.48 |
