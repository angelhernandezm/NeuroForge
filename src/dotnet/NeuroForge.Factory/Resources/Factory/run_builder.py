# =============================================================================
# NeuroForge
# File: run_builder.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Wrapper script for executing ANN builders from C#
#
# License: MIT
# =============================================================================
#
# MIT License
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in all
# copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
# SOFTWARE.
# =============================================================================

import sys
import json
import os
from pathlib import Path

def main():
    if len(sys.argv) < 3:
        print("Usage: run_builder.py <config_path> <output_path>", file=sys.stderr)
        sys.exit(1)

    config_path = sys.argv[1]
    output_path = sys.argv[2]

    print(f"[NeuroForge] Loading configuration from: {config_path}")

    # Load configuration
    try:
        with open(config_path, 'r') as f:
            config = json.load(f)
    except Exception as e:
        print(f"[ERROR] Failed to load configuration: {e}", file=sys.stderr)
        sys.exit(1)

    model_type = config.get('type')
    if not model_type:
        print("[ERROR] Missing 'type' in configuration", file=sys.stderr)
        sys.exit(1)

    print(f"[NeuroForge] Building model type: {model_type}")
    print(f"[NeuroForge] Output path: {output_path}")

    # Import model factory
    try:
        # Add current directory to path for imports
        sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

        from model_factory import ModelFactory

        print(f"[NeuroForge] Creating {model_type} builder...")

        # Create and build the model
        builder_class = ModelFactory.get_builder_class(model_type)
        builder = builder_class(config)

        print(f"[NeuroForge] Building model architecture...")
        result = builder.build()

        # Handle different return types from builders
        if isinstance(result, dict):
            model = result.get('model')
            if model is None:
                print("[ERROR] Builder returned dict but no 'model' key found", file=sys.stderr)
                sys.exit(1)
        else:
            model = result

        print(f"[NeuroForge] Model architecture created successfully")
        print(f"[NeuroForge] Model summary:")
        if hasattr(model, 'summary'):
            model.summary()

        # Create output directory
        os.makedirs(output_path, exist_ok=True)

        # Train the model if training config is provided
        if 'training' in config and 'dataset' in config:
            print(f"[NeuroForge] Training configuration found")
            training_config = config['training']
            dataset_config = config['dataset']

            epochs = training_config.get('epochs', 10)
            batch_size = training_config.get('batch_size', 32)

            print(f"[NeuroForge] Training parameters: epochs={epochs}, batch_size={batch_size}")
            print(f"[NeuroForge] Dataset source: {dataset_config.get('source', 'unknown')}")

            # Load dataset
            print(f"[NeuroForge] Loading dataset...")
            from dataset_loader import DatasetLoader

            dataset_loader = DatasetLoader(dataset_config)
            X_train, y_train, X_val, y_val = dataset_loader.load()

            print(f"[NeuroForge] Dataset loaded: Training samples={len(X_train)}, Validation samples={len(X_val)}")

            # Compile model (if not already compiled by builder)
            if not model.optimizer:
                print(f"[NeuroForge] Compiling model...")
                optimizer = config.get('params', {}).get('optimizer', 'adam')
                loss = config.get('params', {}).get('loss', 'sparse_categorical_crossentropy')
                model.compile(optimizer=optimizer, loss=loss, metrics=['accuracy'])

            # Train model
            print(f"[NeuroForge] Starting training...")
            history = model.fit(
                X_train, y_train,
                validation_data=(X_val, y_val),
                epochs=epochs,
                batch_size=batch_size,
                verbose=1
            )

            print(f"[NeuroForge] Training completed")

            # Save training history
            history_path = os.path.join(output_path, 'training_history.json')
            with open(history_path, 'w') as f:
                json.dump(history.history, f, indent=2)
            print(f"[NeuroForge] Training history saved to: {history_path}")

        # Save model in H5 format
        model_path = os.path.join(output_path, 'model.h5')
        model.save(model_path)
        print(f"[NeuroForge] Model saved to: {model_path}")

        # Save model in ONNX format
        try:
            print(f"[NeuroForge] Converting model to ONNX format...")
            import tf2onnx
            import tensorflow as tf

            # Get input shape from config or model
            input_shape = config.get('input_shape')
            if input_shape:
                # Add batch dimension
                spec = (tf.TensorSpec((None, *input_shape), tf.float32, name="input"),)
            else:
                # Try to infer from model
                spec = None

            onnx_model_path = os.path.join(output_path, 'model.onnx')

            # Convert to ONNX
            model_proto, _ = tf2onnx.convert.from_keras(
                model,
                input_signature=spec,
                opset=13,
                output_path=onnx_model_path
            )

            print(f"[NeuroForge] ONNX model saved to: {onnx_model_path}")

            # Save ONNX metadata
            onnx_metadata = {
                'format': 'ONNX',
                'opset_version': 13,
                'input_shape': input_shape,
                'framework': 'TensorFlow/Keras'
            }
            onnx_meta_path = os.path.join(output_path, 'model_onnx_metadata.json')
            with open(onnx_meta_path, 'w') as f:
                json.dump(onnx_metadata, f, indent=2)
            print(f"[NeuroForge] ONNX metadata saved to: {onnx_meta_path}")

        except Exception as e:
            print(f"[WARNING] Failed to convert model to ONNX: {e}", file=sys.stderr)
            print(f"[WARNING] Model is still available in H5 format", file=sys.stderr)

        # Save configuration used
        config_output_path = os.path.join(output_path, 'config.json')
        with open(config_output_path, 'w') as f:
            json.dump(config, f, indent=2)
        print(f"[NeuroForge] Configuration saved to: {config_output_path}")

        print(f"[NeuroForge] Build completed successfully!")
        return 0

    except Exception as e:
        import traceback
        print(f"[ERROR] Failed to build model: {e}", file=sys.stderr)
        traceback.print_exc(file=sys.stderr)
        sys.exit(1)

if __name__ == '__main__':
    sys.exit(main())