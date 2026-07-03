// ============================================================================
// NeuroForge
// File: DatasetConfigTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Unit tests for DatasetConfig class
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
/// Unit tests for DatasetConfig class
/// </summary>
[TestClass]
public class DatasetConfigTests {
    [TestMethod]
    public void DatasetConfig_DefaultConstructor_InitializesProperties() {
        // Arrange & Act
        var config = new DatasetConfig();

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(string.Empty, config.Source);
        Assert.AreEqual(string.Empty, config.Type);
        Assert.AreEqual(false, config.Normalize);
        Assert.IsNull(config.Path);
        Assert.IsNull(config.ValidationSplit);
    }

    [TestMethod]
    public void DatasetConfig_SetProperties_ValuesAreSet() {
        // Arrange
        var config = new DatasetConfig();
        var expectedSource = "file";
        var expectedPath = @"C:\Data\dataset.csv";
        var expectedType = "tabular";
        var expectedNormalize = true;
        var expectedSplit = 0.2;

        // Act
        config.Source = expectedSource;
        config.Path = expectedPath;
        config.Type = expectedType;
        config.Normalize = expectedNormalize;
        config.ValidationSplit = expectedSplit;

        // Assert
        Assert.AreEqual(expectedSource, config.Source);
        Assert.AreEqual(expectedPath, config.Path);
        Assert.AreEqual(expectedType, config.Type);
        Assert.AreEqual(expectedNormalize, config.Normalize);
        Assert.AreEqual(expectedSplit, config.ValidationSplit);
    }

    [TestMethod]
    public void DatasetConfig_Deserialize_FromValidJson_Success() {
        // Arrange
        var json = """
            {
                "source": "file",
                "path": "data/tabular.csv",
                "type": "tabular",
                "normalize": true,
                "validation_split": 0.2
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<DatasetConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("file", config.Source);
        Assert.AreEqual("data/tabular.csv", config.Path);
        Assert.AreEqual("tabular", config.Type);
        Assert.AreEqual(true, config.Normalize);
        Assert.AreEqual(0.2, config.ValidationSplit);
    }

    [TestMethod]
    public void DatasetConfig_Serialize_ToJson_Success() {
        // Arrange
        var config = new DatasetConfig {
            Source = "cifar10",
            Type = "image",
            Normalize = true,
            ValidationSplit = 0.15
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("cifar10"));
        Assert.IsTrue(json.Contains("image"));
        Assert.IsTrue(json.Contains("true") || json.Contains("True"));
        Assert.IsTrue(json.Contains("0.15"));
    }

    [TestMethod]
    public void DatasetConfig_WithExtensionData_HandlesAdditionalProperties() {
        // Arrange
        var json = """
            {
                "source": "file",
                "type": "image",
                "normalize": true,
                "resize": [64, 64],
                "augmentation": true
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<DatasetConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("file", config.Source);
        Assert.IsNotNull(config.ExtensionData);
        Assert.IsTrue(config.ExtensionData.ContainsKey("resize") ||
                      config.ExtensionData.ContainsKey("Resize"));
    }
}
