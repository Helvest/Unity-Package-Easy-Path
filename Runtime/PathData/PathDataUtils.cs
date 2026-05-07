using System.Collections.Generic;
using UnityEngine;

namespace EasyPath
{
public static class PathDataUtils
{
	private static readonly Dictionary<PathSystem, string> SystemPathCache = new();

	public static string GetSystemPath(PathSystem system, string customPath = "")
	{
		switch (system)
		{
			case PathSystem.CustomPathSystem:
				return customPath;
			case PathSystem.None or PathSystem.Resources:
				return string.Empty;
		}

		if (SystemPathCache.TryGetValue(system, out string path))
		{
			return path;
		}

		path = system switch
		{
			PathSystem.GameData => Application.dataPath,
			PathSystem.StreamingAssets => Application.streamingAssetsPath,
			PathSystem.PersistentData => Application.persistentDataPath,
			PathSystem.TemporaryCache => Application.temporaryCachePath,
			PathSystem.ConsoleLog => Application.consoleLogPath,
			PathSystem.AbsoluteURL => Application.absoluteURL,
			_ => string.Empty
		};
		SystemPathCache[system] = path;
		return path;
	}

	public static PathSystem PathToPathSystem(string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return PathSystem.None;
		}

		/*
		PathSystem.StreamingAssets,
		PathSystem.PersistentData,
		PathSystem.TemporaryCache,
		PathSystem.GameData,
		PathSystem.ConsoleLog,
		PathSystem.AbsoluteURL,
		*/
		for (int i = 3; i <= 8; i++)
		{
			var system = (PathSystem)i;
			string sysPath = GetSystemPath(system);
			if (!string.IsNullOrEmpty(sysPath) && path.StartsWith(sysPath))
			{
				return system;
			}
		}

		return PathSystem.CustomPathSystem;
	}
}
}
