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

namespace app
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //string path = @"C:\Users\e.yumagulov\Desktop\системы\lifen\program\data\app\dt.db";
        //string path = @"C:\Users\e.yumagulov\AppData\Local\Packages\f5a53da5-17be-49c0-8ecd-463b1460ced1_2wpxf91wdknyr\LocalState\sqliteSample.db";
        //string path = @"C:\Users\e.yumagulov\Downloads\sqliteSample.db";


        public MainPage()
        {
            this.InitializeComponent();

            SQLite.initialize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SQLite.create_db();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SQLite.create_table();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            List<string> table = SQLite.get_data();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            SQLite.AddData("новый элемент");
        }
    }
}
