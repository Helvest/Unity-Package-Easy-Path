using UnityEngine;

namespace EasyPath
{
	[CreateAssetMenu(menuName = "EasyPath/Scriptable Path")]
	public class ScriptablePath : ScriptablePathAbstract
	{
		public PathData pathData = new PathData();

		public override PathData GetPathData() => pathData;

	}
}
