using System;
using Localization.Editor.EditorSystems;
using UnityEditor;
using UnityEngine;
#if LOCALIZE_EXPERIMENTAL
using UnityEditor.Experimental.GraphView;
#endif

namespace Localization.Editor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(LocalizeStringAttribute), false)]
	public class LocalizeStringDrawer : PropertyDrawer
	{
		private const string NoValue = "No value...";
		
		private static SystemLanguage _language = SystemLanguage.Unknown;

		private GUIStyle _btnKeyStyle;
		private Texture _btnKeyIcon;
		private GUIStyle _localizeBoxStyle;
		private float _localizeBoxSize = 24f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (_language == SystemLanguage.Unknown)
			{
				_language = LocalizeSettings.MakeInstance().DefaultLanguage;
			}
			
			if (_btnKeyStyle == null)
			{
				_btnKeyStyle = new GUIStyle(GUI.skin.button);
				_btnKeyStyle.padding = new RectOffset(1, 1, 1, 1);
			}

			if (_btnKeyIcon == null)
				_btnKeyIcon = EditorGUIUtility.IconContent("Icon Dropdown").image;
			
			var propertyHeight = EditorGUI.GetPropertyHeight(property, label, false);
#if LOCALIZE_EXPERIMENTAL
			var propertyWidth = position.width - propertyHeight;
#else
			var propertyWidth = position.width;
#endif
			var propertyRect = new Rect(position.x, position.y, propertyWidth, propertyHeight);
			EditorGUI.PropertyField(propertyRect, property, label, false);

#if LOCALIZE_EXPERIMENTAL
			var findRect = new Rect(position.x + position.width - propertyHeight, position.y, propertyHeight,
				propertyHeight);
			if (GUI.Button(findRect, _btnKeyIcon, _btnKeyStyle))
			{
				var searchProvider = LocaleKeyProvider.Make(_language, property);
				SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
					searchProvider);
				Event.current.Use();
			}
#endif

			var languageRect = new Rect(position.x, position.y + propertyRect.height, 70f, _localizeBoxSize);
			_language = (SystemLanguage)EditorGUI.EnumPopup(languageRect, new GUIContent(""), _language, HasLanguage);
			
			if (EditorLocalizeSystem.TryGetValue(_language, property.stringValue, out var value))
			{
				var valuePosition = new Rect(position.x + 70f, position.y + propertyRect.height, position.width - 70f, _localizeBoxSize);
				EditorGUI.HelpBox(valuePosition, value, MessageType.Info);
			}
			else
			{
				var valuePosition = new Rect(position.x + 70f, position.y + propertyRect.height, position.width - 70f, _localizeBoxSize);
				EditorGUI.HelpBox(valuePosition, NoValue, MessageType.Info);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (_localizeBoxStyle == null)
				_localizeBoxStyle = GUI.skin.GetStyle("helpbox");
			
			if (!EditorLocalizeSystem.TryGetValue(_language, property.stringValue, out var value))
			{
				value = NoValue;
			}

			if (_localizeBoxStyle != null)
			{
				var size = _localizeBoxStyle.CalcSize(new GUIContent(value));
				_localizeBoxSize = size.y + 4f;
			}
			else
			{
				_localizeBoxSize = 24f;
			}

			return EditorGUI.GetPropertyHeight(property, label, false) + _localizeBoxSize;
		}

		private bool HasLanguage(Enum language)
		{
			return Array.Exists(LocalizeSettings.MakeInstance().Locales, p => p.Language == (SystemLanguage)language);
		}
	}
}