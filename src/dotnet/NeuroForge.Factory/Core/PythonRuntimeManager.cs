// ============================================================================
// NeuroForge
// File: PythonRuntimeManager.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Manages Python runtime download and package installation
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
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace NeuroForge.Factory.Core;

/// <summary>
/// Manages Python runtime download and package installation
/// </summary>
public class PythonRuntimeManager : IPythonRuntimeManager {
    /// <summary>
    /// The install directory
    /// </summary>
    private readonly string _installDirectory;

    /// <summary>
    /// The HTTP client
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="PythonRuntimeManager"/> class.
    /// </summary>
    /// <param name="installDirectory">The install directory.</param>
    /// <exception cref="System.ArgumentNullException">installDirectory</exception>
    public PythonRuntimeManager(string installDirectory) {
        _installDirectory = installDirectory ?? throw new ArgumentNullException(nameof(installDirectory));
        _httpClient = new HttpClient();
    }

    /// <summary>
    /// Downloads Python runtime and installs specified packages
    /// </summary>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SetupPythonEnvironmentAsync(IProgress<string>? progress = null, CancellationToken cancellationToken = default) {
        progress?.Report("Loading Python runtime configuration...");
        var runtimeConfig = LoadPythonRuntimeConfig();
        progress?.Report("Loading package list configuration...");
        var packageConfig = LoadPackageListConfig();
        progress?.Report($"Downloading Python {runtimeConfig.PythonVersion}...");
        var installerPath = await DownloadPythonAsync(runtimeConfig, progress, cancellationToken);
        progress?.Report("Installing Python runtime...");
        await InstallPythonAsync(installerPath, runtimeConfig, cancellationToken);
        progress?.Report("Installing Python packages...");
        await InstallPackagesAsync(packageConfig, progress, cancellationToken);
        progress?.Report("Python environment setup completed successfully!");
    }

    /// <summary>
    /// Loads Python runtime configuration from embedded resource
    /// </summary>
    /// <returns>PythonRuntimeConfig.</returns>
    /// <exception cref="System.InvalidOperationException">Could not find embedded resource: {resourceName}</exception>
    /// <exception cref="System.InvalidOperationException">Failed to deserialize Python runtime configuration</exception>
    private PythonRuntimeConfig LoadPythonRuntimeConfig() {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "NeuroForge.Factory.Resources.Runtime.python_runtime.json";

        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var config = JsonSerializer.Deserialize<PythonRuntimeConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize Python runtime configuration");

        return config;
    }

    /// <summary>
    /// Loads package list configuration from embedded resource
    /// </summary>
    /// <returns>PackageListConfig.</returns>
    /// <exception cref="System.InvalidOperationException">Could not find embedded resource: {resourceName}</exception>
    /// <exception cref="System.InvalidOperationException">Failed to deserialize package list configuration</exception>
    private PackageListConfig LoadPackageListConfig() {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "NeuroForge.Factory.Resources.Runtime.package_list.json";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");

        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();

        var config = JsonSerializer.Deserialize<PackageListConfig>(json, new JsonSerializerOptions {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize package list configuration");

        return config;
    }

    /// <summary>
    /// Downloads Python installer
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="progress">The progress.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
    private async Task<string> DownloadPythonAsync(PythonRuntimeConfig config, IProgress<string>? progress, CancellationToken cancellationToken) {
        Directory.CreateDirectory(_installDirectory);
        var fileName = Path.GetFileName(new Uri(config.DownloadUrl).LocalPath);
        var installerPath = Path.Combine(_installDirectory, fileName);

        if (File.Exists(installerPath)) {
            progress?.Report($"Python installer already exists at: {installerPath}");
            return installerPath;
        }

        progress?.Report($"Downloading from: {config.DownloadUrl}");
        using var response = await _httpClient.GetAsync(config.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        var totalBytes = response.Content.Headers.ContentLength ?? 0;
        var downloadedBytes = 0L;
        var bytesRead = 0;
        var buffer = new byte[8192];
        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var fileStream = new FileStream(installerPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

        while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0) {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            downloadedBytes += bytesRead;

            if (totalBytes > 0) {
                var percentage = (double)downloadedBytes / totalBytes * 100;
                progress?.Report($"Downloaded {downloadedBytes:N0} / {totalBytes:N0} bytes ({percentage:F1}%)");
            }
        }
        progress?.Report($"Download completed: {installerPath}");

        return installerPath;
    }

    /// <summary>
    /// Installs Python using the downloaded installer
    /// </summary>
    /// <param name="installerPath">The installer path.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="System.InvalidOperationException">Python installation failed with exit code {process.ExitCode}. " +
    ///                 $"Output: {output} Error: {error}</exception>
    private async Task InstallPythonAsync(string installerPath, PythonRuntimeConfig config, CancellationToken cancellationToken) {
        var startInfo = new ProcessStartInfo {
            FileName = installerPath,
            Arguments = string.Join(" ", config.InstallArgs),
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var output = await outputTask;
        var error = await errorTask;

        if (process.ExitCode != 0) {
            throw new InvalidOperationException($"Python installation failed with exit code {process.ExitCode}. " +
                                                $"Output: {output} Error: {error}");
        }
    }

    /// <summary>
    /// Installs Python packages using pip
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="progress">The progress.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task InstallPackagesAsync(PackageListConfig config, IProgress<string>? progress, CancellationToken cancellationToken) {
        if (config.Packages.Length == 0) {
            progress?.Report("No packages to install");
            return;
        }

        var pythonPath = FindPythonExecutable();
        progress?.Report($"Using Python at: {pythonPath}");

        foreach (var package in config.Packages) {
            progress?.Report($"Installing package: {package}");
            await InstallPackageAsync(pythonPath, package, cancellationToken);
            progress?.Report($"Successfully installed: {package}");
        }
    }

    /// <summary>
    /// Installs a single Python package using pip
    /// </summary>
    /// <param name="pythonPath">The python path.</param>
    /// <param name="package">The package.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    /// <exception cref="System.InvalidOperationException">Package installation failed for '{package}' with exit code {process.ExitCode}. " +
    ///                 $"Output: {output} Error: {error}</exception>
    private async Task InstallPackageAsync(string pythonPath, string package, CancellationToken cancellationToken) {
        var startInfo = new ProcessStartInfo {
            FileName = pythonPath,
            Arguments = $"-m pip install {package}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        var output = await outputTask;
        var error = await errorTask;

        if (process.ExitCode != 0) {
            throw new InvalidOperationException($"Package installation failed for '{package}' with exit code {process.ExitCode}. " +
                                                $"Output: {output} Error: {error}");
        }
    }

    /// <summary>
    /// Finds the Python executable on the system
    /// </summary>
    /// <returns>System.String.</returns>
    /// <exception cref="System.InvalidOperationException">Could not find Python executable. Please ensure Python is installed and in the system PATH.</exception>
    private string FindPythonExecutable() {
        // Common Python installation paths
        var potentialPaths = new[] {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Python311", "python.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Python", "Python311", "python.exe"),
            "python.exe", // Try from PATH
            "python3.exe",
        };

        foreach (var path in potentialPaths) {
            try {
                if (File.Exists(path)) {
                    return path;
                }

                // Try to execute from PATH
                if (path.EndsWith(".exe") && !Path.IsPathRooted(path)) {
                    var startInfo = new ProcessStartInfo {
                        FileName = path,
                        Arguments = "--version",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using var process = Process.Start(startInfo);
                    if (process != null) {
                        process.WaitForExit(1000);
                        if (process.ExitCode == 0) {
                            return path;
                        }
                    }
                }
            } catch {
                // Continue to next path
            }
        }
        throw new InvalidOperationException("Could not find Python executable. Please ensure Python is installed and in the system PATH.");
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
        _httpClient?.Dispose();
    }
}