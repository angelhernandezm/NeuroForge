// ============================================================================
// NeuroForge
// File: ModelConfig.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Root configuration for model factory
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

using System.Text.Json.Serialization;

namespace NeuroForge.Factory.Core;

/// <summary>
/// Root configuration for model factory (maps to factory_config.json)
/// </summary>
public class ModelConfig {
    /// <summary>
    /// Gets or sets the models dictionary
    /// </summary>
    [JsonPropertyName("models")]
    public Dictionary<string, AnnBuilderConfig> Models { get; set; } = new();

    /// <summary>
    /// Gets the available model types
    /// </summary>
    public IEnumerable<string> AvailableModels => Models.Keys;

    /// <summary>
    /// Tries to get a model configuration by name
    /// </summary>
    /// <param name="modelName">Name of the model</param>
    /// <param name="config">The configuration if found</param>
    /// <returns>True if model exists, false otherwise</returns>
    public bool TryGetModel(string modelName, out AnnBuilderConfig? config) {
        return Models.TryGetValue(modelName.ToLowerInvariant(), out config);
    }
}