using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace lifen
{
    internal static class ObservableCollectionExtension
    {

        // !!! вставить в метод пузырька проверку на пустые проходы - остановку сортировки, если всё уже отсортированно

        public static void BubbleSort(this ObservableCollection<Objective> o)
        {
            for (int i = o.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    Objective o1 = o[j - 1];
                    Objective o2 = o[j];
                    if (o1.Importance > o2.Importance)
                    {
                        o.Remove(o1);
                        o.Insert(j, o1);
                    }
                }
            }

            //o.Reverse();
        }

        public static void ReverceMy (this ObservableCollection<Objective> c)
        {
            for (int i = 0; i < c.Count; i++)
                c.Move(c.Count - 1, i);
        }


        //public static void Refresh<T>(this ObservableCollection<T> value)
        //{
        //    //CollectionViewSource.GetDefaultView(value).Refresh();
        //    //CollectionViewSource.ViewProperty(value).Refresh();
        //}
    }
}
