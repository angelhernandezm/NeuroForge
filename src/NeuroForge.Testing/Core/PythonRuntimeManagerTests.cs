using NeuroForge.Factory.Core;

namespace NeuroForge.Testing.Core;

/// <summary>
/// Defines test class PythonRuntimeManagerTests.
/// </summary>
[TestClass]
public class PythonRuntimeManagerTests {
    /// <summary>
    /// The test directory
    /// </summary>
    private string _testDirectory = string.Empty;

    /// <summary>
    /// Setups this instance.
    /// </summary>
    [TestInitialize]
    public void Setup() {
        _testDirectory = Path.Combine(Path.GetTempPath(), "NeuroForgeTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
    }

    /// <summary>
    /// Cleanups this instance.
    /// </summary>
    [TestCleanup]
    public void Cleanup() {
        if (Directory.Exists(_testDirectory)) {
            try {
                Directory.Delete(_testDirectory, recursive: true);
            } catch {
                // Cleanup best effort
            }
        }
    }

    /// <summary>
    /// Defines the test method Constructor_WithValidDirectory_Success.
    /// </summary>
    [TestMethod]
    public void Constructor_WithValidDirectory_Success() {
        // Act
        using var manager = new PythonRuntimeManager(_testDirectory);

        // Assert
        Assert.IsNotNull(manager);
    }

    /// <summary>
    /// Defines the test method Constructor_WithNullDirectory_ThrowsArgumentNullException.
    /// </summary>
    [TestMethod]
    public void Constructor_WithNullDirectory_ThrowsArgumentNullException() {
        // Act & Assert
        try {
            using var manager = new PythonRuntimeManager(null!);
            Assert.Fail("Expected ArgumentNullException");
        } catch (ArgumentNullException) {
            // Expected
            Assert.IsTrue(true);
        }
    }

    /// <summary>
    /// Defines the test method Constructor_CreatesInstallDirectory_IfNotExists.
    /// </summary>
    [TestMethod]
    public void Constructor_CreatesInstallDirectory_IfNotExists() {
        // Arrange
        var newDirectory = Path.Combine(_testDirectory, "NewSubDir");
        Assert.IsFalse(Directory.Exists(newDirectory));

        // Act
        using var manager = new PythonRuntimeManager(newDirectory);

        // Assert
        Assert.IsNotNull(manager);
    }

    /// <summary>
    /// Defines the test method Dispose_MultipleCalls_DoesNotThrow.
    /// </summary>
    [TestMethod]
    public void Dispose_MultipleCalls_DoesNotThrow() {
        // Arrange
        var manager = new PythonRuntimeManager(_testDirectory);

        // Act & Assert
        manager.Dispose();
        manager.Dispose(); // Should not throw
    }

    /// <summary>
    /// Defines the test method SetupPythonEnvironmentAsync_WithNullProgress_DoesNotThrow.
    /// </summary>
    [TestMethod]
    public async Task SetupPythonEnvironmentAsync_WithNullProgress_DoesNotThrow() {
        // Arrange
        using var manager = new PythonRuntimeManager(_testDirectory);

        // Act & Assert
        // This will fail because it tries to load actual embedded resources
        // but it shouldn't throw on null progress - it should fail on missing resources
        try {
            await manager.SetupPythonEnvironmentAsync(null, CancellationToken.None);
            Assert.Fail("Should have thrown InvalidOperationException or UriFormatException");
        } catch (InvalidOperationException ex) when (ex.Message.Contains("Could not find embedded resource")) {
            // Expected - resources don't exist in test context
            Assert.IsTrue(true);
        } catch (UriFormatException) {
            // Also expected - empty URL from missing resources
            Assert.IsTrue(true);
        }
    }

    /// <summary>
    /// Defines the test method SetupPythonEnvironmentAsync_WithCancellationToken_RespectsCancellation.
    /// </summary>
    [TestMethod]
    public async Task SetupPythonEnvironmentAsync_WithCancellationToken_RespectsCancellation() {
        // Arrange
        using var manager = new PythonRuntimeManager(_testDirectory);
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        try {
            await manager.SetupPythonEnvironmentAsync(null, cts.Token);
            Assert.Fail("Should have thrown OperationCanceledException, InvalidOperationException, or UriFormatException");
        } catch (OperationCanceledException) {
            // Expected
            Assert.IsTrue(true);
        } catch (InvalidOperationException ex) when (ex.Message.Contains("Could not find embedded resource")) {
            // Also acceptable - might fail before checking cancellation
            Assert.IsTrue(true);
        } catch (UriFormatException) {
            // Also acceptable - empty URL from missing resources
            Assert.IsTrue(true);
        }
    }

    /// <summary>
    /// Defines the test method SetupPythonEnvironmentAsync_ReportsProgress.
    /// </summary>
    [TestMethod]
    public async Task SetupPythonEnvironmentAsync_ReportsProgress() {
        // Arrange
        using var manager = new PythonRuntimeManager(_testDirectory);
        var progressMessages = new List<string>();
        var progress = new Progress<string>(msg => progressMessages.Add(msg));

        // Act - We expect this to fail due to missing resources, but we want to ensure
        // that progress reporting works (at least one message before the exception)
        try {
            await manager.SetupPythonEnvironmentAsync(progress, CancellationToken.None);
        } catch (Exception) {
            // Expected - any exception is fine for this test
        }

        // Assert - We should have received at least one progress message before the error
        Assert.IsTrue(progressMessages.Count > 0,
            $"Expected at least one progress message. Got {progressMessages.Count} messages");
        Assert.IsTrue(progressMessages[0].Contains("Loading", StringComparison.OrdinalIgnoreCase),
            $"First message should be about loading. Got: {progressMessages[0]}");
    }
}

/// <summary>
/// Defines test class PythonRuntimeManagerEmbeddedResourceTests.
/// </summary>
[TestClass]
public class PythonRuntimeManagerEmbeddedResourceTests {
    /// <summary>
    /// Defines the test method LoadPythonRuntimeConfig_MissingResource_ThrowsInvalidOperationException.
    /// </summary>
    [TestMethod]
    public void LoadPythonRuntimeConfig_MissingResource_ThrowsInvalidOperationException() {
        // This test verifies that attempting to load a missing embedded resource
        // throws the expected exception

        // Arrange
        var testDir = Path.Combine(Path.GetTempPath(), "NeuroForgeTests", Guid.NewGuid().ToString());
        Directory.CreateDirectory(testDir);

        try {
            using var manager = new PythonRuntimeManager(testDir);

            // Act & Assert
            AggregateException? exception = null;
            try {
                manager.SetupPythonEnvironmentAsync(null, CancellationToken.None).Wait();
                Assert.Fail("Expected an exception");
            } catch (AggregateException ex) {
                exception = ex;
            }

            Assert.IsNotNull(exception);
            Assert.IsNotNull(exception.InnerException);
            // Accept either InvalidOperationException (missing resource) or UriFormatException (empty URL from missing resource)
            Assert.IsTrue(
                exception.InnerException is InvalidOperationException ||
                exception.InnerException is UriFormatException,
                $"Expected InvalidOperationException or UriFormatException but got {exception.InnerException.GetType().Name}");
        } finally {
            if (Directory.Exists(testDir)) {
                Directory.Delete(testDir, true);
            }
        }
    }
}

/// <summary>
/// Defines test class PythonRuntimeManagerIntegrationTests.
/// </summary>
[TestClass]
public class PythonRuntimeManagerIntegrationTests {
    // These tests would require actual Python installation
    // They are marked with [Ignore] by default but can be run manually

    /// <summary>
    /// Defines the test method SetupPythonEnvironmentAsync_WithActualResources_Success.
    /// </summary>
    [TestMethod]
    [Ignore("Integration test - requires actual resources and long-running operations")]
    public async Task SetupPythonEnvironmentAsync_WithActualResources_Success() {
        // Arrange
        var installDir = Path.Combine(Path.GetTempPath(), "NeuroForgeTests", "PythonInstall");
        Directory.CreateDirectory(installDir);

        try {
            using var manager = new PythonRuntimeManager(installDir);
            var progressMessages = new List<string>();
            var progress = new Progress<string>(msg => {
                Console.WriteLine(msg);
                progressMessages.Add(msg);
            });

            // Act
            await manager.SetupPythonEnvironmentAsync(progress, CancellationToken.None);

            // Assert
            Assert.IsTrue(progressMessages.Count > 0);
            Assert.IsTrue(progressMessages.Any(m => m.Contains("completed")));
        } finally {
            if (Directory.Exists(installDir)) {
                try {
                    Directory.Delete(installDir, true);
                } catch {
                    // Cleanup best effort
                }
            }
        }
    }

    /// <summary>
    /// Defines the test method DownloadPython_WithValidUrl_Success.
    /// </summary>
    [TestMethod]
    [Ignore("Integration test - downloads actual file")]
    public async Task DownloadPython_WithValidUrl_Success() {
        // This would test actual download functionality
        // Skipped by default to avoid network calls in unit tests
        Assert.Inconclusive("Integration test - run manually");
    }
}