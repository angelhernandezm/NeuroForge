# NeuroForge

**Build production-grade neural network architectures without leaving C#.**

NeuroForge lets .NET developers build, train, and export neural networks — CNNs, RNNs, GANs,
Transformers, and more — using nothing but strongly-typed C# configuration. Under the hood it
manages a Python/TensorFlow runtime for you and hands back an [ONNX](https://onnx.ai/) model you can
run anywhere: ML.NET, ONNX Runtime, Azure ML, edge devices, mobile.

> Independent open-source project; not affiliated with or endorsed by Microsoft.

## Install

```
dotnet add package NeuroForge
```

## Quick start

```csharp
using NeuroForge.Factory;
using NeuroForge.Factory.Core;

// 1. Set up the Python/TensorFlow runtime (one-time, downloads Python 3.11 + TF 2.15)
var factory = new NeuroForgeFactory();
await factory.InitializeAsync();

// 2. Configure your ANN
var config = new AnnBuilderConfig {
    Type = "cnn",
    InputShape = new[] { 32, 32, 3 },
    NumClasses = 10,
    Dataset = new DatasetConfig { Source = "cifar10", Type = "image", Normalize = true },
    Training = new TrainingConfig { Epochs = 50, BatchSize = 128 }
};

// 3. Build, train, and export
var manager = factory.CreateAnnBuilderManager();
await manager.BuildModelAsync("my_cnn", config);

// Done — model.h5 and model.onnx are ready in your output directory.
```

> **Before you run this:** `InitializeAsync()` installs Python system-wide (`InstallAllUsers=1`),
> which requires an elevated (admin) shell on Windows. Run your terminal as Administrator the first
> time you initialize the runtime.

## Supported architectures

| Architecture | Best for |
|---|---|
| **MLP** | Tabular data, feature-based prediction, simple regression |
| **CNN** | Image classification, defect detection, computer vision |
| **RNN / LSTM** | Time series, forecasting, sequential sensor data |
| **Autoencoder** | Anomaly detection, compression, feature learning |
| **GAN** | Synthetic data generation, data augmentation |
| **Transformer** | Text classification, sentiment, sequence tasks |

Datasets load from image folders, CSV/Excel, NumPy arrays, or built-in sets (CIFAR-10, MNIST, IMDB).
Every trained model is automatically exported to ONNX, so no TensorFlow is required at inference time.

## Requirements

- .NET 10 or later
- Windows 10/11 (Linux/macOS support planned)
- Administrator shell for first-time Python runtime setup
- ~500 MB disk space for the Python environment

## Documentation

Full documentation, architecture parameter reference, dataset guide and ONNX export guide live in the
repository: <https://github.com/angelhernandezm/NeuroForge>

## License

MIT © Angel Hernandez
