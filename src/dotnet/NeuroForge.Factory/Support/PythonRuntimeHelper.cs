// ============================================================================
// NeuroForge
// File: PythonRuntimeHelper.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Helper utilities for Python runtime management
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

namespace NeuroForge.Factory.Support;

/// <summary>
/// Helper utilities for Python runtime management
/// </summary>
public static class PythonRuntimeHelper {
    /// <summary>
    /// Gets the default Python installation directory
    /// </summary>
    /// <returns>System.String.</returns>
    public static string GetDefaultInstallDirectory() {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NeuroForge", "Python");
    }

    /// <summary>
    /// Checks if Python is already installed on the system
    /// </summary>
    /// <returns><c>true</c> if [is python installed]; otherwise, <c>false</c>.</returns>
    public static bool IsPythonInstalled() {
        try {
            var pythonPaths = new[] {
                "python.exe", "python3.exe",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Python311", "python.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Python", "Python311", "python.exe")
            };

            foreach (var path in pythonPaths) {
                try {
                    var startInfo = new System.Diagnostics.ProcessStartInfo {
                        FileName = path,
                        Arguments = "--version",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using var process = System.Diagnostics.Process.Start(startInfo);
                    if (process != null) {
                        process.WaitForExit(2000);
                        if (process.ExitCode == 0) {
                            return true;
                        }
                    }
                } catch {
                    continue;
                }
            }

            return false;
        } catch {
            return false;
        }
    }

    /// <summary>
    /// Gets the installed Python version
    /// </summary>
    /// <returns>System.Nullable{System.String}.</returns>
    public static string? GetPythonVersion() {
        try {
            var startInfo = new System.Diagnostics.ProcessStartInfo {
                FileName = "python.exe",
                Arguments = "--version",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var process = System.Diagnostics.Process.Start(startInfo);
            if (process != null) {
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit(2000);
                if (process.ExitCode == 0) {
                    return output.Trim();
                }
            }
        } catch {
            // Python not in PATH
        }

        return null;
    }

    /// <summary>
    /// Creates a progress reporter that writes to console
    /// </summary>
    /// <param name="includeTimestamp">if set to <c>true</c> [include timestamp].</param>
    /// <returns>IProgress{System.String}.</returns>
    public static IProgress<string> CreateConsoleProgress(bool includeTimestamp = true) {
        return new Progress<string>(message => Console.WriteLine(includeTimestamp ? $"[{DateTime.Now:HH:mm:ss}] {message}" : message)); 
    }

    /// <summary>
    /// Creates a progress reporter that writes to a log file
    /// </summary>
    /// <param name="logFilePath">The log file path.</param>
    /// <returns>IProgress{System.String}.</returns>
    public static IProgress<string> CreateFileProgress(string logFilePath) {
        return new Progress<string>(message => {
            try {
                var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
            } catch {
                // Ignore logging errors
            }
        });
    }

    /// <summary>
    /// Validates that the required embedded resources exist
    /// </summary>
    /// <param name="missingResource">The missing resource.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    public static bool ValidateEmbeddedResources(out string? missingResource) {
        var assembly = typeof(PythonRuntimeManager).Assembly;
        var requiredResources = new[] {
            "NeuroForge.Factory.Resources.Runtime.python_runtime.json",
            "NeuroForge.Factory.Resources.Runtime.package_list.json"
        };

        foreach (var resource in requiredResources) {
            using var stream = assembly.GetManifestResourceStream(resource);
            if (stream == null) {
                missingResource = resource;
                return false;
            }
        }
        missingResource = null;

        return true;
    }
}