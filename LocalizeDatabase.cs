using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
	public class LocalizeDatabase
	{
		private SystemLanguage _language = SystemLanguage.Unknown;
		private readonly Dictionary<string, string> _values = new();

		public SystemLanguage Language => _language;
		public IReadOnlyDictionary<string, string> Values => _values;

		public bool Load(SystemLanguage language)
		{
			_values.Clear();
			var locale = LoadLocaleAsset(language);
			if (locale == null)
				return false;

			return Load(locale);
		}

		public bool Load(LocaleAsset locale)
		{
			foreach (var item in locale.Items)
			{
				_values[item.Key] = item.Value;
			}
			_language = locale.Language;
			
			return true;
		}

		private static LocalizeSettings _settings;
		public static LocaleAsset LoadLocaleAsset(SystemLanguage language, bool silent = false)
		{
			if(_settings == null)
				_settings = LocalizeSettings.MakeInstance();
			
			var localeIndex = Array.FindIndex(_settings.Locales, p => p.Language == language);
			if (localeIndex < 0)
			{
				if(!silent)
					Debug.LogError($"Locale '{language}' is not found.");
				return null;
			}

			return _settings.Locales[localeIndex].Load();
		}
	}
}