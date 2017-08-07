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
    /// Interaction logic for Graph.xaml
    /// </summary>
    public class Graph : UserControl
    {
        private const int ITEMS_COUNT = 44;
        private const int GRID_SIZE = 6;

        private Queue<int> data = new Queue<int>(ITEMS_COUNT);

        public Graph()
        {
            DefaultStyleKey = typeof(Graph);
            Width = 265;
            Height = 60;

            //"Clear the stack"
            for (int i = 0; i < ITEMS_COUNT; i++)
            {
                data.Enqueue(0);
            }
        }

        public void Add(int value)
        {
            data.Dequeue();
            //Hacky Clamp method
            data.Enqueue(value < 0 ? 0 : value > 100 ? 100 : value);
        }

        protected override void OnRender(DrawingContext dc)
        {
            //Draw background
            dc.DrawRectangle(Background, null, new Rect(0, 0, Width, Height));

            int i = 0;
            foreach (int val in data)
            {
                double height = (val / 100f) * Height;
                dc.DrawRectangle(Foreground, null, new Rect(i * GRID_SIZE, Height - height, GRID_SIZE, height));
                i++;
            }

            //Draw grid so we cubes, not columns
            Pen pen = new Pen(Background, 1);
            for (double j = 0.5; j < Height; j+=GRID_SIZE)
            {
                dc.DrawLine(pen, new Point(0, j), new Point(Width, j));
            }

            for (double j = 0.5; j < Width; j+= GRID_SIZE)
            {
                dc.DrawLine(pen, new Point(j, 0), new Point(j, Height));
            }
        }
    }
}
