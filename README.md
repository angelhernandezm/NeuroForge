<p align="center">
  <img src="https://img.shields.io/badge/NeuroForge-ANN%20Engine%20for%20.NET-blue?style=for-the-badge" alt="NeuroForge" />
</p>

# NeuroForge

**Build production-grade neural network architectures without leaving C#.**

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![TensorFlow](https://img.shields.io/badge/TensorFlow-2.15-FF6F00?style=flat&logo=tensorflow)](https://www.tensorflow.org/)
[![ONNX](https://img.shields.io/badge/ONNX-Export-5C3EE8?style=flat)](https://onnx.ai/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat)](LICENSE)
[![Tests](https://img.shields.io/badge/tests-3%2C100%2B%20lines-brightgreen?style=flat)](src/dotnet/NeuroForge.Testing)

[Quick Start](#-quick-start) • [Architectures](#-six-ann-architectures) • [How It Works](#-how-it-works) • [Docs](#-documentation) • [Project Status](#-project-status)

---

## 🎯 What is NeuroForge?

**NeuroForge** lets .NET developers build, train, and export neural networks — CNNs, RNNs, GANs, Transformers, and more — using nothing but strongly-typed C# configuration. Under the hood, it manages a Python/TensorFlow runtime for you and hands back an [ONNX](https://onnx.ai/) model you can run anywhere: ML.NET, Azure ML, edge devices, mobile.

You write C#. NeuroForge handles the rest.

> **A quick note on "battle-tested":** the six architectures below (CNN, RNN/LSTM, GAN, etc.) are decades of proven, peer-reviewed ML research — that part is genuinely battle-tested. NeuroForge itself, the C#-to-Python engine wrapping them, is a young, actively-developed project. See [Project Status](#-project-status) for where things honestly stand.

### Why "NeuroForge"?

In metallurgy, a forge transforms raw material into a finished tool. NeuroForge transforms your data into a trained, deployable neural network — all from the comfort of .NET.

---

## 🧠 The Problem It Solves

Training a neural network normally means leaving your .NET codebase entirely:

```
Without NeuroForge                    With NeuroForge
─────────────────────                 ─────────────────
Install Python manually          →    Bootstrapped automatically
Manage TensorFlow/venv conflicts →    Locked, tested dependency set
Write & debug Python scripts     →    Strongly-typed C# config
Export models by hand            →    ONNX export built in
Stitch together separate infra   →    One C# API, start to finish
```

You still get real TensorFlow under the hood — you just never have to touch it directly.

---

## 🏗️ Six ANN Architectures

Each is configurable via a typed C# object or plain JSON, and runs through the same unified API.

| Architecture | Best for |
|---|---|
| 🔗 **MLP** | Tabular data, feature-based prediction, simple regression |
| 🖼️ **CNN** | Image classification, defect detection, computer vision |
| 🔄 **RNN / LSTM** | Time series, forecasting, sequential sensor data |
| 🔐 **Autoencoder** | Anomaly detection, compression, feature learning |
| 🎨 **GAN** | Synthetic data generation, data augmentation |
| 🔮 **Transformer** | Text classification, sentiment, sequence tasks |

<details>
<summary><strong>Example: CNN for quality-control defect detection</strong></summary>

```csharp
var config = new AnnBuilderConfig {
    Type = "cnn",
    InputShape = new[] { 224, 224, 3 },
    NumClasses = 4,  // Good, Scratch, Dent, Discolored
    Dataset = new DatasetConfig {
        Type = "image",
        Path = "data/product_images",
        ExtensionData = new Dictionary<string, object> {
            ["target_size"] = new[] { 224, 224 }
        }
    }
};
```
</details>

<details>
<summary><strong>Example: LSTM for energy consumption forecasting</strong></summary>

```csharp
var config = new AnnBuilderConfig {
    Type = "rnn",
    InputShape = new[] { 168, 1 },  // 1 week of hourly data
    NumClasses = 1,
    Params = new Dictionary<string, object> {
        ["use_lstm"] = true,
        ["units"] = 256
    }
};
```
</details>

<details>
<summary><strong>Example: Autoencoder for fraud/anomaly detection</strong></summary>

```csharp
var config = new AnnBuilderConfig {
    Type = "autoencoder",
    InputShape = new[] { 50 },
    Params = new Dictionary<string, object> {
        ["encoder_units"] = new[] { 32, 16 },
        ["bottleneck"] = 8,
        ["decoder_units"] = new[] { 16, 32 }
    }
};
// High reconstruction error = anomaly
```
</details>

Full parameter reference for every architecture: [ANN_BUILDER_MANAGER_README.md](src/dotnet/NeuroForge.Factory/ANN_BUILDER_MANAGER_README.md)

---

## 🚀 Quick Start

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

> ⚠️ **Before you run this:** `InitializeAsync()` installs Python system-wide (`InstallAllUsers=1`), which requires an elevated (admin) shell on Windows. Run your terminal as Administrator the first time you initialize the runtime.

---

## ⚙️ How It Works

```
┌─────────────────────────────────────────────────────┐
│                  NeuroForge Engine                   │
├─────────────────────────────────────────────────────┤
│   C# API  →  Config Validation  →  Python Runtime    │
│                       ↓                              │
│         ANN Builder Factory (6 architectures)        │
│                       ↓                              │
│    Dataset Loader          Training Engine           │
│  (image/CSV/NumPy)         (TensorFlow, tracked)     │
│                       ↓                              │
│         model.h5  +  model.onnx  (auto-export)       │
└─────────────────────────────────────────────────────┘
```

Datasets load from image folders, CSV/Excel, NumPy arrays, or built-in sets (CIFAR-10, MNIST, IMDB). Every trained model is automatically exported to **ONNX**, so it runs in ML.NET, ONNX Runtime, Azure ML, or on edge/mobile — no TensorFlow required at inference time.

```csharp
// Deploy with ML.NET — no Python needed here
var mlContext = new MLContext();
var onnxModel = mlContext.Transforms.ApplyOnnxModel("outputs/classifier.onnx");
```

---

## 📚 Documentation

| Document | Covers |
|---|---|
| [QUICK_START.md](src/dotnet/NeuroForge.Factory/QUICK_START.md) | 5-minute getting-started walkthrough |
| [DATASETS.md](src/dotnet/NeuroForge.Factory/DATASETS.md) | Dataset formats and download links |
| [ANN_BUILDER_MANAGER_README.md](src/dotnet/NeuroForge.Factory/ANN_BUILDER_MANAGER_README.md) | Deep dive into each architecture's parameters |
| [PYTHON_RUNTIME_MANAGER_README.md](src/dotnet/NeuroForge.Factory/PYTHON_RUNTIME_MANAGER_README.md) | How the Python bootstrap works |
| [ONNX_EXPORT_GUIDE.md](src/dotnet/NeuroForge.Factory/ONNX_EXPORT_GUIDE.md) | Export and deployment strategies |

---

## 🧪 Project Status

NeuroForge is an early-stage, actively-developed, single-maintainer project — not a mature framework yet. In the interest of being upfront:

**What's solid today:**
- A real test suite (~3,100 lines) covering config parsing, dataset loading, and package handling, plus integration tests that exercise the full build pipeline against a live Python install.
- The GAN builder implements a correct alternating adversarial training loop (frozen-discriminator combined model, `train_on_batch` for both networks) — this is hand-written, not templated.
- The six wrapped architectures are standard, well-understood ML approaches with years of production use behind them elsewhere.

**What's still rough:**
- **Windows only** for now; Linux/macOS support is planned, not yet built.
- The Python installer download has no checksum verification yet.
- `InstallAllUsers=1` requires an elevated shell — there's no automatic elevation prompt yet.
- No published release/NuGet package yet — clone and build from source.

If any of that matters for your use case, please open an issue — it helps prioritize what gets fixed next.

---

## 🔧 Requirements

- .NET 10 or later
- Windows 10/11 (Linux/macOS support planned)
- Administrator shell for first-time Python runtime setup
- ~500MB disk space for the Python environment

---

## 🤝 Contributing

Contributions are very welcome, especially in:

- 🐧 Linux/macOS support
- 🔒 Checksum verification for the Python installer download
- 🔬 Additional ANN architectures
- 📦 Additional dataset loaders (streaming/batched image loading for large datasets)
- 🧪 More test coverage

See [open issues](https://github.com/angelhernandezm/NeuroForge/issues) or start a [discussion](https://github.com/angelhernandezm/NeuroForge/discussions).

---

## 📄 License

MIT License — see [LICENSE](LICENSE).

## 👨‍💻 Author

**Angel Hernandez**
📧 me@angelhernandezm.com
🐙 [github.com/angelhernandezm](https://github.com/angelhernandezm)

---

<p align="center"><strong>⭐ Star this repo if NeuroForge is useful to you — it helps others find it too.</strong></p>
