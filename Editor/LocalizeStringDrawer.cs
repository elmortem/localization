using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Localization.Editor
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
			var propertyRect = new Rect(position.x, position.y, position.width - propertyHeight, propertyHeight);
			EditorGUI.PropertyField(propertyRect, property, label, false);

			var findRect = new Rect(position.x + position.width - propertyHeight, position.y, propertyHeight,
				propertyHeight);
			if (GUI.Button(findRect, _btnKeyIcon, _btnKeyStyle))
			{
				var searchProvider = LocaleKeyProvider.Make(_language, property);
				SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
					searchProvider);
				Event.current.Use();
			}

			var languageRect = new Rect(position.x, position.y + propertyRect.height, 70f, _localizeBoxSize);
			_language = (SystemLanguage)EditorGUI.EnumPopup(languageRect, _language);
			
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
	}
}