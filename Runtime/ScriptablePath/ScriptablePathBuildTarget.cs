using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyPath
{
	[CreateAssetMenu(menuName = "EasyPath/Scriptable Path Build Target")]
	public class ScriptablePathBuildTarget : ScriptablePathAbstract
	{
		[Serializable]
		public class PathDataWithBuildTarget : PathDataWithParent
		{
			public List<RuntimePlatform> buildTargets = new List<RuntimePlatform>() { RuntimePlatform.WindowsPlayer };
		}

		public List<PathDataWithBuildTarget> pathDataBuilds = new List<PathDataWithBuildTarget>()
		{
			new PathDataWithBuildTarget()
		};

		public override PathData GetPathData()
		{
			foreach (var item in pathDataBuilds)
			{
				if (item.buildTargets.Contains(Application.platform))
				{
					return item;
				}
			}

			return null;
		}
	}

	
}
