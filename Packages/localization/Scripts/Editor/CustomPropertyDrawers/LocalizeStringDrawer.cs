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
		private const float LocalizeBoxMaxSize = 72f;
		
		private static SystemLanguage _language = SystemLanguage.Unknown;

		private GUIStyle _btnKeyStyle;
		private Texture _btnKeyIcon;
		private GUIStyle _localizeBoxStyle;
		private float _localizeBoxSize;
		private GUIContent _localizeBoxContent;

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

			var languageRect = new Rect(position.x, position.y + propertyRect.height, 70f, 20f);
			_language = (SystemLanguage)EditorGUI.EnumPopup(languageRect, new GUIContent(""), _language, HasLanguage);
			
			if (_localizeBoxStyle == null)
				_localizeBoxStyle = GUI.skin.GetStyle("helpbox");

			var availableWidth = position.width - 70f;

			if (_localizeBoxContent != null)
			{
				var valuePosition = new Rect(position.x + 70f, position.y + propertyRect.height, availableWidth,
					_localizeBoxSize);
				EditorGUI.HelpBox(valuePosition, _localizeBoxContent.text, MessageType.None);
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
			
			if(_localizeBoxContent == null)
				_localizeBoxContent = new GUIContent();
			_localizeBoxContent.text = value;

			if (_localizeBoxStyle != null)
			{
				var availableWidth = EditorGUIUtility.currentViewWidth - 70f;
				_localizeBoxSize = _localizeBoxStyle.CalcHeight(_localizeBoxContent, availableWidth) + 4f;
				_localizeBoxSize = Mathf.Min(_localizeBoxSize, LocalizeBoxMaxSize);
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