using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.Extensions.DependencyInjection;

namespace AutoDI.Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class RegistrationBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            // warm up the ServiceCollection type before benchmarks run
            // so JIT compilation doesn't skew the first benchmark
            var warmup = new ServiceCollection();
            warmup.AddScoped<IS001, S001>();
        }

        [Benchmark(Baseline = true, Description = "Manual registration")]
        public ServiceProvider Baseline_ManualRegistration()
        {
            var services = new ServiceCollection();

            services.AddScoped<IS001, S001>(); services.AddScoped<IS002, S002>();
            services.AddScoped<IS003, S003>(); services.AddScoped<IS004, S004>();
            services.AddScoped<IS005, S005>(); services.AddScoped<IS006, S006>();
            services.AddScoped<IS007, S007>(); services.AddScoped<IS008, S008>();
            services.AddScoped<IS009, S009>(); services.AddScoped<IS010, S010>();
            services.AddScoped<IS011, S011>(); services.AddScoped<IS012, S012>();
            services.AddScoped<IS013, S013>(); services.AddScoped<IS014, S014>();
            services.AddScoped<IS015, S015>(); services.AddScoped<IS016, S016>();
            services.AddScoped<IS017, S017>(); services.AddScoped<IS018, S018>();
            services.AddScoped<IS019, S019>(); services.AddScoped<IS020, S020>();
            services.AddScoped<IS021, S021>(); services.AddScoped<IS022, S022>();
            services.AddScoped<IS023, S023>(); services.AddScoped<IS024, S024>();
            services.AddScoped<IS025, S025>(); services.AddScoped<IS026, S026>();
            services.AddScoped<IS027, S027>(); services.AddScoped<IS028, S028>();
            services.AddScoped<IS029, S029>(); services.AddScoped<IS030, S030>();
            services.AddScoped<IS031, S031>(); services.AddScoped<IS032, S032>();
            services.AddScoped<IS033, S033>(); services.AddScoped<IS034, S034>();
            services.AddScoped<IS035, S035>(); services.AddScoped<IS036, S036>();
            services.AddScoped<IS037, S037>(); services.AddScoped<IS038, S038>();
            services.AddScoped<IS039, S039>(); services.AddScoped<IS040, S040>();
            services.AddScoped<IS041, S041>(); services.AddScoped<IS042, S042>();
            services.AddScoped<IS043, S043>(); services.AddScoped<IS044, S044>();
            services.AddScoped<IS045, S045>(); services.AddScoped<IS046, S046>();
            services.AddScoped<IS047, S047>(); services.AddScoped<IS048, S048>();
            services.AddScoped<IS049, S049>(); services.AddScoped<IS050, S050>();

            return services.BuildServiceProvider();
        }

        [Benchmark(Description = "AutoDI compile-time")]
        public ServiceProvider AutoDI_CompileTime()
        {
            var services = new ServiceCollection();
            services.AddAutoRegisteredServices();
            return services.BuildServiceProvider();
        }

        [Benchmark(Description = "Scrutor assembly scan")]
        public ServiceProvider Scrutor_AssemblyScan()
        {
            var services = new ServiceCollection();
            services.Scan(scan => scan
                .FromAssemblyOf<S001>()
                .AddClasses()
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            return services.BuildServiceProvider();
        }
    }
}