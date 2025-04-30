# MyReverseEngineeringTool
A powerful reverse engineering tool that extracts class diagram information from compiled code using .NET reflection capabilities. 
This tool is designed to analyze DLL or EXE files and generate UML class diagrams in various formats.

## Overview
MyReverseEngineeringTool leverages the System.Reflection namespace to extract class structure information from compiled code. 
It can identify classes, interfaces, methods, fields, and the relationships between them (inheritance, implementation, association, and dependency).

## Features
- Extract class diagram information from compiled DLL or EXE files
- Generate output in text format or in formats compatible with UML drawing tools (yuml, plantuml)
- Configurable output options:

  - Class filtering (ignore specific namespaces)
  - Toggle fully qualified class names
  - Show/hide method names
  - Show/hide attributes

- Identify four types of relationships:

  - Extends (inheritance)
  - Implements (interface implementation)
  - Association
  - Dependency

- Support for generic types

## Project Structure

**MyReverseEngineeringTool/**

├── **ClassDiagramExtractor.cs**    # Core class for extracting diagram information

├── **DiagramModel.cs**            # Data model for representing class diagrams

├── **DiagramOptions.cs**          # Configuration options for the tool

├── **Factory.cs**                 # Factory pattern implementation for formatters

├── **IDiagramFormatter.cs**       # Interface for different output formatters

├── **PlantumlFormatter.cs**       # PlantUML specific formatter

├── **Program.cs**                 # Main program entry point

├── **RunTool.bat**                # Batch script for running the tool

├── **TextFormatter.cs**           # Text output formatter

├── **YumlFormatter.cs**           # yUML specific formatter

└── **test_files/**                # Directory containing test files

## Design Patterns
The tool implements the following design patterns:

**Factory Pattern** - Implemented in Factory.cs to create the appropriate formatter based on user options

**Strategy Pattern** - Using IDiagramFormatter interface to allow different formatting strategies

## Installation
### Prerequisities
- .NET 6.0 or higher
- Visual Studio 2022 or Visual Studio Code (for building from source)

### Building from source
```
git clone https://github.com/yourusername/MyReverseEngineeringTool.git
cd MyReverseEngineeringTool
dotnet build --configuration Release
```

This will create executable files in the bin/Release/net6.0 directory.

## Usage
### Basic Command
```
MyReverseEngineeringTool.exe <input-file> [options]
```
Or use the provided batch file:
```
RunTool.bat <input-file> [options]
```

Where <input-file> is the compiled file (DLL or EXE) you want to analyze.

### Options
- -f, --format <format> - Output format (text, yuml, plantuml)
- -i, --ignore <patterns> - Comma-separated patterns of namespaces to ignore (e.g., System.,Microsoft.)
- -fq, --fully-qualified - Use fully qualified type names
- -nm, --no-methods - Do not show method information in the diagram
- -na, --no-attributes - Do not show attribute information in the diagram
- -o, --output <directory> - Directory to write output files (default: current directory)

### Examples
Generate text compatible output:
```
MyReverseEngineeringTool.exe <file_name>
```
or
```
MyReverseEngineeringTool.exe <file_name> -f text
```
Generate yUML compatible output:
```
MyReverseEngineeringTool.exe <file_name> -f yuml
```
Generate PlantUML compatible output:
```
MyReverseEngineeringTool.exe <file_name> -f plantuml
```
## Implementation Details

### Core components

- ClassDiagramExtractor: Handles the loading and analysis of compiled assemblies using System.Reflection
- DiagramModel: Contains the internal representation of the class diagram
- DiagramOptions: Stores configuration options for the tool
- Factory: Creates the appropriate formatter based on the requested output format
- IDiagramFormatter: Interface that all formatters implement

  - TextFormatter: Generates plain text output
  - YumlFormatter: Generates yUML compatible output
  - PlantumlFormatter: Generates PlantUML compatible output

### Adding new output formats
To add support for a new UML tool format:

1. Create a new formatter class that implements the IDiagramFormatter interface
2. Implement the required methods to convert the diagram model to the specific format
3. Add the new formatter to the Factory class

No changes to the core components are needed when adding new output formats.


## Limitations
- Cannot distinguish between aggregation and composition relationships (all are treated as associations)
- Does not detect cardinality of associations
- Only analyzes types available at runtime through reflection

## License
MIT License - free for educational and personal use
