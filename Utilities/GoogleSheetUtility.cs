using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Core.LocalizeSystems.Utilities
{
	public static class GoogleSheetUtility
	{
		private const int LoopMax = 100000000;

		public static bool DownloadCsv(string docId, Action<string> callback = null, string assetPath = null, string sheetId = null)
		{
			string url = "https://docs.google.com/spreadsheets/d/" + docId + "/export?format=csv";
			if(!string.IsNullOrEmpty(sheetId))
				url += "&gid=" + sheetId;

			int loop = 0;
			UnityWebRequest request = UnityWebRequest.Get(url);
			request.SendWebRequest();
			while(!request.isDone && string.IsNullOrEmpty(request.error))
			{
				if(request.downloadedBytes <= 0 && ++loop >= LoopMax)
				{
					Debug.LogError("No data at " + url);
					return false;
				}
			}

			if(!string.IsNullOrEmpty(request.error))
			{
				Debug.LogError(request.error);
				return false;
			}

			var text = request.downloadHandler.text;

			callback?.Invoke(text);

			if(!string.IsNullOrEmpty(assetPath))
			{
				File.WriteAllText(assetPath, text);
			}

			return true;
		}
	}
}
