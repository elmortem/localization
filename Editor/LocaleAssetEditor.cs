using System.Collections.Generic;
using Core.LocalizeSystems.Utilities;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
	[CustomEditor(typeof(LocaleAsset))]
	public class LocaleAssetEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			var localeAsset = (LocaleAsset)target;
			
			if (GUILayout.Button("Load from Google Sheet"))
			{
				EditorLocalizeSystem.LoadFromGoogleSheet(new List<LocaleAsset>{localeAsset});
			}
			
			base.OnInspectorGUI();
		}
	}
}