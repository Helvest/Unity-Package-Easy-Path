using UnityEngine;

namespace EasyPath
{
	[CreateAssetMenu(menuName = "EasyPath/Scriptable Path With Parent")]
	public class ScriptablePathWithParent : ScriptablePathAbstract
	{
		public PathDataWithParent pathData = new PathDataWithParent();

		public override PathData GetPathData() => pathData;

	}

}
