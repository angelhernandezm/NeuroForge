// ============================================================================
// NeuroForge
// File: PythonSetupRunner.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Implementation of the IPythonSetupRunner interface for running Python setup tasks.
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

namespace NeuroForge.Factory.Support;

/// <summary>
/// Example usage of the Python Runtime Manager
/// </summary>
public class PythonSetupRunner : IPythonSetupRunner {
    /// <summary>
    /// Sets up Python environment with progress reporting
    /// </summary>
    /// <param name="installDirectory">Directory where Python installer will be downloaded</param>
    public async Task SetupPythonAsync(string? installDirectory = null) {
        // Use a default install directory if none provided
        installDirectory ??= PythonRuntimeHelper.GetDefaultInstallDirectory();

        var manager = new PythonRuntimeManager(installDirectory);

        // Create a progress reporter
        var progress = new Progress<string>(message => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}"));

        try {
            await manager.SetupPythonEnvironmentAsync(progress, CancellationToken.None);
            Console.WriteLine("\n✓ Python environment setup completed successfully!");
        } catch (Exception ex) {
            Console.WriteLine($"\n✗ Error setting up Python environment: {ex.Message}");
            throw;
        } finally {
            manager.Dispose();
        }
    }

    /// <summary>
    /// Example of setting up Python with custom cancellation support
    /// </summary>
    public async Task SetupPythonWithCancellationAsync(string installDirectory, CancellationToken cancellationToken) {
        using var manager = new PythonRuntimeManager(installDirectory);
        var progress = new Progress<string>(message => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}"));
        await manager.SetupPythonEnvironmentAsync(progress, cancellationToken);
    }
}