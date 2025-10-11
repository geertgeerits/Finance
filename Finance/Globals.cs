namespace Finance
{
    //// Global variables and methods
    internal static class Globals
    {
        // Global variables
        public static string cTheme = "";
        public static bool bDateFormatSystem;
        public static string cSysDateFormat = "";
        public static string cDateFormat = "";
        public static string cISOCurrencyCode = "";
        public static string cKeyboard = "";
        public static string cLanguage = "";
        public static bool bLanguageChanged;
        public static string cPageFormat = "";
        public static bool bLicense;

        /// <summary>
        /// Set the theme
        /// </summary>
        public static void SetTheme()
        {
            Application.Current!.UserAppTheme = cTheme switch
            {
                "Light" => AppTheme.Light,
                "Dark" => AppTheme.Dark,
                _ => AppTheme.Unspecified,
            };
        }

        /// <summary>
        /// Set the current UI culture of the selected language 
        /// </summary>
        public static void SetCultureSelectedLanguage()
        {
            try
            {
                CultureInfo switchToCulture = new(cLanguage);
                LocalizationResourceManager.Instance.SetCulture(switchToCulture);
            }
            catch
            {
                // Do nothing
            }
        }
    }
}