using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

namespace NeuroForge.Factory.TestConsole;

/// <summary>
/// Test console application to verify package installation progress
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Python Package Installation Test ===\n");

        // Check if Python is installed
        if (!PythonRuntimeHelper.IsPythonInstalled())
        {
            Console.WriteLine("Python is not installed. Please install Python first.");
            return;
        }

        var version = PythonRuntimeHelper.GetPythonVersion();
        Console.WriteLine($"Found Python: {version}\n");

        // Create a custom install directory for testing
        var testDir = Path.Combine(Path.GetTempPath(), "NeuroForgeTest");
        Directory.CreateDirectory(testDir);

        Console.WriteLine($"Test directory: {testDir}\n");
        Console.WriteLine("Starting package installation test...\n");

        using var manager = new PythonRuntimeManager(testDir);

        // Create detailed progress reporter
        var progress = new Progress<string>(message =>
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        });

        try
        {
            // This will try to setup the environment
            // It should show real-time output now
            await manager.SetupPythonEnvironmentAsync(progress, CancellationToken.None);

            Console.WriteLine("\n✓ Setup completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
