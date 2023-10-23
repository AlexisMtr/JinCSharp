using NJsonSchema;
using System.Collections.Generic;

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

        return $"{_rootTypeName}{defaultTypeName}";
    }
}