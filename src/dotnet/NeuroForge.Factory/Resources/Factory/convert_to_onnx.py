# =============================================================================
# NeuroForge
# File: convert_to_onnx.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Utility to convert Keras/TensorFlow models to ONNX format
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
import os
import json
import argparse
import subprocess
import shutil

def convert_model_to_onnx(model_path, output_path=None, opset=17):
    """
    Convert a Keras/TensorFlow model to ONNX format using tf2onnx CLI

    Args:
        model_path: Path to the H5 model file
        output_path: Path for the output ONNX file (optional)
        opset: ONNX opset version (default: 17)

    Returns:
        Path to the saved ONNX model
    """
    try:
        import tensorflow as tf

        print(f"[NeuroForge] Loading model from: {model_path}")

        # Load the Keras model
        model = tf.keras.models.load_model(model_path)

        print(f"[NeuroForge] Model loaded successfully")
        print(f"[NeuroForge] Model summary:")
        model.summary()

        # Determine output path
        if output_path is None:
            base_path = os.path.splitext(model_path)[0]
            output_path = f"{base_path}.onnx"

        print(f"[NeuroForge] Converting to ONNX (opset {opset}) using tf2onnx CLI...")

        # Create temporary directory for SavedModel format
        saved_model_dir = os.path.join(os.path.dirname(model_path), "saved_model_temp")

        # Export to SavedModel format (required for tf2onnx CLI)
        print(f"[NeuroForge] Exporting to SavedModel format...")
        model.export(saved_model_dir)

        print(f"[NeuroForge] Running tf2onnx conversion...")

        # Use tf2onnx CLI for conversion (works better with TF 2.15+)
        result = subprocess.run([
            sys.executable, "-m", "tf2onnx.convert",
            "--saved-model", saved_model_dir,
            "--output", output_path,
            "--opset", str(opset)
        ], check=True, capture_output=True, text=True)

        # Print tf2onnx output
        if result.stdout:
            print(result.stdout)

        # Clean up temporary SavedModel directory
        if os.path.exists(saved_model_dir):
            shutil.rmtree(saved_model_dir)
            print(f"[NeuroForge] Cleaned up temporary SavedModel directory")

        print(f"[NeuroForge] ONNX model saved to: {output_path}")

        # Save metadata
        metadata = {
            'format': 'ONNX',
            'opset_version': opset,
            'source_model': os.path.basename(model_path),
            'framework': 'TensorFlow/Keras',
            'conversion_method': 'tf2onnx CLI (subprocess)',
            'input_shapes': [list(input_layer.shape) for input_layer in model.inputs] if model.inputs else None,
            'output_shapes': [list(output_layer.shape) for output_layer in model.outputs] if model.outputs else None
        }

        metadata_path = f"{os.path.splitext(output_path)[0]}_metadata.json"
        with open(metadata_path, 'w') as f:
            json.dump(metadata, f, indent=2)

        print(f"[NeuroForge] Metadata saved to: {metadata_path}")
        print(f"[NeuroForge] Conversion completed successfully!")

        return output_path

    except subprocess.CalledProcessError as e:
        print(f"[ERROR] tf2onnx conversion failed with exit code {e.returncode}", file=sys.stderr)
        if e.stdout:
            print(f"[ERROR] stdout: {e.stdout}", file=sys.stderr)
        if e.stderr:
            print(f"[ERROR] stderr: {e.stderr}", file=sys.stderr)

        # Clean up on failure
        saved_model_dir = os.path.join(os.path.dirname(model_path), "saved_model_temp")
        if os.path.exists(saved_model_dir):
            shutil.rmtree(saved_model_dir)

        print(f"[WARNING] ONNX conversion failed, but H5 model is still available at: {model_path}", file=sys.stderr)
        return None

    except ImportError as e:
        print(f"[ERROR] Required package not found: {e}", file=sys.stderr)
        print(f"[ERROR] Please install: pip install tensorflow tf2onnx", file=sys.stderr)
        return None

    except Exception as e:
        print(f"[ERROR] Conversion failed: {e}", file=sys.stderr)
        import traceback
        traceback.print_exc(file=sys.stderr)
        sys.exit(1)

def main():
    parser = argparse.ArgumentParser(
        description='Convert Keras/TensorFlow models to ONNX format',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  # Convert a single model
  python convert_to_onnx.py model.h5

  # Convert with custom output path
  python convert_to_onnx.py model.h5 -o output/model.onnx

  # Convert with specific opset version
  python convert_to_onnx.py model.h5 --opset 14
        """
    )

    parser.add_argument('model_path', help='Path to the H5 model file')
    parser.add_argument('-o', '--output', help='Output path for ONNX file (optional)')
    parser.add_argument('--opset', type=int, default=13, help='ONNX opset version (default: 13)')

    args = parser.parse_args()

    # Check if model file exists
    if not os.path.exists(args.model_path):
        print(f"[ERROR] Model file not found: {args.model_path}", file=sys.stderr)
        sys.exit(1)

    # Convert the model
    convert_model_to_onnx(args.model_path, args.output, args.opset)

if __name__ == '__main__':
    main()