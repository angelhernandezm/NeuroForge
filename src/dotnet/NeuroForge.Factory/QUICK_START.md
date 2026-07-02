# Quick Start Guide - Python Runtime Manager

## Installation

The Python Runtime Manager is part of the `NeuroForge.Factory` namespace and uses embedded JSON resources.

## Quickest Start (One Line)

```csharp
using NeuroForge.Factory;

var factory = new NeuroForgeFactory();
await factory.InitializeAsync();
```

## Simple Example with PythonSetupRunner

```csharp
using NeuroForge.Factory.Support;

// Initialize Python environment
var runner = new PythonSetupRunner();
await runner.SetupPythonAsync();
```

## Step-by-Step Example

```csharp
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

// 1. Check if Python is already installed
if (PythonRuntimeHelper.IsPythonInstalled())
{
	var version = PythonRuntimeHelper.GetPythonVersion();
	Console.WriteLine($"Python found: {version}");
}

// 2. Get default install directory
var installDir = PythonRuntimeHelper.GetDefaultInstallDirectory();

// 3. Create the manager
using var manager = new PythonRuntimeManager(installDir);

// 4. Create progress reporter
var progress = PythonRuntimeHelper.CreateConsoleProgress();

// 5. Setup environment
await manager.SetupPythonEnvironmentAsync(progress, CancellationToken.None);
```

## Customization Examples

### Custom Install Directory
```csharp
using NeuroForge.Factory.Support;

var runner = new PythonSetupRunner();
await runner.SetupPythonAsync(@"D:\MyApps\Python");
```

### With Logging to File
```csharp
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

var logPath = "python_setup.log";
var fileProgress = PythonRuntimeHelper.CreateFileProgress(logPath);

using var manager = new PythonRuntimeManager(@"C:\Python");
await manager.SetupPythonEnvironmentAsync(fileProgress, CancellationToken.None);
```

### With Timeout
```csharp
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));

using var manager = new PythonRuntimeManager(@"C:\Python");
await manager.SetupPythonEnvironmentAsync(
	PythonRuntimeHelper.CreateConsoleProgress(),
	cts.Token);
```

## Configuration

Edit the embedded JSON files to customize:

**`Resources/Runtime/python_runtime.json`** - Python version and installer
```json
{
  "python_version": "3.11.4",
  "download_url": "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe",
  "install_args": [ "/quiet", "InstallAllUsers=1", "PrependPath=1" ]
}
```

**`Resources/Runtime/package_list.json`** - Packages to install
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

## Key Methods

| Method | Purpose |
|--------|---------|
| `NeuroForgeFactory.InitializeAsync()` | High-level initialization with validation |
| `PythonSetupRunner.SetupPythonAsync()` | Simple setup with progress reporting |
| `PythonRuntimeManager.SetupPythonEnvironmentAsync()` | Complete setup (download, install, packages) |
| `PythonRuntimeHelper.IsPythonInstalled()` | Check if Python exists |
| `PythonRuntimeHelper.GetPythonVersion()` | Get installed version |
| `PythonRuntimeHelper.GetDefaultInstallDirectory()` | Get default install path |
| `PythonRuntimeHelper.CreateConsoleProgress()` | Create console progress reporter |
| `PythonRuntimeHelper.CreateFileProgress()` | Create file progress reporter |
| `PythonRuntimeHelper.ValidateEmbeddedResources()` | Verify JSON resources exist |

## Error Handling

```csharp
using NeuroForge.Factory.Core;

try
{
	using var manager = new PythonRuntimeManager(installDir);
	await manager.SetupPythonEnvironmentAsync(progress, token);
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

## Common Scenarios

### Check Before Installing
```csharp
using NeuroForge.Factory.Support;

if (!PythonRuntimeHelper.IsPythonInstalled())
{
	var runner = new PythonSetupRunner();
	await runner.SetupPythonAsync();
}
else
{
	Console.WriteLine("Python already available");
}
```

### Using NeuroForgeFactory with Validation
```csharp
using NeuroForge.Factory;
using NeuroForge.Factory.Support;

if (!PythonRuntimeHelper.ValidateEmbeddedResources(out var missing))
{
	Console.WriteLine($"Missing resource: {missing}");
	return;
}

var factory = new NeuroForgeFactory();
await factory.InitializeAsync();
```

### Background Installation with UI Progress
```csharp
using NeuroForge.Factory.Core;

var progress = new Progress<string>(message =>
{
	Dispatcher.Invoke(() =>
	{
		StatusTextBlock.Text = message;
		LogListBox.Items.Add(message);
	});
});

using var manager = new PythonRuntimeManager(installDir);
await manager.SetupPythonEnvironmentAsync(progress, CancellationToken.None);
```

### Validate Resources at Startup
```csharp
using NeuroForge.Factory.Support;

if (!PythonRuntimeHelper.ValidateEmbeddedResources(out var missing))
{
	MessageBox.Show($"Missing resource: {missing}");
	return;
}
```
	return;
}
```

## Important Notes

- ✅ The manager automatically caches downloaded installers
- ✅ Progress is reported for all major operations
- ✅ Full cancellation support via `CancellationToken`
- ✅ Thread-safe progress reporting via `IProgress<T>`
- ⚠️ Administrator privileges may be required for system-wide installation
- ⚠️ Ensure network connectivity for downloads
- ⚠️ Sufficient disk space required (~500MB+ for Python + packages)
