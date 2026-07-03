// ============================================================================
// NeuroForge
// File: DatasetConfig.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Configuration for dataset loading
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
/// Configuration for dataset loading
/// </summary>
public class DatasetConfig {
    /// <summary>
    /// Gets or sets the data source (e.g., "cifar10", "file", "tiny-imagenet")
    /// </summary>
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the dataset file or directory
    /// </summary>
    [JsonPropertyName("path")]
    public string? Path {
        get; set;
    }

    /// <summary>
    /// Gets or sets the dataset type (e.g., "image", "tabular", "sequence", "text")
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to normalize the data
    /// </summary>
    [JsonPropertyName("normalize")]
    public bool Normalize {
        get; set;
    }

    /// <summary>
    /// Gets or sets the validation split ratio
    /// </summary>
    [JsonPropertyName("validation_split")]
    public double? ValidationSplit {
        get; set;
    }

    /// <summary>
    /// Gets or sets additional properties as a dictionary
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? ExtensionData {
        get; set;
    }
}