// ============================================================================
// NeuroForge
// File: DatasetLoaderTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Unit tests for DatasetLoader functionality
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
using System.Text.Json;

namespace NeuroForge.Testing.Core;

/// <summary>
/// Unit tests for DatasetLoader Python integration
/// </summary>
[TestClass]
public class DatasetLoaderTests {
    /// <summary>
    /// Defines the test method DatasetConfig_WithImageFolder_SerializesCorrectly.
    /// </summary>
    [TestMethod]
    public void DatasetConfig_WithImageFolder_SerializesCorrectly() {
        // Arrange
        var config = new DatasetConfig {
            Type = "image",
            Source = "folder",
            Path = "data/images",
            Normalize = true,
            ExtensionData = new Dictionary<string, object> {
                ["target_size"] = new[] { 128, 128 },
                ["color_mode"] = "rgb",
                ["batch_size"] = 32
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("image"));
        Assert.IsTrue(json.Contains("data/images"));
    }

    /// <summary>
    /// Defines the test method DatasetConfig_WithCSV_SerializesCorrectly.
    /// </summary>
    [TestMethod]
    public void DatasetConfig_WithCSV_SerializesCorrectly() {
        // Arrange
        var config = new DatasetConfig {
            Type = "tabular",
            Source = "file",
            Path = "data/iris.csv",
            Normalize = true,
            ExtensionData = new Dictionary<string, object> {
                ["target_column"] = "species"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("tabular"));
        Assert.IsTrue(json.Contains("iris.csv"));
    }

    /// <summary>
    /// Defines the test method DatasetConfig_WithNumPyFile_SerializesCorrectly.
    /// </summary>
    [TestMethod]
    public void DatasetConfig_WithNumPyFile_SerializesCorrectly() {
        // Arrange
        var config = new DatasetConfig {
            Type = "tabular",
            Source = "file",
            Path = "data/features.npy",
            Normalize = false,
            ExtensionData = new Dictionary<string, object> {
                ["target_column"] = 10
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("tabular"));
        Assert.IsTrue(json.Contains("features.npy"));
    }

    /// <summary>
    /// Defines the test method DatasetConfig_WithNumPyArchive_SerializesCorrectly.
    /// </summary>
    [TestMethod]
    public void DatasetConfig_WithNumPyArchive_SerializesCorrectly() {
        // Arrange
        var config = new DatasetConfig {
            Type = "tabular",
            Source = "file",
            Path = "data/dataset.npz",
            Normalize = true,
            ExtensionData = new Dictionary<string, object> {
                ["x_key"] = "features",
                ["y_key"] = "labels"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("tabular"));
        Assert.IsTrue(json.Contains("dataset.npz"));
    }

    /// <summary>
    /// Defines the test method DatasetConfig_WithExcel_SerializesCorrectly.
    /// </summary>
    [TestMethod]
    public void DatasetConfig_WithExcel_SerializesCorrectly() {
        // Arrange
        var config = new DatasetConfig {
            Type = "tabular",
            Source = "file",
            Path = "data/analysis.xlsx",
            Normalize = true,
            ExtensionData = new Dictionary<string, object> {
                ["target_column"] = "result",
                ["features"] = new[] { "col1", "col2", "col3" }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("tabular"));
        Assert.IsTrue(json.Contains("analysis.xlsx"));
    }

    /// <summary>
    /// Defines the test method AnnBuilderConfig_WithImageFolderDataset_RoundTrip.
    /// </summary>
    [TestMethod]
    public void AnnBuilderConfig_WithImageFolderDataset_RoundTrip() {
        // Arrange
        var json = """
            {
                "type": "cnn",
                "input_shape": [128, 128, 3],
                "num_classes": 5,
                "dataset": {
                    "type": "image",
                    "source": "folder",
                    "path": "data/my_images",
                    "target_size": [128, 128],
                    "color_mode": "rgb",
                    "normalize": true
                },
                "training": {
                    "epochs": 50,
                    "batch_size": 64
                }
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<AnnBuilderConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("cnn", config.Type);
        Assert.AreEqual(5, config.NumClasses);
        Assert.IsNotNull(config.Dataset);
        Assert.AreEqual("image", config.Dataset.Type);
    }

    /// <summary>
    /// Defines the test method AnnBuilderConfig_WithCSVDataset_RoundTrip.
    /// </summary>
    [TestMethod]
    public void AnnBuilderConfig_WithCSVDataset_RoundTrip() {
        // Arrange
        var json = """
            {
                "type": "mlp",
                "input_shape": [4],
                "num_classes": 3,
                "dataset": {
                    "type": "tabular",
                    "source": "file",
                    "path": "data/iris.csv",
                    "target_column": "species",
                    "normalize": true
                },
                "training": {
                    "epochs": 100,
                    "batch_size": 16
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
        Assert.AreEqual(3, config.NumClasses);
        Assert.IsNotNull(config.Dataset);
        Assert.AreEqual("tabular", config.Dataset.Type);
    }

    /// <summary>
    /// Defines the test method DatasetConfig_ImageFolder_AllParameters_SerializesCorrectly.
    /// </summary>
    [TestMethod]
    public void DatasetConfig_ImageFolder_AllParameters_SerializesCorrectly() {
        // Arrange
        var config = new DatasetConfig {
            Type = "image",
            Source = "folder",
            Path = "/path/to/images",
            Normalize = true,
            ExtensionData = new Dictionary<string, object> {
                ["target_size"] = new int[] { 224, 224 },
                ["color_mode"] = "rgb",
                ["batch_size"] = 64
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        var deserialized = JsonSerializer.Deserialize<DatasetConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual("image", deserialized.Type);
        Assert.AreEqual(true, deserialized.Normalize);
    }

    /// <summary>
    /// Defines the test method DatasetConfig_TabularCSV_ColumnNames_SerializesCorrectly.
    /// </summary>
    [TestMethod]
    public void DatasetConfig_TabularCSV_ColumnNames_SerializesCorrectly() {
        // Arrange
        var config = new DatasetConfig {
            Type = "tabular",
            Source = "file",
            Path = "wine_quality.csv",
            Normalize = true,
            ExtensionData = new Dictionary<string, object> {
                ["target_column"] = "quality",
                ["separator"] = ",",
                ["features"] = new[] { "alcohol", "pH", "density" }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config);
        var deserialized = JsonSerializer.Deserialize<DatasetConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual("tabular", deserialized.Type);
        Assert.AreEqual(true, deserialized.Normalize);
    }

    /// <summary>
    /// Defines the test method DatasetConfig_TabularNumPy_ColumnIndex_SerializesCorrectly.
    /// </summary>
    [TestMethod]
    public void DatasetConfig_TabularNumPy_ColumnIndex_SerializesCorrectly() {
        // Arrange
        var config = new DatasetConfig {
            Type = "tabular",
            Source = "file",
            Path = "features_labels.npy",
            Normalize = false,
            ExtensionData = new Dictionary<string, object> {
                ["target_column"] = 20
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config);
        var deserialized = JsonSerializer.Deserialize<DatasetConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual("tabular", deserialized.Type);
        Assert.AreEqual(false, deserialized.Normalize);
    }

    /// <summary>
    /// Defines the test method ModelConfig_WithMultipleDatasetTypes_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void ModelConfig_WithMultipleDatasetTypes_HandlesCorrectly() {
        // Arrange
        var config = new ModelConfig();

        config.Models["cnn_images"] = new AnnBuilderConfig {
            Type = "cnn",
            InputShape = new[] { 128, 128, 3 },
            NumClasses = 10,
            Dataset = new DatasetConfig {
                Type = "image",
                Source = "folder"
            }
        };

        config.Models["mlp_csv"] = new AnnBuilderConfig {
            Type = "mlp",
            InputShape = new[] { 20 },
            NumClasses = 5,
            Dataset = new DatasetConfig {
                Type = "tabular",
                Source = "file"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        var deserialized = JsonSerializer.Deserialize<ModelConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(2, deserialized.Models.Count);
        Assert.IsTrue(deserialized.Models.ContainsKey("cnn_images"));
        Assert.IsTrue(deserialized.Models.ContainsKey("mlp_csv"));
        Assert.AreEqual("image", deserialized.Models["cnn_images"].Dataset.Type);
        Assert.AreEqual("tabular", deserialized.Models["mlp_csv"].Dataset.Type);
    }
}