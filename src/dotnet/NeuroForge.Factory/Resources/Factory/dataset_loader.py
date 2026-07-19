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

import os
import numpy as np
from pathlib import Path

class DatasetLoader:
    """
    Loads datasets based on the unified JSON schema.
    """

    # Built-in image datasets bundled with tf.keras.datasets. Grayscale
    # datasets (mnist/fashion_mnist) are auto-expanded with a channel axis
    # so they are compatible with Conv2D-based builders (CNN/GAN).
    _BUILTIN_IMAGE_DATASETS = ("cifar10", "cifar100", "mnist", "fashion_mnist")

    # Built-in sequence/text datasets bundled with tf.keras.datasets. These
    # are returned as integer word-index sequences padded to a fixed length.
    _BUILTIN_SEQUENCE_DATASETS = ("imdb", "reuters")

    @staticmethod
    def load(dataset_cfg: dict):
        source = dataset_cfg.get("source")
        dtype = dataset_cfg.get("type")

        if source in DatasetLoader._BUILTIN_IMAGE_DATASETS:
            return DatasetLoader._load_builtin_image_dataset(source, dataset_cfg)

        if source in DatasetLoader._BUILTIN_SEQUENCE_DATASETS:
            return DatasetLoader._load_builtin_sequence_dataset(source, dataset_cfg)

        if dtype == "image":
            return DatasetLoader._load_image_folder(dataset_cfg)

        if dtype == "tabular":
            return DatasetLoader._load_tabular(dataset_cfg)

        if dtype == "sequence":
            # Placeholder: JSON/text sequence loader
            raise NotImplementedError("Sequence loader not implemented yet.")

        if dtype == "text":
            # Placeholder: text corpus loader
            raise NotImplementedError("Text loader not implemented yet.")

        raise ValueError(f"Unsupported dataset type: {dtype}")

    @staticmethod
    def _load_builtin_image_dataset(source: str, dataset_cfg: dict):
        """
        Load a built-in tf.keras.datasets image dataset (cifar10, cifar100,
        mnist, fashion_mnist).

        Config parameters:
            - class_selection: Optional label to filter on (used by GAN builders
              that train on a single class, e.g. CIFAR-10 "airplane")
            - normalize: Whether to normalize pixel values to [-1, 1] (default: True)
        """
        import tensorflow as tf

        dataset_module = getattr(tf.keras.datasets, source)
        (x_train, y_train), (_, _) = dataset_module.load_data()

        # Grayscale datasets (mnist/fashion_mnist) don't have a channel axis;
        # add one so they are compatible with Conv2D-based builders.
        if x_train.ndim == 3:
            x_train = np.expand_dims(x_train, axis=-1)

        # Optional class selection (GAN)
        cls = dataset_cfg.get("class_selection")
        if cls is not None:
            mask = y_train.flatten() == cls
            x_train = x_train[mask]
            y_train = y_train[mask]

        # Normalize
        if dataset_cfg.get("normalize", True):
            x_train = (x_train.astype("float32") / 127.5) - 1.0

        return x_train, y_train

    @staticmethod
    def _load_builtin_sequence_dataset(source: str, dataset_cfg: dict):
        """
        Load a built-in tf.keras.datasets sequence/text dataset (imdb, reuters).

        Returns integer word-index sequences padded/truncated to a fixed length.

        Config parameters:
            - vocab_size: Maximum number of distinct words to keep (default: 10000)
            - max_length: Sequence length to pad/truncate to (default: 200)
        """
        import tensorflow as tf

        vocab_size = dataset_cfg.get("vocab_size", 10000)
        max_length = dataset_cfg.get("max_length", 200)

        dataset_module = getattr(tf.keras.datasets, source)
        (x_train, y_train), (_, _) = dataset_module.load_data(num_words=vocab_size)

        x_train = tf.keras.preprocessing.sequence.pad_sequences(x_train, maxlen=max_length)

        return x_train, y_train

    @staticmethod
    def _load_image_folder(dataset_cfg: dict):
        """
        Load images from a directory structure.

        Expected structure:
            path/
                class_0/
                    image1.jpg
                    image2.png
                    ...
                class_1/
                    image1.jpg
                    ...

        Config parameters:
            - path: Root directory path
            - target_size: Tuple (height, width) for resizing images
            - color_mode: 'rgb' or 'grayscale'
            - normalize: Whether to normalize pixel values
        """
        import tensorflow as tf
        from tensorflow.keras.preprocessing import image as keras_image

        path = dataset_cfg.get("path")
        if not path:
            raise ValueError("'path' parameter required for image folder loading")

        if not os.path.exists(path):
            raise FileNotFoundError(f"Image directory not found: {path}")

        target_size = dataset_cfg.get("target_size", (128, 128))
        color_mode = dataset_cfg.get("color_mode", "rgb")
        normalize = dataset_cfg.get("normalize", True)
        batch_size = dataset_cfg.get("batch_size", 32)

        # Use ImageDataGenerator for loading
        if normalize:
            datagen = tf.keras.preprocessing.image.ImageDataGenerator(
                rescale=1./255
            )
        else:
            datagen = tf.keras.preprocessing.image.ImageDataGenerator()

        # Load images from directory
        generator = datagen.flow_from_directory(
            path,
            target_size=target_size,
            color_mode=color_mode,
            class_mode='sparse',
            batch_size=batch_size,
            shuffle=True
        )

        # Load all images into memory
        num_samples = generator.n
        num_batches = (num_samples + batch_size - 1) // batch_size

        images = []
        labels = []

        for i in range(num_batches):
            batch_images, batch_labels = next(generator)
            images.append(batch_images)
            labels.append(batch_labels)

        x_data = np.concatenate(images, axis=0)[:num_samples]
        y_data = np.concatenate(labels, axis=0)[:num_samples]

        return x_data, y_data

    @staticmethod
    def _load_tabular(dataset_cfg: dict):
        """
        Load tabular data from CSV, Excel, or NumPy files.

        Config parameters:
            - path: File path (CSV, Excel, or .npy/.npz)
            - target_column: Name or index of target column (for CSV/Excel)
            - features: List of feature column names/indices (optional)
            - normalize: Whether to normalize features
            - test_size: Fraction for train/test split (optional)
        """
        path = dataset_cfg.get("path")
        if not path:
            raise ValueError("'path' parameter required for tabular data loading")

        if not os.path.exists(path):
            raise FileNotFoundError(f"Data file not found: {path}")

        file_ext = Path(path).suffix.lower()

        # Load based on file type
        if file_ext == '.npy':
            data = np.load(path)
            target_column = dataset_cfg.get("target_column")

            if target_column is not None:
                if isinstance(target_column, int):
                    x_data = np.delete(data, target_column, axis=1)
                    y_data = data[:, target_column]
                else:
                    raise ValueError("For .npy files, target_column must be an integer index")
            else:
                x_data = data
                y_data = None

        elif file_ext == '.npz':
            data = np.load(path)
            x_key = dataset_cfg.get("x_key", "x")
            y_key = dataset_cfg.get("y_key", "y")

            x_data = data[x_key]
            y_data = data[y_key] if y_key in data else None

        elif file_ext in ['.csv', '.tsv', '.txt']:
            import pandas as pd

            separator = dataset_cfg.get("separator", "," if file_ext == ".csv" else "\t")
            df = pd.read_csv(path, sep=separator)

            target_column = dataset_cfg.get("target_column")
            features = dataset_cfg.get("features")

            if target_column is not None:
                if isinstance(target_column, str):
                    y_data = df[target_column].values
                    df = df.drop(columns=[target_column])
                elif isinstance(target_column, int):
                    y_data = df.iloc[:, target_column].values
                    df = df.drop(df.columns[target_column], axis=1)
                else:
                    raise ValueError("target_column must be a string (column name) or int (column index)")
            else:
                y_data = None

            if features:
                if isinstance(features[0], str):
                    x_data = df[features].values
                else:
                    x_data = df.iloc[:, features].values
            else:
                x_data = df.values

        elif file_ext in ['.xlsx', '.xls']:
            import pandas as pd

            df = pd.read_excel(path)

            target_column = dataset_cfg.get("target_column")
            features = dataset_cfg.get("features")

            if target_column is not None:
                if isinstance(target_column, str):
                    y_data = df[target_column].values
                    df = df.drop(columns=[target_column])
                elif isinstance(target_column, int):
                    y_data = df.iloc[:, target_column].values
                    df = df.drop(df.columns[target_column], axis=1)
                else:
                    raise ValueError("target_column must be a string (column name) or int (column index)")
            else:
                y_data = None

            if features:
                if isinstance(features[0], str):
                    x_data = df[features].values
                else:
                    x_data = df.iloc[:, features].values
            else:
                x_data = df.values
        else:
            raise ValueError(f"Unsupported file format: {file_ext}")

        # Convert to float32
        x_data = x_data.astype("float32")
        if y_data is not None:
            # Keep y_data as is if it's already numeric, otherwise try to convert
            if y_data.dtype == object:
                from sklearn.preprocessing import LabelEncoder
                le = LabelEncoder()
                y_data = le.fit_transform(y_data)

        # Normalize if requested
        if dataset_cfg.get("normalize", False):
            mean = x_data.mean(axis=0)
            std = x_data.std(axis=0)
            std[std == 0] = 1  # Avoid division by zero
            x_data = (x_data - mean) / std

        return x_data, y_data
