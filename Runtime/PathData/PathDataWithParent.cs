using System;
using UnityEngine;

namespace EasyPath
{
[Serializable]
public class PathDataWithParent : FilePathData
{
	[Space] [Tooltip("Optional parent ScriptableDirectoryPath to inherit values from if they are not defined here.")]
	public ScriptableFilePathAbstract parent;

	public FilePathData ParentPath => parent?.GetPathData();

	public bool HasParent => parent != null;

	public override PathSystem PathSystem
	{
		get
		{
			if (HasParent && base.PathSystem == PathSystem.None)
			{
				return ParentPath.PathSystem;
			}

			return base.PathSystem;
		}
	}

	public override string CustomPathSystem
	{
		get
		{
			if (HasParent && string.IsNullOrWhiteSpace(base.CustomPathSystem))
			{
				return ParentPath.CustomPathSystem;
			}

			return base.CustomPathSystem;
		}
	}

	public override string SubPath
	{
		get
		{
			if (HasParent && string.IsNullOrWhiteSpace(base.SubPath))
			{
				return ParentPath.SubPath;
			}

			return base.SubPath;
		}
	}

	public override string FileName
	{
		get
		{
			if (HasParent && string.IsNullOrWhiteSpace(base.FileName))
			{
				return ParentPath.FileName;
			}

			return base.FileName;
		}
	}

	public override string Extension
	{
		get
		{
			if (HasParent && string.IsNullOrWhiteSpace(base.Extension))
			{
				return ParentPath.Extension;
			}

			return base.Extension;
		}
	}
}
}
