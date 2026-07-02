# NeuroForge Python Runtime Manager

A comprehensive .NET 10 library for automating Python runtime installation and package management.

## Overview

NeuroForge.Factory provides a robust solution for downloading, installing, and configuring Python environments programmatically within .NET applications. It handles everything from downloading Python installers to installing required packages via pip.

## Quick Start

```csharp
using NeuroForge.Factory;

// Initialize Python environment
var factory = new NeuroForgeFactory();
await factory.InitializeAsync();
```

## Features

- ✅ **Automated Python Installation** - Downloads and installs Python from configured sources
- ✅ **Package Management** - Automatically installs Python packages via pip
- ✅ **Progress Reporting** - Real-time progress updates during operations
- ✅ **Cancellation Support** - Full async/await with cancellation token support
- ✅ **Resource Validation** - Validates embedded configuration resources
- ✅ **Flexible Configuration** - JSON-based configuration for Python version and packages
- ✅ **Error Handling** - Comprehensive exception handling with detailed messages
- ✅ **Helper Utilities** - Check Python installation, get version info, etc.

## Project Structure

```
NeuroForge/
├── src/
│   ├── dotnet/
│   │   └── NeuroForge.Factory/
│   │       ├── Abstractions/          # Interfaces
│   │       ├── Core/                  # Core implementation
│   │       │   ├── PythonRuntimeManager.cs
│   │       │   ├── PythonRuntimeConfig.cs
│   │       │   └── PackageListConfig.cs
│   │       ├── Support/               # Helper utilities
│   │       │   ├── PythonSetupRunner.cs
│   │       │   └── PythonRuntimeHelper.cs
│   │       ├── Resources/
│   │       │   └── Runtime/
│   │       │       ├── python_runtime.json
│   │       │       └── package_list.json
│   │       ├── NeuroForgeFactory.cs   # High-level factory
│   │       ├── PYTHON_RUNTIME_MANAGER_README.md
│   │       └── QUICK_START.md
│   └── NeuroForge.Testing/            # Unit tests (44 passing)
```

## Usage Patterns

### Pattern 1: High-Level Factory (Recommended)

```csharp
using NeuroForge.Factory;

var factory = new NeuroForgeFactory();
await factory.InitializeAsync();
```

### Pattern 2: Setup Runner

```csharp
using NeuroForge.Factory.Support;

var runner = new PythonSetupRunner();
await runner.SetupPythonAsync(@"C:\MyApp\Python");
```

### Pattern 3: Low-Level Manager

```csharp
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

var installDir = PythonRuntimeHelper.GetDefaultInstallDirectory();
using var manager = new PythonRuntimeManager(installDir);

var progress = new Progress<string>(msg => Console.WriteLine(msg));
await manager.SetupPythonEnvironmentAsync(progress, CancellationToken.None);
```

## Configuration

### Python Runtime Configuration
**File:** `Resources/Runtime/python_runtime.json`

```json
{
  "python_version": "3.11.4",
  "download_url": "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe",
  "install_args": [ "/quiet", "InstallAllUsers=1", "PrependPath=1" ]
}
```

### Package Configuration
**File:** `Resources/Runtime/package_list.json`

```json
{
  "packages": [
	"tensorflow==2.15.0",
	"tf2onnx",
	"numpy",
	"matplotlib"
  ]
}
```

## Key Classes

### NeuroForgeFactory
High-level factory for initializing Python environment with validation.

```csharp
public class NeuroForgeFactory : INeuroForgeFactory
{
	Task InitializeAsync(string? customInstallDirectory = null);
}
```

### PythonSetupRunner
Simplified runner for common setup scenarios.

```csharp
public class PythonSetupRunner : IPythonSetupRunner
{
	Task SetupPythonAsync(string? installDirectory = null);
	Task SetupPythonWithCancellationAsync(string, CancellationToken);
}
```

### PythonRuntimeManager
Core manager for low-level control.

```csharp
public class PythonRuntimeManager : IPythonRuntimeManager, IDisposable
{
	Task SetupPythonEnvironmentAsync(IProgress<string>?, CancellationToken);
}
```

### PythonRuntimeHelper
Static utility class with helper methods.

```csharp
public static class PythonRuntimeHelper
{
	string GetDefaultInstallDirectory();
	bool IsPythonInstalled();
	string? GetPythonVersion();
	IProgress<string> CreateConsoleProgress(bool includeTimestamp = true);
	IProgress<string> CreateFileProgress(string logFilePath);
	bool ValidateEmbeddedResources(out string? missingResource);
}
```

## Documentation

- **[Python Runtime Manager README](src/dotnet/NeuroForge.Factory/PYTHON_RUNTIME_MANAGER_README.md)** - Comprehensive guide
- **[Quick Start Guide](src/dotnet/NeuroForge.Factory/QUICK_START.md)** - Get started quickly

## Testing

The project includes comprehensive unit tests with **44 passing tests** covering:

- ✅ Configuration deserialization (JSON parsing)
- ✅ Runtime manager functionality
- ✅ Helper utility methods
- ✅ Progress reporting
- ✅ Error handling
- ✅ Cancellation support
- ✅ Edge cases and advanced scenarios

Run tests:
```bash
dotnet test src/NeuroForge.Testing/NeuroForge.Testing.csproj
```

## Requirements

- **.NET 10** - Target framework
- **Network Access** - For downloading Python and packages
- **Disk Space** - ~500MB+ for Python and packages
- **Permissions** - May require admin privileges for system-wide installation

## Error Handling

```csharp
try
{
	var factory = new NeuroForgeFactory();
	await factory.InitializeAsync();
}
catch (InvalidOperationException ex)
{
	// Configuration or installation error
	Console.WriteLine($"Setup failed: {ex.Message}");
}
catch (HttpRequestException ex)
{
	// Network/download error
	Console.WriteLine($"Download failed: {ex.Message}");
}
catch (OperationCanceledException)
{
	// User cancelled
	Console.WriteLine("Installation cancelled");
}
```

## Progress Reporting

### Console Progress
```csharp
var progress = PythonRuntimeHelper.CreateConsoleProgress(includeTimestamp: true);
```

### File Progress
```csharp
var progress = PythonRuntimeHelper.CreateFileProgress("setup.log");
```

### Custom Progress
```csharp
var progress = new Progress<string>(message =>
{
	// Your custom handling
	logger.LogInformation(message);
});
```

## Cancellation

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

var runner = new PythonSetupRunner();
await runner.SetupPythonWithCancellationAsync(installDir, cts.Token);
```

## License

MIT License - See LICENSE file for details

## Author

Angel Hernandez (me@angelhernandezm.com)

## Contributing

Contributions are welcome! Please ensure:
- All tests pass
- Code follows existing patterns
- Documentation is updated
- New functionality includes tests

## Version History

### Current Version
- ✅ .NET 10 support
- ✅ Comprehensive unit tests (44 tests)
- ✅ Full async/await implementation
- ✅ Progress reporting
- ✅ Cancellation support
- ✅ Helper utilities
- ✅ Complete documentation

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/angelhernandezm/NeuroForge).
