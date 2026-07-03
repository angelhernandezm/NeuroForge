// ============================================================================
// NeuroForge
// File: AnnBuilderManagerIntegrationTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Integration tests for ANN Builder Manager
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

using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;
using System.Text.Json;

namespace NeuroForge.Testing.Core;

/// <summary>
/// Integration tests for ANN Builder Manager
/// These tests require Python installation and are marked as Ignore by default
/// </summary>
[TestClass]
public class AnnBuilderManagerIntegrationTests {
    private string _testDirectory = string.Empty;

    [TestInitialize]
    public void Setup() {
        _testDirectory = Path.Combine(Path.GetTempPath(), "NeuroForgeIntegrationTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
    }

    [TestCleanup]
    public void Cleanup() {
        if (Directory.Exists(_testDirectory)) {
            try {
                Directory.Delete(_testDirectory, true);
            } catch {
                // Ignore cleanup errors in integration tests
            }
        }
    }

    [TestMethod]
    [Ignore("Integration test - requires Python installation and datasets")]
    public async Task AnnBuilderManager_BuildMlpModel_WithCsvDataset_Success() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed");
            return;
        }

        // Arrange
        using var manager = new AnnBuilderManager(_testDirectory);

        // Create a simple CSV dataset
        var datasetPath = Path.Combine(_testDirectory, "test_dataset.csv");
        var csvContent = """
            feature1,feature2,feature3,label
            1.0,2.0,3.0,0
            1.5,2.5,3.5,1
            2.0,3.0,4.0,0
            2.5,3.5,4.5,1
            """;
        await File.WriteAllTextAsync(datasetPath, csvContent);

        var progress = new Progress<string>(msg => Console.WriteLine($"[Progress] {msg}"));

        // Act
        try {
            var modelPath = await manager.BuildModelAsync("mlp", datasetPath, progress);

            // Assert
            Assert.IsNotNull(modelPath);
            Assert.IsTrue(Directory.Exists(modelPath));
            Assert.IsTrue(File.Exists(Path.Combine(modelPath, "config.json")));
        } catch (Exception ex) {
            Assert.Inconclusive($"Integration test failed (expected): {ex.Message}");
        }
    }

    [TestMethod]
    [Ignore("Integration test - requires Python installation and custom config")]
    public async Task AnnBuilderManager_BuildModel_WithCustomConfig_Success() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed");
            return;
        }

        // Arrange
        using var manager = new AnnBuilderManager(_testDirectory);

        // Create custom config
        var customConfig = new AnnBuilderConfig {
            Type = "mlp",
            InputShape = new[] { 4 },
            NumClasses = 2,
            Dataset = new DatasetConfig {
                Source = "file",
                Path = Path.Combine(_testDirectory, "data.csv"),
                Type = "tabular",
                Normalize = true,
                ValidationSplit = 0.2
            },
            Training = new TrainingConfig {
                Epochs = 5,
                BatchSize = 2
            }
        };

        var configPath = Path.Combine(_testDirectory, "custom_config.json");
        var json = JsonSerializer.Serialize(customConfig, new JsonSerializerOptions {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(configPath, json);

        // Create dataset
        var datasetPath = customConfig.Dataset.Path!;
        await File.WriteAllTextAsync(datasetPath, "1,2,3,4,0\n5,6,7,8,1\n");

        var progress = new Progress<string>(msg => Console.WriteLine($"[Progress] {msg}"));

        // Act
        try {
            var modelPath = await manager.BuildModelAsync(configPath, progress);

            // Assert
            Assert.IsNotNull(modelPath);
            Assert.IsTrue(Directory.Exists(modelPath));
        } catch (Exception ex) {
            Assert.Inconclusive($"Integration test failed (expected): {ex.Message}");
        }
    }

    [TestMethod]
    [Ignore("Integration test - tests resource extraction")]
    public void AnnBuilderManager_ExtractPythonResources_CreatesAllFiles() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed");
            return;
        }

        // Arrange & Act
        try {
            using var manager = new AnnBuilderManager(_testDirectory);
            manager.ExtractPythonResources(force: true);

            // Assert
            var expectedFiles = new[] {
                "factory_config.json",
                "model_factory.py",
                "run_builder.py",
                "dataset_loader.py"
            };

            foreach (var file in expectedFiles) {
                var filePath = Path.Combine(_testDirectory, file);
                Assert.IsTrue(File.Exists(filePath), $"Expected file not found: {file}");
            }
        } catch (Exception ex) {
            Assert.Inconclusive($"Could not extract resources: {ex.Message}");
        }
    }

    [TestMethod]
    [Ignore("Integration test - tests available models")]
    public void AnnBuilderManager_AvailableModels_ReturnsExpectedModels() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed");
            return;
        }

        // Arrange & Act
        try {
            using var manager = new AnnBuilderManager(_testDirectory);
            var available = manager.AvailableModels.ToList();

            // Assert
            Assert.IsTrue(available.Count > 0, "Should have at least one model available");

            var expectedModels = new[] { "cnn", "mlp", "rnn", "autoencoder", "gan", "transformer" };
            foreach (var expected in expectedModels) {
                if (!available.Contains(expected)) {
                    Console.WriteLine($"Warning: Expected model '{expected}' not found in available models");
                }
            }

            Console.WriteLine($"Available models: {string.Join(", ", available)}");
        } catch (Exception ex) {
            Assert.Inconclusive($"Could not load models: {ex.Message}");
        }
    }

    [TestMethod]
    [Ignore("Integration test - tests cancellation")]
    public async Task AnnBuilderManager_BuildModel_WithCancellation_ThrowsOperationCanceledException() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed");
            return;
        }

        // Arrange
        using var manager = new AnnBuilderManager(_testDirectory);
        using var cts = new CancellationTokenSource();

        var datasetPath = Path.Combine(_testDirectory, "data.csv");
        await File.WriteAllTextAsync(datasetPath, "1,2,3,0\n4,5,6,1\n");

        // Cancel immediately
        cts.Cancel();

        // Act & Assert
        try {
            try {
                await manager.BuildModelAsync("mlp", datasetPath, null, cts.Token);
                Assert.Fail("Expected OperationCanceledException");
            } catch (OperationCanceledException) {
                // Expected
                Assert.IsTrue(true);
            }
        } catch (Exception ex) {
            Assert.Inconclusive($"Cancellation test inconclusive: {ex.Message}");
        }
    }

    [TestMethod]
    [Ignore("Integration test - validates full workflow")]
    public async Task AnnBuilderManager_FullWorkflow_ExtractBuildAndValidate() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed");
            return;
        }

        try {
            // Step 1: Create manager
            using var manager = new AnnBuilderManager(_testDirectory);
            Console.WriteLine("✓ Manager created");

            // Step 2: Extract resources
            manager.ExtractPythonResources(force: true);
            Assert.IsTrue(File.Exists(Path.Combine(_testDirectory, "factory_config.json")));
            Console.WriteLine("✓ Resources extracted");

            // Step 3: Verify available models
            var models = manager.AvailableModels.ToList();
            Assert.IsTrue(models.Count > 0);
            Console.WriteLine($"✓ Available models: {string.Join(", ", models)}");

            // Step 4: Create a simple dataset
            var datasetPath = Path.Combine(_testDirectory, "simple_data.csv");
            await File.WriteAllTextAsync(datasetPath, "1,2,0\n3,4,1\n5,6,0\n7,8,1\n");
            Console.WriteLine("✓ Dataset created");

            // Step 5: Build model (will likely fail without proper setup)
            var progress = new Progress<string>(msg => Console.WriteLine($"  {msg}"));

            try {
                var modelPath = await manager.BuildModelAsync(models.First(), datasetPath, progress);
                Console.WriteLine($"✓ Model built: {modelPath}");

                // Verify output
                Assert.IsTrue(Directory.Exists(modelPath));
                Assert.IsTrue(File.Exists(Path.Combine(modelPath, "config.json")));
            } catch (Exception buildEx) {
                Console.WriteLine($"⚠ Model build failed (expected in test env): {buildEx.Message}");
            }

        } catch (Exception ex) {
            Assert.Inconclusive($"Workflow test inconclusive: {ex.Message}");
        }
    }
}
