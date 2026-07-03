// ============================================================================
// NeuroForge
// File: ModelConfigTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Unit tests for ModelConfig class
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

using System.Text.Json;
using NeuroForge.Factory.Core;

namespace NeuroForge.Testing.Core;

/// <summary>
/// Unit tests for ModelConfig class
/// </summary>
[TestClass]
public class ModelConfigTests {
    [TestMethod]
    public void ModelConfig_DefaultConstructor_InitializesProperties() {
        // Arrange & Act
        var config = new ModelConfig();

        // Assert
        Assert.IsNotNull(config);
        Assert.IsNotNull(config.Models);
        Assert.AreEqual(0, config.Models.Count);
    }

    [TestMethod]
    public void ModelConfig_AddModel_ModelIsAdded() {
        // Arrange
        var config = new ModelConfig();
        var modelConfig = new AnnBuilderConfig {
            Type = "mlp",
            InputShape = new[] { 784 },
            NumClasses = 10
        };

        // Act
        config.Models["mlp"] = modelConfig;

        // Assert
        Assert.AreEqual(1, config.Models.Count);
        Assert.IsTrue(config.Models.ContainsKey("mlp"));
        Assert.AreEqual("mlp", config.Models["mlp"].Type);
    }

    [TestMethod]
    public void ModelConfig_AvailableModels_ReturnsModelKeys() {
        // Arrange
        var config = new ModelConfig();
        config.Models["cnn"] = new AnnBuilderConfig { Type = "cnn" };
        config.Models["mlp"] = new AnnBuilderConfig { Type = "mlp" };
        config.Models["rnn"] = new AnnBuilderConfig { Type = "rnn" };

        // Act
        var available = config.AvailableModels.ToList();

        // Assert
        Assert.AreEqual(3, available.Count);
        CollectionAssert.Contains(available, "cnn");
        CollectionAssert.Contains(available, "mlp");
        CollectionAssert.Contains(available, "rnn");
    }

    [TestMethod]
    public void ModelConfig_TryGetModel_ExistingModel_ReturnsTrue() {
        // Arrange
        var config = new ModelConfig();
        var mlpConfig = new AnnBuilderConfig {
            Type = "mlp",
            InputShape = new[] { 1024 },
            NumClasses = 10
        };
        config.Models["mlp"] = mlpConfig;

        // Act
        var result = config.TryGetModel("mlp", out var retrievedConfig);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(retrievedConfig);
        Assert.AreEqual("mlp", retrievedConfig.Type);
        Assert.AreEqual(10, retrievedConfig.NumClasses);
    }

    [TestMethod]
    public void ModelConfig_TryGetModel_NonExistingModel_ReturnsFalse() {
        // Arrange
        var config = new ModelConfig();
        config.Models["cnn"] = new AnnBuilderConfig { Type = "cnn" };

        // Act
        var result = config.TryGetModel("gan", out var retrievedConfig);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(retrievedConfig);
    }

    [TestMethod]
    public void ModelConfig_TryGetModel_CaseInsensitive_ReturnsTrue() {
        // Arrange
        var config = new ModelConfig();
        config.Models["mlp"] = new AnnBuilderConfig {
            Type = "mlp",
            NumClasses = 5
        };

        // Act
        var result = config.TryGetModel("MLP", out var retrievedConfig);

        // Assert
        Assert.IsTrue(result);
        Assert.IsNotNull(retrievedConfig);
        Assert.AreEqual("mlp", retrievedConfig.Type);
    }

    [TestMethod]
    public void ModelConfig_Deserialize_FromFactoryJson_Success() {
        // Arrange
        var json = """
            {
                "models": {
                    "cnn": {
                        "type": "cnn",
                        "input_shape": [64, 64, 3],
                        "num_classes": 200,
                        "dataset": {
                            "source": "tiny-imagenet",
                            "type": "image",
                            "normalize": true
                        },
                        "training": {
                            "epochs": 10,
                            "batch_size": 64
                        }
                    },
                    "mlp": {
                        "type": "mlp",
                        "input_shape": [1024],
                        "num_classes": 10,
                        "dataset": {
                            "source": "file",
                            "type": "tabular",
                            "normalize": true
                        },
                        "training": {
                            "epochs": 20,
                            "batch_size": 32
                        }
                    }
                }
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<ModelConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(2, config.Models.Count);
        Assert.IsTrue(config.Models.ContainsKey("cnn"));
        Assert.IsTrue(config.Models.ContainsKey("mlp"));

        var cnn = config.Models["cnn"];
        Assert.AreEqual("cnn", cnn.Type);
        Assert.AreEqual(200, cnn.NumClasses);
        Assert.IsNotNull(cnn.InputShape);
        Assert.AreEqual(3, cnn.InputShape.Length);

        var mlp = config.Models["mlp"];
        Assert.AreEqual("mlp", mlp.Type);
        Assert.AreEqual(10, mlp.NumClasses);
    }

    [TestMethod]
    public void ModelConfig_Serialize_ToJson_Success() {
        // Arrange
        var config = new ModelConfig();
        config.Models["cnn"] = new AnnBuilderConfig {
            Type = "cnn",
            InputShape = new[] { 32, 32, 3 },
            NumClasses = 10,
            Dataset = new DatasetConfig {
                Source = "cifar10",
                Type = "image"
            },
            Training = new TrainingConfig {
                Epochs = 50,
                BatchSize = 128
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("models"));
        Assert.IsTrue(json.Contains("cnn"));
        Assert.IsTrue(json.Contains("cifar10"));
    }

    [TestMethod]
    public void ModelConfig_WithMultipleModels_HandlesCorrectly() {
        // Arrange
        var config = new ModelConfig();
        var modelTypes = new[] { "cnn", "mlp", "rnn", "autoencoder", "gan", "transformer" };

        // Act
        foreach (var type in modelTypes) {
            config.Models[type] = new AnnBuilderConfig {
                Type = type,
                InputShape = new[] { 100 }
            };
        }

        // Assert
        Assert.AreEqual(6, config.Models.Count);
        foreach (var type in modelTypes) {
            Assert.IsTrue(config.AvailableModels.Contains(type));
            Assert.IsTrue(config.TryGetModel(type, out var model));
            Assert.IsNotNull(model);
        }
    }

    [TestMethod]
    public void ModelConfig_EmptyModels_ReturnsEmptyAvailable() {
        // Arrange
        var config = new ModelConfig();

        // Act
        var available = config.AvailableModels.ToList();

        // Assert
        Assert.AreEqual(0, available.Count);
    }
}
