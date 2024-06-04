using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Core.LocalizeSystems.Utilities
{
	public static class CsvUtility
	{
		//CSV reader from https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/
		private const string SplitRe = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
		private const string LineSplitRe = @"\r\n|\n\r|\n|\r";
		private static readonly char[] _trimChars = { '\"' };

		public static List<List<string>> ReadCsvFromAsset(string assetName)
		{
			var data = Resources.Load(assetName) as TextAsset;
			if (data == null)
				return new List<List<string>>();
			return ParseCsv(data.text);
		}

		public static List<List<string>> ReadCsvFromFile(string fileName)
		{
			var data = File.ReadAllText(fileName);
			return ParseCsv(data);
		}

		public static List<List<string>> ParseCsv(string text)
		{
			text = CleanReturnInCsvTexts(text);
			var list = new List<List<string>>();
			var lines = Regex.Split(text, LineSplitRe);
			if(lines.Length <= 1)
				return list;
			var header = Regex.Split(lines[0], SplitRe);
			foreach(var line in lines)
			{
				var values = Regex.Split(line, SplitRe);
				var entry = new List<string>();
				for(var j = 0; j < header.Length && j < values.Length; j++)
				{
					var value = values[j];
					value = DecodeSpecialCharsFromCsv(value);
					entry.Add(value);
				}
				list.Add(entry);
			}
			return list;
		}

		public static string DecodeSpecialCharsFromCsv(string value)
		{
			value = value.TrimStart(_trimChars).TrimEnd(_trimChars).Replace("\\", "").Replace("<br>", "\n").Replace("<c>", ",");
			return value;
		}

		public static string CleanReturnInCsvTexts(string text)
		{
			text = text.Replace("\"\"", "'");
			if(text.IndexOf("\"", StringComparison.Ordinal) > -1)
			{
				string clean = "";
				bool insideQuote = false;
				for(int j = 0; j < text.Length; j++)
				{
					if(!insideQuote && text[j] == '\"')
					{
						insideQuote = true;
					}
					else if(insideQuote && text[j] == '\"')
					{
						insideQuote = false;
					}
					else if(insideQuote)
					{
						if(text[j] == '\n')
							clean += "<br>";
						else if(text[j] == ',')
							clean += "<c>";
						else
							clean += text[j];
					}
					else
					{
						clean += text[j];
					}
				}
				text = clean;
			}
			return text;
		}
	}
}
