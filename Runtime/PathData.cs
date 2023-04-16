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

		protected virtual string GetDirectPath()
		{
			return path;
		}

		public string GetFullPath()
		{
			return pathSystem switch
			{
				PathSystem.GameData => Path.Combine(Application.dataPath, path, fileName + extension),
				PathSystem.StreamingAssets => Path.Combine(Application.streamingAssetsPath, path, fileName + extension),
				PathSystem.PersistentData => Path.Combine(Application.persistentDataPath, path, fileName + extension),
				PathSystem.TemporaryCache => Path.Combine(Application.temporaryCachePath, path, fileName + extension),
				PathSystem.Resources => Path.Combine(path, fileName),
				PathSystem.ConsoleLog => Path.Combine(Application.consoleLogPath, path, fileName + extension),
				PathSystem.AbsoluteURL => Path.Combine(Application.absoluteURL, path, fileName + extension),
				_ => Path.Combine(GetDirectPath(), fileName + extension)
			};
		}

		public string GetDirectoryPath()
		{
			return pathSystem switch
			{
				PathSystem.GameData => Path.Combine(Application.dataPath, path),
				PathSystem.StreamingAssets => Path.Combine(Application.streamingAssetsPath, path),
				PathSystem.PersistentData => Path.Combine(Application.persistentDataPath, path),
				PathSystem.TemporaryCache => Path.Combine(Application.temporaryCachePath, path),
				PathSystem.Resources => path,
				PathSystem.ConsoleLog => Path.Combine(Application.consoleLogPath, path),
				PathSystem.AbsoluteURL => Path.Combine(Application.absoluteURL, path),
				_ => GetDirectPath()
			};
		}

		public string GetPartialPath()
		{
			return pathSystem switch
			{
				PathSystem.GameData => Path.Combine(path, fileName + extension),
				PathSystem.StreamingAssets => Path.Combine(path, fileName + extension),
				PathSystem.PersistentData => Path.Combine(path, fileName + extension),
				PathSystem.TemporaryCache => Path.Combine(path, fileName + extension),
				PathSystem.Resources => Path.Combine(path, fileName),
				PathSystem.ConsoleLog => Path.Combine(path, fileName + extension),
				PathSystem.AbsoluteURL => Path.Combine(path, fileName + extension),
				_ => Path.Combine(GetDirectPath(), fileName + extension)
			};
		}

		public string GetSytemPath()
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

		public string GetFileName()
		{
			return fileName + extension;
		}

		public string GetFileNameWithoutExtension()
		{
			return fileName;
		}

		public string GetExtension()
		{
			return extension;
		}

		#endregion

		#region Set

		public void SetFromFullPath(params string[] fullPath)
		{
			SetFromFullPath(Path.Combine(fullPath));
		}

		public void SetFromFullPath(string fullPath)
		{
			pathSystem = PathToPathSystem(fullPath);

			Debug.LogError("pathSystem: " + pathSystem);

			if (pathSystem != PathSystem.DirectPath)
			{
				var systemPath = GetSytemPath(pathSystem).Length;
				fullPath = fullPath.Remove(0, systemPath + 1);
			}

			SetFromPartialPath(fullPath);
		}

		public void SetFromPartialPath(string partialPath)
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

		protected override string GetDirectPath()
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
