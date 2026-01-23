# TypeScript Generator

A robust C# utility for generating TypeScript type definitions from C# classes using TypeLite.

##  Design Philosophy

**By Default: Include Everything**
- No configuration needed to get started
- Just specify the namespace, all public types are exported
- Use `excludeTypes` to filter out unwanted types
- Use `includeTypes` only when you need strict control

## Features

-  **Configurable Assembly Loading**: Specify which assembly to scan
-  **Namespace Filtering**: Target specific namespaces and DTO patterns
-  **Flexible Include/Exclude**: Both whitelist and blacklist patterns supported
-  **Generic Type Filtering**: Separate control for generic types like `Result<T>`
-  **Static Class Exclusion**: Automatically excludes static utility classes (configurable)
-  **Automatic Interface Naming**: Adds 'I' prefix to generated interfaces
-  **CamelCase Conversion**: Converts C# PascalCase to TypeScript camelCase
-  **Type Mapping**: Maps C# types (like Guid) to appropriate TypeScript types
-  **Logging Support**: Optional ILogger integration for debugging
-  **Comprehensive Validation**: Validates configuration before generation
-  **Smart Path Resolution**: Automatically finds solution root or uses configured base path

## Configuration

Create a `typescriptgenconfig.json` file:

```json
{
  "assemblyName": "RetailPortal.Model",
  "outputPath": "ClientApp/src/types",
  "outputFileName": "models.gen.ts",
  "baseDirectory": null,
  "namespaces": [
    {
      "namespace": "RetailPortal.Model.Dtos",
      "includeNested": false,
      "excludeTypes": ["Internal", "Base"]
    }
  ]
}
```

### Configuration Options

| Property | Type | Required | Default         | Description |
|----------|------|----------|-----------------|-------------|
| `assemblyName` | string | Yes | -               | Name of the assembly to load (e.g., "MyApp.Models") |
| `outputPath` | string | Yes | -               | Output directory (relative or absolute) |
| `outputFileName` | string | Yes | "models.gen.ts" | Name of the generated TypeScript file |
| `baseDirectory` | string | No | null            | Base directory for relative paths (auto-detected if null) |
| `includeStaticClasses` | boolean | No | false           | Include static classes in output (excluded by default) |
| `namespaces` | array | Yes | -               | List of namespace configurations |

#### Namespace Configuration

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `namespace` | string | Yes | - | Fully qualified namespace to scan |
| `includeTypes` | array | No | null | Whitelist of type patterns (prefix/suffix/exact). If specified, ONLY matching types are included |
| `excludeTypes` | array | No | null | Blacklist of type patterns (applied after includeTypes) |
| `includeGenericTypes` | array | No | null | Whitelist of generic type base names. If specified, ONLY matching generic types are included |
| `excludeGenericTypes` | array | No | null | Blacklist of generic type base names (applied after includeGenericTypes) |
| `includeNested` | boolean | No | false | Include types from nested namespaces |

### Filter Processing Order

Filters are applied in this order:
1. **includeTypes** - If specified, only types matching these patterns pass
2. **excludeTypes** - Remove types matching these patterns
3. **includeGenericTypes** - If specified, only generic types matching these patterns pass (non-generic types are unaffected)
4. **excludeGenericTypes** - Remove generic types matching these patterns

**Default Behavior (No Filters):** ALL public types in the namespace are included.

**With Only Exclude Filters:** Everything is included EXCEPT the excluded patterns.

**With Include Filters:** ONLY types matching the include patterns are considered (stricter control).

## Usage

### Basic Usage

```csharp
// Load config and generate
var config = TypeScriptGenerator.LoadConfig();
var generator = new TypeScriptGenerator(config);
generator.Generate();
```

### With Custom Config Path

```csharp
var config = TypeScriptGenerator.LoadConfig("path/to/config.json");
var generator = new TypeScriptGenerator(config);
generator.Generate();
```

### With Logging

```csharp
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Debug);
});

var logger = loggerFactory.CreateLogger<TypeScriptGenerator>();
var config = TypeScriptGenerator.LoadConfig();
var generator = new TypeScriptGenerator(config, logger);
generator.Generate();
```

### Command Line

```bash
# Use default config location
dotnet run

# Specify config path
dotnet run path/to/typescriptgenconfig.json
```

## How It Works

1. **Loads Configuration**: Reads and validates the JSON configuration file
2. **Loads Assembly**: Dynamically loads the specified .NET assembly
3. **Discovers Types**: Scans configured namespaces for all public types (excluding specified patterns)
4. **Builds Model**: Creates a TypeLite model from discovered types
5. **Generates Code**: Applies formatters and generates TypeScript code
6. **Post-Processes**: Cleans up generated code (removes empty declarations, etc.)
7. **Writes Output**: Saves the final TypeScript to the specified location

## Configuration Examples

### 1. Include Everything (Default)
```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Models.Dtos"
    }
  ]
}
```
**Includes all public types in the namespace** - This is the default behavior and your original request!

### 2. Exclude Specific Patterns
```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Models",
      "excludeTypes": ["Base", "Abstract", "Internal"]
    }
  ]
}
```
**Includes everything EXCEPT types matching these patterns** - This is usually what you want!

### 3. Exclude Generic Wrapper Types
```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Models",
      "excludeGenericTypes": ["Result", "Option", "Either"]
    }
  ]
}
```
**Includes all types except** `Result<T>`, `Option<T>`, `Either<TLeft, TRight>`, etc.

### 4. Include Only Specific Patterns
```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Models",
      "includeTypes": ["Dto", "Request", "Response"]
    }
  ]
}
```
**ONLY includes types ending/starting with "Dto", "Request", or "Response"** - Use this for strict control.

### 5. Include Specific Generic Types Only
```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Models",
      "includeGenericTypes": ["PagedResult", "ValidationResult"]
    }
  ]
}
```
For generic types: **ONLY** includes `PagedResult<T>`, `ValidationResult<T>`, etc.
For non-generic types: Still includes everything (includeGenericTypes only affects generic types).

### 6. Combined Include and Exclude
```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Models",
      "includeTypes": ["Dto", "Response"],
      "excludeTypes": ["BaseDto"],
      "excludeGenericTypes": ["Result"]
    }
  ]
}
```
1. Only types with "Dto" or "Response" in the name
2. Except those with "BaseDto"
3. Except generic types like `Result<T>`

### 7. Include Static Classes
```json
{
  "assemblyName": "MyApp.Core",
  "outputPath": "ClientApp/src/types",
  "outputFileName": "models.ts",
  "includeStaticClasses": true,
  "namespaces": [
    {
      "namespace": "MyApp.Models"
    }
  ]
}
```
By default, static classes (often utility classes) are excluded. Set `includeStaticClasses: true` to include them.

### Include Nested Namespaces
```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Models",
      "includeNested": true,
      "excludeTypes": ["BaseDto"]
    }
  ]
}
```
This will include types from:
- `MyApp.Models`
- `MyApp.Models.Dtos`
- `MyApp.Models.Dtos.Requests`
- etc.

### Multiple Namespaces with Different Rules
```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Models.Dtos",
      "excludeTypes": ["InternalDto"]
    },
    {
      "namespace": "MyApp.Models.ViewModels",
      "includeNested": true
    },
    {
      "namespace": "MyApp.Shared.Enums"
    }
  ]
}
```

## Type Mappings

| C# Type | TypeScript Type | Notes |
|---------|----------------|---|
| `Guid` | `string` | All Guid variations mapped to string |
| `Guid?` | `string \| null` | Nullable Guid |
| `DateTime` | `Date` |  |
| `DateTime?` | `Date \| null` | Nullable DateTime |
| `int`, `long`, `decimal`, `double` | `number` | |
| `int?`, `long?`, etc. | `number \| null` | Nullable value types |
| `bool` | `boolean` | |
| `bool?` | `boolean \| null` | Nullable boolean |
| `string` | `string` | |
| `string?` | `string \| null` | Nullable reference type (C# 8.0+) |
| Enums | `enum` | |
| `MyEnum?` | `MyEnum \| null` | Nullable enum |
| Classes | `interface` | With `I` prefix |

## Filter Patterns Explained

### Automatically Excluded Types

By default, the following types are **automatically excluded** without configuration:
- **Static classes** (e.g., `public static class StringHelper`) - Set `includeStaticClasses: true` to include
- **Non-public types** (private, internal, protected)
- **Nested types** (classes defined inside other classes)

### Pattern Matching Rules
All pattern matching supports three types:
- **Prefix**: `"Dto"` matches `DtoBase`, `DtoUser`, etc.
- **Suffix**: `"Dto"` matches `UserDto`, `OrderDto`, etc.
- **Exact**: `"Internal"` matches only `Internal`

### Regular Type Filters (`includeTypes` / `excludeTypes`)

**Include Types (Whitelist):**
```csharp
// Config: "includeTypes": ["Dto", "Request", "Response"]

// INCLUDED:
UserDto              // ✓ Ends with "Dto"
DtoBase              // ✓ Starts with "Dto"
CreateUserRequest    // ✓ Ends with "Request"
ApiResponse          // ✓ Ends with "Response"

// EXCLUDED:
User                 // ✗ Doesn't match any pattern
Entity               // ✗ Doesn't match any pattern
Helper               // ✗ Doesn't match any pattern
```

**Exclude Types (Blacklist):**
```csharp
// Config: "excludeTypes": ["Base", "Abstract", "Internal"]

// INCLUDED:
UserDto              // ✓ Doesn't match exclusions
OrderEntity          // ✓ Doesn't match exclusions

// EXCLUDED:
BaseDto              // ✗ Starts with "Base"
DtoBase              // ✗ Ends with "Base"
AbstractEntity       // ✗ Starts with "Abstract"
Internal             // ✗ Exact match
```

### Generic Type Filters (`includeGenericTypes` / `excludeGenericTypes`)

Matches against the **base generic type name** (without `<T>` parameters).

**Include Generic Types (Whitelist):**
```csharp
// Config: "includeGenericTypes": ["PagedResult", "ValidationResult"]

// INCLUDED:
PagedResult<T>                  // ✓ Matches "PagedResult"
PagedResult<User, Meta>         // ✓ Matches "PagedResult"
ValidationResult<TError>        // ✓ Matches "ValidationResult"

// EXCLUDED (if includeGenericTypes is specified):
Result<T>                       // ✗ Doesn't match
Option<T>                       // ✗ Doesn't match
Either<TLeft, TRight>          // ✗ Doesn't match

// Note: Non-generic types are unaffected by generic type filters
UserDto                         // ✓ Still included (not generic)
```

**Exclude Generic Types (Blacklist):**
```csharp
// Config: "excludeGenericTypes": ["Result", "Option", "Either"]

// INCLUDED:
PagedResult<T>                  // ✓ Different base name
ResultDto                       // ✓ Not a generic type
UserResult                      // ✓ Not a generic type

// EXCLUDED:
Result<T>                       // ✗ Matches "Result"
Result<T, TError>              // ✗ Matches "Result"
Option<T>                       // ✗ Matches "Option"
Either<TLeft, TRight>          // ✗ Matches "Either"
```

### Filter Combination Examples

**Example 1: Include DTOs, Exclude Base Classes**
```json
{
  "includeTypes": ["Dto"],
  "excludeTypes": ["Base"]
}
```
Result: Only types with "Dto" in name, except "BaseDto" or "DtoBase".

**Example 2: All Types Except Generic Wrappers**
```json
{
  "excludeGenericTypes": ["Result", "Option"]
}
```
Result: Everything except `Result<T>` and `Option<T>`.

**Example 3: Only Specific Generic Types**
```json
{
  "includeGenericTypes": ["PagedResult"],
  "excludeTypes": ["Internal"]
}
```
Result:
- Generic types: Only `PagedResult<T>`
- Non-generic types: Everything except those with "Internal"

## Generated Code Features

- **Interface Naming**: C# class `UserDto` becomes TypeScript `IUserDto` (interfaces only, enums keep original names)
- **Property Naming**: C# `FirstName` becomes TypeScript `firstName`
- **Nullable Types**: C# nullable types (`int?`, `string?`, `Guid?`) become union types with `| null` in TypeScript
- **Generic Handling**: Preserves generic type parameters (T, TKey, etc.)
- **Enum Support**: Generates TypeScript enums from C# enums (no I prefix)
- **Guid Mapping**: C# `Guid` properties become TypeScript `string` (no IGuid interface or System.IGuid references)

### Understanding Nullable Type Mapping

**C# Nullable Types → TypeScript Union Types:**

```csharp
// C# Code
public class Example
{
    public int Required { get; set; }           // Must have a value
    public int? Nullable { get; set; }          // Can be null
}
```

```typescript
// TypeScript Output
interface IExample {
    required: number;              // Must exist, must be a number
    nullable: number | null;       // Must exist, can be number or null
}
```

**Why `| null` instead of `?`:**
- `property?: type` means the property may not exist at all (undefined)
- `property: type | null` means the property exists but the value can be null
- C# nullable types (`int?`, `string?`) represent a value that **exists but can be null**, not a property that might not exist

This ensures proper null-safety when working with APIs that return nullable values.

## Example Output

**C# Input:**
```csharp
namespace MyApp.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public int? TimezoneOffsetMinutes { get; set; }
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Admin,
        User,
        Guest
    }
}
```

**TypeScript Output:**
```typescript
declare module MyApp.Models.Dtos {
    interface IUserDto {
        id: string;
        firstName: string;
        middleName: string | null;
        lastName: string;
        timezoneOffsetMinutes: number | null;
        role: MyApp.Models.Dtos.UserRole;
    }

    enum UserRole {
        Admin = 0,
        User = 1,
        Guest = 2
    }
}
```

**Note:**
- Classes become interfaces with `I` prefix
- Enums keep their original name (no `I` prefix)
- `Guid` properties are mapped to `string` (no System.IGuid references)
- Nullable types (`int?`, `string?`) become union types with `| null` in TypeScript
- No `IGuid` interface is generated

## Error Handling

The generator provides clear error messages for common issues:

-  **Configuration file not found**
-  **Invalid JSON configuration**
-  **Missing required configuration properties**
-  **Assembly not found or cannot be loaded**
-  **No types found matching patterns**
-  **Output directory cannot be created**

## Common Use Cases

### Use Case 1: Only Export DTOs
**Scenario:** You only want to export Data Transfer Objects to TypeScript.

```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Core.Models",
      "includeTypes": ["Dto", "Request", "Response"]
    }
  ]
}
```

### Use Case 2: Everything Except Infrastructure Types
**Scenario:** Export all models but exclude framework wrapper types and utility classes.

```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Core.Models",
      "includeNested": true,
      "excludeTypes": ["Base", "Abstract", "Internal", "Helper"],
      "excludeGenericTypes": ["Result", "Option", "Either"]
    }
  ]
}
```
Note: Static classes (like `StringHelper`, `Constants`) are already excluded by default.

### Use Case 3: Specific Generic Types Only
**Scenario:** Only export certain generic types you actually use in your API.

```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Core.Models",
      "includeGenericTypes": ["PagedResult", "ValidationResult", "ApiResponse"]
    }
  ]
}
```

### Use Case 4: Mix Public API and Internal Models
**Scenario:** Different rules for different namespaces.

```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Core.PublicApi",
      "includeNested": true
    },
    {
      "namespace": "MyApp.Core.Internal",
      "includeTypes": ["Dto"],
      "excludeTypes": ["Base"]
    }
  ]
}
```

## Best Practices

1. **Start Simple**: Begin with no filters - just specify the namespace and let everything through
2. **Use Exclude Patterns**: Add `excludeTypes` and `excludeGenericTypes` to filter out unwanted types
3. **Use Include Only When Needed**: Use `includeTypes` only when you need strict, explicit control
4. **Test Your Filters**: Enable logging to see what gets included/excluded
5. **Common Exclusions**: Typically exclude "Base", "Abstract", "Internal", "Helper" types
6. **Generic Type Filters**: Use `excludeGenericTypes` for framework types (`Result<T>`, `Option<T>`)
7. **Version Control**: Add generated TypeScript files to `.gitignore` if regenerating on build
8. **Build Integration**: Hook into MSBuild to regenerate on each build
9. **Validation**: Use TypeScript's `tsc` to validate generated files
10. **Namespace Organization**: Keep related types in organized namespaces
11. **Nested Namespaces**: Use `includeNested: true` when you have a logical folder structure
12. **Multiple Assemblies**: Create separate config files for different assemblies if needed

### Configuration Pattern

```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Models",
      "includeNested": true,
      "excludeTypes": ["Base", "Abstract", "Internal"],
      "excludeGenericTypes": ["Result", "Option"]
    }
  ]
}
```

This includes everything by default, then excludes only what you don't want - clean and simple!

## Dependencies

- **TypeLitePlus**: For TypeScript generation
- **Microsoft.Extensions.Logging** (optional): For logging support
- **.NET 8.0+**: Target framework

## Troubleshooting

### Assembly not loading
- Ensure the assembly is built before running the generator
- Check that the assembly name matches exactly (case-sensitive)
- Verify the assembly is in the output directory or GAC

### No types found
- Verify namespace names are correct and match exactly (case-sensitive)
- Check that classes are public (private/internal classes are excluded)
- Static classes are excluded by default - set `includeStaticClasses: true` if needed
- If using `includeTypes`: Ensure your type names match the patterns
- If using `excludeTypes`: Review patterns - you might be excluding everything
- If using `includeGenericTypes`: Remember non-generic types still pass through (only affects generic types)
- Try setting `includeNested: true` if your types are in child namespaces
- Enable debug logging to see which types are being filtered and why

### Output path issues
- Use forward slashes or escaped backslashes in JSON
- Verify you have write permissions to the output directory
- Check that `baseDirectory` points to the correct location
