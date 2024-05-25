using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
	[CustomEditor(typeof(LocalizeSettings))]
	public class LocalizeSettingsEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
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
		}
	}
}