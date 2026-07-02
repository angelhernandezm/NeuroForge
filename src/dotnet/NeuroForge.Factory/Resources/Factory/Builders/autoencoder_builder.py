# =============================================================================
# NeuroForge
# File: autoencoder_builder.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Builder class for creating Autoencoder models based on configuration.
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

class AutoencoderBuilder(BaseBuilder):

    def build(self):
        x_train, _ = DatasetLoader.load(self.dataset)

        input_shape = tuple(self.config["input_shape"])
        encoder_units = self.get("encoder_units", [128, 64])
        decoder_units = self.get("decoder_units", [64, 128])
        bottleneck = self.get("bottleneck", 32)
        optimizer = self.get("optimizer", "adam")
        loss = self.get("loss", "mse")

        inputs = tf.keras.Input(shape=input_shape)
        x = inputs

        for u in encoder_units:
            x = tf.keras.layers.Dense(u, activation="relu")(x)

        encoded = tf.keras.layers.Dense(bottleneck, activation="relu")(x)

        x = encoded
        for u in decoder_units:
            x = tf.keras.layers.Dense(u, activation="relu")(x)

        outputs = tf.keras.layers.Dense(input_shape[0], activation="sigmoid")(x)

        model = tf.keras.Model(inputs, outputs)
        model.compile(optimizer=optimizer, loss=loss)

        return {
            "model": model,
            "dataset": x_train
        }
