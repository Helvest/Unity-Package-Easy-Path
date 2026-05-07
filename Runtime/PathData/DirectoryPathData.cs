using System;
using System.IO;
using UnityEngine;

namespace EasyPath
{
#region Enums

[Tooltip("Enum representing different root path systems.")]
public enum PathSystem
{
	None,
	Resources,
	CustomPathSystem,
	StreamingAssets,
	PersistentData,
	TemporaryCache,
	GameData,
	ConsoleLog,
	AbsoluteURL,
}

#endregion

[Serializable]
public class DirectoryPathData : IDirectoryPath
{
	public DirectoryPathData()
	{
		separator.Set(pathSystemBlock);
		directory.Set(separator, pathSystemBlock, subPath);
	}

#region Fields & Properties

	[SerializeField, Tooltip("The root path system to use.")]
	protected PathSystemBlock pathSystemBlock = new();

	[SerializeField] protected SeparatorSystemBlock separator = new();

	public virtual PathSystem PathSystem
	{
		get => pathSystemBlock.System;
		set => pathSystemBlock.System = value;
	}

	public virtual string CustomPathSystem
	{
		get => pathSystemBlock.CustomPathSystem;
		set => pathSystemBlock.CustomPathSystem = value;
	}

	[SerializeField, Tooltip("The folder structure after the root.")]
	protected StringBlock subPath = new();

	public virtual string SubPath
	{
		get => subPath;
		set => subPath.Content = separator.Clean(value);
	}

	public PathBuilder directory = new();

#endregion

#region Cache & Logic

	[Tooltip("Returns the appropriate separator. URLs use '/', local systems use OS default.")]
	protected char GetSeparator() =>
		pathSystemBlock.System == PathSystem.AbsoluteURL ? '/' : Path.DirectorySeparatorChar;

#endregion

#region Identification & Root Paths

	public virtual string GetFullPath()
	{
		return GetDirectoryPath();
	}

	[Tooltip("Parses a full absolute path to automatically fill fields.")]
	public virtual void SetFromFullPath(string fullPath)
	{
		if (string.IsNullOrEmpty(fullPath))
		{
			return;
		}

		string path = fullPath.Replace('\\', '/');

		pathSystemBlock.System = PathDataUtils.PathToPathSystem(path);

		if (PathSystem != PathSystem.None)
		{
			string sys = GetSystemPath(PathSystem);
			if (path.StartsWith(sys, StringComparison.Ordinal))
			{
				path = path[sys.Length..].TrimStart('/');
			}
		}

		SetFromPartialPath(path);
	}

	public virtual string GetPartialPath()
	{
		return SubPath;
	}

	[Tooltip("Parses a relative path to fill SubPath.")]
	public virtual void SetFromPartialPath(string partialPath)
	{
		SubPath = partialPath;
	}

	public virtual string GetSystemPath() => GetSystemPath(pathSystemBlock.System);

	public string GetSystemPath(PathSystem system) =>
		PathDataUtils.GetSystemPath(system, pathSystemBlock.CustomPathSystem);

	[Tooltip("Returns the absolute path to the directory.")]
	public string GetDirectoryPath()
	{
		return directory;
	}

	[Tooltip("Checks if the directory exists on disk.")]
	public bool DirectoryExist() => Directory.Exists(GetDirectoryPath());

#endregion

#region Equality & Copy

	public virtual void Copy(DirectoryPathData other)
	{
		pathSystemBlock.System = other.pathSystemBlock.System;
		pathSystemBlock.CustomPathSystem = other.pathSystemBlock.CustomPathSystem;
		subPath.Content = other.subPath.Content;
	}

	public static implicit operator string(DirectoryPathData pathData) =>
		pathData?.GetFullPath() ?? string.Empty;

#endregion
}
}
