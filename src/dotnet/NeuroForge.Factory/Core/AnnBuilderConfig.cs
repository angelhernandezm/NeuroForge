// ============================================================================
// NeuroForge
// File: AnnBuilderConfig.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Configuration for individual ANN builder
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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace NeuroForge.Factory.Core;

/// <summary>
/// Configuration for individual ANN builder
/// </summary>
public class AnnBuilderConfig {
    /// <summary>
    /// Gets or sets the model type (e.g., "cnn", "mlp", "rnn")
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the input shape
    /// </summary>
    [JsonPropertyName("input_shape")]
    public int[]? InputShape {
        get; set;
    }

    /// <summary>
    /// Gets or sets the number of classes (null for unsupervised models)
    /// </summary>
    [JsonPropertyName("num_classes")]
    public int? NumClasses {
        get; set;
    }

    /// <summary>
    /// Gets or sets the dataset configuration
    /// </summary>
    [JsonPropertyName("dataset")]
    public DatasetConfig Dataset { get; set; } = new();

    /// <summary>
    /// Gets or sets the model parameters
    /// </summary>
    [JsonPropertyName("params")]
    public JsonElement? Params {
        get; set;
    }

    /// <summary>
    /// Gets or sets the training configuration
    /// </summary>
    [JsonPropertyName("training")]
    public TrainingConfig Training {
        get; set;
    } = new();
}
