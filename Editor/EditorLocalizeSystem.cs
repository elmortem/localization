using System;
using System.Collections.Generic;
using Core.LocalizeSystems.Utilities;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor
{
	public static class EditorLocalizeSystem
	{
		private static LocaleAsset _localeAsset;
		
		public static bool TryGetValue(SystemLanguage language, string key, out string value)
		{
			if(_localeAsset == null || _localeAsset.Language != language)
			{
				_localeAsset = LocalizeDatabase.LoadLocaleAsset(language, true);
			}

			if (_localeAsset != null)
			{
				var itemIndex = _localeAsset.Items.FindIndex(p => p.Key == key);
				if (itemIndex >= 0)
				{
					value = _localeAsset.Items[itemIndex].Value;
					return true;
				}
			}

			value = string.Empty;
			return false;
		}

		public static List<string> GetKeys(SystemLanguage language)
		{
			List<string> results = new();
			
			if(_localeAsset == null || _localeAsset.Language != language)
			{
				_localeAsset = LocalizeDatabase.LoadLocaleAsset(language);
			}

			if (_localeAsset != null)
			{
				foreach (var item in _localeAsset.Items)
				{
					results.Add(item.Key);
				}
			}

			return results;
		}

		public static List<(string group, string key)> GetGroupedKeys(SystemLanguage language)
		{
			List<(string group, string key)> results = new();
			
			if(_localeAsset == null || _localeAsset.Language != language)
			{
				_localeAsset = LocalizeDatabase.LoadLocaleAsset(language);
			}
			
			if (_localeAsset != null)
			{
				foreach (var item in _localeAsset.Items)
				{
					results.Add((item.Group, item.Key));
				}
				
				results.Sort((a, b) => string.Compare(a.group, b.group, StringComparison.Ordinal));
			}

			return results;
		}
		
		public static void LoadFromGoogleSheet(List<LocaleAsset> localeAssets)
		{
			var settings = LocalizeSettings.MakeInstance();

			foreach (var localeAsset in localeAssets)
			{
				localeAsset.Items = new();
			}

			var groups = settings.Groups;
			if (groups == null || groups.Length <= 0)
				groups = new[] { new LocalizeSheet { Group = "Default", SheetId = "0" } };

			foreach (var groupItem in groups)
			{
				var group = groupItem.Group;
				GoogleSheetUtility.DownloadCsv(settings.DocId, str =>
				{
					foreach (var localeAsset in localeAssets)
					{
						if (string.IsNullOrEmpty(str))
						{
							Debug.LogError("No localization data.");
							return;
						}

						AddLocaleItems(settings, localeAsset, group, str);
						EditorUtility.SetDirty(localeAsset);
					}
				}, null, groupItem.SheetId);
			}
		}
		
		private static void AddLocaleItems(LocalizeSettings settings, LocaleAsset localeAsset, string sheetGroup, string str)
		{
			var data = CsvUtility.ParseCsv(str);
			if (data.Count > 0)
			{
				var hasKeyGroup = settings.GroupColumn >= 0;
				for(var i = settings.SkipLine; i < data.Count; ++i)
				{
					var item = data[i];
					var keyGroup = sheetGroup;
					if (hasKeyGroup)
					{
						var itemGroup = item[settings.GroupColumn];
						if(!string.IsNullOrEmpty(itemGroup))
							keyGroup = itemGroup;
					}

					localeAsset.Items.Add(new LocaleAsset.LocaleItem
					{
						Key = item[settings.KeyColumn],
						Group = keyGroup,
						Comment = item[settings.CommentColumn],
						Value = item[localeAsset.ValueColumn]
					});
				}
			}
			else
			{
				Debug.LogWarning("Empty data.");
			}
		}
	}
}