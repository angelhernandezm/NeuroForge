# =============================================================================
# NeuroForge Dataset Loader Examples
# File: dataset_examples.py
# Author: Angel Hernandez (me@angelhernandezm.com)
# Description:
#     Example usage of the DatasetLoader for various data types
# =============================================================================

import sys
import os

# Add Factory to path
sys.path.insert(0, os.path.join(os.path.dirname(__file__), '..'))

from Factory.dataset_loader import DatasetLoader

def example_image_folder():
    """
    Example: Load images from a folder structure
    """
    print("\n=== Image Folder Example ===")

    # Configuration for image folder
    config = {
        "type": "image",
        "path": "path/to/image/dataset",  # Replace with actual path
        "target_size": [128, 128],
        "color_mode": "rgb",
        "normalize": True,
        "batch_size": 32
    }

    try:
        x_data, y_data = DatasetLoader.load(config)
        print(f"Images shape: {x_data.shape}")
        print(f"Labels shape: {y_data.shape}")
        print(f"Pixel value range: [{x_data.min():.2f}, {x_data.max():.2f}]")
    except Exception as e:
        print(f"Error: {e}")

def example_csv_with_column_names():
    """
    Example: Load CSV file with named columns
    """
    print("\n=== CSV with Column Names Example ===")

    # Configuration for CSV
    config = {
        "type": "tabular",
        "path": "iris.csv",  # Replace with actual path
        "target_column": "species",
        "normalize": True
    }

    try:
        x_data, y_data = DatasetLoader.load(config)
        print(f"Features shape: {x_data.shape}")
        print(f"Labels shape: {y_data.shape}")
        print(f"Feature range: [{x_data.min():.2f}, {x_data.max():.2f}]")
        print(f"Unique labels: {len(set(y_data))}")
    except Exception as e:
        print(f"Error: {e}")

def example_csv_with_column_indices():
    """
    Example: Load CSV file with column indices
    """
    print("\n=== CSV with Column Indices Example ===")

    # Configuration for CSV with indices
    config = {
        "type": "tabular",
        "path": "wine_quality.csv",  # Replace with actual path
        "target_column": 11,  # Last column
        "normalize": True
    }

    try:
        x_data, y_data = DatasetLoader.load(config)
        print(f"Features shape: {x_data.shape}")
        print(f"Labels shape: {y_data.shape}")
    except Exception as e:
        print(f"Error: {e}")

def example_numpy_array():
    """
    Example: Load NumPy array file
    """
    print("\n=== NumPy Array Example ===")

    # Configuration for .npy file
    config = {
        "type": "tabular",
        "path": "dataset.npy",  # Replace with actual path
        "target_column": -1,  # Last column
        "normalize": False
    }

    try:
        x_data, y_data = DatasetLoader.load(config)
        print(f"Features shape: {x_data.shape}")
        if y_data is not None:
            print(f"Labels shape: {y_data.shape}")
    except Exception as e:
        print(f"Error: {e}")

def example_numpy_archive():
    """
    Example: Load NumPy archive (.npz) file
    """
    print("\n=== NumPy Archive Example ===")

    # Configuration for .npz file
    config = {
        "type": "tabular",
        "path": "dataset.npz",  # Replace with actual path
        "x_key": "features",
        "y_key": "labels",
        "normalize": True
    }

    try:
        x_data, y_data = DatasetLoader.load(config)
        print(f"Features shape: {x_data.shape}")
        print(f"Labels shape: {y_data.shape}")
    except Exception as e:
        print(f"Error: {e}")

def example_excel_file():
    """
    Example: Load Excel file
    """
    print("\n=== Excel File Example ===")

    # Configuration for Excel
    config = {
        "type": "tabular",
        "path": "data.xlsx",  # Replace with actual path
        "target_column": "label",
        "features": ["feature1", "feature2", "feature3"],
        "normalize": True
    }

    try:
        x_data, y_data = DatasetLoader.load(config)
        print(f"Features shape: {x_data.shape}")
        print(f"Labels shape: {y_data.shape}")
    except Exception as e:
        print(f"Error: {e}")

def example_cifar10():
    """
    Example: Load built-in CIFAR-10 dataset
    """
    print("\n=== CIFAR-10 Example ===")

    # Configuration for CIFAR-10
    config = {
        "source": "cifar10",
        "type": "image",
        "normalize": True
    }

    try:
        x_data, y_data = DatasetLoader.load(config)
        print(f"Images shape: {x_data.shape}")
        print(f"Pixel value range: [{x_data.min():.2f}, {x_data.max():.2f}]")
    except Exception as e:
        print(f"Error: {e}")

def example_cifar10_single_class():
    """
    Example: Load single class from CIFAR-10 (for GAN training)
    """
    print("\n=== CIFAR-10 Single Class Example ===")

    # Configuration for CIFAR-10 with class selection
    config = {
        "source": "cifar10",
        "type": "image",
        "class_selection": 0,  # Class 0 = airplane
        "normalize": True
    }

    try:
        x_data, y_data = DatasetLoader.load(config)
        print(f"Images shape: {x_data.shape}")
        print(f"Only one class selected (airplanes)")
    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    print("NeuroForge DatasetLoader Examples")
    print("=" * 50)

    # Run built-in dataset example first (doesn't require external files)
    example_cifar10()
    example_cifar10_single_class()

    # The following examples require actual data files
    # Uncomment and provide actual paths to test

    # example_image_folder()
    # example_csv_with_column_names()
    # example_csv_with_column_indices()
    # example_numpy_array()
    # example_numpy_archive()
    # example_excel_file()

    print("\n" + "=" * 50)
    print("Examples complete!")
