using System;
using System.Diagnostics;
using System.Windows;

namespace PAEE_FINAL
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: cargando configuración");
            Config.recuperaConfig();
            Config.GetInstance().load();
            Config.GetInstance().cambiaIdioma();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: cargando componentes");
            InitializeComponent();
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: componente cargado!");
            framePrincipal.Navigate(new PageHome());
        }

        private void ButtonBackOnClick(object sender, RoutedEventArgs e)
        {
            framePrincipal.Navigate(new PageHome());
        }

        private void ButtonMenuOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: Navegando a PageMeals");
            framePrincipal.Navigate(new PageMeals());
        }

        private void ButtonTablesOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: Navegando a PageTables");
            framePrincipal.Navigate(new PageTables());
        }

        private void ButtonCommandsOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: Navegando a PageCommands");
            framePrincipal.Navigate(new PageCommands());
        }

        private void ButtonInventoryOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: Navegando a PageInventory");
            framePrincipal.Navigate(new PageInventory());
        }

        private void ButtonConfigurationOnClick(object sender, RoutedEventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: Navegando a PageConfiguration");
            PageConfiguration page = new PageConfiguration();
            page.ChangeConfig += new EventHandler(UpdateLenguage);
            framePrincipal.Navigate(page);
        }

        private void UpdateLenguage(object sender, EventArgs e)
        {
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: actualizando idioma!");
            ButtonBack.Content = Properties.Resources.home;
            ButtonMenu.Content = Properties.Resources.menu;
            ButtonTables.Content = Properties.Resources.tables;
            ButtonCommands.Content = Properties.Resources.commands;
            ButtonInventory.Content = Properties.Resources.inventory;
            ButtonConfig.Content = Properties.Resources.configuration;
            Config.logger.TraceEvent(TraceEventType.Information, 1, "MainWindow: idioma actualizado!");
        }
    }
}
