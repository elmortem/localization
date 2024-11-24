using System;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Collections.Generic;

namespace Localization
{
    public static class LocalizeSystem
    {
        public static readonly Regex ParamsRegExpFirst = new(@"{(\d+)(?::(\d+))?}");
        public static readonly Regex ParamsRegExpSecond = new(@"\[(\d):([^\]]+)\]");
        
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

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("Key is empty.");
                return string.Empty;
            }

            if (_database.Values.TryGetValue(key, out var value))
                return value;
            
            return string.IsNullOrEmpty(defaultValue) ? key : defaultValue;
        }
        
        public static string LocalizeFormat(string key, string defaultValue, params object[] args)
        {
            var value = Localize(key, defaultValue);
            
            if (!_database.Pluralizme)
                return string.Format(value, args);

            value = ParamsRegExpFirst.Replace(value, delegate(Match m)
            {
                var k = int.Parse(m.Groups[1].Value);
                string rep = m.Value;

                if (args != null && args.Length > k && args[k] != null)
                {
                    if (m.Groups[2].Success)
                    {
                        var caseIndex = int.Parse(m.Groups[2].Value);
                        if (args[k] is IList<string> cases)
                        {
                            rep = caseIndex < cases.Count ? cases[caseIndex] : m.Value;
                        }
                        else if (args[k] is string str)
                        {
                            rep = str;
                        }
                    }
                    else
                    {
                        if (args[k] is IList<string> cases && cases.Count > 0)
                        {
                            rep = cases[0];
                        }
                        else
                        {
                            rep = args[k].ToString();
                        }
                    }
                }
                
                return rep;
            });

            value = ParamsRegExpSecond.Replace(value, delegate(Match m)
            {
                var variableIdx = int.Parse(m.Groups[1].Value);

                var count = args.Length > variableIdx ? Mathf.Abs(int.Parse(args[variableIdx].ToString())) : 0;
                var cat = m.Groups[2].Value.Split('|');
                if (cat.Length <= 0)
                    return string.Empty;

                if (_database.Language == SystemLanguage.Russian && cat.Length > 3)
                {
                    var one = cat[0];
                    var few = cat[1];
                    var many = cat[2];
                    var other = cat[3];

                    if (count % 10 == 1 && count % 100 != 11)
                        return one;
                    if (InRange(count % 10, 2, 4) && !InRange(count % 100, 12, 14))
                        return few;
                    if (count % 10 == 0 || InRange(count % 10, 5, 9) || InRange(count % 100, 11, 14))
                        return many;
                    return other;
                }

                if (_database.Language == SystemLanguage.SerboCroatian && cat.Length > 3)
                {
                    var one = cat[0];
                    var few = cat[1];
                    var many = cat[2];
                    var other = cat[3];

                    if (count == 0)
                        return other;
                    
                    var rem = count % 10;
                    var remHundred = count % 100;
                    
                    if (rem == 1 && remHundred != 11)
                        return one;
                    if (InRange(rem, 2, 4) && !InRange(remHundred, 12, 14))
                        return few;
                    
                    return many;
                }

                if (_database.Language == SystemLanguage.French && cat.Length > 1)
                {
                    var one = cat[0];
                    var other = cat[1];

                    if (InRange(count, 0, 2) && count != 2)
                        return one;

                    return other;
                }

                if (cat.Length > 0 && (_database.Language == SystemLanguage.Korean || 
                                     _database.Language == SystemLanguage.Chinese ||
                                     _database.Language == SystemLanguage.Japanese || 
                                     _database.Language == SystemLanguage.Turkish))
                {
                    var other = cat[0];
                    return other;
                }

                if (cat.Length > 1)
                {
                    var one = cat[0];
                    var other = cat[1];

                    if (count == 1)
                        return one;
                    return other;
                }

                Debug.LogError($"Incorrect format for localization key '{key}'.");
                return key;
            });

            return value;
        }
        
        private static bool InRange(int value, int from, int to)
        {
            return value >= from && value <= to;
        }
    }
}