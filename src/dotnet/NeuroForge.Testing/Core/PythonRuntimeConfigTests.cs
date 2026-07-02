using System.Text.Json;
using NeuroForge.Factory.Core;

namespace NeuroForge.Testing.Core;

/// <summary>
/// Defines test class PythonRuntimeConfigTests.
/// </summary>
[TestClass]
public class PythonRuntimeConfigTests {
    /// <summary>
    /// Defines the test method PythonRuntimeConfig_DefaultConstructor_InitializesProperties.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_DefaultConstructor_InitializesProperties() {
        // Arrange & Act
        var config = new PythonRuntimeConfig();

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual(string.Empty, config.PythonVersion);
        Assert.AreEqual(string.Empty, config.DownloadUrl);
        Assert.IsNotNull(config.InstallArgs);
        Assert.AreEqual(0, config.InstallArgs.Length);
    }

    /// <summary>
    /// Defines the test method PythonRuntimeConfig_SetProperties_ValuesAreSet.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_SetProperties_ValuesAreSet() {
        // Arrange
        var config = new PythonRuntimeConfig();
        var expectedVersion = "3.11.4";
        var expectedUrl = "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe";
        var expectedArgs = new[] { "/quiet", "InstallAllUsers=1" };

        // Act
        config.PythonVersion = expectedVersion;
        config.DownloadUrl = expectedUrl;
        config.InstallArgs = expectedArgs;

        // Assert
        Assert.AreEqual(expectedVersion, config.PythonVersion);
        Assert.AreEqual(expectedUrl, config.DownloadUrl);
        CollectionAssert.AreEqual(expectedArgs, config.InstallArgs);
    }

    [TestMethod]
    public void PythonRuntimeConfig_Deserialize_FromValidJson_Success() {
        // Arrange
        var json = """
            {
                "python_version": "3.11.4",
                "download_url": "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe",
                "install_args": ["/quiet", "InstallAllUsers=1", "PrependPath=1"]
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PythonRuntimeConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("3.11.4", config.PythonVersion);
        Assert.AreEqual("https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe", config.DownloadUrl);
        Assert.IsNotNull(config.InstallArgs);
        Assert.AreEqual(3, config.InstallArgs.Length);
        Assert.AreEqual("/quiet", config.InstallArgs[0]);
        Assert.AreEqual("InstallAllUsers=1", config.InstallArgs[1]);
        Assert.AreEqual("PrependPath=1", config.InstallArgs[2]);
    }

    /// <summary>
    /// Defines the test method PythonRuntimeConfig_Serialize_ToJson_Success.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_Serialize_ToJson_Success() {
        // Arrange
        var config = new PythonRuntimeConfig {
            PythonVersion = "3.11.4",
            DownloadUrl = "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe",
            InstallArgs = ["/quiet", "InstallAllUsers=1"]
        };

        // Act
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        // Assert
        Assert.IsNotNull(json);
        Assert.IsTrue(json.Contains("python_version") || json.Contains("pythonVersion"));
        Assert.IsTrue(json.Contains("3.11.4"));
        Assert.IsTrue(json.Contains("download_url") || json.Contains("downloadUrl"));
        Assert.IsTrue(json.Contains("install_args") || json.Contains("installArgs"));
    }

    /// <summary>
    /// Defines the test method PythonRuntimeConfig_Deserialize_EmptyJson_ReturnsDefaultValues.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_Deserialize_EmptyJson_ReturnsDefaultValues() {
        // Arrange
        var json = "{}";

        // Act
        var config = JsonSerializer.Deserialize<PythonRuntimeConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        // When deserializing empty JSON, properties will be null, not default values
        Assert.IsTrue(config.PythonVersion == null || config.PythonVersion == string.Empty);
        Assert.IsTrue(config.DownloadUrl == null || config.DownloadUrl == string.Empty);
        Assert.IsTrue(config.InstallArgs == null || config.InstallArgs.Length == 0);
    }

    /// <summary>
    /// Defines the test method PythonRuntimeConfig_Deserialize_WithMissingProperties_HandlesGracefully.
    /// </summary>
    [TestMethod]
    public void PythonRuntimeConfig_Deserialize_WithMissingProperties_HandlesGracefully() {
        // Arrange
        var json = """
            {
                "python_version": "3.11.4"
            }
            """;

        // Act
        var config = JsonSerializer.Deserialize<PythonRuntimeConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.IsNotNull(config);
        Assert.AreEqual("3.11.4", config.PythonVersion);
        // Missing properties will be null
        Assert.IsTrue(config.DownloadUrl == null || config.DownloadUrl == string.Empty);
        Assert.IsTrue(config.InstallArgs == null || config.InstallArgs.Length == 0);
    }
}