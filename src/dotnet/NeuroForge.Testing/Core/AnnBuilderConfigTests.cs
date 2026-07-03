// ============================================================================
// NeuroForge
// File: AnnBuilderConfigTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Unit tests for AnnBuilderConfig class
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
/// Unit tests for AnnBuilderConfig class
/// </summary>
[TestClass]
public class AnnBuilderConfigTests {
    [TestMethod]
    public void AnnBuilderConfig_DefaultConstructor_InitializesProperties() {
        // Arrange & Act
        var config = new AnnBuilderConfig();

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(string.Empty, config.Type);
        Assert.IsNull(config.InputShape);
        Assert.IsNull(config.NumClasses);
        Assert.IsNotNull(config.Dataset);
        Assert.IsNotNull(config.Training);
    }

    [TestMethod]
    public void AnnBuilderConfig_SetProperties_ValuesAreSet() {
        // Arrange
        var config = new AnnBuilderConfig();
        var expectedType = "cnn";
        var expectedInputShape = new[] { 64, 64, 3 };
        var expectedNumClasses = 10;

        // Act
        config.Type = expectedType;
        config.InputShape = expectedInputShape;
        config.NumClasses = expectedNumClasses;

        // Assert
        Assert.AreEqual(expectedType, config.Type);
        CollectionAssert.AreEqual(expectedInputShape, config.InputShape);
        Assert.AreEqual(expectedNumClasses, config.NumClasses);
    }

    [TestMethod]
    public void AnnBuilderConfig_Deserialize_FromValidJson_Success() {
        // Arrange
        var json = """
            {
                "type": "mlp",
                "input_shape": [784],
                "num_classes": 10,
                "dataset": {
                    "source": "file",
                    "path": "data/mnist.csv",
                    "type": "tabular",
                    "normalize": true,
                    "validation_split": 0.2
                },
                "params": {
                    "units": [128, 64, 32],
                    "dropout": 0.2,
                    "optimizer": "adam"
                },
                "training": {
                    "epochs": 20,
                    "batch_size": 32
                }
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<AnnBuilderConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("mlp", config.Type);
        Assert.IsNotNull(config.InputShape);
        Assert.AreEqual(1, config.InputShape.Length);
        Assert.AreEqual(784, config.InputShape[0]);
        Assert.AreEqual(10, config.NumClasses);
        Assert.IsNotNull(config.Dataset);
        Assert.AreEqual("file", config.Dataset.Source);
        Assert.IsNotNull(config.Training);
        Assert.AreEqual(20, config.Training.Epochs);
        Assert.IsTrue(config.Params.HasValue);
    }

    [TestMethod]
    public void AnnBuilderConfig_Serialize_ToJson_Success() {
        // Arrange
        var config = new AnnBuilderConfig {
            Type = "cnn",
            InputShape = new[] { 32, 32, 3 },
            NumClasses = 100,
            Dataset = new DatasetConfig {
                Source = "cifar100",
                Type = "image",
                Normalize = true
            },
            Training = new TrainingConfig {
                Epochs = 50,
                BatchSize = 64
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("cnn"));
        Assert.IsTrue(json.Contains("32"));
        Assert.IsTrue(json.Contains("100"));
        Assert.IsTrue(json.Contains("cifar100"));
    }

    [TestMethod]
    public void AnnBuilderConfig_WithNullNumClasses_HandlesCorrectly() {
        // Arrange
        var json = """
            {
                "type": "autoencoder",
                "input_shape": [1024],
                "num_classes": null,
                "dataset": {
                    "source": "file",
                    "type": "tabular"
                },
                "training": {
                    "epochs": 100,
                    "batch_size": 32
                }
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<AnnBuilderConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("autoencoder", config.Type);
        Assert.IsNull(config.NumClasses);
    }

    [TestMethod]
    public void AnnBuilderConfig_RoundTrip_PreservesData() {
        // Arrange
        var original = new AnnBuilderConfig {
            Type = "rnn",
            InputShape = new[] { 50, 32 },
            NumClasses = 5,
            Dataset = new DatasetConfig {
                Source = "file",
                Path = "sequences.json",
                Type = "sequence",
                Normalize = false,
                ValidationSplit = 0.25
            },
            Training = new TrainingConfig {
                Epochs = 30,
                BatchSize = 16
            }
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<AnnBuilderConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(original.Type, deserialized.Type);
        CollectionAssert.AreEqual(original.InputShape, deserialized.InputShape);
        Assert.AreEqual(original.NumClasses, deserialized.NumClasses);
        Assert.AreEqual(original.Dataset.Source, deserialized.Dataset.Source);
        Assert.AreEqual(original.Training.Epochs, deserialized.Training.Epochs);
    }

    [TestMethod]
    public void AnnBuilderConfig_ComplexInputShape_HandlesCorrectly() {
        // Arrange
        var config = new AnnBuilderConfig {
            Type = "transformer",
            InputShape = new[] { 128, 64 }
        };

        // Act
        var json = JsonSerializer.Serialize(config);
        var deserialized = JsonSerializer.Deserialize<AnnBuilderConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.IsNotNull(deserialized.InputShape);
        Assert.AreEqual(2, deserialized.InputShape.Length);
        Assert.AreEqual(128, deserialized.InputShape[0]);
        Assert.AreEqual(64, deserialized.InputShape[1]);
    }
}
