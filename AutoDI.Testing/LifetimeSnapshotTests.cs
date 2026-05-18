using AutoDI.SourceGenerator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace AutoDI.Testing
{
    public class LifetimeSnapshotTests
    {
        private static Task VerifyGenerator(string source)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
                MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
                MetadataReference.CreateFromFile(typeof(RegisterScopedAttribute).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create(
                assemblyName: "TestAssembly",
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var generator = new AutoDIGenerator();

            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                generators: new[] { generator.AsSourceGenerator() }
            );

            driver = driver.RunGenerators(compilation);

            return Verifier.Verify(driver);
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
    }
}
