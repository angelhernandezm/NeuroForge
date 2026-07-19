# =============================================================================
# NeuroForge
# File: onnx_export.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Shared helper for exporting a Keras/TensorFlow model to ONNX format.
#     Used by both run_builder.py and convert_to_onnx.py so the SavedModel
#     + tf2onnx CLI conversion logic lives in a single place.
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

import os
import sys
import subprocess
import shutil


def export_to_onnx(model, output_path, opset=17):
    """
    Exports a Keras/TensorFlow model to ONNX format.

    Converts the model to SavedModel format first, then invokes the tf2onnx
    CLI as a subprocess (works better than the in-process API with TF 2.15+),
    and cleans up the temporary SavedModel directory afterwards.

    Args:
        model: Keras model instance to export.
        output_path: Destination path for the .onnx file.
        opset: ONNX opset version (default: 17).

    Returns:
        True if conversion succeeded, False otherwise. Failures are logged
        to stderr but never raise, so callers can fall back to the H5 model.
    """
    saved_model_dir = os.path.join(os.path.dirname(output_path), "saved_model_temp")

    try:
        print(f"[NeuroForge] Exporting to SavedModel format...")
        model.export(saved_model_dir)

        print(f"[NeuroForge] Running tf2onnx CLI conversion...")
        result = subprocess.run(
            [
                sys.executable, "-m", "tf2onnx.convert",
                "--saved-model", saved_model_dir,
                "--output", output_path,
                "--opset", str(opset)
            ],
            check=True, capture_output=True, text=True
        )

        if result.stdout:
            for line in result.stdout.split('\n'):
                if line.strip():
                    print(f"[tf2onnx] {line}")

        print(f"[NeuroForge] ONNX model saved to: {output_path}")
        return True

    except subprocess.CalledProcessError as e:
        print(f"[WARNING] tf2onnx conversion failed with exit code {e.returncode}", file=sys.stderr)
        if e.stdout:
            print(f"[WARNING] stdout: {e.stdout}", file=sys.stderr)
        if e.stderr:
            print(f"[WARNING] stderr: {e.stderr}", file=sys.stderr)
        return False

    except Exception as e:
        print(f"[WARNING] ONNX conversion failed: {e}", file=sys.stderr)
        return False

    finally:
        if os.path.exists(saved_model_dir):
            shutil.rmtree(saved_model_dir, ignore_errors=True)
            print(f"[NeuroForge] Cleaned up temporary SavedModel directory")
