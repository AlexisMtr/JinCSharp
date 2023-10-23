namespace JinCSharp.SourceGenerator.Attributes;
internal static class SchemaSourceConstants
{
    internal const string AttributeName = "SchemaSource";
    internal const string Code = $$"""
        using System;

        namespace JinCSharp.SourceGenerator;
        
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class {{AttributeName}}Attribute : Attribute
        {
            public {{AttributeName}}Attribute(string schemaFile)
            {
            }
        }
        """;
}
