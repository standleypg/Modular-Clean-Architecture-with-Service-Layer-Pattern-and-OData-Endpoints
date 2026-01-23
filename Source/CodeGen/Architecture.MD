# TypeScript Generator - Architecture

## Project Structure

```
CodeGen/
├── Configuration/           # Configuration models and program entry
│   ├── TypeScriptGenConfig.cs
│   ├── NamespaceConfig.cs
│   └── Program.cs
│
├── Discovery/              # Type discovery from assemblies
│   └── TypeDiscovery.cs   # Loads assemblies and finds types
│
├── Filters/               # Type filtering logic
│   └── TypeFilter.cs      # Include/exclude pattern matching
│
├── Formatters/            # TypeScript formatting
│   ├── TypeFormatter.cs          # Type conversions (Guid→string, etc.)
│   ├── IdentifierFormatter.cs    # Naming (camelCase, I prefix)
│   └── TypeVisibilityFormatter.cs # Show/hide types
│
├── Processing/            # Post-processing
│   └── CodePostProcessor.cs # Cleanup generated code
│
├── Utilities/            # Helper utilities
│   └── PathResolver.cs   # Output path resolution
│
└── TypeScriptGenerator.cs # Main orchestrator
```

## Component Responsibilities

### TypeScriptGenerator (Main Orchestrator)
- **Purpose**: Coordinates all components to generate TypeScript
- **Responsibilities**:
    - Orchestrates the generation workflow
    - Configures TypeLite generator with formatters
    - Writes output files
- **Dependencies**: All other components

### Configuration
- **Purpose**: Configuration models and validation
- **Components**:
    - `TypeScriptGenConfig`: Main configuration
    - `NamespaceConfig`: Per-namespace settings
    - `Program`: Entry point

### Discovery/TypeDiscovery
- **Purpose**: Finds types in assemblies
- **Responsibilities**:
    - Loads assemblies by name
    - Discovers types matching namespace patterns
    - Delegates filtering to TypeFilter

### Filters/TypeFilter
- **Purpose**: Applies include/exclude rules
- **Responsibilities**:
    - Namespace matching (exact and nested)
    - Static class detection
    - Include/exclude pattern matching
    - Generic type filtering

### Formatters
#### TypeFormatter
- **Purpose**: Type conversion and nullable handling
- **Responsibilities**:
    - Maps C# types to TypeScript (Guid→string, DateTime→Date)
    - Detects nullable types
    - Adds `| null` for nullables

#### IdentifierFormatter
- **Purpose**: Naming conventions
- **Responsibilities**:
    - Converts properties to camelCase
    - Adds 'I' prefix to interfaces (not enums)
    - Detects generic type parameters

#### TypeVisibilityFormatter
- **Purpose**: Controls which types appear in output
- **Responsibilities**:
    - Hides Guid type definitions
    - Can be extended for other hiding rules

### Processing/CodePostProcessor
- **Purpose**: Cleans up generated code
- **Responsibilities**:
    - Removes leftover Guid declarations
    - Cleans up extra whitespace
    - Regex-based text cleanup

### Utilities/PathResolver
- **Purpose**: Resolves output file paths
- **Responsibilities**:
    - Finds solution/project base directory
    - Resolves relative paths
    - Creates output directories

## Data Flow

```
1. Load Config
   ↓
2. TypeDiscovery.LoadAssembly()
   ↓
3. TypeDiscovery.DiscoverTypes()
   ├→ TypeFilter.IsInTargetNamespace()
   ├→ TypeFilter.IsNotStaticClass()
   └→ TypeFilter.IsTypeIncluded()
   ↓
4. BuildTypeScriptModel()
   ↓
5. ConfigureGenerator()
   ├→ IdentifierFormatter
   ├→ TypeFormatter
   ├→ IdentifierFormatter (module)
   └→ TypeVisibilityFormatter
   ↓
6. Generate TypeScript
   ↓
7. CodePostProcessor.Process()
   ↓
8. PathResolver.ResolveOutputPath()
   ↓
9. Write File
```

## Adding New Features

### Add a New Type Mapping
**File**: `Formatters/TypeFormatter.cs`
```csharp
return typeName switch
{
    "System.DateTime" => "Date",
    "YourType" => "YourTsType",  // Add here
    _ => typeName
};
```

### Add a New Exclusion Rule
**File**: `Filters/TypeFilter.cs`
```csharp
public bool IsTypeIncluded(Type type, NamespaceConfig nsConfig)
{
    // Add your custom filter logic here
}
```

### Add Post-Processing Cleanup
**File**: `Processing/CodePostProcessor.cs`
```csharp
public static string Process(string tsCode)
{
    // Add regex cleanup here
}
```

### Change Naming Convention
**File**: `Formatters/IdentifierFormatter.cs`
```csharp
public string FormatPropertyName(TsProperty formatter)
{
    // Modify naming logic here
}
```

## Benefits of This Structure

**Single Responsibility**: Each class has one clear purpose
**Easy to Test**: Components can be unit tested independently
**Easy to Extend**: New features go in obvious places
**Easy to Understand**: Clear separation of concerns
**Easy to Maintain**: Changes are localized to specific files
**Reusable**: Components can be used independently

## Migration from Old Code

The old monolithic `TypeScriptGenerator.cs` has been split into:
- **Type discovery** → `Discovery/TypeDiscovery.cs`
- **Filtering logic** → `Filters/TypeFilter.cs`
- **Type formatting** → `Formatters/TypeFormatter.cs`
- **Naming** → `Formatters/IdentifierFormatter.cs`
- **Visibility** → `Formatters/TypeVisibilityFormatter.cs`
- **Post-processing** → `Processing/CodePostProcessor.cs`
- **Path handling** → `Utilities/PathResolver.cs`
- **Orchestration** → `TypeScriptGenerator.cs` (now ~130 lines instead of 463!)

The public API remains the same - just instantiate and call `Generate()`.
