using NJsonSchema;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace JinCSharp.SourceGenerator.SchemaGenerators;

internal class SchemaTypeNameGenerator : DefaultTypeNameGenerator
{
    private const string ANONYMOUS = "Anonymous";

    private readonly string _rootTypeName;

    public SchemaTypeNameGenerator(string rootTypeName)
    {
        _rootTypeName = rootTypeName;
    }

    public override string Generate(JsonSchema schema, string typeNameHint, IEnumerable<string> reservedTypeNames)
    {
        var defaultTypeName = base.Generate(schema, typeNameHint, reservedTypeNames);
        if (defaultTypeName is null || defaultTypeName == ANONYMOUS)
        {
            return _rootTypeName;
        }

        if (defaultTypeName.StartsWith(_rootTypeName))
        {
            return defaultTypeName;
        }

        if (schema.Parent is not null && schema.Parent is JsonSchemaProperty parentSchemaProperty && reservedTypeNames.Contains($"{_rootTypeName}{defaultTypeName}"))
        {
            return $"{_rootTypeName}{Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(parentSchemaProperty.Name)}{defaultTypeName}";
        }

        return $"{_rootTypeName}{defaultTypeName}";
    }
}