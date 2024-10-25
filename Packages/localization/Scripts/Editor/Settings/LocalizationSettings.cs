using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor.Settings
{
	[CreateAssetMenu(
		fileName = "LocalizationEditorSettings",
		menuName = "Localization/Settings",
		order = 0)]
	public class LocalizationSettings : ScriptableObject
	{
		private static LocalizationSettings _instance;

		[SerializeField]
		[UsedImplicitly]
		private bool _usedExperimentalApi;
		[SerializeField]
		[UsedImplicitly]
		private bool _hideToolbarMenu;
		public static LocalizationSettings GetOrCreateSettings()
		{
			if (_instance != null)
				return _instance;

			_instance = CreateInstance<LocalizationSettings>();
			if (File.Exists(EditorConstants.SettingsPath))
				JsonUtility.FromJsonOverwrite(File.ReadAllText(EditorConstants.SettingsPath), _instance);

			return _instance;
		}

		public static void SaveSettings()
			=> File.WriteAllText(EditorConstants.SettingsPath, JsonUtility.ToJson(_instance, true));

		[SettingsProvider]
		internal static SettingsProvider CreateSettingsProvider()
			=> new LocalizationSettingsProvider();
	}
}