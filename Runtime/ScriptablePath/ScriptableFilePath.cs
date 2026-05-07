using UnityEngine;

namespace EasyPath
{
[CreateAssetMenu(menuName = "EasyPath/Scriptable Path")]
public class ScriptableFilePath : ScriptableFilePathAbstract
{
	public FilePathData pathPathData = new();

	public override FilePathData GetPathData() => pathPathData;
}
}
