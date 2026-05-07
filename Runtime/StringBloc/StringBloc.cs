using System;
using System.IO;
using UnityEngine;

namespace EasyPath
{
public abstract class AbstractStringBlock
{
	internal Action onContentChanged;

	public abstract string Content { get; set; }

	public static implicit operator string(AbstractStringBlock block) =>
		block.Content;

	public override string ToString()
	{
		return Content;
	}
}

public abstract class AbstractCharBlock
{
	internal Action onContentChanged;

	public abstract char Content { get; set; }

	public static implicit operator char(AbstractCharBlock block) =>
		block.Content;

	public override string ToString()
	{
		return Content.ToString();
	}

	public string Combine(bool useExtension, params string[] array)
	{
		int totalLength = array.Length - 1;
		for (int i = 0; i < array.Length; i++)
		{
			totalLength += array[i].Length;
		}

		Span<char> buffer = stackalloc char[totalLength];
		int currentIndex = 0;

		for (int i = 0; i < array.Length; i++)
		{
			string s = array[i];

			for (int j = 0; j < s.Length; j++)
			{
				buffer[currentIndex++] = s[j];
			}

			if (i < array.Length - 1)
			{
				if (useExtension && i == array.Length - 2)
				{
					buffer[currentIndex++] = '.';
				}
				else
				{
					buffer[currentIndex++] = Content;
				}
			}
		}

		return new(buffer);
	}

	public string Clean(string path)
	{
		if (string.IsNullOrWhiteSpace(path)) return string.Empty;

		ReadOnlySpan<char> span = path.AsSpan().Trim();

		while (span.Length > 0 && (span[0] == '/' || span[0] == '\\'))
		{
			span = span[1..];
		}

		while (span.Length > 0 && (span[^1] == '/' || span[^1] == '\\'))
		{
			span = span[..^1];
		}

		if (span.IsEmpty) return string.Empty;

		Span<char> buffer = stackalloc char[span.Length];

		for (int i = 0; i < span.Length; i++)
		{
			char c = span[i];
			buffer[i] = c is '/' or '\\' ? Content : c;
		}

		return new(buffer);
	}

	public void CopyTo(SeparatorBlock other)
	{
		if (other == null) return;
		other.Content = Content;
	}
}


[Serializable]
public class SeparatorBlock : AbstractCharBlock
{
	[SerializeField] private char content = '/';

	public override char Content
	{
		get => content;
		set
		{
			if (content == value) return;
			content = value;
			onContentChanged?.Invoke();
		}
	}
}

[Serializable]
public class SeparatorSystemBlock : SeparatorBlock
{
	private PathSystemBlock _system;
	[SerializeField] private bool useOverride = false;
	[SerializeField] private char overrideSeparator = '/';

	public void Set(PathSystemBlock system)
	{
		_system = system;
		system.onContentChanged += UpdateSeparator;
		UpdateSeparator();
	}

	public bool UseOverride
	{
		get => useOverride;
		set
		{
			if (useOverride == value) return;
			useOverride = value;
			UpdateSeparator();
		}
	}

	public char OverrideSeparator
	{
		get => overrideSeparator;
		set
		{
			if (overrideSeparator == value) return;
			overrideSeparator = value;
			UpdateSeparator();
		}
	}

	protected virtual void UpdateSeparator()
	{
		if (UseOverride)
		{
			Content = OverrideSeparator;
			return;
		}

		if (_system is { System: PathSystem.AbsoluteURL })
		{
			Content = '/';
			return;
		}

		Content = Path.DirectorySeparatorChar;
	}

	public void CopyTo(SeparatorSystemBlock other)
	{
		other.useOverride = useOverride;
		other.overrideSeparator = overrideSeparator;

		base.CopyTo(other);
		//other.UpdateSeparator();
	}
}

[Serializable]
public class PathSystemBlock : AbstractStringBlock
{
	[SerializeField] private PathSystem system = PathSystem.StreamingAssets;
	[SerializeField] private string customPathSystem;
	private string _content;

	public PathSystem System
	{
		get => system;
		set
		{
			if (system == value) return;
			system = value;
			onContentChanged?.Invoke();
		}
	}

	public string CustomPathSystem
	{
		get => customPathSystem;
		set
		{
			if (customPathSystem == value) return;
			customPathSystem = value;
			if (system != PathSystem.CustomPathSystem) return;
			onContentChanged?.Invoke();
		}
	}

	public override string Content
	{
		get => PathDataUtils.GetSystemPath(system, customPathSystem);
		set {}
	}

	public void CopyTo(PathSystemBlock other)
	{
		other.system = system;
		other.customPathSystem = customPathSystem;

		other.onContentChanged?.Invoke();
	}
}

[Serializable]
public class StringBlock : AbstractStringBlock, IEquatable<StringBlock>
{
	[SerializeField] private string content;

	public StringBlock()
	{
		content = string.Empty;
	}

	public StringBlock(string content)
	{
		this.content = content;
	}

	public override string Content
	{
		get => content;
		set
		{
			if (content == value) return;
			content = value;
			onContentChanged?.Invoke();
		}
	}

	public override string ToString()
	{
		return Content;
	}

	// 1. Implémentation de IEquatable<StringBlock> (évite le boxing)
	public bool Equals(StringBlock other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return content == other.content;
	}

	// 2. Override de Equals(object) obligatoire
	public override bool Equals(object obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != GetType()) return false;
		return Equals((StringBlock)obj);
	}

	// 3. Override obligatoire de GetHashCode
	// Important pour l'utilisation dans les Dictionary ou HashSet
	public override int GetHashCode()
	{
		return content != null ? content.GetHashCode() : 0;
	}

	// 4. Surcharge des opérateurs
	public static bool operator ==(StringBlock left, StringBlock right)
	{
		return Equals(left, right);
	}

	public static bool operator !=(StringBlock left, StringBlock right)
	{
		return !Equals(left, right);
	}
}


public class PathBuilder : AbstractStringBlock
{
	private AbstractCharBlock _separator;

	private AbstractStringBlock[] _stringBlocks;

	private string _content;
	private bool _needsUpdate = true;

	public bool useExtension = false;

	private void OnChange()
	{
		if (_needsUpdate) return;
		_needsUpdate = true;
		onContentChanged?.Invoke();
	}

	public void Set(AbstractCharBlock separator, params AbstractStringBlock[] stringBlocks)
	{
		_stringBlocks = stringBlocks;
		_separator = separator;

		separator.onContentChanged += OnChange;
		foreach (var stringBlock in stringBlocks)
		{
			stringBlock.onContentChanged += OnChange;
		}
	}

	public override string Content
	{
		get
		{
			if (_needsUpdate)
			{
				_needsUpdate = false;

				_content = BuildString();
			}

			return _content;
		}
		set {}
	}

	private string BuildString()
	{
		if (_stringBlocks == null || _stringBlocks.Length == 0) return string.Empty;

		// 1. Identifier les blocs valides (non vides / pas d'espaces)
		// On stocke les indices des blocs valides pour éviter de re-scanner
		Span<int> validIndices = stackalloc int[_stringBlocks.Length];
		int validCount = 0;
		int totalLength = 0;

		for (int i = 0; i < _stringBlocks.Length; i++)
		{
			string s = _stringBlocks[i].Content;
			// On vérifie si le bloc est utile (pas null, pas vide, pas que des espaces)
			if (!string.IsNullOrWhiteSpace(s))
			{
				validIndices[validCount++] = i;
				totalLength += s.Length;
			}
		}

		if (validCount == 0) return string.Empty;

		// 2. Calculer la longueur totale avec les séparateurs
		// Il y a (validCount - 1) séparateurs au total
		totalLength += validCount - 1;

		// 3. Allocation sur la pile
		Span<char> buffer = stackalloc char[totalLength];
		int currentIndex = 0;

		// 4. Remplissage
		for (int i = 0; i < validCount; i++)
		{
			int originalIndex = validIndices[i];
			string content = _stringBlocks[originalIndex].Content;

			// Copie du texte
			for (int j = 0; j < content.Length; j++)
			{
				buffer[currentIndex++] = content[j];
			}

			// Ajout du séparateur/point si ce n'est pas le dernier bloc valide
			if (i < validCount - 1)
			{
				if (useExtension && i == validCount - 2)
				{
					buffer[currentIndex++] = '.';
				}
				else
				{
					buffer[currentIndex++] = _separator.Content;
				}
			}
		}

		// 5. Création de la string finale
		return new(buffer);
	}
}
}
