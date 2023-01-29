using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PAEE_FINAL
{
    /// <summary>
    /// Lógica de interacción para PageCommands.xaml
    /// </summary>
    public partial class PageCommands : Page
    {
        public PageCommands()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: cargando componentes");
            InitializeComponent();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: componente cargado!");
            UpdateLenguage();
            ApplyConfig();
            System.Windows.Data.CollectionViewSource commandsViewSource = (System.Windows.Data.CollectionViewSource)(this.FindResource("commandsViewSource"));
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: cargando datos de la base de datos");
            Config.GetInstance().db.Comandas.Load();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: datos cargados!");
            commandsViewSource.Source = Config.GetInstance().db.Comandas.Local;
        }

        private void BtnAdd(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: click button add!");
            cleanFormAdd();
            gridListCommands.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Hidden;
            gridAddCommand.Margin = new Thickness(5, 0, 5, 4);
            gridAddCommand.Visibility = Visibility.Visible;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: end button add!");
        }

        private void BtnSearchOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: click en buscando!");
            System.Windows.Data.CollectionViewSource commandsViewSource = (System.Windows.Data.CollectionViewSource)(this.FindResource("commandsViewSource"));
            string entrada = "%"+ textSearch.Text+ "%";
            if (entrada != "") {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: consulta de => ( " + entrada + ")");
                commandsViewSource.Source = Config.GetInstance().db.Comandas.SqlQuery("Select * from Comandas where Comandas.Mesa LIKE @input or Comandas.Platos LIKE @input",
                    new SqlParameter("@input", entrada)).ToListAsync().Result;
            } else {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: consulta de campo vacio!");
                commandsViewSource.Source = Config.GetInstance().db.Comandas.Local;
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: click button edit!");
            gridListCommands.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Hidden;
            gridEditCommand.Margin = new Thickness(5, 0, 5, 4);
            gridEditCommand.Visibility = Visibility.Visible;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: end button edit!");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: Eliminando elemento");
            Comanda p = (Comanda)gridListCommands.SelectedItem;
            MessageBoxResult res = MessageBox.Show(Properties.Resources.askDelete + " '" + p.Id + "'?", Properties.Resources.attention, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: respuesta Si al messagebox Eliminar");
                try
                {
                    Config.GetInstance().db.Comandas.Remove(p);
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: elemento eliminado '" + p.Id + "'");
                    MessageBox.Show(Properties.Resources.commands +"'" + p.Id + "' "+ Properties.Resources.deleteCorrect + ".",  Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageCommands: error eliminando => ( " + ex.Message + ")");
                    MessageBox.Show(Properties.Resources.errorDelete + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddMealOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: Añadiendo elemento");
            TextBox[] textBox = { tablesNewMeal, dishesNewMeal };
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: Verificando datos");
            bool datos = VerifyData(textBox);

            if (datos)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: Datos verificados");
                try
                {
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: creando elemento");
                    Comanda nuevo = new Comanda();
                    nuevo.Mesa = Convert.ToInt32(tablesNewMeal.Text);
                    nuevo.Platos = Convert.ToInt32(dishesNewMeal.Text);
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: añadiendo elemento");
                    Config.GetInstance().db.Comandas.Add(nuevo);
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: elemento añadido (" + nuevo.Id + ')');
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: Base de datos actualizada!");
                    gridVisible();

                    MessageBox.Show(Properties.Resources.command +" " + nuevo.Id + " " + Properties.Resources.addCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageCommands: error añadiendo dato => (" + ex.Message + ")");
                    MessageBox.Show(Properties.Resources.errorAdded + " " + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBackAddOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "v: volver del vista añadir");
            gridVisible();
        }

        private void btnEditOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: editando datos");
            TextBox[] textBox = { tablesTextBoxEdit, dishesTextBoxEdit };

            bool datos = VerifyData(textBox);

            if (datos)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: datos verificados!");
                try
                {
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: editando datos");
                    Comanda p = (Comanda)gridListCommands.SelectedItem;
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: datos actualizados en la base de datos");
                    gridVisible();
                    MessageBox.Show(Properties.Resources.command +" '" + p.Id + "' " + Properties.Resources.editCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageCommands: error editando datos => (" + ex.Message + ")");
                    MessageBox.Show(Properties.Resources.errorEdited + " "+ ex.Message, Properties.Resources.attention , MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void BtnEditBack(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: volver del vista editar");
            gridVisible();
        }

        private void gridVisible()
        {
            gridAddCommand.Visibility = Visibility.Hidden;
            gridEditCommand.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Visible;
            gridListCommands.Visibility = Visibility.Visible;
        }

        private bool VerifyData(TextBox[] textBox)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: verificando datos");
            bool result = true;
            foreach (TextBox txt in textBox)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    MessageBox.Show(Properties.Resources.missingData);
                    result = false;
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: faltan datos, Datos aportado => (" + textBox.ToString() + ")");
                    break;
                }
            }
            return result;
        }

        private void cleanFormAdd()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: limpiando formulario");
            tablesNewMeal.Text = "";
            dishesNewMeal.Text = "";
        }

        private void UpdateLenguage() 
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: actualizando idioma!");
            lblTitle.Content = Properties.Resources.commands;
            textBlockAdd.Text = Properties.Resources.add;
            btnSearch.Content = Properties.Resources.search;
            idColumn.Header = Properties.Resources.code;
            numberColumn.Header = Properties.Resources.numberTable;
            dishesColumn.Header = Properties.Resources.dishes;
            lblAddCommand.Content = Properties.Resources.addCommand;
            btnBackAdd.Content = Properties.Resources.back;
            lbltablesNewMeal.Content = Properties.Resources.numberTable;
            lbldishesNewMeal.Content = Properties.Resources.dishes;
            lblEditTitle.Content = Properties.Resources.commandEdit;
            lblTablesEdit.Content = Properties.Resources.numberTable;
            lblDishesEdit.Content = Properties.Resources.dishes;
            btnEditProd.Content = Properties.Resources.modify;
            btnVolverEdit.Content = Properties.Resources.back;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageCommands: idioma actualizado!");
        }

        private void ApplyConfig()
        {
            var col  = Config.GetInstance().buttonBgColor;
            var col2 = Config.GetInstance().buttonFgColor;
            var col3 = Config.GetInstance().labelFgColor;
            var col4 = Config.GetInstance().tableBgColor;
            var col5 = Config.GetInstance().tableFgColor;
            if (col != null && col2 != null && col3!= null && col4 != null && col5 != null)
            {
                var bgButtonColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col));
                var fgButtonColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col2));
                var fgLabelColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col3));
                var bgTableColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col4));
                var fgTableColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(col5));

                btnSearch.Background = bgButtonColor;
                btnAdd.Background = bgButtonColor;
                btnAddCommand.Background = bgButtonColor;
                btnBackAdd.Background = bgButtonColor;
                btnEditProd.Background = bgButtonColor;
                btnVolverEdit.Background = bgButtonColor;
                btnSearch.Foreground = fgButtonColor;
                btnAdd.Foreground = fgButtonColor;
                btnAddCommand.Foreground = fgButtonColor;
                btnBackAdd.Foreground = fgButtonColor;
                btnEditProd.Foreground = fgButtonColor;
                btnVolverEdit.Foreground = fgButtonColor;
                lblTitle.Foreground = fgLabelColor;
                lblAddCommand.Foreground = fgLabelColor;
                lbltablesNewMeal.Foreground = fgLabelColor;
                lbldishesNewMeal.Foreground = fgLabelColor;
                lblEditTitle.Foreground = fgLabelColor;
                lblTablesEdit.Foreground = fgLabelColor;
                lblDishesEdit.Foreground = fgLabelColor;
                gridListCommands.Background = bgTableColor;
                gridListCommands.AlternatingRowBackground = fgTableColor;
            }
        }
    }
}
