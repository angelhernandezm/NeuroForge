using NeuroForge.Factory.Abstractions;
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

namespace NeuroForge.Factory;

/// <summary>
/// Example demonstrating Python runtime installation
/// </summary>
public class NeuroForgeFactory : INeuroForgeFactory {
    /// <summary>
    /// Initializes Python environment if not already present
    /// </summary>
    public async Task InitializeAsync(string? customInstallDirectory = null) {
        Console.WriteLine("=== NeuroForge Python Environment Setup ===\n");

        // Validate embedded resources
        if (!PythonRuntimeHelper.ValidateEmbeddedResources(out var missingResource)) {
            throw new InvalidOperationException($"Missing embedded resource: {missingResource}");
        }

        // Check if Python is already installed
        if (PythonRuntimeHelper.IsPythonInstalled()) {
            var version = PythonRuntimeHelper.GetPythonVersion();
            Console.WriteLine($"Python is already installed: {version}");
            Console.WriteLine("Skipping installation, but will install/update packages...\n");
        } else {
            Console.WriteLine("Python is not installed. Starting installation process...\n");
        }

        // Setup Python environment
        var installDir = customInstallDirectory ?? PythonRuntimeHelper.GetDefaultInstallDirectory();
        Console.WriteLine($"Installation directory: {installDir}\n");

        using var manager = new PythonRuntimeManager(installDir);
        var progress = PythonRuntimeHelper.CreateConsoleProgress();

        try {
            await manager.SetupPythonEnvironmentAsync(progress, CancellationToken.None);
            Console.WriteLine("\n✓ Python environment is ready!");
        } catch (Exception ex) {
            Console.WriteLine($"\n✗ Setup failed: {ex.Message}");
            throw;
        }
    }
}