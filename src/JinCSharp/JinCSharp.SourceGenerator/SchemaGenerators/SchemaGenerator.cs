using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using NJsonSchema.CodeGeneration.CSharp;
using System.Collections.Immutable;
using System.Threading;
using System.IO;
using System.Linq;
using NJsonSchema;
using JinCSharp.SourceGenerator.Attributes;

namespace JinCSharp.SourceGenerator.SchemaGenerators;

[Generator]
public class SchemaGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classesDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(IsSchemaSourcedClass, FetchSchemaSourcedClass)
            .Where(e => e is not null);
        var additionnalFiles = context.AdditionalTextsProvider.Select((e, ct) => (name: Path.GetFileName(e.Path), content: e.GetText(ct)!.ToString()));
        var withCompilation = context.CompilationProvider.Combine(classesDeclarations.Collect().Combine(additionnalFiles.Collect()));

        context.RegisterSourceOutput(withCompilation, (ctx, data) => Execute(ctx, data.Left, data.Right.Left!, data.Right.Right));
    }

    private static bool IsSchemaSourcedClass(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax classDeclaration && classDeclaration.AttributeLists.Count > 0;
    }

    private static ClassDeclarationSyntax? FetchSchemaSourcedClass(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var node = context.Node as ClassDeclarationSyntax;
        foreach (var attr in node!.AttributeLists.SelectMany(e => e.Attributes))
        {
            var symbol = context.SemanticModel.GetSymbolInfo(attr).Symbol;
            if (symbol is not null && symbol.ContainingType.ToDisplayString().Contains(SchemaSourceConstants.AttributeName))
            {
                return node;
            }
        }

        return null;
    }

    private void Execute(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classes,
        ImmutableArray<(string Name, string Content)> files)
    {
        if (classes.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var classDeclaration in classes)
        {
            var model = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
            if (model.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol symbol)
            {
                continue;
            }

            var @namespace = string.Join(".", symbol.ContainingNamespace.ConstituentNamespaces);
            var className = symbol.Name;
            var schemaSourceAttribute = symbol.GetAttributes().Single(e => e.AttributeClass is not null && e.AttributeClass.Name.Contains(SchemaSourceConstants.AttributeName));
            var schemaSourceUri = schemaSourceAttribute.ConstructorArguments.First().Value as string;

            var parentClassName = string.Empty;
            if (classDeclaration.Parent is ClassDeclarationSyntax parentClass)
            {
                var parentModel = compilation.GetSemanticModel(parentClass.SyntaxTree);
                if (parentModel.GetDeclaredSymbol(parentClass) is INamedTypeSymbol parentSymbol)
                {
                    parentClassName = parentSymbol.Name;
                }
            }

            var schemaContent = files.FirstOrDefault(e => e.Name == schemaSourceUri).Content;
            if (string.IsNullOrWhiteSpace(schemaContent))
            {
                continue;
            }

            var jsonSchema = JsonSchema.FromJsonAsync(schemaContent).Result;
            var codeGenerator = new CSharpGenerator(jsonSchema, new CSharpGeneratorSettings
            {
                JsonLibrary = CSharpJsonLibrary.SystemTextJson,
                ClassStyle = CSharpClassStyle.Poco,
                SchemaType = SchemaType.JsonSchema,
                TypeNameGenerator = new SchemaTypeNameGenerator(className),
                GenerateJsonMethods = true,
                Namespace = string.IsNullOrWhiteSpace(parentClassName) ? @namespace : $"{@namespace}.{parentClassName}",
                RequiredPropertiesMustBeDefined = true
            });

            _ = codeGenerator.GenerateFile();
            var types = codeGenerator.GenerateTypes();

            foreach (var type in types)
            {
                var fileName = $"{type.TypeName}.cs";
                if (string.IsNullOrWhiteSpace(parentClassName))
                {
                    context.AddSource(fileName, $$"""
                        namespace {{@namespace}};

                        {{type.Code}}
                        """);
                }
                else
                {
                    context.AddSource(fileName, $$"""
                        namespace {{@namespace}};

                        public partial class {{parentClassName}}
                        {
                            {{type.Code.Replace("\n", "\n\t")}}
                        }
                        """);
                }
            }
        }
    }
}
