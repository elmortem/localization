using System;
using System.Linq;
using UnityEngine;

namespace Localization
{
	[Serializable]
	public class LocalizeSheet
	{
		public string Group;
		public string SheetId;
	}
	
	[CreateAssetMenu(menuName = "Localization/Settings")]
	public class LocalizeSettings : ScriptableObject
	{
		[Serializable]
		public struct LocaleItem
		{
			public SystemLanguage Language;
			public string LocalePath;

			public LocaleAsset Load()
			{
				return Resources.Load<LocaleAsset>(LocalePath);
			}
		}
		
		[HideInInspector]
		public LocaleItem[] Locales;
		public SystemLanguage DefaultLanguage = SystemLanguage.English;

#if UNITY_EDITOR
		[Header("Google Sheets")]
		public string DocId;
		public LocalizeSheet[] Groups;
		public int SkipLine = 1;
		public int KeyColumn;
		public int GroupColumn = -1;
		public int CommentColumn;
#endif
		
		public bool HasLanguage(SystemLanguage language)
		{
			return Locales.Any(p => p.Language == language);
		}
		
		public static LocalizeSettings MakeInstance()
		{
			var settingsList = Resources.LoadAll<LocalizeSettings>("");
			if (settingsList == null || settingsList.Length != 1)
				throw new Exception($"LocalizationSettings is not found.");
			return settingsList[0];
		}
		
#if UNITY_EDITOR
		private void OnValidate()
		{
			var localeAssetGuids = UnityEditor.AssetDatabase.FindAssets("t:LocaleAsset");
			if (Locales != null && localeAssetGuids.Length == Locales.Length)
				return;
			
			Locales = new LocaleItem[localeAssetGuids.Length];
			for (int i = 0; i < localeAssetGuids.Length; i++)
			{
				var localeAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(localeAssetGuids[i]);
				var localeAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<LocaleAsset>(localeAssetPath);
				var startLength = "Assets/Resources/".Length;
				var endLength = ".asset".Length;
				Locales[i] = new LocaleItem { Language = localeAsset.Language, LocalePath = localeAssetPath.Substring(startLength, localeAssetPath.Length - startLength - endLength) };
			}
			
			UnityEditor.EditorUtility.SetDirty(this);
			UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
		}
#endif
	}
}