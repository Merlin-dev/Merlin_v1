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

namespace Merlin.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            for (int i = 0; i < 44; i++)
            {
                PART_CapacityGraph.Add((int)((i/44f) * 200));
            }

            SwitchToTools(null,null);
           
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MerlinServer.Instance.Start();

            PART_InvTypeSelector.ContextMenu.Width = PART_InvTypeSelector.ActualWidth;
            PART_InvTypeSelector.ContextMenuOpening += PART_InvTypeSelector_ContextMenuOpening;
            PART_InvTypeSelector.PreviewMouseLeftButtonUp += PART_InvTypeSelector_PreviewMouseLeftButtonUp;
        }

        private void PART_InvTypeSelector_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PART_InvTypeSelector.ContextMenu != null)
            {
                PART_InvTypeSelector.ContextMenu.PlacementTarget = PART_InvTypeSelector;
                PART_InvTypeSelector.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                ContextMenuService.SetPlacement(PART_InvTypeSelector, System.Windows.Controls.Primitives.PlacementMode.Bottom);
                PART_InvTypeSelector.ContextMenu.IsOpen = true;
            }
        }

        private void PART_InvTypeSelector_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            PART_InvTypeSelector.ContextMenu.IsOpen = false;
            e.Handled = true;
        }

        private void SwitchToInventory(object sender, RoutedEventArgs e)
        {
            PART_InvTypeSelector.Content = "INVENTORY";
            PART_InventoryView.Items.Clear();
            PART_InventoryView.Items.Add(new Controls.ItemList.Item("Granite", 999));
            PART_InventoryView.Items.Add(new Controls.ItemList.Item("Granite", 50));
            PART_InventoryView.Items.Add(new Controls.ItemList.Item("Iron ore", 155));
        }
        private void SwitchToTools(object sender, RoutedEventArgs e)
        {
            PART_InvTypeSelector.Content = "TOOLS";
            PART_InventoryView.Items.Clear();
            PART_InventoryView.Items.Add(new Controls.ItemList.Item("Pickaxe", 999,showCount:false));
            PART_InventoryView.Items.Add(new Controls.ItemList.Item("Axe", 750, showCount: false));
            PART_InventoryView.Items.Add(new Controls.ItemList.Item("Hammer", 500, showCount: false));
        }
    }
}
