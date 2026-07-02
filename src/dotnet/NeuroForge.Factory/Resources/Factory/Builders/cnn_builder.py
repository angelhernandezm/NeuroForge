# =============================================================================
# NeuroForge
# File: cnn_builder.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Builder class for creating CNN models based on configuration.
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

import tensorflow as tf
from Factory.Builders.base_builder import BaseBuilder
from Factory.dataset_loader import DatasetLoader

class CnnBuilder(BaseBuilder):

    def build(self):
        # Load dataset
        x_train, _ = DatasetLoader.load(self.dataset)

        input_shape = tuple(self.config["input_shape"])
        num_classes = self.config["num_classes"]

        filters = self.get("filters", [32, 64, 128])
        kernel_size = self.get("kernel_size", 3)
        dense_units = self.get("dense_units", 256)
        dropout = self.get("dropout", 0.3)
        optimizer = self.get("optimizer", "adam")
        loss = self.get("loss", "sparse_categorical_crossentropy")

        layers = [tf.keras.Input(shape=input_shape)]

        for f in filters:
            layers.append(tf.keras.layers.Conv2D(f, kernel_size, activation="relu", padding="same"))
            layers.append(tf.keras.layers.MaxPooling2D())

        layers.extend([
            tf.keras.layers.Flatten(),
            tf.keras.layers.Dense(dense_units, activation="relu"),
            tf.keras.layers.Dropout(dropout),
            tf.keras.layers.Dense(num_classes, activation="softmax")
        ])

        model = tf.keras.Sequential(layers)
        model.compile(optimizer=optimizer, loss=loss, metrics=["accuracy"])

        return {
            "model": model,
            "dataset": x_train
        }