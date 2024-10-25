using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Localization.Editor.Settings
{
	public class LocalizationSettingsProvider : SettingsProvider
	{
		public LocalizationSettingsProvider(
		)
			: base(
				EditorConstants.ProjectSettingsPath,
				SettingsScope.Project,
				new HashSet<string>(new[] { "Localization" }))
		{
			label = "Localization";
			guiHandler = OnGuiHandler;
		}

		private void OnGuiHandler(string _)
		{
			var settings = new SerializedObject(LocalizationSettings.GetOrCreateSettings());
			using (new EditorGUILayout.HorizontalScope())
			{
				GUILayout.Space(10f);
				using (new EditorGUILayout.VerticalScope())
				{
					using (var changeCheck = new EditorGUI.ChangeCheckScope())
					{
						//EditorGUILayout.BeginVertical(GUI.skin.GetStyle("helpbox"));
						EditorGUILayout.LabelField(
							$"This fields is change Scripting Define Symbols in PlayerSettings. "
							+ $"\nAdd or remove {EditorConstants.ExperimentalAPIDefinition} for use experimental API. "
							+ $"\nAnd {EditorConstants.HideToolbarMenuDefinition} for hide or show toolbar menu.",
							GUI.skin.GetStyle("helpbox"));
						//EditorGUILayout.EndVertical();
						var usedExperimentalApi = DrawUsedExperimentalApiProperty(settings);
						var hideToolbarMenu = DrawHideToolbarProperty(settings);

						if (!changeCheck.changed)
							return;

						SetDefinitionState(EditorConstants.ExperimentalAPIDefinition, usedExperimentalApi);
						SetDefinitionState(EditorConstants.HideToolbarMenuDefinition, hideToolbarMenu);
						settings.ApplyModifiedProperties();

						LocalizationSettings.SaveSettings();
					}
				}
			}
		}

		private static bool DrawHideToolbarProperty(SerializedObject settings)
		{
			var hideToolbarMenuProperty = settings.FindProperty("_hideToolbarMenu");
			hideToolbarMenuProperty.boolValue = IsHasDefinition(EditorConstants.HideToolbarMenuDefinition, out _);
			EditorGUILayout.PropertyField(hideToolbarMenuProperty);
			return hideToolbarMenuProperty.boolValue;
		}

		private static bool DrawUsedExperimentalApiProperty(SerializedObject settings)
		{
			var usedExperimentalApiProperty = settings.FindProperty("_usedExperimentalApi");
			usedExperimentalApiProperty.boolValue = IsHasDefinition(
				EditorConstants.ExperimentalAPIDefinition,
				out _);
			EditorGUILayout.PropertyField(usedExperimentalApiProperty);
			return usedExperimentalApiProperty.boolValue;
		}

		private void SetDefinitionState(string definition, bool state)
		{
			var hasExperimentalApi = IsHasDefinition(
				definition,
				out var allProjectDefinitions);

			if (state)
			{
				if (!hasExperimentalApi)
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(
						BuildTargetGroup.Standalone,
						allProjectDefinitions.Concat(new[] { definition }).ToArray());
				}
			}
			else
			{
				if (hasExperimentalApi)
				{
					PlayerSettings.SetScriptingDefineSymbolsForGroup(
						BuildTargetGroup.Standalone,
						allProjectDefinitions
							.Where(element => element != definition)
							.ToArray());
				}
			}
		}

		private static bool IsHasDefinition(string definition, out string[] allProjectDefinitions)
		{
			PlayerSettings.GetScriptingDefineSymbolsForGroup(
				BuildTargetGroup.Standalone,
				out allProjectDefinitions);
			return
				allProjectDefinitions.Any(element => element == definition);
		}
	}
}