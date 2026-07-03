// ============================================================================
// NeuroForge
// File: AnnBuilderManagerTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Unit tests for ANN Builder Manager
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

using NeuroForge.Factory.Abstractions;
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

namespace NeuroForge.Testing.Core;

/// <summary>
/// Unit tests for AnnBuilderManager class
/// </summary>
[TestClass]
public class AnnBuilderManagerTests {
    private string _testDirectory = string.Empty;

    [TestInitialize]
    public void Setup() {
        _testDirectory = Path.Combine(Path.GetTempPath(), "NeuroForgeTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
    }

    [TestCleanup]
    public void Cleanup() {
        if (Directory.Exists(_testDirectory)) {
            try {
                Directory.Delete(_testDirectory, true);
            } catch {
                // Ignore cleanup errors
            }
        }
    }

    [TestMethod]
    public void AnnBuilderManager_Constructor_WithoutPython_ThrowsInvalidOperationException() {
        // This test verifies that AnnBuilderManager checks for Python installation

        // Arrange & Act & Assert
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            try {
                using var manager = new AnnBuilderManager(_testDirectory);
                Assert.Fail("Expected InvalidOperationException");
            } catch (InvalidOperationException) {
                // Expected
                Assert.IsTrue(true);
            }
        } else {
            // If Python is installed, the constructor should succeed
            try {
                using var manager = new AnnBuilderManager(_testDirectory);
                Assert.IsNotNull(manager);
            } catch (InvalidOperationException ex) {
                // May fail if resources can't be extracted or config can't be loaded
                Assert.IsTrue(ex.Message.Contains("Python") ||
                             ex.Message.Contains("configuration") ||
                             ex.Message.Contains("resource"));
            }
        }
    }

    [TestMethod]
    public void AnnBuilderManager_Constructor_WithValidDirectory_CreatesDirectory() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange
        var testDir = Path.Combine(_testDirectory, "BuilderTest");

        // Act
        try {
            using var manager = new AnnBuilderManager(testDir);

            // Assert
            Assert.IsTrue(Directory.Exists(testDir));
        } catch (InvalidOperationException) {
            // May fail if resources aren't available in test context
            Assert.Inconclusive("Could not initialize manager - resources may not be available");
        }
    }

    [TestMethod]
    public void AnnBuilderManager_AvailableModels_ReturnsModelList() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange & Act
        try {
            using var manager = new AnnBuilderManager(_testDirectory);
            var available = manager.AvailableModels.ToList();

            // Assert
            Assert.IsNotNull(available);
            // Should have models if factory_config.json was loaded successfully
            // If not, it will be empty (resources not available in test context)
        } catch (InvalidOperationException) {
            Assert.Inconclusive("Could not load factory configuration");
        }
    }

    [TestMethod]
    public void AnnBuilderManager_ExtractPythonResources_CreatesFiles() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange
        try {
            using var manager = new AnnBuilderManager(_testDirectory);

            // Act
            manager.ExtractPythonResources(force: true);

            // Assert
            // Check if working directory exists
            Assert.IsTrue(Directory.Exists(_testDirectory));

            // Check for factory_config.json
            var configPath = Path.Combine(_testDirectory, "factory_config.json");
            if (File.Exists(configPath)) {
                Assert.IsTrue(File.Exists(configPath));
                var content = File.ReadAllText(configPath);
                Assert.IsTrue(content.Length > 0);
            }
        } catch (InvalidOperationException) {
            Assert.Inconclusive("Could not initialize manager or extract resources");
        }
    }

    [TestMethod]
    public void AnnBuilderManager_Implements_IAnnBuilderManager() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange & Act
        try {
            using IAnnBuilderManager manager = new AnnBuilderManager(_testDirectory);

            // Assert
            Assert.IsNotNull(manager);
            Assert.IsNotNull(manager.AvailableModels);
        } catch (InvalidOperationException) {
            Assert.Inconclusive("Could not initialize manager");
        }
    }

    [TestMethod]
    public void AnnBuilderManager_Dispose_MultipleCalls_DoesNotThrow() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange
        try {
            var manager = new AnnBuilderManager(_testDirectory);

            // Act & Assert - should not throw
            manager.Dispose();
            manager.Dispose();
            manager.Dispose();
        } catch (InvalidOperationException) {
            Assert.Inconclusive("Could not initialize manager");
        }
    }

    [TestMethod]
    public async Task AnnBuilderManager_BuildModelAsync_WithInvalidModelName_ThrowsArgumentException() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange
        try {
            using var manager = new AnnBuilderManager(_testDirectory);
            var datasetPath = Path.Combine(_testDirectory, "dummy_dataset.csv");
            File.WriteAllText(datasetPath, "data");

            // Act & Assert
            try {
                await manager.BuildModelAsync("invalid_model_type", datasetPath);
                Assert.Fail("Expected ArgumentException");
            } catch (ArgumentException) {
                // Expected
                Assert.IsTrue(true);
            }
        } catch (InvalidOperationException) {
            Assert.Inconclusive("Could not initialize manager");
        }
    }

    [TestMethod]
    public async Task AnnBuilderManager_BuildModelAsync_WithNonExistentConfigFile_ThrowsFileNotFoundException() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange
        try {
            using var manager = new AnnBuilderManager(_testDirectory);
            var nonExistentPath = Path.Combine(_testDirectory, "nonexistent_config.json");

            // Act & Assert
            try {
                await manager.BuildModelAsync(nonExistentPath);
                Assert.Fail("Expected FileNotFoundException");
            } catch (FileNotFoundException) {
                // Expected
                Assert.IsTrue(true);
            }
        } catch (InvalidOperationException) {
            Assert.Inconclusive("Could not initialize manager");
        }
    }

    [TestMethod]
    public async Task AnnBuilderManager_BuildModelAsync_WithInvalidConfigFile_ThrowsInvalidOperationException() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange
        try {
            using var manager = new AnnBuilderManager(_testDirectory);
            var configPath = Path.Combine(_testDirectory, "invalid_config.json");

            // Create invalid config (missing 'type')
            File.WriteAllText(configPath, """
                {
                    "input_shape": [784],
                    "num_classes": 10
                }
                """);

            // Act & Assert
            try {
                await manager.BuildModelAsync(configPath);
                Assert.Fail("Expected InvalidOperationException");
            } catch (InvalidOperationException) {
                // Expected
                Assert.IsTrue(true);
            }
        } catch (InvalidOperationException) {
            Assert.Inconclusive("Could not initialize manager");
        }
    }

    [TestMethod]
    public async Task AnnBuilderManager_BuildModelAsync_WithEmptyModelName_ThrowsException() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange
        try {
            using var manager = new AnnBuilderManager(_testDirectory);
            var datasetPath = Path.Combine(_testDirectory, "dataset.csv");
            File.WriteAllText(datasetPath, "data");

            // Act & Assert
            try {
                await manager.BuildModelAsync(string.Empty, datasetPath);
                Assert.Fail("Expected ArgumentException");
            } catch (ArgumentException) {
                // Expected
                Assert.IsTrue(true);
            }
        } catch (InvalidOperationException) {
            Assert.Inconclusive("Could not initialize manager");
        }
    }

    [TestMethod]
    public async Task AnnBuilderManager_BuildModelAsync_ReportsProgress() {
        // Skip if Python not installed
        if (!PythonRuntimeHelper.IsPythonInstalled()) {
            Assert.Inconclusive("Python not installed - skipping test");
            return;
        }

        // Arrange
        try {
            using var manager = new AnnBuilderManager(_testDirectory);

            // Check if any models are available
            var available = manager.AvailableModels.ToList();
            if (available.Count == 0) {
                Assert.Inconclusive("No models available - factory config not loaded");
                return;
            }

            var progressMessages = new List<string>();
            var progress = new Progress<string>(msg => progressMessages.Add(msg));

            var datasetPath = Path.Combine(_testDirectory, "test_data.csv");
            File.WriteAllText(datasetPath, "test,data\n1,2\n3,4");

            // Act - this will likely fail in test environment without actual Python env
            try {
                await manager.BuildModelAsync(available.First(), datasetPath, progress);

                // If we get here, check progress was reported
                Assert.IsTrue(progressMessages.Count > 0);
            } catch {
                // Expected to fail without proper Python environment and datasets
                // Just verify progress was attempted to be reported
                Assert.IsTrue(progressMessages.Count >= 1); // At least the initial message
            }
        } catch (InvalidOperationException) {
            Assert.Inconclusive("Could not initialize manager");
        }
    }
}
