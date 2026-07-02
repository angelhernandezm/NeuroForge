# Python Runtime Manager

## Overview

The Python Runtime Manager provides a complete solution for downloading Python and installing packages programmatically in .NET applications using embedded JSON configuration files.

## Features

- **Automated Python Download**: Downloads Python installer from configured URL
- **Progress Reporting**: Real-time progress updates during download and installation
- **Package Installation**: Automatically installs Python packages using pip
- **Embedded Configuration**: Uses embedded JSON resources for configuration
- **Cancellation Support**: Full support for cancellation tokens
- **Error Handling**: Comprehensive error handling and reporting

## Configuration Files

### python_runtime.json
Located at: `Resources/Runtime/python_runtime.json`

```json
{
  "python_version": "3.11.4",
  "download_url": "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe",
  "install_args": [ "/quiet", "InstallAllUsers=1", "PrependPath=1" ]
}
```

- **python_version**: Version of Python to install
- **download_url**: URL to download the Python installer
- **install_args**: Arguments to pass to the installer

### package_list.json
Located at: `Resources/Runtime/package_list.json`

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

- **packages**: Array of package specifications (supports pip format with version pins)

## Usage

### Basic Usage

```csharp
using NeuroForge.Factory;
using NeuroForge.Factory.Support;

// Using PythonSetupRunner (simple setup with default directory)
var runner = new PythonSetupRunner();
await runner.SetupPythonAsync();
```

### Using NeuroForgeFactory

```csharp
using NeuroForge.Factory;

// Initialize Python environment with NeuroForgeFactory
var factory = new NeuroForgeFactory();
await factory.InitializeAsync();
```

### Custom Installation Directory

```csharp
using NeuroForge.Factory.Support;

var customDirectory = @"C:\MyApp\Python";
var runner = new PythonSetupRunner();
await runner.SetupPythonAsync(customDirectory);
```

### Advanced Usage with Custom Progress Reporting

```csharp
using NeuroForge.Factory.Core;

var installDir = Path.Combine(
	Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
	"MyApp", "Python");

using var manager = new PythonRuntimeManager(installDir);

var progress = new Progress<string>(message =>
{
	Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
	// Or log to file, send to UI, etc.
});

await manager.SetupPythonEnvironmentAsync(progress, CancellationToken.None);
```

### With Cancellation Support

```csharp
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

var cts = new CancellationTokenSource();

// Cancel after 5 minutes
cts.CancelAfter(TimeSpan.FromMinutes(5));

try
{
	var runner = new PythonSetupRunner();
	await runner.SetupPythonWithCancellationAsync(@"C:\MyApp\Python", cts.Token);
}
catch (OperationCanceledException)
{
	Console.WriteLine("Installation was cancelled");
}
```

## Components

### NeuroForgeFactory
High-level factory class for initializing the Python environment.

**Methods:**
- `InitializeAsync(string?)`: Initializes Python environment with optional custom install directory

### PythonSetupRunner
Runner class for executing Python setup tasks.

**Methods:**
- `SetupPythonAsync(string?)`: Sets up Python with optional custom directory
- `SetupPythonWithCancellationAsync(string, CancellationToken)`: Sets up Python with cancellation support

### PythonRuntimeManager
Core class that orchestrates the Python setup process.

**Methods:**
- `SetupPythonEnvironmentAsync(IProgress<string>?, CancellationToken)`: Complete setup process
- `Dispose()`: Cleans up resources

### PythonRuntimeHelper
Static helper class with utility methods.

**Methods:**
- `GetDefaultInstallDirectory()`: Returns default install path
- `IsPythonInstalled()`: Checks if Python is available
- `GetPythonVersion()`: Gets installed Python version
- `CreateConsoleProgress(bool)`: Creates console progress reporter
- `CreateFileProgress(string)`: Creates file-based progress reporter
- `ValidateEmbeddedResources(out string?)`: Validates resource availability

### PythonRuntimeConfig
Configuration model for Python runtime settings.

**Properties:**
- `PythonVersion`: Version string
- `DownloadUrl`: Installer download URL
- `InstallArgs`: Array of installer arguments

### PackageListConfig
Configuration model for Python packages.

**Properties:**
- `Packages`: Array of package specifications

## Process Flow

1. **Load Configuration**: Reads embedded JSON resources
2. **Download Python**: Downloads installer from configured URL with progress tracking
3. **Install Python**: Executes installer with specified arguments
4. **Find Python**: Locates installed Python executable
5. **Install Packages**: Installs each package using pip

## Error Handling

The manager throws `InvalidOperationException` in the following cases:
- Cannot find embedded resource files
- Download fails
- Python installation fails (non-zero exit code)
- Package installation fails
- Cannot locate Python executable after installation

All exceptions include detailed error messages with output and error streams.

## Requirements

- .NET 10
- Network access for downloading Python and packages
- Sufficient disk space for Python installation
- Administrative privileges may be required depending on install arguments

## Notes

- The downloaded installer is cached in the installation directory
- If the installer already exists, it will be reused
- Python executable is searched in common installation paths and PATH
- All operations support cancellation via `CancellationToken`

## Example Output

```
[14:23:45] Loading Python runtime configuration...
[14:23:45] Loading package list configuration...
[14:23:45] Downloading Python 3.11.4...
[14:23:45] Downloading from: https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe
[14:23:50] Downloaded 10,485,760 / 26,214,400 bytes (40.0%)
[14:23:55] Downloaded 20,971,520 / 26,214,400 bytes (80.0%)
[14:24:00] Downloaded 26,214,400 / 26,214,400 bytes (100.0%)
[14:24:00] Download completed: C:\Users\...\Python\python-3.11.4-amd64.exe
[14:24:00] Installing Python runtime...
[14:24:30] Installing Python packages...
[14:24:30] Using Python at: C:\Program Files\Python311\python.exe
[14:24:30] Installing package: tensorflow==2.15.0
[14:25:00] Successfully installed: tensorflow==2.15.0
[14:25:00] Installing package: tf2onnx
[14:25:10] Successfully installed: tf2onnx
[14:25:10] Installing package: numpy
[14:25:15] Successfully installed: numpy
[14:25:15] Installing package: matplotlib
[14:25:20] Successfully installed: matplotlib
[14:25:20] Python environment setup completed successfully!

✓ Python environment setup completed successfully!
```
