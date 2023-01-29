using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PAEE_FINAL
{
    /// <summary>
    /// Lógica de interacción para PageConfiguration.xaml
    /// </summary>
    public partial class PageConfiguration : Page
    {
        const string LANG_ENG = "en_US";
        const string LANG_ES = "es_ES";
        const string LANG_FR = "fr_FR";

        public event EventHandler ChangeConfig;
        public PageConfiguration()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: cargando componentes");
            InitializeComponent();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: componente cargado!");

            Config.GetInstance().load();
            var lang = Config.GetInstance().lang;
            if (lang != null)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: lang cargado (" + lang + ")");
                if (lang.Equals(LANG_ES))
                {
                    rbLangEs.IsChecked = true;
                }
                else if (lang.Equals(LANG_ENG))
                {
                    rbLangEn.IsChecked = true;
                }
                else if (lang.Equals(LANG_FR))
                {
                    rbLangFr.IsChecked = true;
                }
            }

            var color = Config.GetInstance().buttonBgColor;
            if (color != null)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: color cargado (" + color + ")");
                cpButtonBg.SelectedColor = (Color?)ColorConverter.ConvertFromString(color);
            }
            color = Config.GetInstance().buttonFgColor;
            if (color != null)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: color cargado (" + color + ")");
                cpButtonFg.SelectedColor = (Color?)ColorConverter.ConvertFromString(color);
            }
            color = Config.GetInstance().labelFgColor;
            if (color != null)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: color cargado (" + color + ")");
                cpLabelFg.SelectedColor = (Color?)ColorConverter.ConvertFromString(color);
            }
            color = Config.GetInstance().tableBgColor;
            if (color != null)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: color cargado (" + color + ")");
                cpTableBg.SelectedColor = (Color?)ColorConverter.ConvertFromString(color);
            }
            color = Config.GetInstance().tableFgColor;
            if (color != null)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: color cargado (" + color + ")");
                cpTableFg.SelectedColor = (Color?)ColorConverter.ConvertFromString(color);
            }
            UpdateLenguage();
            ApplyConfig();
        }

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {

        }

        private void SaveOnClick(object sender, RoutedEventArgs e)
        { 
            if ((bool)rbLangEs.IsChecked)
            {
                Config.GetInstance().lang = LANG_ES;
            }
            else if ((bool)rbLangEn.IsChecked)
            {
                Config.GetInstance().lang = LANG_ENG;
            }
            else if ((bool)rbLangFr.IsChecked)
            {
                Config.GetInstance().lang = LANG_FR;
            }
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: lang cambiado a (" + Config.GetInstance().lang + ")");
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: color cambiado a (" + Config.GetInstance().buttonBgColor + ")");
            Config.GetInstance().buttonBgColor = cpButtonBg.SelectedColor.Value.ToString();
            Config.GetInstance().buttonFgColor = cpButtonFg.SelectedColor.Value.ToString();
            Config.GetInstance().labelFgColor = cpLabelFg.SelectedColor.Value.ToString();
            Config.GetInstance().tableBgColor = cpTableBg.SelectedColor.Value.ToString();
            Config.GetInstance().tableFgColor = cpTableFg.SelectedColor.Value.ToString();
            Config.GetInstance().Save();
            Config.GetInstance().cambiaIdioma();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: config actualizado!");
            ChangeConfig(sender, new EventArgs());
            UpdateLenguage();
            ApplyConfig();
        }

        private void UpdateLenguage() {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: actualizando lenguage!");
            groupBoxLenguages.Header = Properties.Resources.lenguages;
            rbLangEn.Content = Properties.Resources.english;
            rbLangEs.Content = Properties.Resources.spanish;
            rbLangFr.Content = Properties.Resources.french;
            groupBoxColors.Header = Properties.Resources.colors;
            Cancel.Content = Properties.Resources.cancel;
            Save.Content = Properties.Resources.accept;
            buttonsTitle.Content = Properties.Resources.buttons;
            backgroundButtons.Content = Properties.Resources.background;
            foregroundButtons.Content = Properties.Resources.foreground;
            labelsTitle.Content = Properties.Resources.texts;
            foregroundLabels.Content = Properties.Resources.foreground;
            tablesTitle.Content = Properties.Resources.tablesT;
            backgroundTables.Content = Properties.Resources.background;
            foregroundTables.Content = Properties.Resources.foreground;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageConfiguration: lenguage actualizado!");
        }

        private void ApplyConfig() 
        {
            var col = Config.GetInstance().buttonBgColor;
            var col2 = Config.GetInstance().buttonFgColor;
            var col3 = Config.GetInstance().labelFgColor;
            if (col != null && col2 != null && col3 != null)
            {
                var bgButtonColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col));
                var fgButtonColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col2));
                var fgLabelColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col3));
                Cancel.Background = bgButtonColor;
                Cancel.Foreground = fgButtonColor;
                Save.Background = bgButtonColor;
                Save.Foreground = fgButtonColor;
                groupBoxLenguages.Foreground = fgLabelColor;
                groupBoxColors.Foreground = fgLabelColor;
                rbLangEn.Foreground = fgLabelColor;
                rbLangEs.Foreground = fgLabelColor;
                rbLangFr.Foreground = fgLabelColor;
                buttonsTitle.Foreground = fgLabelColor;
                backgroundButtons.Foreground = fgLabelColor;
                foregroundButtons.Foreground = fgLabelColor;
                labelsTitle.Foreground = fgLabelColor;
                foregroundLabels.Foreground = fgLabelColor;
                tablesTitle.Foreground = fgLabelColor;
                backgroundTables.Foreground = fgLabelColor;
                foregroundTables.Foreground = fgLabelColor;
            }
        }
    }
}
