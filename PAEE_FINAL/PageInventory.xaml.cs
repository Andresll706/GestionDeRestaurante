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
    /// Lógica de interacción para PageInventory.xaml
    /// </summary>
    public partial class PageInventory : Page
    {
        public PageInventory()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: cargando componentes");
            InitializeComponent();
            UpdateLenguage();
            ApplyConfig();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: componente cargado!");
            System.Windows.Data.CollectionViewSource ingredientsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("ingredientsViewSource")));
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: cargando datos de la base de datos");
            Config.GetInstance().db.Ingredientes.Load();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: datos cargados!");
            ingredientsViewSource.Source = Config.GetInstance().db.Ingredientes.Local;
        }

        private void BtnEditBack(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: volver del vista editar");
            gridVisible();
        }

        private void btnEditOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: editando datos");
            TextBox[] textBox = { nameTextBoxEdit };

            bool datos = VerifyData(textBox);

            if (datos)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: datos verificados!");
                try
                {
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: editando datos");
                    Ingrediente p = (Ingrediente)gridListIngredient.SelectedItem;
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: datos actualizados en la base de datos");
                    gridVisible();
                    MessageBox.Show( Properties.Resources.ingredient + " '" + p.Nombre + " " + Properties.Resources.editCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageInventory: error editando datos => (" + ex.Message + ")");
                    MessageBox.Show(Properties.Resources.errorEdited +  " " + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddIngredientOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: Añadiendo elemento");
            TextBox[] textBox = { nameNewIngredient };
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: Verificando datos");
            bool datos = VerifyData(textBox);

            if (datos)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: Datos verificados");
                try
                {
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: creando elemento");
                    Ingrediente nuevo = new Ingrediente();
                    nuevo.Nombre = nameNewIngredient.Text;
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: añadiendo elemento");
                    Config.GetInstance().db.Ingredientes.Add(nuevo);
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: elemento añadido (" + nuevo.Id + ')');
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: Base de datos actualizada!");
                    gridVisible();

                    MessageBox.Show( Properties.Resources.ingredient + " " + nuevo.Nombre + Properties.Resources.addCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageInventory: error añadiendo dato => (" + ex.Message + ")");
                    MessageBox.Show(Properties.Resources.errorAdded + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSearchOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: click en buscando!");
            System.Windows.Data.CollectionViewSource ingredientsViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("ingredientsViewSource")));
            string entrada = "%" + textSearch.Text + "%";
            if (entrada != "")
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: consulta de => ( " + entrada + ")");
                ingredientsViewSource.Source = Config.GetInstance().db.Ingredientes.SqlQuery("Select * from Ingredientes where Ingredientes.Nombre LIKE @input",
                    new SqlParameter("@input", entrada)).ToListAsync().Result;
            }
            else
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: consulta de campo vacio!");
                ingredientsViewSource.Source = Config.GetInstance().db.Ingredientes.Local;
            }
        }

        private void BtnAdd(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: click button add!");
            cleanFormAdd();
            gridListIngredient.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Hidden;
            gridAddIngredient.Margin = new Thickness(5, 0, 5, 4);
            gridAddIngredient.Visibility = Visibility.Visible;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: end button add!");
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: click button edit!");
            gridListIngredient.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Hidden;
            gridEditIngredient.Margin = new Thickness(5, 0, 5, 4);
            gridEditIngredient.Visibility = Visibility.Visible;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: end button edit!");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: Eliminando elemento");
            Ingrediente p = (Ingrediente)gridListIngredient.SelectedItem;
            MessageBoxResult res = MessageBox.Show(Properties.Resources.askDelete + " '" + p.Nombre + "'?", Properties.Resources.attention, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: respuesta Si al messagebox Eliminar");
                try
                {
                    Config.GetInstance().db.Ingredientes.Remove(p);
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: elemento eliminado '" + p.Id + "'");
                    MessageBox.Show(Properties.Resources.ingredient + " '" + p.Nombre + "' " + Properties.Resources.deleteCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageInventory: error eliminando => ( " + ex.Message + ")");
                    MessageBox.Show(Properties.Resources.errorDelete + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBackAddOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: volver del vista añadir");
            gridVisible();
        }

        private void gridVisible()
        {
            gridAddIngredient.Visibility = Visibility.Hidden;
            gridEditIngredient.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Visible;
            gridListIngredient.Visibility = Visibility.Visible;
        }

        private bool VerifyData(TextBox[] textBox)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: verificando datos");
            bool result = true;
            foreach (TextBox txt in textBox)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    MessageBox.Show(Properties.Resources.missingData);
                    result = false;
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: faltan datos, Datos aportado => (" + textBox.ToString() + ")");
                    break;
                }
            }
            return result;
        }

        private void cleanFormAdd()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: limpiando formulario");
            nameNewIngredient.Text = "";
        }

        private void UpdateLenguage()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: actualizando idioma!");
            lblTitle.Content = Properties.Resources.inventory;
            textBlockAdd.Text = Properties.Resources.add;
            btnSearch.Content = Properties.Resources.search;
            idColumn.Header = Properties.Resources.code;
            nameColumn.Header = Properties.Resources.name;
            lblAddIngredient.Content = Properties.Resources.add;
            btnBackAdd.Content = Properties.Resources.back;
            lblnameNewIngredient.Content = Properties.Resources.name;
            lblEditTitle.Content = Properties.Resources.edit;
            btnAddIngredient.Content = Properties.Resources.add;
            lblnameEdit.Content = Properties.Resources.name;
            btnEditIngredient.Content = Properties.Resources.modify;
            btnVolverEdit.Content = Properties.Resources.back;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageInventory: idioma actualizado!");
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
                btnAddIngredient.Background = bgButtonColor;
                btnBackAdd.Background = bgButtonColor;
                btnEditIngredient.Background = bgButtonColor;
                btnVolverEdit.Background = bgButtonColor;
                btnSearch.Foreground = fgButtonColor;
                btnAdd.Foreground = fgButtonColor;
                btnAddIngredient.Foreground = fgButtonColor;
                btnBackAdd.Foreground = fgButtonColor;
                btnEditIngredient.Foreground = fgButtonColor;
                btnVolverEdit.Foreground = fgButtonColor;
                lblTitle.Foreground = fgLabelColor;
                lblAddIngredient.Foreground = fgLabelColor;
                lblnameNewIngredient.Foreground = fgLabelColor;
                lblEditTitle.Foreground = fgLabelColor;
                lblnameEdit.Foreground = fgLabelColor;
                gridListIngredient.Background = bgTableColor;
                gridListIngredient.AlternatingRowBackground = fgTableColor;
            }
        }
    }
}
