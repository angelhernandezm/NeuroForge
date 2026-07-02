using NeuroForge.Factory.Core;

namespace NeuroForge.Testing.Helpers;

/// <summary>
/// Helper methods for creating test data and configurations
/// </summary>
public static class TestDataHelper {
    /// <summary>
    /// Creates a valid PythonRuntimeConfig for testing
    /// </summary>
    public static PythonRuntimeConfig CreateValidRuntimeConfig() {
        return new PythonRuntimeConfig {
            PythonVersion = "3.11.4",
            DownloadUrl = "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe",
            InstallArgs = ["/quiet", "InstallAllUsers=1", "PrependPath=1"]
        };
    }

    /// <summary>
    /// Creates a valid PackageListConfig for testing
    /// </summary>
    public static PackageListConfig CreateValidPackageConfig() {
        return new PackageListConfig {
            Packages = [
                "tensorflow==2.15.0",
                "tf2onnx",
                "numpy",
                "matplotlib"
            ]
        };
    }

    /// <summary>
    /// Creates valid JSON string for PythonRuntimeConfig
    /// </summary>
    public static string CreateValidRuntimeConfigJson() {
        return """
            {
                "python_version": "3.11.4",
                "download_url": "https://www.python.org/ftp/python/3.11.4/python-3.11.4-amd64.exe",
                "install_args": ["/quiet", "InstallAllUsers=1", "PrependPath=1"]
            }
            """;
    }

    /// <summary>
    /// Creates valid JSON string for PackageListConfig
    /// </summary>
    public static string CreateValidPackageConfigJson() {
        return """
            {
                "packages": [
                    "tensorflow==2.15.0",
                    "tf2onnx",
                    "numpy",
                    "matplotlib"
                ]
            }
            """;
    }

    /// <summary>
    /// Creates a temporary test directory
    /// </summary>
    public static string CreateTempTestDirectory() {
        var testDir = Path.Combine(Path.GetTempPath(), "NeuroForgeTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);
        return testDir;
    }

    /// <summary>
    /// Safely deletes a test directory
    /// </summary>
    public static void DeleteTestDirectory(string directory) {
        if (Directory.Exists(directory)) {
            try {
                Directory.Delete(directory, recursive: true);
            } catch {
                // Best effort cleanup
            }
        }
    }

    /// <summary>
    /// Creates a test progress reporter that collects messages
    /// </summary>
    public static (IProgress<string> Progress, List<string> Messages) CreateTestProgress() {
        var messages = new List<string>();
        var progress = new Progress<string>(msg => messages.Add(msg));
        return (progress, messages);
    }

    /// <summary>
    /// Creates fake Python installer content for testing
    /// </summary>
    public static byte[] CreateFakePythonInstaller() {
        // Create a small fake installer file (just some bytes)
        var fakeContent = new byte[1024];
        new Random().NextBytes(fakeContent);
        return fakeContent;
    }

    /// <summary>
    /// Verifies that a list of progress messages contains expected patterns
    /// </summary>
    public static void AssertProgressContains(List<string> messages, params string[] expectedPatterns) {
        foreach (var pattern in expectedPatterns) {
            Assert.IsTrue(
                messages.Any(m => m.Contains(pattern, StringComparison.OrdinalIgnoreCase)),
                $"Expected progress message containing '{pattern}' was not found. Messages: {string.Join(", ", messages)}");
        }
    }
}