// ============================================================================
// NeuroForge
// File: AnnBuilderManager.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Manager for ANN builder execution and resource management
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

using System.Reflection;
using System.Text;
using System.Text.Json;
using NeuroForge.Factory.Abstractions;
using NeuroForge.Factory.Support;

namespace NeuroForge.Factory.Core;

/// <summary>
/// Manager for ANN builder execution and resource management
/// </summary>
public class AnnBuilderManager : IAnnBuilderManager {
    /// <summary>
    /// The working directory
    /// </summary>
    private readonly string _workingDirectory;

    /// <summary>
    /// The python executable
    /// </summary>
    private readonly string _pythonExecutable;

    /// <summary>
    /// The model configuration
    /// </summary>
    private ModelConfig? _modelConfig;

    /// <summary>
    /// The disposed
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnnBuilderManager" /> class
    /// </summary>
    /// <param name="workingDirectory">Working directory for extracted resources</param>
    /// <exception cref="System.InvalidOperationException">Python is not installed. Please run NeuroForgeFactory.InitializeAsync() first to install Python.</exception>
    /// <exception cref="System.InvalidOperationException">Could not locate Python executable.</exception>
    public AnnBuilderManager(string? workingDirectory = null) {
        // Check Python installation first
        if (!PythonRuntimeHelper.IsPythonInstalled()) 
            throw new InvalidOperationException("Python is not installed. Please run NeuroForgeFactory.InitializeAsync() first to install Python.");

        // Find Python executable
        _pythonExecutable = FindPythonExecutable() ?? throw new InvalidOperationException("Could not locate Python executable.");

        // Set working directory
        _workingDirectory = workingDirectory ?? Path.Combine(Path.GetTempPath(), "NeuroForge", "Factory");
        Directory.CreateDirectory(_workingDirectory);

        // Extract resources and load configuration.
        // Force re-extraction so stale copies left over in the working directory
        // (e.g. from a previous version) never shadow the current embedded builder
        // scripts, which would otherwise keep running with old/hardcoded values
        // instead of the dynamically supplied configuration.
        ExtractPythonResources(force: true);
        LoadFactoryConfig();
    }

    /// <summary>
    /// Gets the available model types
    /// </summary>
    /// <value>The available models.</value>
    public IEnumerable<string> AvailableModels => _modelConfig?.AvailableModels ?? Enumerable.Empty<string>();

    /// <summary>
    /// Builds a model using the specified model name and dataset path
    /// </summary>
    /// <param name="modelName">Name of the model (e.g., "cnn", "mlp", "rnn")</param>
    /// <param name="datasetPath">Path to the dataset</param>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
    /// <exception cref="System.ArgumentException">Unknown model type: {modelName}. Available models: {string.Join(", ", AvailableModels)}</exception>
    /// <exception cref="System.InvalidOperationException">Failed to load configuration for model: {modelName}</exception>
    public async Task<string> BuildModelAsync(string modelName, string datasetPath, IProgress<string>? progress = null, CancellationToken cancellationToken = default) {
        progress?.Report($"Building model '{modelName}' with dataset: {datasetPath}");

        // Validate model exists in registry
        if (_modelConfig == null || !_modelConfig.TryGetModel(modelName, out var modelConfig)) 
            throw new ArgumentException($"Unknown model type: {modelName}. Available models: {string.Join(", ", AvailableModels)}");
        
        // Update dataset path in config
        if (modelConfig != null) {
            modelConfig.Dataset.Path = datasetPath;

            // Create custom config file
            var customConfigPath = Path.Combine(_workingDirectory, $"{modelName}_custom_config.json");
            var json = JsonSerializer.Serialize(modelConfig, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(customConfigPath, json, cancellationToken);

            // Execute Python builder with custom config
            return await ExecutePythonBuilderAsync(modelName, customConfigPath, progress, cancellationToken);
        }

        throw new InvalidOperationException($"Failed to load configuration for model: {modelName}");
    }

    /// <summary>
    /// Builds a model using a custom configuration file
    /// </summary>
    /// <param name="configPath">Path to the configuration file</param>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
    /// <exception cref="System.IO.FileNotFoundException">Configuration file not found: {configPath}</exception>
    /// <exception cref="System.InvalidOperationException">Invalid configuration file: missing 'type' property</exception>
    public async Task<string> BuildModelAsync(string configPath, IProgress<string>? progress = null, CancellationToken cancellationToken = default) {
        progress?.Report($"Building model from config file: {configPath}");

        if (!File.Exists(configPath)) 
            throw new FileNotFoundException($"Configuration file not found: {configPath}");

        // Load and validate config
        var configJson = await File.ReadAllTextAsync(configPath, cancellationToken);
        var config = JsonSerializer.Deserialize<AnnBuilderConfig>(configJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (config == null || string.IsNullOrEmpty(config.Type)) 
            throw new InvalidOperationException("Invalid configuration file: missing 'type' property");

        // Execute Python builder
        return await ExecutePythonBuilderAsync(config.Type, configPath, progress, cancellationToken);
    }

    /// <summary>
    /// Builds a model using a strongly-typed configuration object (e.g., loaded
    /// from a JSON-driven <see cref="ModelConfig"/> and iterated over its models)
    /// </summary>
    /// <param name="modelName">Name to associate with this build (used for output/temp file naming)</param>
    /// <param name="config">The strongly-typed ANN builder configuration</param>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
    /// <exception cref="System.ArgumentNullException">config is null</exception>
    /// <exception cref="System.InvalidOperationException">Invalid configuration: missing 'type' property</exception>
    public async Task<string> BuildModelAsync(string modelName, AnnBuilderConfig config, IProgress<string>? progress = null, CancellationToken cancellationToken = default) {
        progress?.Report($"Building model '{modelName}' from provided configuration...");

        if (config == null)
            throw new ArgumentNullException(nameof(config));

        if (string.IsNullOrEmpty(config.Type))
            throw new InvalidOperationException("Invalid configuration: missing 'type' property");

        // Serialize the strongly-typed config to a temporary JSON file so it can
        // be consumed the same way as any other config-driven build.
        var customConfigPath = Path.Combine(_workingDirectory, $"{modelName}_config.json");
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(customConfigPath, json, cancellationToken);

        return await ExecutePythonBuilderAsync(config.Type, customConfigPath, progress, cancellationToken);
    }

    /// <summary>
    /// Extracts Python resources to the working directory
    /// </summary>
    /// <param name="force">Force re-extraction even if files exist</param>
    public void ExtractPythonResources(bool force = false) {
        var assembly = Assembly.GetExecutingAssembly();
        var resourcePrefix = "NeuroForge.Factory.Resources.Factory";

        var resources = assembly.GetManifestResourceNames().Where(r => r.StartsWith(resourcePrefix));

        foreach (var resource in resources) {
            // Calculate relative path
            var relativePath = resource.Substring(resourcePrefix.Length + 1)
                .Replace("Builders.", "Builders" + Path.DirectorySeparatorChar);

            var targetPath = Path.Combine(_workingDirectory, relativePath);
            var targetDir = Path.GetDirectoryName(targetPath);

            if (!string.IsNullOrEmpty(targetDir))
                Directory.CreateDirectory(targetDir);

            // Skip if file exists and not forcing
            if (File.Exists(targetPath) && !force)
                continue;

            // Extract resource
            using var stream = assembly.GetManifestResourceStream(resource);
            if (stream != null) {
                using var fileStream = File.Create(targetPath);
                stream.CopyTo(fileStream);
            }
        }
    }

    /// <summary>
    /// Loads the factory configuration from embedded resource
    /// </summary>
    /// <exception cref="System.InvalidOperationException">Factory configuration file not found after extraction.</exception>
    /// <exception cref="System.InvalidOperationException">Failed to load factory configuration.</exception>
    private void LoadFactoryConfig() {
        var configPath = Path.Combine(_workingDirectory, "factory_config.json");

        if (!File.Exists(configPath))
            throw new InvalidOperationException("Factory configuration file not found after extraction.");

        var json = File.ReadAllText(configPath);
        _modelConfig = JsonSerializer.Deserialize<ModelConfig>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (_modelConfig == null) 
            throw new InvalidOperationException("Failed to load factory configuration.");
    }

    /// <summary>
    /// Finds the Python executable on the system
    /// </summary>
    /// <returns>System.Nullable{System.String}.</returns>
    private string? FindPythonExecutable() {
        var possiblePaths = new[] {
            "python.exe",
            "python3.exe",
            Path.Combine(PythonRuntimeHelper.GetDefaultInstallDirectory(), "python.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Python311", "python.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", "Python", "Python311", "python.exe")
        };

        foreach (var path in possiblePaths) {
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
                        return path;
                    }
                }
            } catch {
                continue;
            }
        }

        return null;
    }

    /// <summary>
    /// Executes the Python builder script
    /// </summary>
    /// <param name="modelType">Type of the model.</param>
    /// <param name="configPath">The configuration path.</param>
    /// <param name="progress">The progress.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;System.String&gt; representing the asynchronous operation.</returns>
    /// <exception cref="System.InvalidOperationException">Python builder failed with exit code {process.ExitCode}. Errors: {errors}</exception>
    private async Task<string> ExecutePythonBuilderAsync(string modelType, string configPath, IProgress<string>? progress, CancellationToken cancellationToken) {
        progress?.Report($"Executing Python builder for model type: {modelType}");
        var runnerScript = Path.Combine(_workingDirectory, "run_builder.py");
        var outputPath = Path.Combine(_workingDirectory, "models", $"{modelType}_{DateTime.Now:yyyyMMdd_HHmmss}");
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? _workingDirectory);

        var startInfo = new System.Diagnostics.ProcessStartInfo {
            FileName = _pythonExecutable,
            Arguments = $"\"{runnerScript}\" \"{configPath}\" \"{outputPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = _workingDirectory
        };

        using var process = new System.Diagnostics.Process { StartInfo = startInfo };
        var output = new StringBuilder();
        var errors = new StringBuilder();

        process.OutputDataReceived += (sender, e) => {
            if (e.Data != null) {
                output.AppendLine(e.Data);
                progress?.Report(e.Data);
            }
        };

        process.ErrorDataReceived += (sender, e) => {
            if (e.Data != null) {
                errors.AppendLine(e.Data);
                progress?.Report($"[ERROR] {e.Data}");
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0) {
            throw new InvalidOperationException(
                $"Python builder failed with exit code {process.ExitCode}. Errors: {errors}");
        }

        progress?.Report($"Model built successfully: {outputPath}");

        return outputPath;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose() {
        if (_disposed)
            return;
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}