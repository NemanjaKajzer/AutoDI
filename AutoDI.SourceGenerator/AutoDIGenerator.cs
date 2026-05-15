using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace AutoDI.SourceGenerator
{
    [Generator]
    public sealed class AutoDIGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Pipeline will be built here in subsequent tasks:
            // P3-02: syntax predicate
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: (node, _) => node is ClassDeclarationSyntax cls
                                        && cls.AttributeLists.Count > 0,
                transform: (ctx, _) => GetRegistrationModel(ctx)
            )
            .Where(model => model != null);
            // P3-04: transform function
            // P3-11: collect + emit
        }

        private static RegistrationModel GetRegistrationModel(GeneratorSyntaxContext ctx)
        {
            // Step 1: get the class symbol from the semantic model
            var classDeclaration = (ClassDeclarationSyntax)ctx.Node;
            var classSymbol = ctx.SemanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

            if (classSymbol == null)
                return null;

            // Step 2: walk AttributeData to find which marker attribute is present
            ServiceLifetime? lifetime = null;

            foreach (var attribute in classSymbol.GetAttributes())
            {
                var fullName = attribute.AttributeClass?.ToDisplayString();

                if (fullName == AttributeNames.RegisterScoped)
                {
                    lifetime = ServiceLifetime.Scoped;
                    break;
                }
                if (fullName == AttributeNames.RegisterSingleton)
                {
                    lifetime = ServiceLifetime.Singleton;
                    break;
                }
                if (fullName == AttributeNames.RegisterTransient)
                {
                    lifetime = ServiceLifetime.Transient;
                    break;
                }
            }

            // class doesn't have any of our marker attributes, discard it
            if (lifetime == null)
                return null;

            // Step 3: capture interface FQNs, excluding IDisposable
            var interfaces = ImmutableArray.CreateBuilder<string>();

            foreach (var iface in classSymbol.Interfaces)
            {
                var ifaceFQN = iface.ToDisplayString();

                if (ifaceFQN == "System.IDisposable")
                    continue;

                interfaces.Add(ifaceFQN);
            }

            return new RegistrationModel(
                implementationFQN: classSymbol.ToDisplayString(),
                interfaceFQNs: interfaces.ToImmutable(),
                lifetime: lifetime.Value
            );
        }
    }
}
