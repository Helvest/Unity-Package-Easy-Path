using UnityEngine;

namespace EasyPath
{
public abstract class ScriptableFilePathAbstract : ScriptableObject, IFilePath
{
	public abstract FilePathData GetPathData();

	public virtual PathSystem PathSystem => GetPathData().PathSystem;
	public virtual string CustomPathSystem => GetPathData().CustomPathSystem;
	public virtual string SubPath => GetPathData().SubPath;
	public virtual string FileName => GetPathData().FileName;
	public virtual string FileNameWithExtension => GetPathData().FileNameWithExtension;
	public virtual string Extension => GetPathData().Extension;

	public virtual bool DirectoryExist() => GetPathData().DirectoryExist();
	public virtual bool FileExist() => GetPathData().FileExist();
	public virtual string GetDirectoryPath() => GetPathData().GetDirectoryPath();
	public virtual string GetFullPath() => GetPathData().GetFullPath();
	public virtual string GetPartialPath() => GetPathData().GetPartialPath();
	public virtual string GetSystemPath() => GetPathData().GetSystemPath();
	public virtual string GetSystemPath(PathSystem system) => GetPathData().GetSystemPath(system);
}
}
