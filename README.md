# Unity Package: Easy Path

**Easy Path** is a simple and efficient Unity package that helps you manage and work with file and directory paths in your Unity projects. This package provides an easy-to-use API to handle various path scenarios and supports different platforms seamlessly.

## Features

- Easy to use API for handling file and directory paths
- Supports multiple path systems (GameData, StreamingAssets, PersistentData, TemporaryCache, Resources, ConsoleLog, AbsoluteURL)
- Ability to use a custom file system
- Platform-specific path overrides for Linux and macOS
- Serializable PathData class for storing and manipulating path information
- Convenience methods for getting full, partial, and system paths
- Automatically detects path systems based on given paths

## Installation

To install the Easy Path package, simply add it to your Unity project using the Unity Package Manager and the GitHub repository URL:

1. Open your Unity project and navigate to `Window` > `Package Manager`.
2. Click the `+` button in the upper left corner and select `Add package from git URL`.
3. Enter the following URL: `https://github.com/Helvest/Unity-Package-Easy-Path.git` and click `Add`.

## Usage

After importing the package, you can start using the `EasyPath` namespace in your scripts:

```csharp
using EasyPath;
```
## Creating PathData Instances

You can create a new PathData instance in various ways:

```csharp

// Create an empty PathData instance
PathData pathData = new PathData();

// Create a PathData instance from a full path
PathData pathDataFromFullPath = new PathData("your/full/path/here.txt");

// Create a PathData instance by copying another instance
PathData copiedPathData = new PathData(existingPathData);
```
## Working with PathData

You can easily manipulate and retrieve information about paths using the provided methods:

```csharp

// Set what you need
pathData.PathSystem = PathSystem.StreamingAssets;
pathData.SubPath = "ExampleFolder";
pathData.FileName = "exampleFile";
pathData.Extension = ".txt";

// Set a path from a full path
pathData.SetFromFullPath("C:/Users/user/Documents/myfile.txt");

// Set a path from a partial path
pathData.SetFromPartialPath("Documents/myfile.txt");

// Get the full path
string fullPath = pathData.GetFullPath();

// Get the directory path
string directoryPath = pathData.GetDirectoryPath();

// Get the partial path
string partialPath = pathData.GetPartialPath();

// Get the system path
string systemPath = pathData.GetSytemPath();

// Get the file name without extension
string fileName = pathData.FileName;

// Get the file name with extension
string fileNameWithoutExtension = pathData.FileNameWithExtension;

// Get the file extension
string fileExtension = pathData.Extension;
```

## PathDataSystemOverride

The PathDataSystemOverride class extends PathData and provides platform-specific path overrides for Linux and macOS:

```csharp

PathDataSystemOverride pathDataSystemOverride = new PathDataSystemOverride();

// Set the Linux override direct path
pathDataSystemOverride.linuxOverrideDirectPath = "/home/user/Documents/myfile.txt";

// Set the macOS override direct path
pathDataSystemOverride.OSXOverrideDirectPath = "/Users/user/Documents/myfile.txt";

// Get the direct path considering platform-specific overrides
string directPath = pathDataSystemOverride.GetDirectPath();
```