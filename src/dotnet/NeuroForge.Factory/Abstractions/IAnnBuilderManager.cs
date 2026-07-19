// ============================================================================
// NeuroForge
// File: IAnnBuilderManager.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Interface for ANN builder manager
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

namespace NeuroForge.Factory.Abstractions;

/// <summary>
/// Interface for managing ANN builder execution
/// </summary>
public interface IAnnBuilderManager : IDisposable {
    /// <summary>
    /// Gets the available model types
    /// </summary>
    IEnumerable<string> AvailableModels {
        get;
    }

    /// <summary>
    /// Builds a model using the specified model name and dataset path
    /// </summary>
    /// <param name="modelName">Name of the model (e.g., "cnn", "mlp", "rnn")</param>
    /// <param name="datasetPath">Path to the dataset</param>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Path to the saved model</returns>
    Task<string> BuildModelAsync(string modelName, string datasetPath, IProgress<string>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds a model using a custom configuration file
    /// </summary>
    /// <param name="configPath">Path to the configuration file</param>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Path to the saved model</returns>
    Task<string> BuildModelAsync(string configPath, IProgress<string>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Builds a model using a strongly-typed configuration object (e.g., loaded
    /// from a JSON-driven <see cref="ModelConfig"/> and iterated over its models)
    /// </summary>
    /// <param name="modelName">Name to associate with this build (used for output/temp file naming)</param>
    /// <param name="config">The strongly-typed ANN builder configuration</param>
    /// <param name="progress">Optional progress callback</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Path to the saved model</returns>
    Task<string> BuildModelAsync(string modelName, AnnBuilderConfig config, IProgress<string>? progress = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts Python resources to the working directory
    /// </summary>
    /// <param name="force">Force re-extraction even if files exist</param>
    void ExtractPythonResources(bool force = false);
}