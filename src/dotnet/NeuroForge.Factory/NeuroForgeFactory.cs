// ============================================================================
// NeuroForge
// File: NeuroForgeFactory.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Factory class for NeuroForge
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

using NeuroForge.Factory.Abstractions;
using NeuroForge.Factory.Core;
using NeuroForge.Factory.Support;

namespace NeuroForge.Factory;

/// <summary>
/// Class NeuroForgeFactory.
/// Implements the <see cref="INeuroForgeFactory" />
/// </summary>
/// <seealso cref="INeuroForgeFactory" />
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

    /// <summary>
    /// Creates a new <see cref="IAnnBuilderManager" /> for building ANN models.
    /// Requires <see cref="InitializeAsync" /> to have completed successfully first.
    /// </summary>
    /// <param name="workingDirectory">Working directory for extracted resources</param>
    /// <returns>A new <see cref="IAnnBuilderManager" /> instance.</returns>
    public IAnnBuilderManager CreateAnnBuilderManager(string? workingDirectory = null) {
        return new AnnBuilderManager(workingDirectory);
    }
}