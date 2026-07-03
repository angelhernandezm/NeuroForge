# =============================================================================
# NeuroForge
# File: model_factory.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Factory class for creating neural network models based on configuration.
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

import importlib

class ModelFactory:
    registry = {
        "cnn": "Builders.cnn_builder.CnnBuilder",
        "mlp": "Builders.mlp_builder.MlpBuilder",
        "autoencoder": "Builders.autoencoder_builder.AutoencoderBuilder",
        "gan": "Builders.gan_builder.GanBuilder",
        "rnn": "Builders.rnn_builder.RnnBuilder",
        "transformer": "Builders.transformer_builder.TransformerBuilder"
    }

    @classmethod
    def get_builder_class(cls, model_type: str):
        """Get the builder class for a specific model type"""
        if model_type not in cls.registry:
            raise ValueError(f"Unknown ANN type: {model_type}")

        module_path, class_name = cls.registry[model_type].rsplit(".", 1)
        module = importlib.import_module(module_path)
        builder_class = getattr(module, class_name)

        return builder_class

    @classmethod
    def create(cls, config: dict):
        model_type = config.get("type")
        if model_type not in cls.registry:
            raise ValueError(f"Unknown ANN type: {model_type}")

        builder_class = cls.get_builder_class(model_type)
        return builder_class(config).build()
