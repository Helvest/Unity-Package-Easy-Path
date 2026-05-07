using UnityEngine;
using UnityEngine.Serialization;

namespace EasyPath
{
[CreateAssetMenu(menuName = "EasyPath/Scriptable Path With Parent")]
public class ScriptableFilePathWithParent : ScriptableFilePathAbstract
{
	[FormerlySerializedAs("pathPathData")] public PathDataWithParent pathData = new();

	public override FilePathData GetPathData() => pathData;
}
}
