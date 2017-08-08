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

namespace Merlin.GUI.Controls
{
    /// <summary>
    /// Interaction logic for InventoryCapacity.xaml
    /// </summary>
    public partial class InventoryCapacity : UserControl
    {
        public int Progress
        {
            get
            {
                return (int)GetValue(ProgressProperty);
            }

            set
            {
                SetValue(ProgressProperty, value);
            }
        }

        private static readonly DependencyProperty ProgressProperty =
          DependencyProperty.Register("progress", typeof(int), typeof(InventoryCapacity),
          new UIPropertyMetadata(0, new PropertyChangedCallback(OnProgressChanged)));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //NOTE: probably limit min val to 0... because it won't be ever less than 0
            InventoryCapacity ic = d as InventoryCapacity;
            int val = (int)e.NewValue;
            ic.PART_Progress.Value = val;
            ic.PART_Value.Text = val + "%";
        }

        public InventoryCapacity()
        {
            InitializeComponent();
        }
    }
}
