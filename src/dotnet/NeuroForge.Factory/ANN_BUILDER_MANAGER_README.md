# ANN Builder Manager - Quick Reference

## Overview
The `AnnBuilderManager` enables C# applications to execute Python-based ANN builders for creating and training neural network models.

## Features
- ✅ Automatic Python installation check
- ✅ Resource extraction (Python scripts and configs)
- ✅ Model registry with 6 pre-configured ANN types
- ✅ Flexible configuration (by name + dataset OR custom config file)
- ✅ Real-time progress reporting
- ✅ Automatic model training and saving
- ✅ **ONNX export** - Models automatically exported to ONNX format

## Supported Model Types
- **CNN** - Convolutional Neural Network (image classification)
- **MLP** - Multi-Layer Perceptron (tabular data)
- **RNN** - Recurrent Neural Network (sequence data)
- **Autoencoder** - Unsupervised learning (dimensionality reduction)
- **GAN** - Generative Adversarial Network (image generation)
- **Transformer** - Attention-based model (NLP tasks)

## Prerequisites
Ensure Python is installed by calling:
```csharp
var factory = new NeuroForgeFactory();
await factory.InitializeAsync();
```

## Basic Usage

### Option 1: Build with Model Name + Dataset
```csharp
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

// Create manager
using var manager = new AnnBuilderManager();

// List available models
Console.WriteLine($"Available: {string.Join(", ", manager.AvailableModels)}");

// Build MLP model with custom dataset
var progress = PythonRuntimeHelper.CreateConsoleProgress();
var modelPath = await manager.BuildModelAsync(
	"mlp",
	@"C:\Data\my_dataset.csv",
	progress
);

Console.WriteLine($"Model saved to: {modelPath}");
```

### Option 2: Build with Custom Config File
```csharp
using NeuroForge.Factory.Core;

// Create manager
using var manager = new AnnBuilderManager();

// Build from custom config
var progress = PythonRuntimeHelper.CreateConsoleProgress();
var modelPath = await manager.BuildModelAsync(
	@"C:\Config\custom_model.json",
	progress
);

Console.WriteLine($"Model saved to: {modelPath}");
```

## Custom Configuration File Format
```json
{
  "type": "cnn",
  "input_shape": [64, 64, 3],
  "num_classes": 10,
  "dataset": {
	"source": "file",
	"path": "C:\\Data\\images",
	"type": "image",
	"normalize": true,
	"validation_split": 0.2
  },
  "params": {
	"filters": [32, 64, 128],
	"kernel_size": 3,
	"dropout": 0.3,
	"optimizer": "adam",
	"loss": "sparse_categorical_crossentropy"
  },
  "training": {
	"epochs": 20,
	"batch_size": 32
  }
}
```

## Progress Reporting
```csharp
// Console progress (with timestamps)
var progress = PythonRuntimeHelper.CreateConsoleProgress(useTimestamp: true);

// File progress
var fileProgress = PythonRuntimeHelper.CreateFileProgress("build_log.txt");

// Custom progress
var customProgress = new Progress<string>(msg => {
	// Your custom handler
	LogToDatabase(msg);
});

await manager.BuildModelAsync("cnn", datasetPath, customProgress);
```

## Working Directory
By default, resources are extracted to:
```
%TEMP%\NeuroForge\Factory\
```

Customize the location:
```csharp
var manager = new AnnBuilderManager(@"C:\MyProject\NeuroForge");
```

## Output Structure
After building, the output directory contains:
```
<output_path>/
├── model.h5                      # Trained Keras model (H5 format)
├── model.onnx                    # Trained model in ONNX format
├── model_onnx_metadata.json      # ONNX model metadata
├── config.json                   # Configuration used
└── training_history.json         # Training metrics
```

**Model Formats:**
- **H5 (.h5)** - Native Keras/TensorFlow format, best for Python usage
- **ONNX (.onnx)** - Open Neural Network Exchange format, cross-platform compatible
  - Can be used with ONNX Runtime, ML.NET, PyTorch, and other frameworks
  - Optimized for inference across different platforms
  - Includes metadata file with opset version and input shape

## Error Handling
```csharp
try {
	var modelPath = await manager.BuildModelAsync("mlp", datasetPath);
} catch (InvalidOperationException ex) {
	// Python not installed or builder failed
	Console.WriteLine($"Build failed: {ex.Message}");
} catch (ArgumentException ex) {
	// Unknown model type
	Console.WriteLine($"Invalid model: {ex.Message}");
} catch (FileNotFoundException ex) {
	// Config or dataset file not found
	Console.WriteLine($"File error: {ex.Message}");
}
```

## Re-extracting Resources
Force re-extraction of Python scripts:
```csharp
manager.ExtractPythonResources(force: true);
```

## API Reference

### AnnBuilderManager Class
**Namespace:** `NeuroForge.Factory.Core`

**Constructor:**
```csharp
public AnnBuilderManager(string? workingDirectory = null)
```

**Properties:**
```csharp
IEnumerable<string> AvailableModels { get; }
```

**Methods:**
```csharp
Task<string> BuildModelAsync(
	string modelName,
	string datasetPath,
	IProgress<string>? progress = null,
	CancellationToken cancellationToken = default)

Task<string> BuildModelAsync(
	string configPath,
	IProgress<string>? progress = null,
	CancellationToken cancellationToken = default)

void ExtractPythonResources(bool force = false)
```

## See Also
- [Python Runtime Manager README](PYTHON_RUNTIME_MANAGER_README.md)
- [Quick Start Guide](QUICK_START.md)
- [Factory Configuration](Resources/Factory/factory_config.json)
