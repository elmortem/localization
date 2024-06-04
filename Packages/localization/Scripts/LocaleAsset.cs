using System;
using System.Collections.Generic;
using UnityEngine;

namespace Localization
{
	[CreateAssetMenu(menuName = "Localization/Locale")]
	public class LocaleAsset : ScriptableObject
	{
		[Serializable]
		public struct LocaleItem
		{
			public string Key;
			public string Group;
			[TextArea(minLines:1, maxLines:10)]
			public string Comment;
			[TextArea(minLines:1, maxLines:10)]
			public string Value;
		}

		public SystemLanguage Language;
		public int ValueColumn;
		public List<LocaleItem> Items = new();
	}
}