namespace EasyPath
{
	public interface IPath
	{
		PathSystem PathSystem { get; }
		string CustomPathSystem { get; }
		string SubPath { get; }
		string FileName { get; }
		string FileNameWithExtension { get; }
		string Extension { get; }

		bool DirectoryExist();
		bool FileExist();
		string GetDirectoryPath();
		string GetFullPath();
		string GetPartialPath();
		string GetSytemPath();
		string GetSytemPath(PathSystem system);

	}
}
