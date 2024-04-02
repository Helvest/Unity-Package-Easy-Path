namespace EasyPath
{
	public interface IPath
	{
		string GetFullPath();

		PathSystem PathSystem { get; }

		string FileName { get; }
	}
}
