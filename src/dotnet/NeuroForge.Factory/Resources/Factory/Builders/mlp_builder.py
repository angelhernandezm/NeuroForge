# =============================================================================
# NeuroForge
# File: mlp_builder.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Builder class for creating MLP models based on configuration.
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
from Builders.base_builder import BaseBuilder

class MlpBuilder(BaseBuilder):

    def build(self):
        input_shape = tuple(self.config["input_shape"])
        num_classes = self.config["num_classes"]

        units = self.get("units", [128, 64, 32])
        dropout = self.get("dropout", 0.2)
        optimizer = self.get("optimizer", "adam")
        loss = self.get("loss", "sparse_categorical_crossentropy")

        layers = [tf.keras.Input(shape=input_shape)]
        for u in units:
            layers.append(tf.keras.layers.Dense(u, activation="relu"))
            layers.append(tf.keras.layers.Dropout(dropout))

        layers.append(tf.keras.layers.Dense(num_classes, activation="softmax"))

        model = tf.keras.Sequential(layers)
        model.compile(optimizer=optimizer, loss=loss, metrics=["accuracy"])

        return {
            "model": model
        }