using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace app_db
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Objective command;

        public MainPage()
        {
            this.InitializeComponent();

            SQLite.initialize();

            Manager.execute();
            command = Manager.objective;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            command.add_task();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            command.add_task_for_today();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            command.delete_task();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            command.delete_task_from_today();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            command.Description = "новое описание";
            command.set_description();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            command.Done = true;
            command.set_done();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            command.Name = "rooot";
            command.set_name();
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {

        }
    }
}
