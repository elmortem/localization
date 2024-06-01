#if LOCALIZE_EXPERIMENTAL
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Localization.Editor
{
	public class LocaleKeyProvider : ScriptableObject, ISearchWindowProvider
	{
		private SystemLanguage _language;
		private SerializedProperty _property;
		private List<SearchTreeEntry> _keyEntries = new();

		public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context) => _keyEntries;

		public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
		{
			_property.serializedObject.UpdateIfRequiredOrScript();
			_property.stringValue = (string)SearchTreeEntry.userData;
			_property.serializedObject.ApplyModifiedProperties();
			return true;
		}
		
		private void MakeKeyEntries()
		{
			_keyEntries.Clear();
			
			var keys = EditorLocalizeSystem.GetKeys(_language);
			
			_keyEntries.Add(new SearchTreeGroupEntry(new GUIContent("Localization"), 0));
			
			_keyEntries.Add(new SearchTreeGroupEntry(new GUIContent("Keys"), 1));
			foreach (var key in keys)
			{
				_keyEntries.Add(new SearchTreeEntry(new GUIContent(key)) { userData = key, level = 2 }); 
			}
			
			_keyEntries.Add(new SearchTreeGroupEntry(new GUIContent("Values"), 1));
			foreach (var key in keys)
			{
				EditorLocalizeSystem.TryGetValue(_language, key, out var value);
				_keyEntries.Add(new SearchTreeEntry(new GUIContent(value)) { userData = key, level = 2 });
			}
			
			var groupedKeys = EditorLocalizeSystem.GetGroupedKeys(_language);
			
			var groups = new List<string>();
			foreach (var (group, key) in groupedKeys)
			{
				if (!groups.Contains(group))
				{
					_keyEntries.Add(new SearchTreeGroupEntry(new GUIContent(group), 1));
					groups.Add(group);
				}
				
				_keyEntries.Add(new SearchTreeEntry(new GUIContent(key)) { userData = key, level = 2 });
			}
		}

		public static LocaleKeyProvider Make(SystemLanguage language, SerializedProperty property)
		{
			var provider = ScriptableObject.CreateInstance<LocaleKeyProvider>();
			provider._language = language;
			provider._property = property;
			provider.MakeKeyEntries();
			
			return provider;
		}
	}
}
#endif