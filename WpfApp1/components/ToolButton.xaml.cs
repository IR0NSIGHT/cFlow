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
using System.Xml.Schema;

namespace WpfApp1.components
{
    /// <summary>
    /// Interaction logic for ToolButton.xaml
    /// </summary>
    public partial class ToolButton : Button
    {
        public ToolButton()
        {
            InitializeComponent();
            SetActive(false);
        }
        public static readonly DependencyProperty OverWidthProperty =
            DependencyProperty.RegisterAttached("OverWidth", typeof(double), typeof(Extensions), new PropertyMetadata(default(double)));

        public static void SetOverWidth(UIElement element, double value)
        {
            element.SetValue(OverWidthProperty, value);
        }

        public static double GetOverWidth(UIElement element)
        {
            return (double)element.GetValue(OverWidthProperty);
        }

        private bool active;
        public void SetActive(bool active)
        {
            this.Background = active ? Brushes.SlateGray : Brushes.DarkSlateGray;
            this.active = active;
        }
    }
}
