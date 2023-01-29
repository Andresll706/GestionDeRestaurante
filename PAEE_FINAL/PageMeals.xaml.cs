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
    /// Lógica de interacción para PageMeals.xaml
    /// </summary>
    public partial class PageMeals : Page
    {
        public PageMeals()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: cargando componentes");
            InitializeComponent();
            UpdateLenguage();
            ApplyConfig();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: componente cargado!");
            System.Windows.Data.CollectionViewSource mealsViewSource = (System.Windows.Data.CollectionViewSource)(this.FindResource("mealsViewSource"));
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: cargando datos de la base de datos");
            Config.GetInstance().db.Platos.Load();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: datos cargados!");
            mealsViewSource.Source = Config.GetInstance().db.Platos.Local;
        }

        private void BtnAdd(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: click button add!");
            cleanFormAdd();
            gridListMeals.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Hidden;
            gridAddMeal.Margin = new Thickness(5, 0, 5, 4);
            gridAddMeal.Visibility = Visibility.Visible;
            CompleteCategories();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: end button add!");
        }

        private void BtnSearchOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: click en buscando!");
            System.Windows.Data.CollectionViewSource mealsViewSource = (System.Windows.Data.CollectionViewSource)(this.FindResource("mealsViewSource"));
            string entrada = "%" + textSearch.Text + "%";
            if (entrada != "")
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: consulta de => ( " + entrada + ")");
                mealsViewSource.Source = Config.GetInstance().db.Platos.SqlQuery("Select * from Platos where Platos.Nombre LIKE @input or" +
                    " Platos.Descripcion LIKE @input or Platos.Ingredientes LIKE @input or Platos.Precio LIKE @input or Platos.Categoria LIKE @input",
                    new SqlParameter("@input", entrada)).ToListAsync().Result;
            }
            else
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: consulta de campo vacio!");
                mealsViewSource.Source = Config.GetInstance().db.Platos.Local;
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: click button edit!");
            gridListMeals.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Hidden;
            gridEditMeals.Margin = new Thickness(5, 0, 5, 4);
            gridEditMeals.Visibility = Visibility.Visible;
            CompleteCategories(slctCategoriesEdit);
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: end button edit!");
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: Eliminando elemento");
            Plato p = (Plato)gridListMeals.SelectedItem;
            MessageBoxResult res = MessageBox.Show(Properties.Resources.askDelete + " '" + p.Nombre + "'?", Properties.Resources.attention, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: respuesta Si al messagebox Eliminar");
                try
                {
                    Config.GetInstance().db.Platos.Remove(p);
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: elemento eliminado '" + p.Id + "'");
                    MessageBox.Show(Properties.Resources.dish + " '" + p.Nombre + "' "+ Properties.Resources.deleteCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageMeals: error eliminando => ( " + ex.Message + ")");
                    MessageBox.Show(Properties.Resources.errorDelete + " " + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnBackAddOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: volver del vista añadir");
            gridVisible();
        }

        private void BtnAddMealOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: Añadiendo elemento");
            TextBox[] textBox = { nameNewMeal, descriptionNewMeal, ingredientNewMeal, priceNewMeal };
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: Verificando datos");
            bool datos = VerifyData(textBox);

            if (datos)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: Datos verificados");
                try
                {
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: creando elemento");
                    Plato nuevo = new Plato();
                    nuevo.Nombre = nameNewMeal.Text;
                    nuevo.Descripcion = descriptionNewMeal.Text;
                    nuevo.Ingredientes = ingredientNewMeal.Text;
                    nuevo.Precio = Convert.ToInt32(priceNewMeal.Text);
                    nuevo.Categoria = Convert.ToInt32(slctCategories.SelectedValue.ToString());
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: añadiendo elemento");
                    Config.GetInstance().db.Platos.Add(nuevo);
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: elemento añadido (" + nuevo.Id + ')');
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: Base de datos actualizada!");
                    gridVisible();

                    MessageBox.Show(Properties.Resources.dish + " " + nuevo.Nombre + " " + Properties.Resources.addCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);

                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageMeals: error añadiendo dato => (" + ex.Message + ")");
                    MessageBox.Show(Properties.Resources.errorAdded + ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEditOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: editando datos");
            TextBox[] textBox = { nameTextBoxEdit, descriptionTextBoxEdit, ingredientsTextBoxEdit, priceTextBoxEdit };

            bool datos = VerifyData(textBox);

            if (datos)
            {
                Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: datos verificados!");
                try
                {
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: editando datos");
                    Plato p = (Plato)gridListMeals.SelectedItem;
                    Config.GetInstance().db.SaveChanges();
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: datos actualizados en la base de datos");
                    gridVisible();
                    MessageBox.Show(Properties.Resources.dish + " '" + p.Nombre + "' " + Properties.Resources.editCorrect, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Config.logger.TraceEvent(TraceEventType.Error, 1, "PageMeals: error editando datos => (" + ex.Message + ")");
                    MessageBox.Show(Properties.Resources.errorEdited + " "+ ex.Message, Properties.Resources.attention, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnEditBack(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: volver del vista editar");
            gridVisible();
        }

        private void gridVisible()
        {
            gridAddMeal.Visibility = Visibility.Hidden;
            gridEditMeals.Visibility = Visibility.Hidden;
            gridHeader.Visibility = Visibility.Visible;
            gridListMeals.Visibility = Visibility.Visible;
        }

        private bool VerifyData(TextBox[] textBox)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: verificando datos");
            bool result = true;
            foreach (TextBox txt in textBox)
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    MessageBox.Show(Properties.Resources.missingData);
                    result = false;
                    Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: faltan datos, Datos aportado => (" + textBox.ToString() + ")");
                    break;
                }
            }
            return result;
        }

        private void cleanFormAdd()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: limpiando formulario");
            nameNewMeal.Text = "";
            descriptionNewMeal.Text = "";
            ingredientNewMeal.Text = "";
            priceNewMeal.Text = "";
            slctCategories.SelectedIndex = 0;
        }


        private void CompleteCategories()
        {
            List<Categoria> cats = Config.GetInstance().db.Categorias.OrderBy(c => c.Nombre).ToList();
            slctCategories.ItemsSource = cats;
            slctCategories.DisplayMemberPath = "Nombre";
            slctCategories.SelectedValuePath = "Id";
        }

        private void CompleteCategories(ComboBox slct)
        {
            List<Categoria> cats = Config.GetInstance().db.Categorias.OrderBy(c => c.Nombre).ToList();

            slctCategoriesEdit.ItemsSource = cats;
            slctCategoriesEdit.DisplayMemberPath = "Nombre";
            slctCategoriesEdit.SelectedValuePath = "Id";
        }

        private void UpdateLenguage()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: actualizando idioma!");
            lblTitle.Content = Properties.Resources.meals;
            textBlockAdd.Text = Properties.Resources.add;
            btnSearch.Content = Properties.Resources.search;
            idColumn.Header = Properties.Resources.code;
            nameColumn.Header = Properties.Resources.name;
            descriptionColumn.Header = Properties.Resources.description;
            ingredientsColumn.Header = Properties.Resources.ingredient;
            priceColumn.Header = Properties.Resources.price;
            categoryColumn.Header = Properties.Resources.category;
            lblAddMeal.Content = Properties.Resources.addMeal;
            btnBackAdd.Content = Properties.Resources.back;
            lblnameNewMeal.Content = Properties.Resources.name;
            lblDescriptionNewMeal.Content = Properties.Resources.description;
            lblingredientNewMeal.Content = Properties.Resources.ingredient;
            lblpriceNewMeal.Content = Properties.Resources.price;
            lblcategoryNewMeal.Content = Properties.Resources.category;
            lblEditTitle.Content = Properties.Resources.mealsEdit;
            btnAddMeal.Content = Properties.Resources.add;
            lblnameEdit.Content = Properties.Resources.name;
            lbldescriptionEdit.Content = Properties.Resources.description;
            lblingredientsEdit.Content = Properties.Resources.ingredient;
            lblpriceEdit.Content = Properties.Resources.price;
            lblcategoryEdit.Content = Properties.Resources.category;
            btnEditProd.Content = Properties.Resources.modify;
            btnVolverEdit.Content = Properties.Resources.back;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "PageMeals: idioma actualizado!");
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
                btnAddMeal.Background = bgButtonColor;
                btnBackAdd.Background = bgButtonColor;
                btnEditProd.Background = bgButtonColor;
                btnVolverEdit.Background = bgButtonColor;
                btnSearch.Foreground = fgButtonColor;
                btnAdd.Foreground = fgButtonColor;
                btnAddMeal.Foreground = fgButtonColor;
                btnBackAdd.Foreground = fgButtonColor;
                btnEditProd.Foreground = fgButtonColor;
                btnVolverEdit.Foreground = fgButtonColor;
                lblTitle.Foreground = fgLabelColor;
                lblAddMeal.Foreground = fgLabelColor;
                lblcategoryNewMeal.Foreground = fgLabelColor;
                lblDescriptionNewMeal.Foreground = fgLabelColor;
                lblingredientNewMeal.Foreground = fgLabelColor;
                lblnameNewMeal.Foreground = fgLabelColor;
                lblpriceNewMeal.Foreground = fgLabelColor;
                lblEditTitle.Foreground = fgLabelColor;
                lblnameEdit.Foreground = fgLabelColor;
                lbldescriptionEdit.Foreground = fgLabelColor;
                lblcategoryEdit.Foreground = fgLabelColor;
                lblingredientsEdit.Foreground = fgLabelColor;
                lblpriceEdit.Foreground = fgLabelColor;
                gridListMeals.Background = bgTableColor;
                gridListMeals.AlternatingRowBackground = fgTableColor;
            }
        }
    }
}
