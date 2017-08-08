using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Merlin.GUI.Controls
{
    /// <summary>
    /// Interaction logic for ItemList.xaml
    /// </summary>
    public partial class ItemList : UserControl
    {
        public struct Item
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public int MaxStack { get; set; }
            public Visibility ShowCount { get; set; }
            public Visibility ShowProgress { get; set; }
            public float Progress { get { return (Count / (float)MaxStack) * 100; } set { } }

            public Item(string name, int count, int maxStack = 999, bool showCount = true, bool showProgress = true)
            {
                Name = name;
                Count = count;
                MaxStack = maxStack;
                ShowCount = showCount ? Visibility.Visible : Visibility.Hidden;
                ShowProgress = showProgress ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private ObservableCollection<Item> _items = new ObservableCollection<Item>();

        public ObservableCollection<Item> Items => _items;

        public ItemList()
        {
            InitializeComponent();
            _items.CollectionChanged += (s, e) => { PART_List.ItemsSource = _items; };
            PART_List.ItemsSource = _items;
        }
    }
}
