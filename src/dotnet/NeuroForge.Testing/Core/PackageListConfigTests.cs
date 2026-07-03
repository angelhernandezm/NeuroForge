// ============================================================================
// NeuroForge
// File: PackageListConfigTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Unit tests for PackageListConfig class
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
/// Defines test class PackageListConfigTests.
/// </summary>
[TestClass]
public class PackageListConfigTests {
    /// <summary>
    /// Defines the test method PackageListConfig_DefaultConstructor_InitializesProperties.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_DefaultConstructor_InitializesProperties() {
        // Arrange & Act
        var config = new PackageListConfig();

        // Assert
        Assert.IsNotNull(config);
        Assert.IsNotNull(config.Packages);
        Assert.AreEqual(0, config.Packages.Length);
    }

    /// <summary>
    /// Defines the test method PackageListConfig_SetPackages_ValuesAreSet.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_SetPackages_ValuesAreSet() {
        // Arrange
        var config = new PackageListConfig();
        var expectedPackages = new[] { "tensorflow==2.15.0", "numpy", "matplotlib" };

        // Act
        config.Packages = expectedPackages;

        // Assert
        CollectionAssert.AreEqual(expectedPackages, config.Packages);
    }

    /// <summary>
    /// Defines the test method PackageListConfig_Deserialize_FromValidJson_Success.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_Deserialize_FromValidJson_Success() {
        // Arrange
        var json = """
            {
                "packages": [
                    "tensorflow==2.15.0",
                    "tf2onnx",
                    "numpy",
                    "matplotlib"
                ]
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PackageListConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.IsNotNull(config.Packages);
        Assert.AreEqual(4, config.Packages.Length);
        Assert.AreEqual("tensorflow==2.15.0", config.Packages[0]);
        Assert.AreEqual("tf2onnx", config.Packages[1]);
        Assert.AreEqual("numpy", config.Packages[2]);
        Assert.AreEqual("matplotlib", config.Packages[3]);
    }

    /// <summary>
    /// Defines the test method PackageListConfig_Serialize_ToJson_Success.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_Serialize_ToJson_Success() {
        // Arrange
        var config = new PackageListConfig {
            Packages = ["tensorflow==2.15.0", "numpy", "matplotlib"]
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("packages"));
        Assert.IsTrue(json.Contains("tensorflow==2.15.0"));
        Assert.IsTrue(json.Contains("numpy"));
        Assert.IsTrue(json.Contains("matplotlib"));
    }

    /// <summary>
    /// Defines the test method PackageListConfig_Deserialize_EmptyPackageList_Success.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_Deserialize_EmptyPackageList_Success() {
        // Arrange
        var json = """
            {
                "packages": []
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PackageListConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.IsNotNull(config.Packages);
        Assert.AreEqual(0, config.Packages.Length);
    }

    /// <summary>
    /// Defines the test method PackageListConfig_Deserialize_WithVersionPinning_Success.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_Deserialize_WithVersionPinning_Success() {
        // Arrange
        var json = """
            {
                "packages": [
                    "tensorflow==2.15.0",
                    "numpy>=1.24.0",
                    "matplotlib<=3.8.0",
                    "pandas~=2.0.0"
                ]
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PackageListConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(4, config.Packages.Length);
        Assert.IsTrue(config.Packages[0].Contains("=="));
        Assert.IsTrue(config.Packages[1].Contains(">="));
        Assert.IsTrue(config.Packages[2].Contains("<="));
        Assert.IsTrue(config.Packages[3].Contains("~="));
    }

    /// <summary>
    /// Defines the test method PackageListConfig_Deserialize_EmptyJson_HandlesGracefully.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_Deserialize_EmptyJson_HandlesGracefully() {
        // Arrange
        var json = "{}";

        // Act
        var config = JsonSerializer.Deserialize<PackageListConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        // Empty JSON will not set the property, so it could be null or default (empty array)
        Assert.IsTrue(config.Packages == null || config.Packages.Length == 0);
    }
}