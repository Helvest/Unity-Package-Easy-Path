namespace EasyPath
{
public interface IDirectoryPath
{
	PathSystem PathSystem { get; }
	string CustomPathSystem { get; }
	string SubPath { get; }
	string GetDirectoryPath(); //PathSystemBlock + SubPath
	string GetSystemPath();//PathSystemBlock
	//string GetSystemPath(PathSystem system);
	bool DirectoryExist();
	string GetFullPath();//GetDirectoryPath
	string GetPartialPath();//SubPath
}

public interface IFilePath : IDirectoryPath
{
	string FileName { get; }
	string FileNameWithExtension { get; }//FileName + Extension
	string Extension { get; }
	bool FileExist();
}
}
