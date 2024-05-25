using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
	[CustomEditor(typeof(LocalizeSettings))]
	public class LocalizeSettingsEditor : UnityEditor.Editor
	{
		private GUIStyle _headerStyle;

		public override void OnInspectorGUI()
		{
			if (_headerStyle == null)
			{
				_headerStyle = new GUIStyle(GUI.skin.label);
				_headerStyle.fontStyle = FontStyle.Bold;
			}
			
			var settings = (LocalizeSettings)target;
			
			if (GUILayout.Button("Load All from Google Sheet"))
			{
				var localeAssets = new List<LocaleAsset>();
				foreach (var locale in settings.Locales)
				{
					localeAssets.Add(locale.Load());
				}
				EditorLocalizeSystem.LoadFromGoogleSheet(localeAssets);
			}
			
			base.OnInspectorGUI();
			
			GUILayout.Space(10f);
			GUILayout.Label("Current Locales", _headerStyle);
			foreach (var locale in settings.Locales)
			{
				if (GUILayout.Button($"{locale.Language}"))
				{
					var localeAsset = locale.Load();
					EditorGUIUtility.PingObject(localeAsset);
				}
			}
		}
	}
}