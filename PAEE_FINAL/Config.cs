using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace PAEE_FINAL
{
    public class Config
    {
        public string lang { get; set; }
        public string buttonBgColor { get; set; }
        public string buttonFgColor { get; set; }
        public string labelBgColor { get; set; }
        public string labelFgColor { get; set; }
        public string tableBgColor { get; set; }
        public string tableFgColor { get; set; }


        private static Config _instance;

        public static TraceSource logger = new TraceSource("PAEE_FINAL");

        public PAEEFinalEntities1 db;

        private Config()
        {
            //InitDefaults();
            db = new PAEEFinalEntities1();
        }

        public static Config GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Config();
            }
            return _instance;
        }

        public void InitDefaults()
        {
            lang = "es-ES";
            buttonBgColor = "#FF0000";
        }

        public void load()
        {
            logger.TraceEvent(TraceEventType.Information, 1, "Config: cargando configuración");
            if (Read("lang") != null)
            {
                lang = Read("lang");
                logger.TraceEvent(TraceEventType.Information, 1, "Config: idioma leído: '" + lang + "'");
            }
            if (Read("buttonBgColor") != null)
            {
                buttonBgColor = Read("buttonBgColor");
                logger.TraceEvent(TraceEventType.Information, 1, "Config: color leído: '" + buttonBgColor + "'");
            }
            if (Read("buttonFgColor") != null)
            {
                buttonFgColor = Read("buttonFgColor");
                logger.TraceEvent(TraceEventType.Information, 1, "Config: color leído: '" + buttonFgColor + "'");
            }
            if (Read("labelBgColor") != null)
            {
                labelBgColor = Read("labelBgColor");
                logger.TraceEvent(TraceEventType.Information, 1, "Config: color leído: '" + labelBgColor + "'");
            }
            if (Read("labelFgColor") != null)
            {
                labelFgColor = Read("labelFgColor");
                logger.TraceEvent(TraceEventType.Information, 1, "Config: color leído: '" + labelFgColor + "'");
            }
            if (Read("tableBgColor") != null)
            {
                tableBgColor = Read("tableBgColor");
                logger.TraceEvent(TraceEventType.Information, 1, "Config: color leído: '" + tableBgColor + "'");
            }
            if (Read("tableFgColor") != null)
            {
                tableFgColor = Read("tableFgColor");
                logger.TraceEvent(TraceEventType.Information, 1, "Config: color leído: '" + tableFgColor + "'");
            }
        }

        private string Read(string s)
        {
            return ConfigurationManager.AppSettings[s];
        }

        private void Write(Configuration configFile, string key, string value)
        {
            var settings = configFile.AppSettings.Settings;
            logger.TraceEvent(TraceEventType.Information, 1, "Config: escribiendo nuevos valores");
            if (settings[key] == null)
            {
                settings.Add(key, value);
                logger.TraceEvent(TraceEventType.Information, 1, "Config: valor escrito en: '" + key + "' = '" + value + "'");
            }
            else
            {
                settings[key].Value = value;
                logger.TraceEvent(TraceEventType.Information, 1, "Config: valor escrito en: '" + key + "' = '" + value + "'");
            }
        }

        public void Save()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Write(configFile, "lang", lang);
            logger.TraceEvent(TraceEventType.Information, 1, "Config: idioma guardado: '" + lang + "'");
            Write(configFile, "buttonBgColor", buttonBgColor);
            logger.TraceEvent(TraceEventType.Information, 1, "Config: color guardado: '" + buttonBgColor + "'");
            Write(configFile, "buttonFgColor", buttonFgColor);
            logger.TraceEvent(TraceEventType.Information, 1, "Config: color guardado: '" + buttonFgColor + "'");
            Write(configFile, "labelBgColor", labelBgColor);
            logger.TraceEvent(TraceEventType.Information, 1, "Config: color guardado: '" + labelBgColor + "'");
            Write(configFile, "labelFgColor", labelFgColor);
            logger.TraceEvent(TraceEventType.Information, 1, "Config: color guardado: '" + labelFgColor + "'");
            Write(configFile, "tableBgColor", tableBgColor);
            logger.TraceEvent(TraceEventType.Information, 1, "Config: color guardado: '" + tableBgColor + "'");
            Write(configFile, "tableFgColor", tableFgColor);
            logger.TraceEvent(TraceEventType.Information, 1, "Config: color guardado: '" + tableFgColor + "'");
            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            SaveConfig();
        }

        public void cambiaIdioma()
        {
            if (lang != null)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            }
        }

        public void SaveConfig()
        {
            logger.TraceEvent(TraceEventType.Information, 1, "Config: guardando configuración ");
            try
            {
                string fichero = Properties.Settings.Default.appConfig.ToString();
                StreamWriter fc = new StreamWriter(fichero, false);
                fc.WriteLine(GetInstance().lang);
                fc.WriteLine(GetInstance().buttonBgColor);
                fc.WriteLine(GetInstance().buttonFgColor);
                fc.WriteLine(GetInstance().labelBgColor);
                fc.WriteLine(GetInstance().labelFgColor);
                fc.WriteLine(GetInstance().tableBgColor);
                fc.WriteLine(GetInstance().tableFgColor);
                fc.Close();
                logger.TraceEvent(TraceEventType.Information, 1, "Config: configuración guardada con exito!");
            }
            catch (Exception ex) {
                logger.TraceEvent(TraceEventType.Error, 1, "Config: error guardando la configuración => (" + ex.Message + ")");
            }
            
        }

        public static void recuperaConfig()
        {
            logger.TraceEvent(TraceEventType.Information, 1, "Config: recuperando configuración ");
            string fichero = Properties.Settings.Default.appConfig.ToString();
            if (File.Exists(fichero))
            {
                StreamReader fc = new StreamReader(fichero);
                GetInstance().lang = fc.ReadLine();
                GetInstance().buttonBgColor = fc.ReadLine();
                GetInstance().buttonFgColor = fc.ReadLine();
                GetInstance().labelBgColor = fc.ReadLine();
                GetInstance().labelFgColor = fc.ReadLine();
                GetInstance().tableBgColor = fc.ReadLine();
                GetInstance().tableFgColor = fc.ReadLine();
                fc.Close();
            }
            else {
                logger.TraceEvent(TraceEventType.Error, 1, "Config: no existe fichero de configuración ");
            }
        }
    }
}
