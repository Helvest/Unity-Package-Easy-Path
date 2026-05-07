using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyPath
{
[CreateAssetMenu(menuName = "EasyPath/Scriptable Path Build Target")]
public class ScriptableFilePathBuildTarget : ScriptableFilePathAbstract
{
	[Serializable]
	public class PathDataWithBuildTarget : PathDataWithParent
	{
		[Tooltip("The platforms this specific path configuration applies to.")]
		public List<RuntimePlatform> buildTargets = new() { RuntimePlatform.WindowsPlayer };
	}

	[Tooltip("List of path configurations per platform. The first match will be used.")]
	public List<PathDataWithBuildTarget> pathDataBuilds = new()
	{
		new()
	};

	private static readonly RuntimePlatform Platform = Application.platform;

	public override FilePathData GetPathData()
	{
		foreach (var item in pathDataBuilds)
		{
			if (item.buildTargets.Contains(Platform))
			{
				return item;
			}
		}

		return null;
	}
}
}