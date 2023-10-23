# JinCSharp *[jink-sharp]*

JinCSharp is a SourceGenerator that generate POCO defined in a schema (JsonSchema) using [NJsonSchema](https://github.com/RicoSuter/NJsonSchema)

## How to use it
With the JsonSchema `employee.schema.json` added to the solution as a `C# analyzer additional file`
<details>
<summary>schema</summary>

```json
{
    "$schema": "http://json-schema.org/draft-04/schema#",
    "$id": "https://example.com/employee.schema.json",
    "title": "Record of employee",
    "description": "This document records the details of an employee",
    "type": "object",
    "properties": {
        "id": {
            "description": "A unique identifier for an employee",
            "type": "number"
        },
        "name": {
            "description": "Full name of the employee",
            "type": "string"
        },
        "age": {
            "description": "Age of the employee",
            "type": "number"
        },
        "hobbies": {
            "description": "Hobbies of the employee",
            "type": "object",
            "properties": {
                "indoor": {
                    "type": "array",
                    "items": {
                        "description": "List of indoor hobbies",
                        "type": "string"
                    }
                },
                "outdoor": {
                    "type": "array",
                    "items": {
                        "description": "List of outdoor hobbies",
                        "type": "string"
                    }
                }
            }
        }
    }
}
```
</details>


And the folowwing code
```csharp
using JinCSharp.SourceGenerator;

namespace JinCSharpSample;

[SchemaSource("employee.schema.json")]
public partial class Employee { }
```

The source generator will create 2 files :
1. Employee.cs
    <details>
    <summary>C# code</summary>

    ```csharp
    namespace JinCSharpSample;

    /// <summary>
    /// This document records the details of an employee
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.9.0.0 (Newtonsoft.Json v13.0.3.0)")]
    public partial class Employee
    {
        /// <summary>
        /// A unique identifier for an employee
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]   
        public double Id { get; set; }

        /// <summary>
        /// Full name of the employee
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("name")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]   
        public string Name { get; set; }

        /// <summary>
        /// Age of the employee
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("age")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]   
        public double Age { get; set; }

        /// <summary>
        /// Hobbies of the employee
        /// </summary>
        [System.Text.Json.Serialization.JsonPropertyName("hobbies")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]   
        public EmployeeHobbies Hobbies { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;
        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

        public string ToJson()
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            return System.Text.Json.JsonSerializer.Serialize(this, options);
        }

        public static Employee FromJson(string data)
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            return System.Text.Json.JsonSerializer.Deserialize<Employee>(data, options);
        }
    }
    ```
    </details>

2. EmployeeHobbies.cs
    <details>
    <summary>C# code</summary>

    ```csharp
    namespace JinCSharpSample;

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.9.0.0 (Newtonsoft.Json v13.0.3.0)")]
    public partial class EmployeeHobbies
    {
        [System.Text.Json.Serialization.JsonPropertyName("indoor")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]   
        public System.Collections.Generic.ICollection<string> Indoor { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("outdoor")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]   
        public System.Collections.Generic.ICollection<string> Outdoor { get; set; }

        private System.Collections.Generic.IDictionary<string, object> _additionalProperties;
        [System.Text.Json.Serialization.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties ?? (_additionalProperties = new System.Collections.Generic.Dictionary<string, object>()); }
            set { _additionalProperties = value; }
        }

        public string ToJson()
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            return System.Text.Json.JsonSerializer.Serialize(this, options);
        }
        public static EmployeeHobbies FromJson(string data)
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            return System.Text.Json.JsonSerializer.Deserialize<EmployeeHobbies>(data, options);
        }
    }
    ```
    </details>