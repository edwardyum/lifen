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

namespace sqlite_uwp
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //string path = @"C:\Users\e.yumagulov\Desktop\системы\lifen\program\sqlite_uwp\db.db";
        //string path = @"C:\Users\e.yumagulov\AppData\Local\Packages\f5a53da5-17be-49c0-8ecd-463b1460ced1_2wpxf91wdknyr\LocalState\sqliteSample.db";
        //string path = @"C:\Users\e.yumagulov\Desktop\системы\lifen\program\4_sqlite\qwe.db";
        string path = @"C:\Users\e.yumagulov\AppData\Local\Packages\f5a53da5-17be-49c0-8ecd-463b1460ced1_2wpxf91wdknyr\LocalState\sqliteSample.db";
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SQLite.create_db(path);
        }
    }
}
