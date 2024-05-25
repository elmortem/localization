# Simple Localization System

[![color:ff69b4](https://img.shields.io/badge/licence-Unlicense-blue)](https://unlicense.org)
![color:ff69b4](https://img.shields.io/badge/Unity-2019.3.x-red)

Localization system with support Google Sheet support.

## Installation

Installation as a unity module via a git link in PackageManager or direct editing of `Packages/manifest' is supported.json:
```
"com.elmortem.localization": "https://github.com/elmortem/localization.git",
```

## Settings

Create and setup LocalizationSettings asset.

## Locale

Create LocaleAsset from Asset menu. Setup import from Google Sheet.

## LocalizeString attribute

Add attribute LocalizeString for string property;
```csharp
[LocalizeString]
public string TextKey;
```

## Localize
```csharp
var loclizationText = TextKey.Localize();
var formattedText = TextKey.LocalizeFormat(arg0, arg1, arg2);
```

## Text Mesh Pro
Add component LocalizeText at the same level as TMP_Text.

## Pluralism
In progress...

Support Unity 2019.3 or later.

Use for free.

Enjoy!