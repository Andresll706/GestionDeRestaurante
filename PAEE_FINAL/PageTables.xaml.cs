using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
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
    /// Lógica de interacción para PageTables.xaml
    /// </summary>
    public partial class PageTables : Page
    {
        public PageTables()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: cargando componentes");
            InitializeComponent();
            UpdateLenguage();
            ApplyConfig();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: componente cargado!");
            System.Windows.Data.CollectionViewSource tablesViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("tablesViewSource")));
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: cargando datos de la base de datos");
            Config.GetInstance().db.Mesas.Load();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: datos cargados!");
            tablesViewSource.Source = Config.GetInstance().db.Mesas.Local;
        }

        private void BtnAdd(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: click button add!");
            cleanFormAdd();
            gridListTables.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Hidden;
            gridAddTable.Margin = new Thickness(5, 0, 5, 4);
            gridAddTable.Visibility = Visibility.Visible;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "Config: end button add!");
        }
        private void cleanFormAdd()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: limpiando formulario");
            numberNewTable.Text = "";
            dinersNewTable.Text = "";
        }

        private void BtnSearchOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: click en buscando!");
            System.Windows.Data.CollectionViewSource tablesViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("tablesViewSource")));
            string entrada = "%" + textSearch.Text + "%";
            if (entrada != "")
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: consulta de => ( " + entrada + ")");
                tablesViewSource.Source = Config.GetInstance().db.Mesas.SqlQuery("Select * from Mesas where Mesas.Numero LIKE @input or" +
                    " Mesas.Comensales LIKE @input ",
                    new SqlParameter("@input", entrada)).ToListAsync().Result;
            }
            else
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: consulta de campo vacio!");
                tablesViewSource.Source = Config.GetInstance().db.Mesas.Local;
            }
        }


        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: click button edit!");
            gridListTables.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Hidden;
            gridEditTables.Margin = new Thickness(5, 0, 5, 4);
            gridEditTables.Visibility = Visibility.Visible;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: end button edit!");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: Eliminando elemento");
            Mesa p = (Mesa)gridListTables.SelectedItem;
            MessageBoxResult res = MessageBox.Show(Properties.Resources.askDelete + " '" + p.Numero + "'?", Properties.Resources.attention, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: respuesta Si al messagebox Eliminar");
                try
                {
                    Config.GetInstance().db.Mesas.Remove(p);
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: elemento eliminado '" + p.Id + "'");
                    MessageBox.Show(Properties.Resources.table + " '" + p.Numero + "' "+ Properties.Resources.deleteCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageTables: error eliminando => ( "  + ex.Message +  ")");
                    MessageBox.Show(Properties.Resources.deleteCorrect + " " + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddTableOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: Añadiendo elemento");
            TextBox[] textBox = { numberNewTable, dinersNewTable};
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: Verificando datos");
            bool datos = VerifyData(textBox);

            if (datos)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: Datos verificados");
                try
                {
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: creando elemento");
                    Mesa nuevo = new Mesa();
                    nuevo.Numero = Convert.ToInt32(numberNewTable.Text);
                    nuevo.Comensales = Convert.ToInt32(dinersNewTable.Text);
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: añadiendo elemento");
                    Config.GetInstance().db.Mesas.Add(nuevo);
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: elemento añadido ("+ nuevo.Id + ')');
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: Base de datos actualizada!");
                    gridVisible();

                    MessageBox.Show(Properties.Resources.table + " " + nuevo.Numero + " "+ Properties.Resources.addCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageTables: error añadiendo dato => (" + ex.Message +")");
                    MessageBox.Show(Properties.Resources.errorAdded + " " + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBackAddOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: volver del vista añadir");
            gridVisible();
        }
        private void gridVisible()
        {
            gridAddTable.Visibility = Visibility.Hidden;
            gridEditTables.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Visible;
            gridListTables.Visibility = Visibility.Visible;
        }

        private bool VerifyData(TextBox[] textBox)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: verificando datos");
            bool result = true;
            foreach (TextBox txt in textBox)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    MessageBox.Show(Properties.Resources.missingData);
                    result = false;
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: faltan datos, Datos aportado => (" + textBox.ToString() + ")");
                    break;
                }
            }
            return result;
        }

        private void BtnEditBack(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: volver del vista editar");
            gridVisible();
        }

        private void btnEditOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: editando datos");
            TextBox[] textBox = { numberTextBoxEdit, dinerTextBoxEdit };

            bool datos = VerifyData(textBox);

            if (datos)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: datos verificados!");
                try
                {
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: editando datos");
                    Mesa p = (Mesa)gridListTables.SelectedItem;
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: datos actualizados en la base de datos");
                    gridVisible();
                    MessageBox.Show(Properties.Resources.table + " '" + p.Numero + "' "+ Properties.Resources.editCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageTables: error editando datos => (" + ex.Message +  ")" );
                    MessageBox.Show(Properties.Resources.errorEdited + " " + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UpdateLenguage()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: actualizando idioma!");
            lblTitle.Content = Properties.Resources.tables;
            textBlockAdd.Text = Properties.Resources.add;
            btnSearch.Content = Properties.Resources.search;
            idColumn.Header = Properties.Resources.code;
            numberColumn.Header = Properties.Resources.number;
            dinersColumn.Header = Properties.Resources.diners;
            lblAddTable.Content = Properties.Resources.addTable;
            btnBackAdd.Content = Properties.Resources.back;
            lblnumberNewTable.Content = Properties.Resources.number;
            lbldinersNewTable.Content = Properties.Resources.diners;
            lblEditTitle.Content = Properties.Resources.mealsEdit;
            btnAddTable.Content = Properties.Resources.add;
            lblEditTitle.Content = Properties.Resources.tablesEdit;
            lblnumberEdit.Content = Properties.Resources.number;
            lbldinersEdit.Content = Properties.Resources.diners;
            btnEditProd.Content = Properties.Resources.modify;
            btnVolverEdit.Content = Properties.Resources.back;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageTables: idioma actualizado!");
        }


        private void ApplyConfig()
        {
            var col = Config.GetInstance().buttonBgColor;
            var col2 = Config.GetInstance().buttonFgColor;
            var col3 = Config.GetInstance().labelFgColor;
            var col4 = Config.GetInstance().tableBgColor;
            var col5 = Config.GetInstance().tableFgColor;
            if (col != null && col2 != null && col3 != null && col4 != null && col5 != null)
            {
                var bgButtonColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col));
                var fgButtonColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col2));
                var fgLabelColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col3));
                var bgTableColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col4));
                var fgTableColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col5));
                btnSearch.Background = bgButtonColor;
                btnAdd.Background = bgButtonColor;
                btnAddTable.Background = bgButtonColor;
                btnBackAdd.Background = bgButtonColor;
                btnEditProd.Background = bgButtonColor;
                btnVolverEdit.Background = bgButtonColor;
                btnSearch.Foreground = fgButtonColor;
                btnAdd.Foreground = fgButtonColor;
                btnAddTable.Foreground = fgButtonColor;
                btnBackAdd.Foreground = fgButtonColor;
                btnEditProd.Foreground = fgButtonColor;
                btnVolverEdit.Foreground = fgButtonColor;
                lblTitle.Foreground = fgLabelColor;
                lblAddTable.Foreground = fgLabelColor;
                lbldinersNewTable.Foreground = fgLabelColor;
                lblnumberNewTable.Foreground = fgLabelColor;
                lblEditTitle.Foreground = fgLabelColor;
                lblnumberEdit.Foreground = fgLabelColor;
                lbldinersEdit.Foreground = fgLabelColor;
                gridListTables.Background = bgTableColor;
                gridListTables.AlternatingRowBackground = fgTableColor;
            }
        }
    }
}
