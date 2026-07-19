# =============================================================================
# NeuroForge
# File: gan_builder.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Builder class for creating GAN models based on configuration.
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

import numpy as np
import tensorflow as tf
from Builders.base_builder import BaseBuilder

class GanBuilder(BaseBuilder):

    def build(self):
        input_shape = tuple(self.config["input_shape"])

        self.latent_dim = self.get("latent_dim", 100)
        generator_units = self.get("generator_units", [256, 512, 1024])
        discriminator_units = self.get("discriminator_units", [512, 256])
        dropout = self.get("dropout", 0.3)
        optimizer = self.get("optimizer", "adam")
        loss = self.get("loss", "binary_crossentropy")

        output_dim = int(np.prod(input_shape))

        self.generator = self._build_generator(self.latent_dim, generator_units, output_dim, input_shape)
        self.discriminator = self._build_discriminator(input_shape, discriminator_units, dropout)
        self.discriminator.compile(optimizer=optimizer, loss=loss, metrics=["accuracy"])

        # The combined (stacked) model trains only the generator: freeze the
        # discriminator's weights while keeping it trainable on its own.
        self.discriminator.trainable = False
        gan_input = tf.keras.Input(shape=(self.latent_dim,))
        generated = self.generator(gan_input)
        validity = self.discriminator(generated)
        self.combined = tf.keras.Model(gan_input, validity, name="gan")
        self.combined.compile(optimizer=optimizer, loss=loss)
        self.discriminator.trainable = True

        return {
            "model": self.generator,
            "generator": self.generator,
            "discriminator": self.discriminator,
            "combined": self.combined
        }

    def _build_generator(self, latent_dim, units, output_dim, input_shape):
        inputs = tf.keras.Input(shape=(latent_dim,))
        x = inputs

        for u in units:
            x = tf.keras.layers.Dense(u, activation="relu")(x)
            x = tf.keras.layers.BatchNormalization()(x)

        x = tf.keras.layers.Dense(output_dim, activation="tanh")(x)
        outputs = tf.keras.layers.Reshape(input_shape)(x)

        return tf.keras.Model(inputs, outputs, name="generator")

    def _build_discriminator(self, input_shape, units, dropout):
        inputs = tf.keras.Input(shape=input_shape)
        x = tf.keras.layers.Flatten()(inputs)

        for u in units:
            x = tf.keras.layers.Dense(u, activation="relu")(x)
            x = tf.keras.layers.Dropout(dropout)(x)

        outputs = tf.keras.layers.Dense(1, activation="sigmoid")(x)

        return tf.keras.Model(inputs, outputs, name="discriminator")

    def custom_train(self, x_train, epochs, batch_size):
        """
        Runs adversarial training: alternately trains the discriminator on
        real vs. generator-produced batches, then trains the generator
        (through the frozen-discriminator combined model) to fool it.
        A standard Keras model.fit() loop cannot express this alternating
        adversarial objective, so GAN training is handled here instead of
        the generic training path in run_builder.py.
        """
        history = {"d_loss": [], "d_accuracy": [], "g_loss": []}
        num_batches = max(1, len(x_train) // batch_size)

        for epoch in range(epochs):
            d_losses, d_accs, g_losses = [], [], []

            for _ in range(num_batches):
                idx = np.random.randint(0, len(x_train), batch_size)
                real_images = x_train[idx]

                noise = np.random.normal(0, 1, (batch_size, self.latent_dim))
                fake_images = self.generator.predict(noise, verbose=0)

                real_labels = np.ones((batch_size, 1))
                fake_labels = np.zeros((batch_size, 1))

                d_loss_real = self.discriminator.train_on_batch(real_images, real_labels)
                d_loss_fake = self.discriminator.train_on_batch(fake_images, fake_labels)
                d_loss = 0.5 * np.add(d_loss_real, d_loss_fake)

                noise = np.random.normal(0, 1, (batch_size, self.latent_dim))
                g_loss = self.combined.train_on_batch(noise, real_labels)

                d_losses.append(d_loss[0])
                d_accs.append(d_loss[1])
                g_losses.append(g_loss)

            avg_d_loss = float(np.mean(d_losses))
            avg_d_acc = float(np.mean(d_accs))
            avg_g_loss = float(np.mean(g_losses))

            history["d_loss"].append(avg_d_loss)
            history["d_accuracy"].append(avg_d_acc)
            history["g_loss"].append(avg_g_loss)

            print(f"[NeuroForge][GAN] Epoch {epoch + 1}/{epochs} - "
                  f"d_loss: {avg_d_loss:.4f} - d_accuracy: {avg_d_acc:.4f} - g_loss: {avg_g_loss:.4f}")

        return history
