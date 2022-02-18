using System;
using System.Collections.Generic;
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

namespace _3_link_properties
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        MyTask task = new MyTask();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.fill();

            task.Name = Manager.table.Rows[0][1].ToString();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            task.Name = "16";

            string s = Manager.table.Rows[0][1].ToString();
        }
    }
}
