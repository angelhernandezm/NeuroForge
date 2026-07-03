// ============================================================================
// NeuroForge
// File: TestDataHelper.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Helper methods for creating test data and configurations
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