using System;
using System.Collections.Generic;
using System.Linq;
using Localization;
using TMPro;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public TMP_Text DynamicText;
    
    [Header("Pluralism")]
    public TMP_Text PriceText;
    [LocalizeString]
    public string PriceKey = "price";
    
    [Header("Cases")]
    public TMP_Text CaseText;
    public TMP_Text NameText;
    [LocalizeString]
    public List<string> AnnaNameKeys;
    [LocalizeString]
    public List<string> MilicaNameKeys;
    [LocalizeString]
    public List<string> TextKeys;

    private string _key;
    private int _price = 0;
    private int _textIndex = 0;
    private List<List<string>> _names = new();
    private int _nameIndex;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        LocalizeSystem.Init(SystemLanguage.English);
    }

    private void OnEnable()
    {
        LocalizeSystem.ChangeEvent += OnLanguageChanged;

        UpdateNames();
        OnAddPrice(0);
        OnAddName(0);
    }

    private void OnDisable()
    {
        LocalizeSystem.ChangeEvent -= OnLanguageChanged;
    }

    private void UpdateNames()
    {
        _names.Clear();
        _names.Add(AnnaNameKeys.Select(p =>p.Localize()).ToList());
        _names.Add(MilicaNameKeys.Select(p =>p.Localize()).ToList());
    }

    private void OnLanguageChanged()
    {
        UpdateNames();
        OnSetKey(_key);
        OnAddPrice(0);
        OnAddName(0);
    }

    public void OnSetLanguage(string languageStr)
    {
        var language = Enum.Parse<SystemLanguage>(languageStr, true);
        LocalizeSystem.Init(language);
    }
    
    public void OnSetKey(string key)
    {
        _key = key;
        DynamicText.text = key.Localize();
    }
    
    public void OnAddPrice(int amount)
    {
        _price += amount;
        PriceText.text = PriceKey.LocalizeFormat(_price);
    }

    public void OnAddName(int amount)
    {
        _nameIndex += amount;
        if (_nameIndex < 0) _nameIndex = _names.Count - 1;
        if (_nameIndex >= _names.Count) _nameIndex = 0;

        NameText.text = _names[_nameIndex][0];
        OnAddText(0);
    }
    
    public void OnAddText(int amount)
    {
        _textIndex += amount;
        if (_textIndex < 0) _textIndex = TextKeys.Count - 1;
        if (_textIndex >= TextKeys.Count) _textIndex = 0;
        
        CaseText.text = TextKeys[_textIndex].LocalizeFormat(_names[_nameIndex]);
    }
}
