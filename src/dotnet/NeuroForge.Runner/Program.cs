// ============================================================================
// NeuroForge
// File: Program.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Runner for NeuroForge - Neural Network Factory
//
// License: MIT
// ============================================================================
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ============================================================================

using NeuroForge.Factory;
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

Console.WriteLine("===========================================");
Console.WriteLine("NeuroForge - Neural Network Factory Runner");
Console.WriteLine("===========================================\n");

try {
    // Step 1: Initialize Python environment
    Console.WriteLine("Step 1: Initializing Python environment...");
    var factory = new NeuroForgeFactory();
    await factory.InitializeAsync();
    Console.WriteLine("✓ Python environment initialized\n");

    // Step 2: Create ANN Builder Manager
    Console.WriteLine("Step 2: Initializing ANN Builder Manager...");
    using var builderManager = new AnnBuilderManager();
    Console.WriteLine($"✓ Builder Manager initialized");
    Console.WriteLine($"Available models: {string.Join(", ", builderManager.AvailableModels)}\n");

    // Step 3: Build a model (example with CNN using CIFAR-10)
    Console.WriteLine("Step 3: Building CNN model with CIFAR-10 dataset...");
    var progress = PythonRuntimeHelper.CreateConsoleProgress();

    // Using built-in CIFAR-10 dataset (simplest option - no download needed)
    Console.WriteLine("\n📦 Using built-in CIFAR-10 dataset");
    Console.WriteLine("   - 50,000 training images (32x32 RGB)");
    Console.WriteLine("   - 10 classes (airplane, car, bird, cat, deer, dog, frog, horse, ship, truck)\n");

    #region "Custom config JSON example - using CIFAR10"

    // Create a custom config JSON and save it temporarily
    var configJson = @"{
        ""type"": ""cnn"",
        ""input_shape"": [32, 32, 3],
        ""num_classes"": 10,
        ""dataset"": {
            ""source"": ""cifar10"",
            ""type"": ""image"",
            ""normalize"": true,
            ""validation_split"": 0.2
        },
        ""params"": {
            ""filters"": [32, 64],
            ""kernel_size"": 3,
            ""dense_units"": 128,
            ""dropout"": 0.3,
            ""optimizer"": ""adam"",
            ""loss"": ""sparse_categorical_crossentropy""
        },
        ""training"": {
            ""epochs"": 5,
            ""batch_size"": 64
        }
    }";

    #endregion

    // Save config to temporary file
    var tempConfigPath = Path.Combine(Path.GetTempPath(), "neuroforge_cnn_cifar10.json");
    await File.WriteAllTextAsync(tempConfigPath, configJson);

    Console.WriteLine("🔧 Configuration prepared. Starting training...\n");
    var modelPath = await builderManager.BuildModelAsync(tempConfigPath, progress);

    Console.WriteLine($"\n✅ Model trained and exported to: {modelPath}");

    // Clean up temp config
    if (File.Exists(tempConfigPath))
        File.Delete(tempConfigPath);

    #region "Other options to build models"

    // Option 2: Build with custom CSV dataset (for tabular/MLP)
    // Uncomment to test with your own data:
    // var datasetPath = @"C:\Data\my_dataset.csv";
    // var modelPath = await builderManager.BuildModelAsync("mlp", datasetPath, progress);

    // Option 3: Build with custom config file
    // Uncomment to test:
    // var configPath = @"C:\Config\my_custom_config.json";
    // var modelPath = await builderManager.BuildModelAsync(configPath, progress);

    #endregion

    Console.WriteLine("\n===========================================");
    Console.WriteLine("✅ NeuroForge Demo Completed Successfully!");
    Console.WriteLine("===========================================");
    Console.WriteLine("\n📊 What just happened:");
    Console.WriteLine("   1. Python + TensorFlow environment initialized");
    Console.WriteLine("   2. CIFAR-10 dataset loaded (50,000 images)");
    Console.WriteLine("   3. CNN model trained for 5 epochs");
    Console.WriteLine("   4. Model exported to ONNX format");
    Console.WriteLine("\n💡 Next steps:");
    Console.WriteLine("   - Deploy the .onnx model with ML.NET");
    Console.WriteLine("   - Try other datasets (see DATASETS.md)");
    Console.WriteLine("   - Experiment with other ANN types (MLP, RNN, GAN, etc.)");
    Console.WriteLine("\n\nPress any key to exit...");

} catch (Exception ex) {
    Console.WriteLine($"\n[ERROR] {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Console.WriteLine("\nPress any key to exit...");
}

Console.ReadLine();