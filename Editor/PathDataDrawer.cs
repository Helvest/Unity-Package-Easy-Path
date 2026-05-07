using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace EasyPath.Editor
{
[CustomPropertyDrawer(typeof(DirectoryPathData), false)]
public class PathDataDrawer : PropertyDrawer
{
	private bool _expanded = false;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		// On récupère les propriétés
		var pathSystemProp = property.FindPropertyRelative("pathSystem");
		var customPathProp = property.FindPropertyRelative("customPathSystem");
		var subPathProp = property.FindPropertyRelative("subPath");
		var fileNameProp = property.FindPropertyRelative("fileName");
		var extensionProp = property.FindPropertyRelative("extension");
		var isDirOnlyProp = property.FindPropertyRelative("isDirectoryOnly");

		// Calcul du chemin pour l'aperçu
		// Note: On utilise Reflection ou on recrée temporairement l'objet pour avoir le preview
		var targetPath = GetTargetObjectOfProperty(property) as DirectoryPathData;
		string fullPathPreview = targetPath != null ? targetPath.GetFullPath() : "Invalid Path";

		// Header avec repliement
		var headerRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		_expanded = EditorGUI.Foldout(headerRect, _expanded, label, true);

		// Bouton rapide pour ouvrir le dossier à droite du header
		var buttonRect = new Rect(position.x + position.width - 60, position.y, 60, EditorGUIUtility.singleLineHeight);
		if (GUI.Button(buttonRect, "Open", EditorStyles.miniButton))
		{
			string dir = targetPath?.GetDirectoryPath();
			if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
			{
				EditorUtility.RevealInFinder(dir);
			}
			else
			{
				Debug.LogWarning($"Directory does not exist: {dir}");
			}
		}

		if (_expanded)
		{
			EditorGUI.indentLevel++;
			float lineH = EditorGUIUtility.singleLineHeight + 2;
			float currentY = position.y + lineH;

			// Affichage des champs
			DrawField(ref currentY, position.width, pathSystemProp);

			if (pathSystemProp.enumValueIndex == (int)PathSystem.CustomPathSystem)
			{
				DrawField(ref currentY, position.width, customPathProp);
			}

			DrawField(ref currentY, position.width, subPathProp);

			var toggleRect = new Rect(position.x + EditorGUIUtility.labelWidth, currentY,
				position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
			isDirOnlyProp.boolValue = EditorGUI.ToggleLeft(toggleRect, "Is Directory Only", isDirOnlyProp.boolValue);
			currentY += lineH;

			if (!isDirOnlyProp.boolValue)
			{
				DrawField(ref currentY, position.width, fileNameProp);
				DrawField(ref currentY, position.width, extensionProp);
			}

			// Aperçu du chemin (Lecture seule)
			GUI.enabled = false;
			var previewRect = new Rect(position.x, currentY, position.width, EditorGUIUtility.singleLineHeight * 1.5f);
			EditorGUI.TextArea(previewRect, fullPathPreview, EditorStyles.helpBox);
			GUI.enabled = true;

			EditorGUI.indentLevel--;
		}

		EditorGUI.EndProperty();
	}

	private void DrawField(ref float y, float width, SerializedProperty prop)
	{
		var rect = new Rect(EditorGUI.IndentedRect(new Rect(0, y, width, EditorGUIUtility.singleLineHeight)));
		EditorGUI.PropertyField(rect, prop);
		y += EditorGUIUtility.singleLineHeight + 2;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!_expanded)
		{
			return EditorGUIUtility.singleLineHeight;
		}

		int lineCount = 4; // Header + System + SubPath + Toggle
		var pathSystemProp = property.FindPropertyRelative("pathSystem");
		var isDirOnlyProp = property.FindPropertyRelative("isDirectoryOnly");

		if (pathSystemProp.enumValueIndex == (int)PathSystem.CustomPathSystem)
		{
			lineCount++;
		}

		if (!isDirOnlyProp.boolValue)
		{
			lineCount += 2;
		}

		return (lineCount * (EditorGUIUtility.singleLineHeight + 2)) + (EditorGUIUtility.singleLineHeight * 1.5f) + 5;
	}

	// Helper pour récupérer l'objet réel derrière la SerializedProperty
	private object GetTargetObjectOfProperty(SerializedProperty prop)
	{
		string path = prop.propertyPath.Replace(".Array.data[", "[");
		object obj = prop.serializedObject.targetObject;
		string[] elements = path.Split('.');
		foreach (string element in elements)
		{
			if (element.Contains("["))
			{
				string elementName = element[..element.IndexOf("[", StringComparison.Ordinal)];
				int index = System.Convert.ToInt32(element[element.IndexOf("[", StringComparison.Ordinal)..]
					.Replace("[", "").Replace("]", ""));
				obj = GetValue_Imp(obj, elementName, index);
			}
			else
			{
				obj = GetValue_Imp(obj, element);
			}
		}

		return obj;
	}

	private object GetValue_Imp(object source, string name)
	{
		if (source == null)
		{
			return null;
		}

		var type = source.GetType();
		var f = type.GetField(name,
			System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public |
			System.Reflection.BindingFlags.Instance);
		if (f == null)
		{
			return null;
		}

		return f.GetValue(source);
	}

	private object GetValue_Imp(object source, string name, int index)
	{
		if (GetValue_Imp(source, name) is not IEnumerable enumerable)
		{
			return null;
		}

		var enm = enumerable.GetEnumerator();
		for (int i = 0; i <= index; i++)
		{
			if (!enm.MoveNext())
			{
				return null;
			}
		}

		return enm.Current;
	}
}
}