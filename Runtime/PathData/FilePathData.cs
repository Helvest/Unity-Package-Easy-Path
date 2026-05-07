using System;
using System.IO;
using UnityEngine;

namespace EasyPath
{
[Serializable]
public class FilePathData : DirectoryPathData, IFilePath
{
	public FilePathData()
	{
		_fileNameWithExtension.Set(separator, fileName, extension);
		_fileNameWithExtension.useExtension = true;
	}

#region Fields

	[SerializeField, Tooltip("The name of the file without its extension.")]
	private StringBlock fileName = new();

	public virtual string FileName
	{
		get => fileName;
		set => fileName.Content = separator.Clean(value);
	}

	[SerializeField, Tooltip("The file extension (e.g., 'json').")]
	private StringBlock extension = new();

	public virtual string Extension
	{
		get => extension;
		set => extension.Content = separator.Clean(value);
	}

	private PathBuilder _fileNameWithExtension = new();

	[Tooltip("Returns the cached FileName combined with the Extension.")]
	public virtual string FileNameWithExtension
	{
		get => _fileNameWithExtension;
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				fileName.Content = string.Empty;
				extension.Content = string.Empty;
			}
			else
			{
				ParseFilePart(value.AsSpan());
			}
		}
	}

#endregion

#region Getters & Setters

	[Tooltip("Parses a full absolute path to automatically fill fields.")]
	public override void SetFromFullPath(string fullPath)
	{
		if (string.IsNullOrEmpty(fullPath))
		{
			pathSystemBlock.System = PathSystem.None;
			SetFromPartialPath(string.Empty);
			return;
		}

		string path = fullPath.Replace('\\', '/');
		pathSystemBlock.System = PathDataUtils.PathToPathSystem(path);
		if (pathSystemBlock.System != PathSystem.None)
		{
			string sys = GetSystemPath(pathSystemBlock.System);
			if (path.StartsWith(sys))
			{
				path = path[sys.Length..].TrimStart('/');
			}
		}

		SetFromPartialPath(path);
	}


	[Tooltip("Parses a relative path to fill SubPath, FileName and Extension.")]
	public override void SetFromPartialPath(string partialPath)
	{
		if (string.IsNullOrEmpty(partialPath))
		{
			subPath.Content = fileName.Content = extension.Content = string.Empty;
			return;
		}

		var span = partialPath.AsSpan();
		int lastSlash = span.LastIndexOfAny('/', '\\');

		if (lastSlash >= 0)
		{
			SubPath = span[..lastSlash].ToString();
			ParseFilePart(span[(lastSlash + 1)..]);
		}
		else
		{
			SubPath = string.Empty;
			ParseFilePart(span);
		}
	}

	public bool FileExist() => File.Exists(GetFullPath());

	private void ParseFilePart(ReadOnlySpan<char> fileSpan)
	{
		int lastDot = fileSpan.LastIndexOf('.');
		if (lastDot >= 0)
		{
			fileName.Content = fileSpan[..lastDot].ToString();
			extension.Content = fileSpan[(lastDot + 1)..].ToString();
		}
		else
		{
			fileName.Content = fileSpan.ToString();
			extension.Content = string.Empty;
		}
	}

#endregion

#region Equality & Copy

	public override void Copy(DirectoryPathData other)
	{
		base.Copy(other);

		if (other is not FilePathData f) return;
		fileName = f.fileName;
		extension = f.extension;
	}

#endregion
}
}
