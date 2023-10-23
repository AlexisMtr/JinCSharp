using Microsoft.CodeAnalysis;

namespace JinCSharp.SourceGenerator.Attributes;

[Generator]
public class AttributeGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForPostInitialization(ctx =>
        {
            ctx.AddSource($"{SchemaSourceConstants.AttributeName}.g.cs", SchemaSourceConstants.Code);
        });
    }
}
