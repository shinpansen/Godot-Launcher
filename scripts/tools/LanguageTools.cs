using Godot;
using GodotLauncher.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Tools;

public static class LanguageTools
{
    public static void ChangeAppLanguage(Language language)
    {
        switch (language)
        {
            case Language.Default:
                string culture = System.Globalization.CultureInfo.InstalledUICulture.Name;
                TranslationServer.SetLocale(culture.Split('-').First());
                break;
            case Language.English:
                TranslationServer.SetLocale("en");
                break;
            case Language.French:
                TranslationServer.SetLocale("fr");
                break;
            case Language.German:
                TranslationServer.SetLocale("de");
                break;
            case Language.Spanish:
                TranslationServer.SetLocale("es");
                break;
            case Language.Italian:
                TranslationServer.SetLocale("it");
                break;
            case Language.Portugese:
                TranslationServer.SetLocale("pt");
                break;
            case Language.Russian:
                TranslationServer.SetLocale("ru");
                break;
            case Language.Japanese:
                TranslationServer.SetLocale("ja");
                break;
            case Language.Chinese:
                TranslationServer.SetLocale("zh");
                break;
            default:
                break;
        }
    }
}
