# Complete File Listing

## All Files in the Refactored Solution

### Root Files
- `TypeScriptGenerator.Refactored.cs` - Main orchestrator class
- `ARCHITECTURE.md` - Architecture documentation
- `README.md` - User documentation

### Configuration/
- `TypeScriptGenConfig.cs` - Main configuration model
- `NamespaceConfig.cs` - Namespace configuration model (part of TypeScriptGenConfig.cs)
- `Program.cs` - Entry point

### Discovery/
- `TypeDiscovery.cs` - Assembly loading and type discovery

### Filters/
- `TypeFilter.cs` - Type filtering logic (include/exclude patterns)

### Formatters/
- `TypeFormatter.cs` - Type conversion and nullable handling
- `IdentifierFormatter.cs` - Naming conventions (camelCase, I prefix)
- `TypeVisibilityFormatter.cs` - Type visibility control

### Processing/
- `CodePostProcessor.cs` - Post-processing cleanup

### Utilities/
- `PathResolver.cs` - Output path resolution
- `ConfigLoader.cs` - Configuration loading and validation

## File Purposes Quick Reference

| File | Purpose |
|------|---------|
| TypeScriptGenerator.cs | Orchestrates all components |
| TypeDiscovery.cs | Finds types in assemblies |
| TypeFilter.cs | Applies filtering rules |
| TypeFormatter.cs | Type conversions & nullables |
| IdentifierFormatter.cs| Naming conventions |
| TypeVisibilityFormatter.cs| Show/hide types |
| CodePostProcessor.cs| Cleanup generated code |
| PathResolver.cs| Resolve output paths |
| ConfigLoader.cs| Load & validate config |
| TypeScriptGenConfig.cs| Configuration models |
| Program.cs | Entry point |

## Namespace Structure

```
CodeGen
├── CodeGen.Configuration
├── CodeGen.Discovery
├── CodeGen.Filters
├── CodeGen.Formatters
├── CodeGen.Processing
├── CodeGen.Utilities
└── CodeGen.Console (Program.cs)
```

## Dependencies Between Components

```
TypeScriptGenerator
├─→ TypeDiscovery
│   └─→ TypeFilter
├─→ TypeFormatter
├─→ IdentifierFormatter
├─→ TypeVisibilityFormatter
├─→ PathResolver
└─→ CodePostProcessor

Program.cs
└─→ ConfigLoader
    └─→ TypeScriptGenConfig
```
