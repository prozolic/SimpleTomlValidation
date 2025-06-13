# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Blazor WebAssembly application that provides TOML validation using a web-based Monaco Editor interface. The app allows users to input TOML text, validate it using the CsToml library, and provides visual feedback with error highlighting.

## Build and Development Commands

- **Build the project**: `dotnet build`
- **Run the development server**: `dotnet run --project SimpleTomlValidation/SimpleTomlValidation.csproj`
- **Build for release**: `dotnet build --configuration Release`
- **Publish for GitHub Pages**: `dotnet publish --configuration Release`

## Architecture

The application consists of:

- **Index.razor/Index.razor.cs**: Main page containing the Monaco Editor and validation logic
- **Program.cs**: WebAssembly host configuration
- **ExtendableArray.cs**: Custom high-performance array structure using ArrayPool for error decorations
- **Layout/MainLayout.razor**: Application layout structure

## Key Components

### TOML Validation Flow
1. User inputs TOML text in Monaco Editor
2. Validation uses `CsTomlSerializer.Deserialize<TomlDocument>()` from CsToml library
3. Parse errors are displayed as line decorations in the editor using `ModelDeltaDecoration`
4. Status message updates based on validation result

### TOML Specification Support
- **TOML v1.0.0**: Standard stable specification
- **TOML v1.1.0**: Pre-release specification with configurable features:
  - Unicode in bare keys
  - Newlines in inline tables
  - Trailing comma in inline tables
  - Seconds omission in time values
  - `\e` escape sequence support
  - `\x` hexadecimal escape sequences

### Monaco Editor Integration
- Uses BlazorMonaco component for code editing
- Supports theme switching (Visual Studio, VS Dark, High Contrast)
- Error highlighting through delta decorations
- Language set to "toml" for syntax highlighting

### Performance Optimizations
- `ExtendableArray<T>` utility uses `ArrayPool<T>` for efficient memory management
- Error decorations are properly disposed after use
- Aggressive inlining for performance-critical methods

## Dependencies

- **CsToml v1.6.5**: TOML parsing and serialization
- **BlazorMonaco v3.3.0**: Monaco Editor integration
- **Target Framework**: .NET 9.0
- **Deployment**: Configured for GitHub Pages publishing