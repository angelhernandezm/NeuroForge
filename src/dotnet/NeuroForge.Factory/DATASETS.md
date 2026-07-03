# NeuroForge Dataset Guide

## Overview

This guide provides comprehensive information about the supported model types in NeuroForge and recommended datasets for training and testing each model architecture.

---

## Supported Model Types

NeuroForge currently supports **six** neural network architectures:

| Model Type | Description | Best Use Cases |
|------------|-------------|----------------|
| **MLP** | Multi-Layer Perceptron | Tabular data, feature vectors, simple classification |
| **CNN** | Convolutional Neural Network | Image classification, computer vision tasks |
| **RNN** | Recurrent Neural Network (LSTM/SimpleRNN) | Time series, sequence prediction, text processing |
| **Autoencoder** | Encoder-Decoder architecture | Dimensionality reduction, feature learning, anomaly detection |
| **GAN** | Generative Adversarial Network | Image generation, data augmentation |
| **Transformer** | Self-attention based architecture | NLP tasks, sequence-to-sequence, text classification |

---

## Dataset Requirements by Model Type

### 1. MLP (Multi-Layer Perceptron)

**Input Shape:** `[feature_count]` (1D array)  
**Data Type:** Tabular/Feature vectors  
**Dataset Format:** CSV, NumPy arrays, Pandas DataFrames

#### Recommended Datasets:

##### **MNIST (Flattened)**
- **Description:** Handwritten digit classification (0-9)
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 60,000
  - Test samples: 10,000
  - Input shape: `[784]` (28x28 pixels flattened)
  - Classes: 10
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, y_train), (x_test, y_test) = tf.keras.datasets.mnist.load_data()
  x_train = x_train.reshape(-1, 784) / 255.0
  x_test = x_test.reshape(-1, 784) / 255.0
  ```

##### **Iris Dataset**
- **Description:** Classic flower species classification
- **Download:** https://archive.ics.uci.edu/ml/datasets/iris
- **Specifications:**
  - Total samples: 150
  - Input shape: `[4]` (sepal/petal measurements)
  - Classes: 3
- **Download URL:** https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data

##### **Wine Quality Dataset**
- **Description:** Wine quality prediction based on chemical properties
- **Download:** https://archive.ics.uci.edu/ml/datasets/wine+quality
- **Specifications:**
  - Red wine samples: 1,599
  - White wine samples: 4,898
  - Input shape: `[11]` features
  - Classes: 6-7 (quality scores)
- **Download URL:** https://archive.ics.uci.edu/ml/machine-learning-databases/wine-quality/

---

### 2. CNN (Convolutional Neural Network)

**Input Shape:** `[height, width, channels]` (3D array)  
**Data Type:** Image  
**Dataset Format:** Image files (JPEG, PNG), NumPy arrays

#### Recommended Datasets:

##### **CIFAR-10**
- **Description:** 10-class image classification
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 50,000
  - Test samples: 10,000
  - Input shape: `[32, 32, 3]`
  - Classes: 10 (airplane, automobile, bird, cat, deer, dog, frog, horse, ship, truck)
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, y_train), (x_test, y_test) = tf.keras.datasets.cifar10.load_data()
  ```

##### **CIFAR-100**
- **Description:** 100-class fine-grained image classification
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 50,000
  - Test samples: 10,000
  - Input shape: `[32, 32, 3]`
  - Classes: 100
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, y_train), (x_test, y_test) = tf.keras.datasets.cifar100.load_data()
  ```

##### **Tiny ImageNet**
- **Description:** Subset of ImageNet with 200 classes
- **Download:** http://cs231n.stanford.edu/tiny-imagenet-200.zip
- **Specifications:**
  - Training samples: 100,000
  - Validation samples: 10,000
  - Input shape: `[64, 64, 3]`
  - Classes: 200
- **Download URL:** http://cs231n.stanford.edu/tiny-imagenet-200.zip

##### **Fashion MNIST**
- **Description:** Clothing item classification
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 60,000
  - Test samples: 10,000
  - Input shape: `[28, 28, 1]`
  - Classes: 10
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, y_train), (x_test, y_test) = tf.keras.datasets.fashion_mnist.load_data()
  x_train = x_train[..., np.newaxis] / 255.0
  ```

---

### 3. RNN (Recurrent Neural Network)

**Input Shape:** `[sequence_length, features]` (2D array)  
**Data Type:** Sequence/Time-series  
**Dataset Format:** CSV, JSON, text files, NumPy arrays

#### Recommended Datasets:

##### **IMDB Movie Reviews**
- **Description:** Binary sentiment analysis (positive/negative)
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 25,000
  - Test samples: 25,000
  - Input shape: `[sequence_length, 1]` (after embedding)
  - Classes: 2
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, y_train), (x_test, y_test) = tf.keras.datasets.imdb.load_data(num_words=10000)
  ```

##### **Time Series - Air Passengers**
- **Description:** Monthly airline passenger counts (forecasting)
- **Download:** https://raw.githubusercontent.com/jbrownlee/Datasets/master/airline-passengers.csv
- **Specifications:**
  - Total samples: 144 months
  - Input shape: `[sequence_length, 1]`
  - Task: Regression/Forecasting
- **Download URL:** https://raw.githubusercontent.com/jbrownlee/Datasets/master/airline-passengers.csv

##### **UCI Human Activity Recognition (HAR)**
- **Description:** Sensor-based human activity classification
- **Download:** https://archive.ics.uci.edu/ml/datasets/human+activity+recognition+using+smartphones
- **Specifications:**
  - Training samples: 7,352
  - Test samples: 2,947
  - Input shape: `[128, 9]` (128 timesteps, 9 sensor readings)
  - Classes: 6 activities
- **Download URL:** https://archive.ics.uci.edu/ml/machine-learning-databases/00240/UCI%20HAR%20Dataset.zip

---

### 4. Autoencoder

**Input Shape:** `[feature_count]` or `[height, width, channels]`  
**Data Type:** Any (typically same as input for reconstruction)  
**Dataset Format:** NumPy arrays, images

#### Recommended Datasets:

##### **MNIST**
- **Description:** Digit reconstruction and feature learning
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 60,000
  - Input shape: `[784]` or `[28, 28, 1]`
  - Task: Unsupervised reconstruction
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, _), (x_test, _) = tf.keras.datasets.mnist.load_data()
  x_train = x_train.reshape(-1, 784) / 255.0
  ```

##### **Fashion MNIST**
- **Description:** Clothing item reconstruction
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 60,000
  - Input shape: `[784]` or `[28, 28, 1]`
  - Task: Unsupervised reconstruction
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, _), (x_test, _) = tf.keras.datasets.fashion_mnist.load_data()
  x_train = x_train.reshape(-1, 784) / 255.0
  ```

##### **Olivetti Faces**
- **Description:** Face image reconstruction
- **Download:** https://scikit-learn.org/stable/modules/generated/sklearn.datasets.fetch_olivetti_faces.html
- **Specifications:**
  - Total samples: 400
  - Input shape: `[4096]` (64x64 images flattened)
  - Task: Face reconstruction, dimensionality reduction
- **Download Code:**
  ```python
  from sklearn.datasets import fetch_olivetti_faces
  data = fetch_olivetti_faces(shuffle=True)
  x_train = data.data
  ```

---

### 5. GAN (Generative Adversarial Network)

**Input Shape:** `[height, width, channels]` for discriminator  
**Data Type:** Image  
**Dataset Format:** Image files, NumPy arrays

#### Recommended Datasets:

##### **CIFAR-10 (Single Class)**
- **Description:** Generate images of a specific class
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Samples per class: 5,000
  - Input shape: `[32, 32, 3]`
  - Recommended: Use `class_selection` parameter to train on one class
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, y_train), _ = tf.keras.datasets.cifar10.load_data()
  # Filter for class 0 (airplane)
  x_train = x_train[y_train.flatten() == 0]
  ```

##### **CelebA Faces**
- **Description:** Celebrity face generation
- **Download:** http://mmlab.ie.cuhk.edu.hk/projects/CelebA.html
- **Specifications:**
  - Training samples: 202,599
  - Input shape: `[218, 178, 3]` (original) or resized to `[64, 64, 3]`
  - Task: Face generation
- **Download URL:** https://drive.google.com/drive/folders/0B7EVK8r0v71pTUZsaXdaSnZBZzg

##### **MNIST**
- **Description:** Handwritten digit generation
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 60,000
  - Input shape: `[28, 28, 1]`
  - Task: Digit generation
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, _), _ = tf.keras.datasets.mnist.load_data()
  x_train = (x_train.astype('float32') / 127.5) - 1.0  # Normalize to [-1, 1]
  ```

---

### 6. Transformer

**Input Shape:** `[sequence_length, embedding_dim]`  
**Data Type:** Sequence/Text  
**Dataset Format:** Text files, tokenized sequences

#### Recommended Datasets:

##### **IMDB Movie Reviews**
- **Description:** Sentiment classification with attention
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 25,000
  - Input shape: `[max_length, embedding_dim]`
  - Classes: 2
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, y_train), (x_test, y_test) = tf.keras.datasets.imdb.load_data(num_words=20000)
  ```

##### **AG News**
- **Description:** News article classification
- **Download:** https://www.kaggle.com/datasets/amananandrai/ag-news-classification-dataset
- **Specifications:**
  - Training samples: 120,000
  - Test samples: 7,600
  - Input shape: `[max_length, embedding_dim]`
  - Classes: 4 (World, Sports, Business, Sci/Tech)
- **Download URL:** https://www.kaggle.com/datasets/amananandrai/ag-news-classification-dataset/download

##### **Reuters Newswire**
- **Description:** Multi-label topic classification
- **Download:** Built-in with TensorFlow/Keras
- **Specifications:**
  - Training samples: 8,982
  - Test samples: 2,246
  - Input shape: `[max_length, embedding_dim]`
  - Classes: 46
- **Download Code:**
  ```python
  import tensorflow as tf
  (x_train, y_train), (x_test, y_test) = tf.keras.datasets.reuters.load_data(num_words=10000)
  ```

---

## Dataset Configuration Examples

### Example 1: CNN with CIFAR-10

```json
{
  "type": "cnn",
  "input_shape": [32, 32, 3],
  "num_classes": 10,
  "dataset": {
	"source": "cifar10",
	"type": "image",
	"normalize": true
  },
  "training": {
	"epochs": 50,
	"batch_size": 128
  }
}
```

### Example 2: MLP with Custom CSV

```json
{
  "type": "mlp",
  "input_shape": [784],
  "num_classes": 10,
  "dataset": {
	"source": "file",
	"type": "tabular",
	"path": "data/my_dataset.csv",
	"normalize": true
  },
  "training": {
	"epochs": 20,
	"batch_size": 32
  }
}
```

### Example 3: RNN with Time Series

```json
{
  "type": "rnn",
  "input_shape": [128, 9],
  "num_classes": 6,
  "dataset": {
	"source": "file",
	"type": "sequence",
	"path": "data/time_series.npy",
	"normalize": true
  },
  "params": {
	"use_lstm": true,
	"units": 128
  },
  "training": {
	"epochs": 30,
	"batch_size": 64
  }
}
```

### Example 4: GAN with CIFAR-10 (Single Class)

```json
{
  "type": "gan",
  "input_shape": [32, 32, 3],
  "num_classes": 1,
  "dataset": {
	"source": "cifar10",
	"type": "image",
	"class_selection": 0,
	"normalize": true
  },
  "training": {
	"epochs": 100,
	"batch_size": 128
  }
}
```

---

## Dataset Loader Implementation Status

The `DatasetLoader` class (in `dataset_loader.py`) currently supports:

| Dataset Type | Status | Notes |
|--------------|--------|-------|
| **CIFAR-10** | ✅ Implemented | Built-in TensorFlow/Keras dataset |
| **Image Folder** | ✅ Implemented | Custom image directory loading with class folders |
| **Tabular (CSV/Excel/NumPy)** | ✅ Implemented | CSV, Excel, NumPy array loading with normalization |
| **Sequence** | ⚠️ Placeholder | JSON/NumPy sequence loading (NotImplementedError) |
| **Text** | ⚠️ Placeholder | Text corpus loading (NotImplementedError) |

---

## Using Image Folder Datasets

The image folder loader supports loading images from a directory structure where each subdirectory represents a class.

### Directory Structure

```
dataset/
    class_0/
        image1.jpg
        image2.png
        ...
    class_1/
        image1.jpg
        image2.png
        ...
    class_2/
        ...
```

### Configuration Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `path` | string | Required | Root directory containing class folders |
| `target_size` | [int, int] | [128, 128] | Image dimensions (height, width) |
| `color_mode` | string | "rgb" | Color mode: "rgb" or "grayscale" |
| `normalize` | boolean | true | Normalize pixel values to [0, 1] |
| `batch_size` | int | 32 | Batch size for loading |

### Example Configuration

```json
{
  "type": "cnn",
  "input_shape": [128, 128, 3],
  "num_classes": 5,
  "dataset": {
    "type": "image",
    "path": "data/my_images",
    "target_size": [128, 128],
    "color_mode": "rgb",
    "normalize": true,
    "batch_size": 32
  },
  "training": {
    "epochs": 50,
    "batch_size": 64
  }
}
```

### Example Usage

```python
# Prepare your dataset
dataset_cfg = {
    "type": "image",
    "path": "/path/to/image/dataset",
    "target_size": [224, 224],
    "color_mode": "rgb",
    "normalize": True
}

from Factory.dataset_loader import DatasetLoader
x_data, y_data = DatasetLoader.load(dataset_cfg)

print(f"Images shape: {x_data.shape}")  # (num_samples, 224, 224, 3)
print(f"Labels shape: {y_data.shape}")  # (num_samples,)
```

---

## Using Tabular Datasets (CSV/Excel/NumPy)

The tabular loader supports multiple file formats and provides flexible column selection.

### Supported File Formats

- **CSV** (`.csv`): Comma-separated values
- **TSV/TXT** (`.tsv`, `.txt`): Tab-separated values
- **Excel** (`.xlsx`, `.xls`): Excel spreadsheets
- **NumPy** (`.npy`): NumPy arrays
- **NumPy Archive** (`.npz`): Multiple NumPy arrays

### Configuration Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `path` | string | Required | File path to dataset |
| `target_column` | string/int | null | Name or index of target/label column |
| `features` | list | null | List of feature column names/indices (optional) |
| `normalize` | boolean | false | Z-score normalization (mean=0, std=1) |
| `separator` | string | "," | Column separator for CSV/TXT files |
| `x_key` | string | "x" | Key for features in .npz files |
| `y_key` | string | "y" | Key for labels in .npz files |

### Example Configurations

#### CSV with Column Names

```json
{
  "type": "mlp",
  "input_shape": [4],
  "num_classes": 3,
  "dataset": {
    "type": "tabular",
    "path": "data/iris.csv",
    "target_column": "species",
    "normalize": true
  },
  "training": {
    "epochs": 100,
    "batch_size": 16
  }
}
```

#### CSV with Column Indices

```json
{
  "type": "mlp",
  "input_shape": [11],
  "num_classes": 7,
  "dataset": {
    "type": "tabular",
    "path": "data/wine_quality.csv",
    "target_column": 11,
    "normalize": true
  },
  "training": {
    "epochs": 50,
    "batch_size": 64
  }
}
```

#### Excel File

```json
{
  "type": "mlp",
  "input_shape": [20],
  "num_classes": 2,
  "dataset": {
    "type": "tabular",
    "path": "data/features.xlsx",
    "target_column": "label",
    "features": ["feature1", "feature2", "feature3"],
    "normalize": true
  },
  "training": {
    "epochs": 30,
    "batch_size": 32
  }
}
```

#### NumPy Array (.npy)

```json
{
  "type": "mlp",
  "input_shape": [784],
  "num_classes": 10,
  "dataset": {
    "type": "tabular",
    "path": "data/mnist_flattened.npy",
    "target_column": 784,
    "normalize": false
  },
  "training": {
    "epochs": 20,
    "batch_size": 128
  }
}
```

#### NumPy Archive (.npz)

```json
{
  "type": "mlp",
  "input_shape": [100],
  "num_classes": 5,
  "dataset": {
    "type": "tabular",
    "path": "data/dataset.npz",
    "x_key": "features",
    "y_key": "labels",
    "normalize": true
  },
  "training": {
    "epochs": 40,
    "batch_size": 64
  }
}
```

### Example Usage

```python
# Load CSV with column names
dataset_cfg = {
    "type": "tabular",
    "path": "iris.csv",
    "target_column": "species",
    "normalize": True
}

from Factory.dataset_loader import DatasetLoader
x_data, y_data = DatasetLoader.load(dataset_cfg)

print(f"Features shape: {x_data.shape}")  # (150, 4)
print(f"Labels shape: {y_data.shape}")    # (150,)
```

### Label Encoding

The tabular loader automatically handles label encoding for categorical target columns:

- **Numeric labels**: Kept as-is
- **String labels**: Automatically encoded using `LabelEncoder` from scikit-learn
  - Example: `["cat", "dog", "cat"]` → `[0, 1, 0]`

---

## Adding Custom Datasets

The DatasetLoader now supports most common dataset formats out of the box. You have several options:

### Option 1: Use Image Folders

Organize your images in class folders and use the built-in image loader:

```
my_dataset/
	class_a/
		img1.jpg
		img2.jpg
	class_b/
		img1.jpg
		img2.jpg
```

Then configure it:

```json
{
  "dataset": {
	"type": "image",
	"path": "my_dataset",
	"target_size": [128, 128],
	"normalize": true
  }
}
```

### Option 2: Use CSV/Excel Files

Simply save your data as CSV or Excel and use the tabular loader:

```json
{
  "dataset": {
	"type": "tabular",
	"path": "my_data.csv",
	"target_column": "label",
	"normalize": true
  }
}
```

### Option 3: Use NumPy Arrays

Save your preprocessed data as NumPy arrays:

```python
import numpy as np
import pandas as pd

# From CSV
df = pd.read_csv("my_data.csv")
X = df.drop("target", axis=1).values
y = df["target"].values

# Save as .npz
np.savez("dataset.npz", x=X, y=y)

# Or as separate .npy files
np.save("x_train.npy", X)
np.save("y_train.npy", y)
```

Then configure:

```json
{
  "dataset": {
	"type": "tabular",
	"path": "dataset.npz",
	"x_key": "x",
	"y_key": "y"
  }
}
```

### Option 4: Extend DatasetLoader

For completely custom formats, modify `dataset_loader.py`:

```python
# In dataset_loader.py
@staticmethod
def load(dataset_cfg: dict):
	source = dataset_cfg.get("source")
	dtype = dataset_cfg.get("type")

	if source == "my_custom_dataset":
		# Your custom loading logic
		import numpy as np
		data = np.load(dataset_cfg.get("path"))
		return data, None

	# ... existing code ...
```

---

## Quick Reference: Download Links

| Dataset | Direct Download Link |
|---------|---------------------|
| Iris | https://archive.ics.uci.edu/ml/machine-learning-databases/iris/iris.data |
| Wine Quality (Red) | https://archive.ics.uci.edu/ml/machine-learning-databases/wine-quality/winequality-red.csv |
| Wine Quality (White) | https://archive.ics.uci.edu/ml/machine-learning-databases/wine-quality/winequality-white.csv |
| Tiny ImageNet | http://cs231n.stanford.edu/tiny-imagenet-200.zip |
| UCI HAR | https://archive.ics.uci.edu/ml/machine-learning-databases/00240/UCI%20HAR%20Dataset.zip |
| Air Passengers | https://raw.githubusercontent.com/jbrownlee/Datasets/master/airline-passengers.csv |
| CelebA | http://mmlab.ie.cuhk.edu.hk/projects/CelebA.html |
| AG News | https://www.kaggle.com/datasets/amananandrai/ag-news-classification-dataset |

---

## Additional Resources

- **TensorFlow Datasets Catalog:** https://www.tensorflow.org/datasets/catalog/overview
- **Keras Datasets API:** https://keras.io/api/datasets/
- **UCI Machine Learning Repository:** https://archive.ics.uci.edu/ml/index.php
- **Kaggle Datasets:** https://www.kaggle.com/datasets
- **Papers With Code Datasets:** https://paperswithcode.com/datasets

---

## License

MIT License - See the main README for full license text.

## Author

Angel Hernandez (me@angelhernandezm.com)

---

**Last Updated:** July 2026
