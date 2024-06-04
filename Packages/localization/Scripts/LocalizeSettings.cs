using System;
using System.Collections.Generic;
using System.IO;
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
		public LocaleItem[] Locales = Array.Empty<LocaleItem>();
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
			if (settingsList == null || settingsList.Length <= 0)
			{
#if UNITY_EDITOR
				if(!Directory.Exists("Assets/Resources"))
					Directory.CreateDirectory("Assets/Resources");

				if (!File.Exists("Assets/Resources/English.asset"))
				{
					var englishLocale = ScriptableObject.CreateInstance<LocaleAsset>();
					englishLocale.Language = SystemLanguage.English;
					UnityEditor.AssetDatabase.CreateAsset(englishLocale, "Assets/Resources/English.asset");
				}

				var settings = ScriptableObject.CreateInstance<LocalizeSettings>();
				UnityEditor.AssetDatabase.CreateAsset(settings, "Assets/Resources/LocalizeSettings.asset");
				return settings;
#endif
				throw new Exception($"LocalizationSettings is not found.");
			}

			if (settingsList.Length > 1)
			{
				Debug.LogError("Need only one localization settings. See. " + string.Join(", ",
					settingsList.Select(p => UnityEditor.AssetDatabase.GetAssetPath(p))));
			}
			return settingsList[0];
		}
		
#if UNITY_EDITOR
		private void OnValidate()
		{
			var localeAssetGuids = UnityEditor.AssetDatabase.FindAssets("t:LocaleAsset");
			//if (Locales != null && localeAssetGuids.Length == Locales.Length)
			//{
			//	return;
			//}

			var locales = new List<LocaleItem>();
			for (int i = 0; i < localeAssetGuids.Length; i++)
			{
				var localeAssetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(localeAssetGuids[i]);
				if (!localeAssetPath.Contains("Assets/Resources/"))
				{
					Debug.LogError($"Incorrect path {localeAssetPath}. Move to Resources folder.");
					continue;
				}
				var localeAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<LocaleAsset>(localeAssetPath);
				var startLength = "Assets/Resources/".Length;
				var endLength = ".asset".Length;
				var localeResourcePath =
					localeAssetPath.Substring(startLength, localeAssetPath.Length - startLength - endLength);
				locales.Add(new LocaleItem { Language = localeAsset.Language, LocalePath = localeResourcePath });
			}

			Locales = locales.ToArray();
			
			UnityEditor.EditorUtility.SetDirty(this);
			//UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
		}
#endif
	}
}