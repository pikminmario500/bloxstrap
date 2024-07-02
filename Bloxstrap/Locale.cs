﻿using System.Windows;

using Bloxstrap.Resources;

namespace Bloxstrap
{
    internal static class Locale
    {
        public static CultureInfo CurrentCulture = CultureInfo.InvariantCulture;

        public static bool RightToLeft { get; private set; } = false;

        public static readonly Dictionary<string, string> SupportedLocales = new()
        {
            { "nil", Strings.Enums_Theme_Default }, // /shrug
            { "en", "English" },
            { "en-US", "English (United States)" },
            { "ar", "العربية" },
            { "bn", "বাংলা" },
            { "bs", "Босански" },
            { "bg", "Български" },
            { "zh-CN", "中文 (简体)" },
            { "zh-HK", "中文 (廣東話)" },
            { "zh-TW", "中文 (繁體)" },
            { "cs", "Čeština" },
            // { "dk", "Dansk" },
            { "nl", "Nederlands" },
            { "fl", "Filipino" },
            { "fi", "Suomi" },
            { "fr", "Français" },
            { "de", "Deutsch" },
            { "he", "עברית‎" },
            { "hr", "Hrvatski" },
            // { "hi", "हिन्दी" },
            { "hu", "Magyar" },
            // { "id", "Bahasa Indonesia" },
            // { "it", "Italiano" },
            { "ja", "日本語" },
            { "ko", "한국어" },
            { "lt", "Lietuvių" },
            // { "no", "Bokmål" },
            { "pl", "Polski" },
            { "pt-BR", "Português (Brasil)" },
            { "ro", "Română" },
            { "ru", "Русский" },
            { "es", "Español" },
            { "sv-SE", "Svenska" },
            { "th", "ภาษาไทย" },
            { "tr", "Türkçe" },
            { "uk", "Yкраїньска" },
            { "vi", "Tiếng Việt" }
        };

        public static string GetIdentifierFromName(string language) => SupportedLocales.FirstOrDefault(x => x.Value == language).Key ?? "nil";

        public static List<string> GetLanguages()
        {
            var languages = new List<string>()
            {
                Strings.Enums_Theme_Default,
                "English",
                "English (United States)"
            };

            languages.AddRange(SupportedLocales.Values.Where(x => !languages.Contains(x)).OrderBy(x => x));

            return languages;
        }

        public static void Set()
        {
            string identifier = App.Settings.Prop.Locale;

            if (!SupportedLocales.ContainsKey(identifier))
                identifier = "nil";

            if (identifier == "nil")
                return;

            CurrentCulture = new CultureInfo(identifier);

            CultureInfo.DefaultThreadCurrentUICulture = CurrentCulture;
            Thread.CurrentThread.CurrentUICulture = CurrentCulture;

            RoutedEventHandler? handler = null;

            if (identifier == "ar" || identifier == "he")
            {
                RightToLeft = true;

                handler = new((sender, _) => 
                { 
                    var window = (Window)sender;
                
                    window.FlowDirection = FlowDirection.RightToLeft;

                    if (window.ContextMenu is not null)
                        window.ContextMenu.FlowDirection = FlowDirection.RightToLeft;
                });
            }
            else if (identifier == "th")
            {
                handler = new((window, _) => ((Window)window).FontFamily = new System.Windows.Media.FontFamily(new Uri("pack://application:,,,/Resources/Fonts/"), "./#Noto Sans Thai"));
            }

            // https://supportcenter.devexpress.com/ticket/details/t905790/is-there-a-way-to-set-right-to-left-mode-in-wpf-for-the-whole-application
            if (handler is not null)
                EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.LoadedEvent, handler);
        }
    }
}
