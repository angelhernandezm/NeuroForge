# =============================================================================
# NeuroForge
# File: dataset_loader.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Loader class for datasets based on configuration.
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

class DatasetLoader:
    """
    Loads datasets based on the unified JSON schema.
    """

    @staticmethod
    def load(dataset_cfg: dict):
        source = dataset_cfg.get("source")
        dtype = dataset_cfg.get("type")

        if source == "cifar10":
            import tensorflow as tf
            (x_train, y_train), (x_test, y_test) = tf.keras.datasets.cifar10.load_data()

            # Optional class selection (GAN)
            cls = dataset_cfg.get("class_selection")
            if cls is not None:
                x_train = x_train[y_train.flatten() == cls]

            # Normalize
            if dataset_cfg.get("normalize", True):
                x_train = (x_train.astype("float32") / 127.5) - 1.0

            return x_train, None

        if dtype == "image":
            # Placeholder: implement folder loader
            raise NotImplementedError("Image folder loader not implemented yet.")

        if dtype == "tabular":
            # Placeholder: CSV/NumPy loader
            raise NotImplementedError("Tabular loader not implemented yet.")

        if dtype == "sequence":
            # Placeholder: JSON/text sequence loader
            raise NotImplementedError("Sequence loader not implemented yet.")

        if dtype == "text":
            # Placeholder: text corpus loader
            raise NotImplementedError("Text loader not implemented yet.")

        raise ValueError(f"Unsupported dataset type: {dtype}")
