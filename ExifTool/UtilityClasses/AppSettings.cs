using System.Configuration;

namespace ExifTool.UtilityClasses
{
    class AppSettings
    {
        public static string GetAppSettings(string key)
        {
            //Laden der AppSettings
            Configuration config = ConfigurationManager.OpenExeConfiguration(
                                    System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Zurückgeben der dem Key zugehörigen Value
            return config.AppSettings.Settings[key].Value;
        }

        public static void SetAppSettings(string key, string value)
        {
            //Laden der AppSettings
            Configuration config = ConfigurationManager.OpenExeConfiguration(
                                    System.Reflection.Assembly.GetExecutingAssembly().Location);
            //Überprüfen ob Key existiert
            if (config.AppSettings.Settings[key] != null)
            {
                //Key existiert. Löschen des Keys zum "überschreiben"
                config.AppSettings.Settings.Remove(key);
            }
            //Anlegen eines neuen KeyValue-Paars
            config.AppSettings.Settings.Add(key, value);
            //Speichern der aktualisierten AppSettings
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
