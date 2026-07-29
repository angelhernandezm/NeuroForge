// ============================================================================
// NeuroForge
// File: TrainingConfig.cs
// Author: Angel Hernandez (me@angelhernandezm.com)
// Description:
//     Configuration for model training parameters
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
/// Configuration for model training parameters
/// </summary>
public class TrainingConfig {
    /// <summary>
    /// Gets or sets the number of training epochs
    /// </summary>
    [JsonPropertyName("epochs")]
    public int Epochs {
        get; set;
    }

    /// <summary>
    /// Gets or sets the batch size
    /// </summary>
    [JsonPropertyName("batch_size")]
    public int BatchSize {
        get; set;
    }

    /// <summary>
    /// Gets or sets the random seed used to make training reproducible.
    /// When set, Python's <c>random</c>, NumPy, and TensorFlow global RNGs
    /// are seeded before the model is built and trained, so running the
    /// same configuration on different machines produces the same result.
    /// Leave unset (<c>null</c>) to keep non-deterministic behavior.
    /// </summary>
    [JsonPropertyName("seed")]
    public int? Seed {
        get; set;
    }

    /// <summary>
    /// Gets or sets whether TensorFlow's deterministic op kernels should be
    /// enabled (<c>tf.config.experimental.enable_op_determinism()</c>) when a
    /// <see cref="Seed"/> is set. Deterministic ops guarantee bit-for-bit
    /// reproducible results, including on GPU, at the cost of some training
    /// speed. Defaults to <c>true</c>; set to <c>false</c> to seed the RNGs
    /// for reproducibility while keeping the faster, non-deterministic
    /// kernels (results may still vary slightly, especially on GPU).
    /// </summary>
    [JsonPropertyName("deterministic_ops")]
    public bool DeterministicOps { get; set; } = true;

    /// <summary>
    /// Gets or sets the EarlyStopping callback configuration. When set, training
    /// stops once the monitored metric stops improving for <c>patience</c> epochs.
    /// </summary>
    [JsonPropertyName("early_stopping")]
    public EarlyStoppingConfig? EarlyStopping {
        get; set;
    }

    /// <summary>
    /// Gets or sets the ReduceLROnPlateau callback configuration. When set, the
    /// learning rate is reduced once the monitored metric stops improving.
    /// </summary>
    [JsonPropertyName("reduce_lr_on_plateau")]
    public ReduceLROnPlateauConfig? ReduceLROnPlateau {
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

/// <summary>
/// Configuration for Keras' <c>EarlyStopping</c> callback
/// (https://keras.io/api/callbacks/early_stopping/)
/// </summary>
public class EarlyStoppingConfig {
    /// <summary>
    /// Gets or sets whether this callback is enabled
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the metric to monitor (e.g., "val_loss", "loss")
    /// </summary>
    [JsonPropertyName("monitor")]
    public string Monitor { get; set; } = "val_loss";

    /// <summary>
    /// Gets or sets the number of epochs with no improvement after which
    /// training will be stopped. Recommended range: 3-5.
    /// </summary>
    [JsonPropertyName("patience")]
    public int Patience { get; set; } = 3;

    /// <summary>
    /// Gets or sets the minimum change in the monitored metric to qualify as
    /// an improvement
    /// </summary>
    [JsonPropertyName("min_delta")]
    public double MinDelta {
        get; set;
    }

    /// <summary>
    /// Gets or sets whether to restore model weights from the epoch with the
    /// best value of the monitored metric
    /// </summary>
    [JsonPropertyName("restore_best_weights")]
    public bool RestoreBestWeights { get; set; } = true;
}

/// <summary>
/// Configuration for Keras' <c>ReduceLROnPlateau</c> callback
/// (https://keras.io/api/callbacks/reduce_lr_on_plateau/)
/// </summary>
public class ReduceLROnPlateauConfig {
    /// <summary>
    /// Gets or sets whether this callback is enabled
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the metric to monitor (e.g., "val_loss", "loss")
    /// </summary>
    [JsonPropertyName("monitor")]
    public string Monitor { get; set; } = "val_loss";

    /// <summary>
    /// Gets or sets the factor by which the learning rate will be reduced
    /// (new_lr = lr * factor)
    /// </summary>
    [JsonPropertyName("factor")]
    public double Factor { get; set; } = 0.1;

    /// <summary>
    /// Gets or sets the number of epochs with no improvement after which the
    /// learning rate will be reduced
    /// </summary>
    [JsonPropertyName("patience")]
    public int Patience { get; set; } = 10;

    /// <summary>
    /// Gets or sets the lower bound on the learning rate
    /// </summary>
    [JsonPropertyName("min_lr")]
    public double MinLr {
        get; set;
    }
}