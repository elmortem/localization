namespace Localization
{
	public static class LocalizeUtility
	{
		public static string Localize(this string key, string defaultValue = null)
		{
			return LocalizeSystem.Localize(key, defaultValue);
		}
		
		public static string LocalizeFormat(this string key, string defaultValue, params object[] args)
		{
			return LocalizeSystem.LocalizeFormat(key, defaultValue, args);
		}
		
		public static string LocalizeFormat(this string key, params object[] args)
		{
			return LocalizeSystem.LocalizeFormat(key, null, args);
		}
	}
}