using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                transform: (ctx, _) => (ClassDeclarationSyntax)ctx.Node
            );
            // P3-03: RegistrationModel
            // P3-04: transform function
            // P3-11: collect + emit
        }
    }
}
