using System;
using UnityEngine;

namespace Localization
{
	public static class LocalizeSystem
	{
		private static LocalizeDatabase _database;

		public static Action ChangeEvent;

		public static SystemLanguage Language => _database.Language;

		public static void Init(SystemLanguage language)
		{
			if (_database == null)
				_database = new LocalizeDatabase();
			if (!_database.Load(language))
			{
				var defaultLanguage = LocalizeSettings.MakeInstance().DefaultLanguage;
				_database.Load(defaultLanguage);
			}
			
			ChangeEvent?.Invoke();
		}
		
		public static string Localize(string key, string defaultValue = null)
		{
			if (_database == null)
				throw new Exception("LocalizeSystem not initialized.");
			
			if (_database.Values.TryGetValue(key, out var value))
				return value;
			
			return string.IsNullOrEmpty(defaultValue) ? key : defaultValue;
		}
		
		public static string LocalizeFormat(string key, string defaultValue, params object[] args)
		{
			return string.Format(Localize(key, defaultValue), args);
		}
	}
}