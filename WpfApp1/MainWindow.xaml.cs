using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            drawCropped();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void drawCropped()
        {
            // Load the original image
            BitmapImage originalImage = new BitmapImage(new Uri("pack://application:,,,/medium_flats.png"));

            // Define the region of interest (ROI) to draw
            Rect roi = new Rect(100, 100, 200, 200); // Example: (x, y, width, height)

            // Create an ImageDrawing based on the original image and ROI
            ImageDrawing imageDrawing = new ImageDrawing(originalImage, roi);

            // Create a DrawingGroup and add the ImageDrawing to it
            DrawingGroup drawingGroup = new DrawingGroup();
            drawingGroup.Children.Add(imageDrawing);

            // Create a DrawingBrush based on the DrawingGroup
            DrawingBrush drawingBrush = new DrawingBrush(drawingGroup);

            // Create a Rectangle to display the DrawingBrush
            Rectangle rectangle = new Rectangle();
            rectangle.Fill = drawingBrush;
            rectangle.Width = roi.Width;
            rectangle.Height = roi.Height;

            // Add the Rectangle to your GUI
            MapGrid.Children.Add(rectangle); // Replace "YourGrid" with the name of your Grid control

        }
    }
}