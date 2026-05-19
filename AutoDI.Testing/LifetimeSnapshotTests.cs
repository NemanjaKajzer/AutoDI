using AutoDI.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace AutoDI.Testing
{
    public class LifetimeSnapshotTests
    {
        private static Task VerifyGenerator(params string[] sources)
        {
            var syntaxTrees = sources
                .Select(s => CSharpSyntaxTree.ParseText(s))
                .ToArray();

            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
                MetadataReference.CreateFromFile(typeof(RegisterScopedAttribute).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                assemblyName: "TestAssembly",
                syntaxTrees: syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new AutoDIGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                generators: new[] { generator.AsSourceGenerator() }
            );

            driver = driver.RunGenerators(compilation);

            return Verify(driver);
        }

        [Fact]
        public Task Scoped_SingleInterface_EmitsAddScoped()
        {
            var source = @"
                using AutoDI;
                namespace TestApp
                {
                    public interface IFooService {}
                    [RegisterScoped]
                    public class FooService : IFooService {}
                }";

            return VerifyGenerator(source);
        }

        [Fact]
        public Task Singleton_SingleInterface_EmitsAddSingleton()
        {
            var source = @"
                using AutoDI;
                namespace TestApp
                {
                    public interface IFooService {}
                    [RegisterSingleton]
                    public class FooService : IFooService {}
                }";

            return VerifyGenerator(source);
        }

        [Fact]
        public Task Transient_SingleInterface_EmitsAddTransient()
        {
            var source = @"
                using AutoDI;
                namespace TestApp
                {
                    public interface IFooService {}
                    [RegisterTransient]
                    public class FooService : IFooService {}
                }";

            return VerifyGenerator(source);
        }

        [Fact]
        public Task Singleton_MultipleInterfaces_EmitsTwoAddSingletonLines()
        {
            var source = @"
                        using AutoDI;
                        namespace TestApp
                        {
                            public interface IFoo {}
                            public interface IBar {}
                            [RegisterSingleton]
                            public class FooBarService : IFoo, IBar {}
                        }";

            return VerifyGenerator(source);
        }

        [Fact]
        public Task Scoped_ClassImplementingIDisposableAndIFoo_IDisposableNotEmitted()
        {
            var source = @"
                        using AutoDI;
                        using System;
                        namespace TestApp
                        {
                            public interface IFoo {}
                            [RegisterScoped]
                            public class FooService : IFoo, IDisposable
                            {
                                public void Dispose() {}
                            }
                        }";

            return VerifyGenerator(source);
        }

        [Fact]
        public Task Scoped_GenericRepository_EmitsOpenGenericTypeof()
        {
            var source = @"
                        using AutoDI;
                        namespace TestApp
                        {
                            public interface IRepo<T> {}
                            [RegisterScoped]
                            public class Repo<T> : IRepo<T> {}
                        }";

            return VerifyGenerator(source);
        }

        [Fact]
        public Task Scoped_InternalClass_IsIncludedInOutput()
        {
            var source = @"
                        using AutoDI;
                        namespace TestApp
                        {
                            public interface IMyService {}
                            [RegisterScoped]
                            internal sealed class MyService : IMyService {}
                        }";

            return VerifyGenerator(source);
        }

        [Fact]
        public Task Scoped_ClassWithNoInterfaces_EmitsSelfRegistration()
        {
            var source = @"
                        using AutoDI;
                        namespace TestApp
                        {
                            [RegisterScoped]
                            public class StandaloneService {}
                        }";

            return VerifyGenerator(source);
        }

        [Fact]
        public Task Scoped_PrivateNestedClass_EmitsEmptyMethodBody()
        {
            var source = @"
                        using AutoDI;
                        namespace TestApp
                        {
                            public class Outer
                            {
                                [RegisterScoped]
                                private class Inner {}
                            }
                        }";

            return VerifyGenerator(source);
        }

        [Fact]
        public Task MultipleClasses_AcrossTwoSources_SortedAlphabetically()
        {
            // two separate source files fed into the same driver run
            var source1 = @"
                        using AutoDI;
                        namespace TestApp
                        {
                            public interface IZService {}
                            [RegisterScoped]
                            public class ZService : IZService {}
                        }";

            var source2 = @"
                        using AutoDI;
                        namespace TestApp
                        {
                            public interface IAService {}
                            [RegisterScoped]
                            public class AService : IAService {}
                        }";

            return VerifyGenerator(source1, source2);
        }
    }
}
