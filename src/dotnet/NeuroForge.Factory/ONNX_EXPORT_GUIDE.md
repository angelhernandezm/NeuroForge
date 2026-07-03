# ONNX Export Integration

## Overview
NeuroForge automatically exports all trained models to **ONNX (Open Neural Network Exchange)** format, in addition to the native Keras H5 format. This enables cross-platform model deployment and inference across multiple frameworks.

## What is ONNX?
ONNX is an open format built to represent machine learning models. It enables:
- **Cross-framework compatibility** - Use models with PyTorch, ML.NET, ONNX Runtime, etc.
- **Cross-platform deployment** - Deploy on Windows, Linux, macOS, mobile, and embedded devices
- **Hardware optimization** - Leverage CPU, GPU, and specialized accelerators
- **Production inference** - Optimized runtime for high-performance serving

## Automatic Export

### During Model Training
When you build a model using `AnnBuilderManager`, the trained model is automatically exported to both formats:

```csharp
var modelPath = await manager.BuildModelAsync("mlp", datasetPath, progress);
```

**Output files:**
- `model.h5` - Native Keras/TensorFlow format
- `model.onnx` - ONNX format (universal)
- `model_onnx_metadata.json` - ONNX conversion metadata

### Conversion Details
- **ONNX Opset**: Version 13 (compatible with most runtimes)
- **Conversion Tool**: tf2onnx (official TensorFlow to ONNX converter)
- **Input Signature**: Automatically inferred from model architecture
- **Error Handling**: If ONNX conversion fails, model is still saved in H5 format with a warning

## Output Structure

```
<output_directory>/
├── model.h5                      # Keras/TensorFlow model
├── model.onnx                    # ONNX model (cross-platform)
├── model_onnx_metadata.json      # Conversion metadata
├── config.json                   # Model configuration
└── training_history.json         # Training metrics
```

## ONNX Metadata
The `model_onnx_metadata.json` file contains:

```json
{
  "format": "ONNX",
  "opset_version": 13,
  "input_shape": [784],
  "framework": "TensorFlow/Keras"
}
```

## Manual Conversion Utility

A standalone conversion utility is included for converting existing H5 models:

### Basic Usage
```bash
python convert_to_onnx.py model.h5
```

### With Custom Output Path
```bash
python convert_to_onnx.py model.h5 -o output/model.onnx
```

### With Specific Opset Version
```bash
python convert_to_onnx.py model.h5 --opset 14
```

### Help
```bash
python convert_to_onnx.py --help
```

## Using ONNX Models

### With ONNX Runtime (Python)
```python
import onnxruntime as ort
import numpy as np

# Load model
session = ort.InferenceSession("model.onnx")

# Get input name
input_name = session.get_inputs()[0].name

# Run inference
input_data = np.random.randn(1, 784).astype(np.float32)
result = session.run(None, {input_name: input_data})
print(result)
```

### With ML.NET (C#)
```csharp
using Microsoft.ML;
using Microsoft.ML.Data;

var mlContext = new MLContext();

// Load ONNX model
var pipeline = mlContext.Transforms.ApplyOnnxModel("model.onnx");

// Create prediction engine
var model = pipeline.Fit(mlContext.Data.LoadFromEnumerable(new List<InputData>()));
var engine = mlContext.Model.CreatePredictionEngine<InputData, OutputData>(model);

// Make prediction
var prediction = engine.Predict(new InputData { Features = inputArray });
```

### With ONNX Runtime (C#)
```csharp
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

// Create session
var session = new InferenceSession("model.onnx");

// Prepare input
var inputTensor = new DenseTensor<float>(inputData, new[] { 1, 784 });
var inputs = new List<NamedOnnxValue> {
	NamedOnnxValue.CreateFromTensor("input", inputTensor)
};

// Run inference
using var results = session.Run(inputs);
var output = results.First().AsEnumerable<float>().ToArray();
```

## Benefits of ONNX Export

### 1. Framework Independence
Train in TensorFlow/Keras, deploy anywhere:
- PyTorch
- ML.NET
- ONNX Runtime
- CoreML
- TensorRT

### 2. Production Optimization
- Smaller model size
- Faster inference
- Hardware-specific optimizations
- Reduced dependencies

### 3. Mobile & Edge Deployment
- iOS (CoreML)
- Android (NNAPI)
- Raspberry Pi
- IoT devices

### 4. Cloud Services
- Azure Machine Learning
- AWS SageMaker
- Google Cloud AI Platform
- Custom inference endpoints

## Requirements

### Python Packages
Already included in `package_list.json`:
```json
{
  "packages": [
	"tensorflow==2.15.0",
	"tf2onnx",
	"numpy"
  ]
}
```

### ONNX Runtime (for inference)
```bash
# Python
pip install onnxruntime

# C# (NuGet)
Install-Package Microsoft.ML.OnnxRuntime
```

## Troubleshooting

### Conversion Fails
If ONNX conversion fails:
1. Model is still saved in H5 format
2. Warning message is displayed
3. Check model architecture compatibility
4. Try different opset version

### Common Issues

**Issue:** "Unsupported operation"
**Solution:** Some custom layers may not be supported. Use standard Keras layers when possible.

**Issue:** "Shape mismatch"
**Solution:** Ensure input shapes are properly defined in the model configuration.

**Issue:** "Opset version too old/new"
**Solution:** Adjust opset version in `convert_to_onnx.py` (default: 13).

## Performance Comparison

| Format | Size (approx) | Load Time | Inference Speed | Portability |
|--------|---------------|-----------|-----------------|-------------|
| H5     | Baseline      | Fast      | Baseline        | TF/Keras only |
| ONNX   | ~90% of H5    | Fast      | 1-2x faster*    | Universal |

*Varies by model and runtime

## Version Compatibility

- **ONNX**: 1.13+
- **tf2onnx**: Latest
- **ONNX Runtime**: 1.14+
- **TensorFlow**: 2.15.0

## Resources

- [ONNX Official Site](https://onnx.ai/)
- [ONNX Runtime](https://onnxruntime.ai/)
- [tf2onnx Documentation](https://github.com/onnx/tensorflow-onnx)
- [ML.NET ONNX](https://docs.microsoft.com/en-us/dotnet/machine-learning/how-to-guides/serve-model-onnx)

## Examples

All trained models through NeuroForge automatically include ONNX exports. Example output:

```
Building MLP model...
[NeuroForge] Training completed
[NeuroForge] Model saved to: C:\Models\mlp_20240115_143022\model.h5
[NeuroForge] Converting model to ONNX format...
[NeuroForge] ONNX model saved to: C:\Models\mlp_20240115_143022\model.onnx
[NeuroForge] ONNX metadata saved to: C:\Models\mlp_20240115_143022\model_onnx_metadata.json
[NeuroForge] Build completed successfully!
```

## Summary

✅ **Automatic ONNX export** for all trained models
✅ **Cross-platform compatibility** with all major frameworks
✅ **Production-ready** optimized inference
✅ **Standalone utility** for converting existing models
✅ **Full metadata** tracking for conversions
✅ **Graceful fallback** if conversion fails

ONNX export is enabled by default and requires no additional configuration. Simply build your models as usual, and both H5 and ONNX formats will be generated automatically.
