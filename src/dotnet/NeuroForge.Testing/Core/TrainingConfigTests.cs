// ============================================================================
// NeuroForge
// File: TrainingConfigTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Unit tests for TrainingConfig class
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
/// Unit tests for TrainingConfig class
/// </summary>
[TestClass]
public class TrainingConfigTests {
    /// <summary>
    /// Defines the test method TrainingConfig_DefaultConstructor_InitializesProperties.
    /// </summary>
    [TestMethod]
    public void TrainingConfig_DefaultConstructor_InitializesProperties() {
        // Arrange & Act
        var config = new TrainingConfig();

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(0, config.Epochs);
        Assert.AreEqual(0, config.BatchSize);
    }

    /// <summary>
    /// Defines the test method TrainingConfig_SetProperties_ValuesAreSet.
    /// </summary>
    [TestMethod]
    public void TrainingConfig_SetProperties_ValuesAreSet() {
        // Arrange
        var config = new TrainingConfig();
        var expectedEpochs = 50;
        var expectedBatchSize = 32;

        // Act
        config.Epochs = expectedEpochs;
        config.BatchSize = expectedBatchSize;

        // Assert
        Assert.AreEqual(expectedEpochs, config.Epochs);
        Assert.AreEqual(expectedBatchSize, config.BatchSize);
    }

    /// <summary>
    /// Defines the test method TrainingConfig_Deserialize_FromValidJson_Success.
    /// </summary>
    [TestMethod]
    public void TrainingConfig_Deserialize_FromValidJson_Success() {
        // Arrange
        var json = """
            {
                "epochs": 100,
                "batch_size": 64
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<TrainingConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(100, config.Epochs);
        Assert.AreEqual(64, config.BatchSize);
    }

    /// <summary>
    /// Defines the test method TrainingConfig_Serialize_ToJson_Success.
    /// </summary>
    [TestMethod]
    public void TrainingConfig_Serialize_ToJson_Success() {
        // Arrange
        var config = new TrainingConfig {
            Epochs = 25,
            BatchSize = 128
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("25"));
        Assert.IsTrue(json.Contains("128"));
    }

    /// <summary>
    /// Defines the test method TrainingConfig_WithExtensionData_HandlesAdditionalProperties.
    /// </summary>
    [TestMethod]
    public void TrainingConfig_WithExtensionData_HandlesAdditionalProperties() {
        // Arrange
        var json = """
            {
                "epochs": 10,
                "batch_size": 32,
                "learning_rate": 0.001,
                "early_stopping": true
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<TrainingConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(10, config.Epochs);
        Assert.AreEqual(32, config.BatchSize);
        Assert.IsNotNull(config.ExtensionData);
    }

    /// <summary>
    /// Defines the test method TrainingConfig_WithLargeValues_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void TrainingConfig_WithLargeValues_HandlesCorrectly() {
        // Arrange
        var config = new TrainingConfig {
            Epochs = 10000,
            BatchSize = 2048
        };

        // Act
        var json = JsonSerializer.Serialize(config);
        var deserialized = JsonSerializer.Deserialize<TrainingConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(10000, deserialized.Epochs);
        Assert.AreEqual(2048, deserialized.BatchSize);
    }
}