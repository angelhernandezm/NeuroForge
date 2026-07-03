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

    // Step 3: Build a model (example with MLP)
    Console.WriteLine("Step 3: Building MLP model...");

    var progress = PythonRuntimeHelper.CreateConsoleProgress();

    // Option 1: Build with model name and dataset path
    // Uncomment to test:
    // var datasetPath = @"C:\Data\my_dataset.csv";
    // var modelPath = await builderManager.BuildModelAsync("mlp", datasetPath, progress);

    // Option 2: Build with custom config file
    // Uncomment to test:
    // var configPath = @"C:\Config\my_custom_config.json";
    // var modelPath = await builderManager.BuildModelAsync(configPath, progress);

    Console.WriteLine("\n===========================================");
    Console.WriteLine("Demo completed successfully!");
    Console.WriteLine("===========================================");
    Console.WriteLine("\nNote: To actually build a model, uncomment one of the");
    Console.WriteLine("BuildModelAsync examples above and provide a valid dataset path.");
    Console.WriteLine("\nPress any key to exit...");

} catch (Exception ex) {
    Console.WriteLine($"\n[ERROR] {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    Console.WriteLine("\nPress any key to exit...");
}

Console.ReadLine();
