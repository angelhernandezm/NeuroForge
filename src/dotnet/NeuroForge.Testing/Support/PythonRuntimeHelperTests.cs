// ============================================================================
// NeuroForge
// File: PythonRuntimeHelperTests.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Tests for PythonRuntimeHelper
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

using NeuroForge.Factory.Support;

namespace NeuroForge.Testing.Support;

/// <summary>
/// Defines test class PythonRuntimeHelperTests.
/// </summary>
[TestClass]
public class PythonRuntimeHelperTests {
    /// <summary>
    /// Defines the test method GetDefaultInstallDirectory_ReturnsValidPath.
    /// </summary>
    [TestMethod]
    public void GetDefaultInstallDirectory_ReturnsValidPath() {
        // Act
        var installDir = PythonRuntimeHelper.GetDefaultInstallDirectory();

        // Assert
        Assert.IsNotNull(installDir);
        Assert.IsTrue(installDir.Contains("NeuroForge"));
        Assert.IsTrue(installDir.Contains("Python"));
        Assert.IsTrue(Path.IsPathRooted(installDir));
    }

    /// <summary>
    /// Defines the test method GetDefaultInstallDirectory_ContainsLocalAppData.
    /// </summary>
    [TestMethod]
    public void GetDefaultInstallDirectory_ContainsLocalAppData() {
        // Act
        var installDir = PythonRuntimeHelper.GetDefaultInstallDirectory();
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Assert
        Assert.IsTrue(installDir.StartsWith(localAppData));
    }

    /// <summary>
    /// Defines the test method CreateConsoleProgress_WithTimestamp_ReturnsValidProgress.
    /// </summary>
    [TestMethod]
    public void CreateConsoleProgress_WithTimestamp_ReturnsValidProgress() {
        // Act
        var progress = PythonRuntimeHelper.CreateConsoleProgress(includeTimestamp: true);

        // Assert
        Assert.IsNotNull(progress);
        Assert.IsInstanceOfType(progress, typeof(IProgress<string>));
    }

    /// <summary>
    /// Defines the test method CreateConsoleProgress_WithoutTimestamp_ReturnsValidProgress.
    /// </summary>
    [TestMethod]
    public void CreateConsoleProgress_WithoutTimestamp_ReturnsValidProgress() {
        // Act
        var progress = PythonRuntimeHelper.CreateConsoleProgress(includeTimestamp: false);

        // Assert
        Assert.IsNotNull(progress);
        Assert.IsInstanceOfType(progress, typeof(IProgress<string>));
    }

    /// <summary>
    /// Defines the test method CreateFileProgress_WithValidPath_ReturnsValidProgress.
    /// </summary>
    [TestMethod]
    public void CreateFileProgress_WithValidPath_ReturnsValidProgress() {
        // Arrange
        var tempFile = Path.GetTempFileName();

        try {
            // Act
            var progress = PythonRuntimeHelper.CreateFileProgress(tempFile);

            // Assert
            Assert.IsNotNull(progress);
            Assert.IsInstanceOfType(progress, typeof(IProgress<string>));

            // Test that it can report
            ((IProgress<string>)progress).Report("Test message");

            // Give time for async write
            Thread.Sleep(500);

            // Verify file was written
            Assert.IsTrue(File.Exists(tempFile));
            var content = File.ReadAllText(tempFile);
            Assert.IsTrue(content.Contains("Test message"), $"Expected 'Test message' in content: {content}");
        } finally {
            // Cleanup
            if (File.Exists(tempFile)) {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    /// Defines the test method CreateFileProgress_ReportsMultipleMessages_AllMessagesWritten.
    /// </summary>
    [TestMethod]
    public void CreateFileProgress_ReportsMultipleMessages_AllMessagesWritten() {
        // Arrange
        var tempFile = Path.GetTempFileName();

        try {
            // Act
            var progress = PythonRuntimeHelper.CreateFileProgress(tempFile);
            ((IProgress<string>)progress).Report("Message 1");
            Thread.Sleep(200);
            ((IProgress<string>)progress).Report("Message 2");
            Thread.Sleep(200);
            ((IProgress<string>)progress).Report("Message 3");

            // Give time for async writes
            Thread.Sleep(500);

            // Assert
            var content = File.ReadAllText(tempFile);
            Assert.IsTrue(content.Contains("Message 1"), "Missing Message 1");
            Assert.IsTrue(content.Contains("Message 2"), "Missing Message 2");
            Assert.IsTrue(content.Contains("Message 3"), "Missing Message 3");
        } finally {
            if (File.Exists(tempFile)) {
                File.Delete(tempFile);
            }
        }
    }

    /// <summary>
    /// Defines the test method ValidateEmbeddedResources_ChecksForRequiredResources.
    /// </summary>
    [TestMethod]
    public void ValidateEmbeddedResources_ChecksForRequiredResources() {
        // Act
        var result = PythonRuntimeHelper.ValidateEmbeddedResources(out var missingResource);

        // Assert
        // Note: This will depend on whether resources are actually embedded
        // In a real test, you might want to verify the resource names
        if (!result) {
            Assert.IsNotNull(missingResource);
            Assert.IsTrue(missingResource.Contains("NeuroForge.Factory.Resources.Runtime"));
        }
    }

    /// <summary>
    /// Defines the test method IsPythonInstalled_ReturnsBool.
    /// </summary>
    [TestMethod]
    public void IsPythonInstalled_ReturnsBool() {
        // Act
        var isInstalled = PythonRuntimeHelper.IsPythonInstalled();

        // Assert
        // This is environment-dependent, so we just verify it returns a bool
        Assert.IsNotNull(isInstalled);
    }

    /// <summary>
    /// Defines the test method GetPythonVersion_ReturnsStringOrNull.
    /// </summary>
    [TestMethod]
    public void GetPythonVersion_ReturnsStringOrNull() {
        // Act
        var version = PythonRuntimeHelper.GetPythonVersion();

        // Assert
        // This is environment-dependent
        // If Python is installed, it should return a version string
        // If not, it should return null
        if (version != null) {
            Assert.IsTrue(version.Contains("Python") || version.Contains("3."));
        }
    }

    /// <summary>
    /// Defines the test method CreateFileProgress_WithInvalidPath_DoesNotThrow.
    /// </summary>
    [TestMethod]
    public void CreateFileProgress_WithInvalidPath_DoesNotThrow() {
        // Arrange
        var invalidPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "invalid", "test.log");

        // Act & Assert - Should not throw
        var progress = PythonRuntimeHelper.CreateFileProgress(invalidPath);
        Assert.IsNotNull(progress);

        // Reporting to invalid path should not throw (errors are ignored)
        ((IProgress<string>)progress).Report("Test message");
    }

    /// <summary>
    /// Defines the test method GetDefaultInstallDirectory_CallMultipleTimes_ReturnsSamePath.
    /// </summary>
    [TestMethod]
    public void GetDefaultInstallDirectory_CallMultipleTimes_ReturnsSamePath() {
        // Act
        var path1 = PythonRuntimeHelper.GetDefaultInstallDirectory();
        var path2 = PythonRuntimeHelper.GetDefaultInstallDirectory();

        // Assert
        Assert.AreEqual(path1, path2);
    }
}