using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;

namespace PAEE_FINAL
{
    /// <summary>
    /// Lógica de interacción para PageHome.xaml
    /// </summary>
    public partial class PageHome : Page
    {
        public PageHome()
        {
            InitializeComponent();
            UpdateLenguage();
            ApplyConfig();
        }

        private void UpdateLenguage()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: actualizando idioma!");
            title.Content = Properties.Resources.titleHome;
            subtitle.Text = Properties.Resources.subTitleHome;
            subtitle2.Text = Properties.Resources.subTitle2Home;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: idioma actualizado!");
        }

        private void ApplyConfig()
        {
            var col = Config.GetInstance().labelFgColor;
            if (col != null)
            {
                var fgLabelColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col));;

                title.Foreground = fgLabelColor;
                subtitle.Foreground = fgLabelColor;
                subtitle2.Foreground = fgLabelColor;
            }
        }
    }
}
