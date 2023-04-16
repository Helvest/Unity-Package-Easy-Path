using System;
using System.IO;
using UnityEngine;

namespace EasyPath
{

	#region Enum

	public enum PathSystem
	{
		DirectPath,
		StreamingAssets,
		GameData,
		PersistentData,
		TemporaryCache,
		Resources,
		ConsoleLog,
		AbsoluteURL
	}

	#endregion

	#region PathData

	[Serializable]
	public class PathData
	{

		#region Fields

		public PathSystem pathSystem = default;

		public string path = default;

		public string fileName = default;

		public string extension = default;

		#endregion

		#region Init

		public PathData() { }

		public PathData(string fullPath)
		{
			SetFromFullPath(fullPath);
		}

		public PathData(PathData pathData)
		{
			Copy(pathData);
		}

		#endregion

		#region PathToPathSystem

		private static readonly PathSystem[] _pathSystemCheckArray = new PathSystem[]
		{
			PathSystem.StreamingAssets,
			PathSystem.GameData,
			PathSystem.PersistentData,
			PathSystem.TemporaryCache,
			PathSystem.ConsoleLog,
			PathSystem.AbsoluteURL
		};

		public static PathSystem PathToPathSystem(string path)
		{
			foreach (var system in _pathSystemCheckArray)
			{
				var systemPath = GetSytemPath(system);

				if (systemPath.Length != 0 && path.Contains(systemPath))
				{
					return system;
				}
			}

			return PathSystem.DirectPath;
		}

		#endregion

		#region Get

		public virtual string GetSytemPath()
		{
			return GetSytemPath(pathSystem);
		}

		public static string GetSytemPath(PathSystem system)
		{
			return system switch
			{
				PathSystem.GameData => Application.dataPath,
				PathSystem.StreamingAssets => Application.streamingAssetsPath,
				PathSystem.PersistentData => Application.persistentDataPath,
				PathSystem.TemporaryCache => Application.temporaryCachePath,
				PathSystem.Resources => string.Empty,
				PathSystem.ConsoleLog => Application.consoleLogPath,
				PathSystem.AbsoluteURL => Application.absoluteURL,
				_ => string.Empty
			};
		}

		protected virtual string GetPath()
		{
			return path;
		}

		public virtual string GetFileName()
		{
			return GetFileNameWithoutExtension() + GetExtension();
		}

		public virtual string GetFileNameWithoutExtension()
		{
			return fileName;
		}

		public virtual string GetExtension()
		{
			return extension;
		}

		public virtual string GetFullPath()
		{
			return pathSystem switch
			{
				PathSystem.GameData => Path.Combine(Application.dataPath, GetPath(), GetFileName()),
				PathSystem.StreamingAssets => Path.Combine(Application.streamingAssetsPath, GetPath(), GetFileName()),
				PathSystem.PersistentData => Path.Combine(Application.persistentDataPath, GetPath(), GetFileName()),
				PathSystem.TemporaryCache => Path.Combine(Application.temporaryCachePath, GetPath(), GetFileName()),
				PathSystem.Resources => Path.Combine(GetPath(), GetFileNameWithoutExtension()),
				PathSystem.ConsoleLog => Path.Combine(Application.consoleLogPath, GetPath(), GetFileName()),
				PathSystem.AbsoluteURL => Path.Combine(Application.absoluteURL, GetPath(), GetFileName()),
				_ => Path.Combine(GetPath(), GetFileName())
			};
		}

		public virtual string GetDirectoryPath()
		{
			return pathSystem switch
			{
				PathSystem.GameData => Path.Combine(Application.dataPath, GetPath()),
				PathSystem.StreamingAssets => Path.Combine(Application.streamingAssetsPath, GetPath()),
				PathSystem.PersistentData => Path.Combine(Application.persistentDataPath, GetPath()),
				PathSystem.TemporaryCache => Path.Combine(Application.temporaryCachePath, GetPath()),
				PathSystem.Resources => GetPath(),
				PathSystem.ConsoleLog => Path.Combine(Application.consoleLogPath, GetPath()),
				PathSystem.AbsoluteURL => Path.Combine(Application.absoluteURL, GetPath()),
				_ => GetPath()
			};
		}

		public virtual string GetPartialPath()
		{
			return pathSystem switch
			{
				PathSystem.GameData => Path.Combine(GetPath(), GetFileName()),
				PathSystem.StreamingAssets => Path.Combine(GetPath(), GetFileName()),
				PathSystem.PersistentData => Path.Combine(GetPath(), GetFileName()),
				PathSystem.TemporaryCache => Path.Combine(GetPath(), GetFileName()),
				PathSystem.Resources => Path.Combine(GetPath(), GetFileNameWithoutExtension()),
				PathSystem.ConsoleLog => Path.Combine(GetPath(), GetFileName()),
				PathSystem.AbsoluteURL => Path.Combine(GetPath(), GetFileName()),
				_ => Path.Combine(GetPath(), GetFileName())
			};
		}

		#endregion

		#region Set

		public virtual void SetFromFullPath(params string[] fullPath)
		{
			SetFromFullPath(Path.Combine(fullPath));
		}

		public virtual void SetFromFullPath(string fullPath)
		{
			pathSystem = PathToPathSystem(fullPath);

			if (pathSystem != PathSystem.DirectPath)
			{
				var systemPath = GetSytemPath(pathSystem).Length;
				fullPath = fullPath.Remove(0, systemPath + 1);
			}

			SetFromPartialPath(fullPath);
		}

		public virtual void SetFromPartialPath(string partialPath)
		{
			path = Path.GetDirectoryName(partialPath);
			fileName = Path.GetFileNameWithoutExtension(partialPath);

			if (Path.HasExtension(partialPath))
			{
				extension = Path.GetExtension(partialPath);
			}
		}

		#endregion

		#region Copy

		public virtual void Copy(PathData pathData)
		{
			pathSystem = pathData.pathSystem;
			path = pathData.path;
			fileName = pathData.fileName;
			extension = pathData.extension;
		}

		#endregion

	}

	#endregion

	#region PathDataSystemOverride

	[Serializable]
	public class PathDataSystemOverride : PathData
	{

		public string linuxOverrideDirectPath = default;

		public string OSXOverrideDirectPath = default;

		protected override string GetPath()
		{
#if DEVELOPMENT_BUILD_LINUX || UNITY_STANDALONE_LINUX
			return string.IsNullOrEmpty(linuxOverrideDirectPath) ? path : linuxOverrideDirectPath;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
			return string.IsNullOrEmpty(OSXOverrideDirectPath) ? path : OSXOverrideDirectPath;
#else
			return path;
#endif
		}
	}

	#endregion

}
