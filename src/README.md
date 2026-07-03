# NeuroForge

<div align="center">

![NeuroForge](https://img.shields.io/badge/NeuroForge-ANN%20Engine%20for%20.NET-blue?style=for-the-badge)

**The Enterprise ANN Creation Engine for .NET Developers**

Build, Train, and Deploy Neural Networks Entirely from C# 

[![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![TensorFlow](https://img.shields.io/badge/TensorFlow-2.15-FF6F00?style=flat-square&logo=tensorflow)](https://www.tensorflow.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)
[![ONNX](https://img.shields.io/badge/ONNX-Export-5C3EE8?style=flat-square)](https://onnx.ai/)

[Quick Start](#-quick-start) • [ANN Architectures](#-six-production-ready-ann-architectures) • [Documentation](#-documentation) • [Examples](#-examples)

</div>

---

## 🎯 What is NeuroForge?

**NeuroForge** is a complete **Artificial Neural Network (ANN) creation engine** for .NET developers. It's your one-stop solution for building, training, and deploying production-ready neural networks without ever leaving the C# ecosystem.

Think of it as a **"Neural Network Factory"** that gives .NET enterprise developers the power to:
- 🏗️ **Build** ANNs using strongly-typed C# configuration
- 🎓 **Train** models with TensorFlow under the hood (automatic setup)
- 📦 **Export** models to ONNX format for universal deployment, so anyone can run inference locally
- 🚀 **Deploy** anywhere: ML.NET, Azure ML, edge devices, mobile apps

### Why "NeuroForge"?

In metallurgy, a **forge** transforms raw materials into powerful tools. Similarly, **NeuroForge transforms your data into intelligent neural networks**—all from the comfort of .NET.

---

## 🧠 The ANN Creation Engine

### What Makes NeuroForge Different?

NeuroForge isn't just another ML wrapper—it's a **purpose-built engine** for creating ANNs in .NET:

```
Traditional Approach:
├─ Install Python manually
├─ Set up virtual environments
├─ Write training scripts
├─ Manage dependencies
├─ Export models separately
├─ Build .NET inference separately
└─ Hope everything works together

NeuroForge Approach:
└─ One C# API call
   └─ Everything handled automatically ✨
```

### The Problem NeuroForge Solves

**For .NET Teams:**
```
❌ Python environment hell
❌ Dependency conflicts between TensorFlow versions
❌ Context switching between languages
❌ No IntelliSense for model configuration
❌ Separate infrastructure for training vs inference
❌ Complex deployment pipelines
```

**NeuroForge's Solution:**
```
✅ Automatic Python environment bootstrap
✅ Locked, tested dependency versions
✅ Pure C# development experience
✅ Strongly-typed config with IntelliSense
✅ ONNX export built-in for inference
✅ Standard .NET deployment
```

---

## 🏗️ Six Production-Ready ANN Architectures

NeuroForge comes with **six battle-tested neural network architectures**, each optimized for specific use cases. All are configurable via JSON and accessible through a unified C# API.

### 1. 🔗 MLP (Multi-Layer Perceptron)

**The Classic Feed-Forward ANN**

**Best For:**
- Tabular data classification
- Feature-based predictions
- Simple regression tasks
- Financial forecasting

**Architecture Details:**
- **Input:** 1D feature vectors (e.g., `[784]` for flattened images, `[20]` for tabular data)
- **Layers:** Configurable dense layers with ReLU activation
- **Dropout:** Built-in regularization to prevent overfitting
- **Output:** Softmax for classification, linear for regression

**Configuration:**
```json
{
  "type": "mlp",
  "input_shape": [784],
  "num_classes": 10,
  "params": {
	"units": [128, 64, 32],  // Hidden layer sizes
	"dropout": 0.2,
	"optimizer": "adam",
	"loss": "sparse_categorical_crossentropy"
  }
}
```

**Real-World Example:**
```csharp
// Predict customer churn from customer features
var config = new AnnBuilderConfig {
	Type = "mlp",
	InputShape = new[] { 20 },  // 20 customer features
	NumClasses = 2,  // Churn or not
	Params = new Dictionary<string, object> {
		["units"] = new[] { 64, 32 },
		["dropout"] = 0.3
	}
};
```

---

### 2. 🖼️ CNN (Convolutional Neural Network)

**The Image Recognition Powerhouse**

**Best For:**
- Image classification
- Object detection
- Computer vision tasks
- Medical image analysis

**Architecture Details:**
- **Input:** 3D image tensors (e.g., `[224, 224, 3]` for RGB images)
- **Layers:** Conv2D → MaxPooling → Flatten → Dense
- **Filters:** Progressive feature extraction (32 → 64 → 128)
- **Output:** Softmax for multi-class classification

**Configuration:**
```json
{
  "type": "cnn",
  "input_shape": [128, 128, 3],
  "num_classes": 5,
  "params": {
	"filters": [32, 64, 128],  // Convolutional filters
	"kernel_size": 3,
	"dense_units": 256,
	"dropout": 0.3
  }
}
```

**Real-World Example:**
```csharp
// Quality control: Classify product defects from images
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

---

### 3. 🔄 RNN/LSTM (Recurrent Neural Network)

**The Sequence Processing Specialist**

**Best For:**
- Time series forecasting
- Stock price prediction
- Sensor data analysis
- Sequential pattern recognition

**Architecture Details:**
- **Input:** 2D sequences (e.g., `[60, 1]` for 60 timesteps with 1 feature)
- **Layers:** LSTM or SimpleRNN → Dropout → Dense
- **Memory:** Long Short-Term Memory handles long sequences
- **Output:** Single value (regression) or classes (classification)

**Configuration:**
```json
{
  "type": "rnn",
  "input_shape": [60, 1],
  "num_classes": 1,
  "params": {
	"use_lstm": true,
	"units": 128,
	"dropout": 0.2,
	"loss": "mse"  // Regression
  }
}
```

**Real-World Example:**
```csharp
// Predict next hour's energy consumption
var config = new AnnBuilderConfig {
	Type = "rnn",
	InputShape = new[] { 168, 1 },  // 1 week of hourly data
	NumClasses = 1,  // Next hour prediction
	Params = new Dictionary<string, object> {
		["use_lstm"] = true,
		["units"] = 256
	}
};
```

---

### 4. 🔐 Autoencoder

**The Dimensionality Reduction Master**

**Best For:**
- Anomaly detection
- Data compression
- Feature learning
- Noise reduction

**Architecture Details:**
- **Input:** Any shape (typically flattened data)
- **Encoder:** Progressive dimension reduction
- **Bottleneck:** Compressed latent representation
- **Decoder:** Reconstruction back to original dimensions

**Configuration:**
```json
{
  "type": "autoencoder",
  "input_shape": [784],
  "params": {
	"encoder_units": [128, 64],
	"bottleneck": 32,
	"decoder_units": [64, 128],
	"loss": "mse"
  }
}
```

**Real-World Example:**
```csharp
// Detect fraudulent transactions by learning normal patterns
var config = new AnnBuilderConfig {
	Type = "autoencoder",
	InputShape = new[] { 50 },  // 50 transaction features
	Params = new Dictionary<string, object> {
		["encoder_units"] = new[] { 32, 16 },
		["bottleneck"] = 8,
		["decoder_units"] = new[] { 16, 32 }
	}
};
// High reconstruction error = anomaly!
```

---

### 5. 🎨 GAN (Generative Adversarial Network)

**The Creative Data Generator**

**Best For:**
- Image generation
- Data augmentation
- Synthetic data creation
- Style transfer

**Architecture Details:**
- **Generator:** Creates fake samples from noise
- **Discriminator:** Distinguishes real from fake
- **Training:** Adversarial learning (generator vs discriminator)
- **Output:** Generated images/data

**Configuration:**
```json
{
  "type": "gan",
  "input_shape": [32, 32, 3],
  "num_classes": 1,
  "dataset": {
	"source": "cifar10",
	"class_selection": 0  // Train on one class
  }
}
```

**Real-World Example:**
```csharp
// Generate synthetic medical images for training data augmentation
var config = new AnnBuilderConfig {
	Type = "gan",
	InputShape = new[] { 128, 128, 1 },  // Grayscale medical images
	Dataset = new DatasetConfig {
		Type = "image",
		Path = "data/xray_images",
		ExtensionData = new Dictionary<string, object> {
			["color_mode"] = "grayscale"
		}
	}
};
```

---

### 6. 🔮 Transformer

**The Attention-Based Language Processor**

**Best For:**
- Text classification
- Sentiment analysis
- Sequence-to-sequence tasks
- Natural language processing

**Architecture Details:**
- **Input:** 2D embeddings (e.g., `[128, 768]` for text)
- **Layers:** Multi-head self-attention
- **Mechanism:** Parallel processing of sequences
- **Output:** Classification or generation

**Configuration:**
```json
{
  "type": "transformer",
  "input_shape": [128, 768],
  "num_classes": 2,
  "params": {
	"num_heads": 8,
	"num_layers": 4,
	"ff_dim": 256,
	"dropout": 0.1
  }
}
```

**Real-World Example:**
```csharp
// Classify customer support tickets by urgency
var config = new AnnBuilderConfig {
	Type = "transformer",
	InputShape = new[] { 256, 512 },  // 256 tokens, 512-dim embeddings
	NumClasses = 3,  // Low, Medium, High urgency
	Params = new Dictionary<string, object> {
		["num_heads"] = 8,
		["num_layers"] = 6
	}
};
```

---

## 🚀 Quick Start: Build Your First ANN in 30 Seconds

```csharp
using NeuroForge.Factory;
using NeuroForge.Factory.Core;

// 1. Initialize the NeuroForge engine
var factory = new NeuroForgeFactory();
await factory.InitializeAsync();  // Handles ALL Python/TensorFlow setup ✨

// 2. Configure your ANN (CNN for image classification)
var config = new AnnBuilderConfig {
	Type = "cnn",
	InputShape = new[] { 32, 32, 3 },
	NumClasses = 10,
	Dataset = new DatasetConfig {
		Source = "cifar10",
		Type = "image",
		Normalize = true
	},
	Training = new TrainingConfig {
		Epochs = 50,
		BatchSize = 128
	}
};

// 3. Create and train your ANN
var manager = factory.CreateAnnBuilderManager();
await manager.BuildModelAsync("my_cnn", config);

// 4. Done! Your ANN is trained and exported to ONNX
Console.WriteLine("✅ ANN trained and saved as model.h5");
Console.WriteLine("✅ ONNX model exported as model.onnx");
```

**Behind the scenes, NeuroForge:**
- ✅ Downloaded Python 3.11
- ✅ Installed TensorFlow 2.15
- ✅ Loaded CIFAR-10 dataset
- ✅ Built the CNN architecture
- ✅ Trained for 50 epochs
- ✅ Saved the Keras model
- ✅ Exported to ONNX format

---

## ⚙️ How the ANN Engine Works

### Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│              NeuroForge Engine                      │
├─────────────────────────────────────────────────────┤
│                                                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────┐ │
│  │              │→ │ Config       │→ │ Python   │ │
│  │   C# API     │  │ Validation   │  │ Runtime  │ │
│  └──────────────┘  └──────────────┘  └──────────┘ │
│         ↓                  ↓                ↓      │
│  ┌──────────────────────────────────────────────┐ │
│  │         ANN Builder Factory                  │ │
│  │  ┌─────┐ ┌─────┐ ┌─────┐ ┌──────┐ ┌──────┐  │ │
│  │  │ MLP │ │ CNN │ │ RNN │ │ Auto │ │ GAN  │  │ │
│  │  └─────┘ └─────┘ └─────┘ └──────┘ └──────┘  │ │
│  │          ┌───────────┐                        │ │
│  │          │Transformer│                        │ │
│  │          └───────────┘                        │ │
│  └──────────────────────────────────────────────┘ │
│         ↓                                          │
│  ┌──────────────────┐  ┌──────────────────┐      │
│  │ Dataset Loader   │  │ Training Engine  │      │
│  │ - Image folders  │  │ - TensorFlow     │      │
│  │ - CSV/Excel      │  │ - Progress track │      │
│  │ - NumPy arrays   │  │ - Validation     │      │
│  └──────────────────┘  └──────────────────┘      │
│         ↓                       ↓                  │
│  ┌──────────────────────────────────────────────┐ │
│  │           Model Export                       │ │
│  │  ┌────────────────┐  ┌──────────────────┐   │ │
│  │  │ model.h5       │  │ model.onnx       │   │ │
│  │  │ (TensorFlow)   │  │ (Universal)      │   │ │
│  │  └────────────────┘  └──────────────────┘   │ │
│  └──────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
```

---

## 📦 Smart Dataset Loading

NeuroForge's dataset engine supports multiple formats with automatic preprocessing:

### Image Folders (CNN-Ready)
```csharp
var config = new DatasetConfig {
	Type = "image",
	Path = "data/my_images",
	ExtensionData = new Dictionary<string, object> {
		["target_size"] = new[] { 224, 224 },
		["color_mode"] = "rgb",
		["normalize"] = true
	}
};
```

### CSV/Excel (MLP-Ready)
```csharp
var config = new DatasetConfig {
	Type = "tabular",
	Path = "data/features.csv",
	Normalize = true,
	ExtensionData = new Dictionary<string, object> {
		["target_column"] = "label"
	}
};
```

### NumPy Arrays (RNN-Ready)
```csharp
var config = new DatasetConfig {
	Type = "tabular",
	Path = "data/sequences.npz",
	ExtensionData = new Dictionary<string, object> {
		["x_key"] = "features",
		["y_key"] = "labels"
	}
};
```

**Supported Formats:**
- ✅ CSV, Excel, TSV
- ✅ NumPy (`.npy`, `.npz`)
- ✅ Image folders with auto class detection
- ✅ Built-in datasets (CIFAR-10, MNIST, etc.)

---

## 🎛️ Configuration-Driven ANN Creation

Define entire neural networks in JSON:

```json
{
  "models": {
	"image_classifier": {
	  "type": "cnn",
	  "input_shape": [224, 224, 3],
	  "num_classes": 100,
	  "params": {
		"filters": [64, 128, 256],
		"dropout": 0.4
	  },
	  "dataset": {
		"type": "image",
		"path": "data/ImageNet_subset"
	  },
	  "training": {
		"epochs": 100,
		"batch_size": 64
	  }
	},
	"fraud_detector": {
	  "type": "autoencoder",
	  "input_shape": [50],
	  "params": {
		"bottleneck": 10
	  },
	  "dataset": {
		"type": "tabular",
		"path": "data/transactions.csv"
	  }
	}
  }
}
```

Load and train:
```csharp
var json = File.ReadAllText("ann_config.json");
var modelConfig = JsonSerializer.Deserialize<ModelConfig>(json);

foreach (var (name, config) in modelConfig.Models) {
	await manager.BuildModelAsync(name, config);
}
```

---

## 🔄 Automatic ONNX Export

Every ANN is automatically exported to **ONNX** (Open Neural Network Exchange) format:

```
outputs/
├── my_model.h5                    # TensorFlow/Keras model
├── my_model.onnx                  # ONNX model (universal!)
└── my_model_onnx_metadata.json    # Model metadata
```

### Why ONNX?

ONNX is the **universal model format** that works everywhere:

✅ **ML.NET** - Run inference in C#  
✅ **ONNX Runtime** - Production servers (Windows/Linux/macOS)  
✅ **Azure ML** - Cloud deployment  
✅ **Edge Devices** - IoT, Raspberry Pi  
✅ **Mobile** - iOS (Core ML), Android (NNAPI)  

**Example: Use your trained ANN in ML.NET:**
```csharp
// Train with NeuroForge
await manager.BuildModelAsync("classifier", config);

// Deploy with ML.NET
var mlContext = new MLContext();
var onnxModel = mlContext.Transforms.ApplyOnnxModel("outputs/classifier.onnx");
```

---

## 📚 Complete Documentation

| Document | Description |
|----------|-------------|
| [QUICK_START.md](src/dotnet/NeuroForge.Factory/QUICK_START.md) | 5-minute getting started guide |
| [DATASETS.md](src/dotnet/NeuroForge.Factory/DATASETS.md) | Dataset guide with download links for all ANN types |
| [ANN_BUILDER_MANAGER_README.md](src/dotnet/NeuroForge.Factory/ANN_BUILDER_MANAGER_README.md) | Deep dive into each ANN architecture |
| [PYTHON_RUNTIME_MANAGER_README.md](src/dotnet/NeuroForge.Factory/PYTHON_RUNTIME_MANAGER_README.md) | Python environment automation |
| [ONNX_EXPORT_GUIDE.md](src/dotnet/NeuroForge.Factory/ONNX_EXPORT_GUIDE.md) | ONNX export and deployment strategies |
| [DATASETLOADER_IMPLEMENTATION.md](src/dotnet/NeuroForge.Factory/DATASETLOADER_IMPLEMENTATION.md) | Dataset loader internals |

---

## 🎓 Real-World Examples

### Example 1: Product Defect Detection (CNN)

```csharp
var factory = new NeuroForgeFactory();
await factory.InitializeAsync();

var config = new AnnBuilderConfig {
	Type = "cnn",
	InputShape = new[] { 224, 224, 3 },
	NumClasses = 4,  // Good, Scratch, Dent, Crack
	Dataset = new DatasetConfig {
		Type = "image",
		Path = "data/quality_control_images",
		ExtensionData = new Dictionary<string, object> {
			["target_size"] = new[] { 224, 224 }
		}
	},
	Params = new Dictionary<string, object> {
		["filters"] = new[] { 32, 64, 128 },
		["dropout"] = 0.3
	},
	Training = new TrainingConfig {
		Epochs = 100,
		BatchSize = 32
	}
};

var manager = factory.CreateAnnBuilderManager();
await manager.BuildModelAsync("defect_detector", config);
```

### Example 2: Sales Forecasting (RNN/LSTM)

```csharp
var config = new AnnBuilderConfig {
	Type = "rnn",
	InputShape = new[] { 30, 5 },  // 30 days, 5 features per day
	NumClasses = 1,  // Next day sales
	Params = new Dictionary<string, object> {
		["use_lstm"] = true,
		["units"] = 128
	},
	Dataset = new DatasetConfig {
		Type = "tabular",
		Path = "data/sales_history.csv",
		Normalize = true
	}
};

await manager.BuildModelAsync("sales_forecaster", config);
```

### Example 3: Customer Segmentation (Autoencoder)

```csharp
var config = new AnnBuilderConfig {
	Type = "autoencoder",
	InputShape = new[] { 100 },  // 100 customer features
	Params = new Dictionary<string, object> {
		["encoder_units"] = new[] { 64, 32 },
		["bottleneck"] = 16,
		["decoder_units"] = new[] { 32, 64 }
	},
	Dataset = new DatasetConfig {
		Type = "tabular",
		Path = "data/customer_data.csv",
		Normalize = true
	}
};

await manager.BuildModelAsync("customer_encoder", config);
// Use bottleneck layer for clustering!
```

---

## 🏆 Why Choose NeuroForge for ANN Creation?

### For .NET Developers

| Without NeuroForge | With NeuroForge |
|-------------------|-----------------|
| Install Python manually | ✅ Automatic |
| Learn TensorFlow API | ✅ C# API only |
| Write training scripts | ✅ JSON config |
| Debug Python errors | ✅ .NET debugging |
| Export models manually | ✅ Auto ONNX export |
| **Est. Time: Days** | **Est. Time: Minutes** |

### For Teams

✅ **Standardization** - One ANN engine across all projects  
✅ **Maintainability** - Pure .NET codebase  
✅ **Productivity** - No context switching  
✅ **Quality** - Type-safe configurations  
✅ **Deployment** - Standard .NET pipelines  

### For Enterprises

✅ **Security** - No external Python in production  
✅ **Compliance** - .NET audit trails  
✅ **Support** - Backed by .NET LTS  
✅ **Scalability** - Kubernetes/Docker ready  
✅ **Cost** - Reduced training time = lower costs  

---

## 🔧 Requirements

- **.NET 10** or later
- **Windows 10/11** (Linux/macOS support planned)
- **~500MB** disk space for Python environment

---


## 🤝 Contributing

We welcome contributions! See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

**Areas we'd love help with:**
- 🐧 Linux/macOS support
- 🔬 Additional ANN architectures
- 📦 More dataset loaders
- 🧪 Test coverage

---

## 📄 License

MIT License - see [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Angel Hernandez**  
📧 [me@angelhernandezm.com](mailto:me@angelhernandezm.com)  
🐙 [github.com/angelhernandezm](https://github.com/angelhernandezm)

---

## 🙏 Acknowledgments

- **TensorFlow Team** - Powering the ANN training
- **ONNX Community** - Universal model format
- **.NET Team** - Modern runtime and tooling

---

<div align="center">

**⭐ Star this repo if NeuroForge helped you build ANNs!**

[Report Bug](https://github.com/angelhernandezm/NeuroForge/issues) • [Request ANN Architecture](https://github.com/angelhernandezm/NeuroForge/issues) • [Discuss](https://github.com/angelhernandezm/NeuroForge/discussions)

---

**Made with ❤️ for .NET developers who want to build ANNs**

![GitHub stars](https://img.shields.io/github/stars/angelhernandezm/NeuroForge?style=social)
![GitHub forks](https://img.shields.io/github/forks/angelhernandezm/NeuroForge?style=social)

</div>
