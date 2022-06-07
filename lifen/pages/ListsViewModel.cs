using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Reflection;
using Windows.UI.Xaml;

namespace lifen
{
    internal class ListsViewModel : DependencyObject
    {
        public static bool forming = false;

        public Objective root
        {
            get { return (Objective)GetValue(rootProperty); }
            set { SetValue(rootProperty, value); }
        }
        // Using a DependencyProperty as the backing store for root.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty rootProperty = DependencyProperty.Register("root", typeof(Objective), typeof(ListsViewModel), new PropertyMetadata(null));


        public Objective today
        {
            get { return (Objective)GetValue(todayProperty); }
            set { SetValue(todayProperty, value); }
        }
        // Using a DependencyProperty as the backing store for today.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty todayProperty = DependencyProperty.Register("today", typeof(Objective), typeof(ListsViewModel), new PropertyMetadata(null));



        public ListsViewModel()
        {
            root = new Objective("1", null);  // корневой узел
            today = new Objective("1", null);
            //root.form();

            //if (!Manager.event_contains_method(typeof(ListsViewModel)))
            //    Manager.refresh += refresh_data;

            //root = Manager.root;
            //today = Manager.today;
        }

        private void refresh_data()
        {
            root = null;
            root = Manager.root;

            today = null;
            today = Manager.today;


        }


    }
}
