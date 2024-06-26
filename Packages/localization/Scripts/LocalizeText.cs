using TMPro;
using UnityEngine;

namespace Localization
{
	public class LocalizeText : MonoBehaviour
	{
		public TMP_Text Text;
		[LocalizeString]
		public string Key;
		public string[] Arguments;

		private void OnEnable()
		{
			LocalizeSystem.ChangeEvent += OnLanguageChange;
			UpdateText();
		}

		private void OnDisable()
		{
			LocalizeSystem.ChangeEvent -= OnLanguageChange;
		}

		private void OnLanguageChange()
		{
			UpdateText();
		}

		private void UpdateText()
		{
			Text.text = string.Format(Key.Localize(Text.text), Arguments);
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				Text = GetComponent<TMP_Text>();
				if (Text != null && Text.text[0] != '.')
					Text.text = "." + Text.text;
			}
		}
#endif
	}
}