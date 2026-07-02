// ============================================================================
// NeuroForge
// File: PythonRuntimeConfig.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Configuration for Python runtime installation
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
/// Configuration for Python runtime installation
/// </summary>
public class PythonRuntimeConfig {
    /// <summary>
    /// Gets or sets the python version.
    /// </summary>
    /// <value>The python version.</value>
    [JsonPropertyName("python_version")]
    public string PythonVersion { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the download URL.
    /// </summary>
    /// <value>The download URL.</value>
    [JsonPropertyName("download_url")]
    public string DownloadUrl { get; set; } = string.Empty;


    /// <summary>
    /// Gets or sets the install arguments.
    /// </summary>
    /// <value>The install arguments.</value>
    [JsonPropertyName("install_args")]
    public string[] InstallArgs { get; set; } = [];
}