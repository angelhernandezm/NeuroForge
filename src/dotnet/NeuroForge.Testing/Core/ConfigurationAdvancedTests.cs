// ============================================================================
// NeuroForge
// File: ConfigurationAdvancedTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Advanced unit tests for PythonRuntimeConfig
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
using NeuroForge.Testing.Helpers;
using System.Text.Json;

namespace NeuroForge.Testing.Core;

/// <summary>
/// Defines test class PythonRuntimeConfigAdvancedTests.
/// </summary>
[TestClass]
public class PythonRuntimeConfigAdvancedTests {
    /// <summary>
    /// Defines the test method PythonRuntimeConfig_WithSpecialCharactersInUrl_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_WithSpecialCharactersInUrl_HandlesCorrectly() {
        // Arrange
        var config = new PythonRuntimeConfig {
            PythonVersion = "3.11.4",
            DownloadUrl = "https://example.com/python-3.11.4%20(2).exe",
            InstallArgs = ["/quiet"]
        };

        // Act
        var json = JsonSerializer.Serialize(config);
        var deserialized = JsonSerializer.Deserialize<PythonRuntimeConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(config.DownloadUrl, deserialized.DownloadUrl);
    }

    /// <summary>
    /// Defines the test method PythonRuntimeConfig_WithEmptyInstallArgs_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_WithEmptyInstallArgs_HandlesCorrectly() {
        // Arrange
        var json = """
            {
                "python_version": "3.11.4",
                "download_url": "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe",
                "install_args": []
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PythonRuntimeConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.IsNotNull(config.InstallArgs);
        Assert.AreEqual(0, config.InstallArgs.Length);
    }

    /// <summary>
    /// Defines the test method PythonRuntimeConfig_WithLongUrl_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_WithLongUrl_HandlesCorrectly() {
        // Arrange
        var longUrl = "https://www.example.com/" + new string('a', 1000) + "/python.exe";
        var config = new PythonRuntimeConfig {
            PythonVersion = "3.11.4",
            DownloadUrl = longUrl,
            InstallArgs = ["/quiet"]
        };

        // Act
        var json = JsonSerializer.Serialize(config);
        var deserialized = JsonSerializer.Deserialize<PythonRuntimeConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(longUrl, deserialized.DownloadUrl);
    }

    /// <summary>
    /// Defines the test method PythonRuntimeConfig_WithUnicodeCharacters_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_WithUnicodeCharacters_HandlesCorrectly() {
        // Arrange
        var config = new PythonRuntimeConfig {
            PythonVersion = "3.11.4 中文",
            DownloadUrl = "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe",
            InstallArgs = ["/quiet", "InstallDir=C:\\Pythön"]
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
        var deserialized = JsonSerializer.Deserialize<PythonRuntimeConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(config.PythonVersion, deserialized.PythonVersion);
    }

    /// <summary>
    /// Defines the test method PythonRuntimeConfig_WithMalformedJson_ThrowsJsonException.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_WithMalformedJson_ThrowsJsonException() {
        // Arrange
        var malformedJson = """
            {
                "python_version": "3.11.4",
                "download_url": "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe"
                "install_args": ["/quiet"]
            }
            """;

        // Act & Assert
        try {
            JsonSerializer.Deserialize<PythonRuntimeConfig>(malformedJson);
            Assert.Fail("Expected JsonException");
        } catch (JsonException) {
            // Expected
            Assert.IsTrue(true);
        }
    }

    /// <summary>
    /// Defines the test method PythonRuntimeConfig_RoundTrip_PreservesAllData.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_RoundTrip_PreservesAllData() {
        // Arrange
        var original = TestDataHelper.CreateValidRuntimeConfig();

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<PythonRuntimeConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(original.PythonVersion, deserialized.PythonVersion);
        Assert.AreEqual(original.DownloadUrl, deserialized.DownloadUrl);
        CollectionAssert.AreEqual(original.InstallArgs, deserialized.InstallArgs);
    }
}

[TestClass]
public class PackageListConfigAdvancedTests {
    /// <summary>
    /// Defines the test method PackageListConfig_WithComplexVersionSpecifiers_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_WithComplexVersionSpecifiers_HandlesCorrectly() {
        // Arrange
        var json = """
            {
                "packages": [
                    "tensorflow==2.15.0",
                    "numpy>=1.24.0,<2.0.0",
                    "matplotlib~=3.8.0",
                    "pandas>=2.0.0",
                    "scipy[extra]==1.11.0"
                ]
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PackageListConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(5, config.Packages.Length);
        Assert.IsTrue(config.Packages[1].Contains(">=") && config.Packages[1].Contains("<"));
        Assert.IsTrue(config.Packages[4].Contains("[extra]"));
    }

    /// <summary>
    /// Defines the test method PackageListConfig_WithGitUrls_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_WithGitUrls_HandlesCorrectly() {
        // Arrange
        var json = """
            {
                "packages": [
                    "git+https://github.com/user/repo.git@main#egg=package",
                    "package @ git+https://github.com/user/repo.git"
                ]
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PackageListConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(2, config.Packages.Length);
        Assert.IsTrue(config.Packages[0].Contains("git+https://"));
    }

    /// <summary>
    /// Defines the test method PackageListConfig_WithLocalPaths_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_WithLocalPaths_HandlesCorrectly() {
        // Arrange
        var json = """
            {
                "packages": [
                    "./local/package",
                    "C:\\packages\\my_package.whl"
                ]
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PackageListConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(2, config.Packages.Length);
    }

    /// <summary>
    /// Defines the test method PackageListConfig_WithHundredsOfPackages_HandlesCorrectly.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_WithHundredsOfPackages_HandlesCorrectly() {
        // Arrange
        var packages = Enumerable.Range(1, 500)
            .Select(i => $"package{i}==1.0.{i}")
            .ToArray();

        var config = new PackageListConfig { Packages = packages };

        // Act
        var json = JsonSerializer.Serialize(config);
        var deserialized = JsonSerializer.Deserialize<PackageListConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        Assert.AreEqual(500, deserialized.Packages.Length);
        Assert.AreEqual("package1==1.0.1", deserialized.Packages[0]);
        Assert.AreEqual("package500==1.0.500", deserialized.Packages[499]);
    }

    /// <summary>
    /// Defines the test method PackageListConfig_WithDuplicatePackages_PreservesDuplicates.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_WithDuplicatePackages_PreservesDuplicates() {
        // Arrange
        var json = """
            {
                "packages": [
                    "numpy==1.24.0",
                    "numpy==1.24.0",
                    "numpy==1.25.0"
                ]
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PackageListConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(3, config.Packages.Length);
    }

    /// <summary>
    /// Defines the test method PackageListConfig_RoundTrip_PreservesOrder.
    /// </summary>
    [TestMethod]
    public void PackageListConfig_RoundTrip_PreservesOrder() {
        // Arrange
        var original = TestDataHelper.CreateValidPackageConfig();

        // Act
        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<PackageListConfig>(json);

        // Assert
        Assert.IsNotNull(deserialized);
        CollectionAssert.AreEqual(original.Packages, deserialized.Packages);
    }
}
