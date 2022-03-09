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

namespace lifen
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            Manager.initialize();
            Manager.execute();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            //if (args.IsSettingsSelected)
            //{
            //    contentFrame.Navigate(typeof(SampleSettingsPage));
            //}
            //else
            //{
            //    var selectedItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.SelectedItem;
            //    if (selectedItem != null)
            //    {
            //        string selectedItemTag = ((string)selectedItem.Tag);
            //        sender.Header = "Sample Page " + selectedItemTag.Substring(selectedItemTag.Length - 1);
            //        string pageName = "AppUIBasics.SamplePages." + selectedItemTag;
            //        Type pageType = Type.GetType(pageName);
            //        contentFrame.Navigate(pageType);
            //    }
            //}
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var selectedItem = args.InvokedItemContainer as NavigationViewItem;

            if (selectedItem == null)
            {

            }
            else
            {
                string selectedItemTag = selectedItem.Tag?.ToString()?? "SettingsPage";
                Type pageType = SectionPages.choose_page(selectedItemTag);
                contentFrame.Navigate(pageType);
            }
        }

        public void refresh()
        {
            Frame rootFrame = new Frame();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
