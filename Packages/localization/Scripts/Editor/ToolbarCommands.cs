#if !HIDE_TOOLBAR_MENU
using System.Collections.Generic;
using Localization.Editor.EditorSystems;
using UnityEditor;

namespace Localization.Editor
{
	public static class ToolbarCommands
	{
		[MenuItem("Tools/Localization/Load all from Google Sheet")]
		public static void LoadAllFromGoogleSheet()
		{
			var settings = LocalizeSettings.MakeInstance();
			var localeAssets = new List<LocaleAsset>();
			foreach (var locale in settings.Locales)
			{
				localeAssets.Add(locale.Load());
			}
			EditorLocalizeSystem.LoadFromGoogleSheet(localeAssets);
		}

		[MenuItem("Tools/Localization/Settings")]
		static void OpenProjectSettings()
		{
			SettingsService.OpenProjectSettings(EditorConstants.ProjectSettingsPath);
		}
	}
}
#endif