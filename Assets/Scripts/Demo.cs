using System;
using Localization;
using TMPro;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public TMP_Text DynamicText;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        LocalizeSystem.Init(SystemLanguage.English);
    }
    
    public void OnSetLanguage(string languageStr)
    {
        var language = Enum.Parse<SystemLanguage>(languageStr, true);
        LocalizeSystem.Init(language);
    }
    
    public void OnSetKey(string key)
    {
        DynamicText.text = key.Localize();
    }
}
