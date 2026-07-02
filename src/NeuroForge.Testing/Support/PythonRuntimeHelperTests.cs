using NeuroForge.Factory.Support;

namespace NeuroForge.Testing.Support;

[TestClass]
public class PythonRuntimeHelperTests
{
    [TestMethod]
    public void GetDefaultInstallDirectory_ReturnsValidPath()
    {
        // Act
        var installDir = PythonRuntimeHelper.GetDefaultInstallDirectory();

        // Assert
        Assert.IsNotNull(installDir);
        Assert.IsTrue(installDir.Contains("NeuroForge"));
        Assert.IsTrue(installDir.Contains("Python"));
        Assert.IsTrue(Path.IsPathRooted(installDir));
    }

    [TestMethod]
    public void GetDefaultInstallDirectory_ContainsLocalAppData()
    {
        // Act
        var installDir = PythonRuntimeHelper.GetDefaultInstallDirectory();
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        // Assert
        Assert.IsTrue(installDir.StartsWith(localAppData));
    }

    [TestMethod]
    public void CreateConsoleProgress_WithTimestamp_ReturnsValidProgress()
    {
        // Act
        var progress = PythonRuntimeHelper.CreateConsoleProgress(includeTimestamp: true);

        // Assert
        Assert.IsNotNull(progress);
        Assert.IsInstanceOfType(progress, typeof(IProgress<string>));
    }

    [TestMethod]
    public void CreateConsoleProgress_WithoutTimestamp_ReturnsValidProgress()
    {
        // Act
        var progress = PythonRuntimeHelper.CreateConsoleProgress(includeTimestamp: false);

        // Assert
        Assert.IsNotNull(progress);
        Assert.IsInstanceOfType(progress, typeof(IProgress<string>));
    }

    [TestMethod]
    public void CreateFileProgress_WithValidPath_ReturnsValidProgress()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();

        try
        {
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
        }
        finally
        {
            // Cleanup
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [TestMethod]
    public void CreateFileProgress_ReportsMultipleMessages_AllMessagesWritten()
    {
        // Arrange
        var tempFile = Path.GetTempFileName();

        try
        {
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
        }
        finally
        {
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
        }
    }

    [TestMethod]
    public void ValidateEmbeddedResources_ChecksForRequiredResources()
    {
        // Act
        var result = PythonRuntimeHelper.ValidateEmbeddedResources(out var missingResource);

        // Assert
        // Note: This will depend on whether resources are actually embedded
        // In a real test, you might want to verify the resource names
        if (!result)
        {
            Assert.IsNotNull(missingResource);
            Assert.IsTrue(missingResource.Contains("NeuroForge.Factory.Resources.Runtime"));
        }
    }

    [TestMethod]
    public void IsPythonInstalled_ReturnsBool()
    {
        // Act
        var isInstalled = PythonRuntimeHelper.IsPythonInstalled();

        // Assert
        // This is environment-dependent, so we just verify it returns a bool
        Assert.IsNotNull(isInstalled);
    }

    [TestMethod]
    public void GetPythonVersion_ReturnsStringOrNull()
    {
        // Act
        var version = PythonRuntimeHelper.GetPythonVersion();

        // Assert
        // This is environment-dependent
        // If Python is installed, it should return a version string
        // If not, it should return null
        if (version != null)
        {
            Assert.IsTrue(version.Contains("Python") || version.Contains("3."));
        }
    }

    [TestMethod]
    public void CreateFileProgress_WithInvalidPath_DoesNotThrow()
    {
        // Arrange
        var invalidPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString(), "invalid", "test.log");

        // Act & Assert - Should not throw
        var progress = PythonRuntimeHelper.CreateFileProgress(invalidPath);
        Assert.IsNotNull(progress);

        // Reporting to invalid path should not throw (errors are ignored)
        ((IProgress<string>)progress).Report("Test message");
    }

    [TestMethod]
    public void GetDefaultInstallDirectory_CallMultipleTimes_ReturnsSamePath()
    {
        // Act
        var path1 = PythonRuntimeHelper.GetDefaultInstallDirectory();
        var path2 = PythonRuntimeHelper.GetDefaultInstallDirectory();

        // Assert
        Assert.AreEqual(path1, path2);
    }
}
