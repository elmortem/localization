# Simple Localization System

[![color:ff69b4](https://img.shields.io/badge/licence-Unlicense-blue)](https://unlicense.org)
![color:ff69b4](https://img.shields.io/badge/Unity-2019.3.x-red)

Localization system with support Google Sheet support.

## Installation

Installation as a unity module via a git link in PackageManager or direct editing of `Packages/manifest' is supported.json:
```
"com.elmortem.localization": "https://github.com/elmortem/localization.git?path=Packages/localization",
```

## Google Sheet example

```https://docs.google.com/spreadsheets/d/1q1MeK6qlmNKO2uf-yuRJlGW5EZZ0BJ15XO6Tu-8TjU0```

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

## Pluralism

The system supports pluralization for different languages. Use square brackets with index and forms:
```
"Price: {0} [0:crystal|crystals]" // English
"Цена: {0} [0:кристалл|кристалла|кристаллов|кристаллов]" // Russian
```

## Cases
Support for grammatical cases in languages that use them. Use colon syntax for case forms:
```csharp
// Basic usage with array of forms
var forms = new[] { "Anna", "Anna's" }; // English needs only 2 forms
var text = "hello".LocalizeFormat(forms); // Will use appropriate form for language
```
Languages that don't use cases (like English) will only use the basic form, while others (like Russian or Serbian) will use the forms they need.

Check the extended **demo** for details.

## Text Mesh Pro
Add component **LocalizeText** at the same level as TMP_Text to static localize text field.

## Toolbar menu
The plugin contains commands for quick access to loading the localization table and opening the settings window on the toolbar **Tools/Localization/Download everything from Google Sheet** and **Tools/Localization/Settings** accordingly.

If for some reason you are not satisfied with the presence of the **Tools/Localization** menu in the Toolbar, you can disable it by setting the **Hide Toolbar Menu** flag to True in **ProjectSettings/Localization**

## Experimental feature
Add define LOCALIZE_EXPERIMENTAL to apply experimental keys selector with search and groups.

You can set Flag Used Experimental Api as True in ProjectSettings/Localization and system automatically adds the required defines. 

## Pluralism
In progress...

### Other

Support Unity 2019.3 or later.

Use for free.

Enjoy!
